using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using Swank.Description;
using Swank.Extensions;

namespace Swank.Specification
{
    public class TypeGraphService
    {
        private readonly Configuration.Configuration _configuration;
        private readonly IDescriptionConvention<PropertyInfo, MemberDescription> _memberConvention;
        private readonly OptionBuilderService _optionBuilderService;
        private readonly IDescriptionConvention<Type, TypeDescription> _typeConvention;

        public TypeGraphService(
            Configuration.Configuration configuration,
            IDescriptionConvention<Type, TypeDescription> typeConvention,
            IDescriptionConvention<PropertyInfo, MemberDescription> memberConvention,
            OptionBuilderService optionBuilderService)
        {
            _configuration = configuration;
            _memberConvention = memberConvention;
            _optionBuilderService = optionBuilderService;
            _typeConvention = typeConvention;
        }

        public DataType BuildForMessage(bool requestGraph, Type type, 
            EndpointDescription description, IApiDescription endpoint)
        {
            var @namespace = description.MethodName;
            var logicalName = requestGraph ? "Request" : "Response";
            var dataType = BuildGraph(type, requestGraph, endpoint, 
                logicalName: logicalName, @namespace: @namespace);
            return dataType;
        }

        public DataType BuildForParameter(Type type,
            EndpointDescription endpointDescription,
            ParameterDescription parameterDescription, 
            IApiDescription endpoint)
        {
            var @namespace = endpointDescription.MethodName;
            var dataType = BuildGraph(type, true, endpoint, @namespace: @namespace);
            return dataType;
        }

        private DataType BuildGraph(
            Type type,
            bool requestGraph,
            IApiDescription endpoint,
            DataType parent = null,
            IEnumerable<Type> ancestors = null,
            MemberDescription memberDescription = null,
            string name = null,
            string logicalName = null,
            string @namespace = null)
        {
            var description = _typeConvention.GetDescription(type);

            var dataType = new DataType
            {
                Comments = description.Comments
            };

            logicalName = logicalName ?? memberDescription?.Name;

            if (type.IsSimpleType())
            {
                BuildSimpleType(parent, name, dataType, description, type, 
                    requestGraph, @namespace, memberDescription);
            }
            else if (type.IsDictionary())
            {
                BuildDictionary(name, dataType, type, description, requestGraph, 
                    endpoint, ancestors, memberDescription, parent,
                    logicalName, @namespace);
            }
            else if (type.IsArray || type.IsEnumerable())
            {
                BuildArray(name, dataType, type, description, requestGraph, 
                    endpoint, ancestors, memberDescription, parent, 
                    logicalName, @namespace);
            }
            else 
            {
                BuildComplexType(parent, name, description, dataType, type, requestGraph, 
                    endpoint, ancestors, logicalName ?? memberDescription?.Name, @namespace);
            }

            return _configuration.TypeOverrides.Apply(new TypeOverrideContext
            {
                Type = type,
                DataType = dataType,
                ApiDescription = endpoint,
                Description = description,
                IsRequest = requestGraph
            }).DataType;
        }

        private void BuildSimpleType(
            DataType parent,
            string name,
            DataType dataType, 
            TypeDescription description, 
            Type type, 
            bool requestGraph,
            string @namespace,
            MemberDescription memberDescription)
        {
            dataType.Name = name ?? description.Name;
            dataType.IsSimple = true;
            dataType.IsNullable = description.Nullable;
            if (type.GetNullableUnderlyingType().IsEnum)
            {
                dataType.Namespace = @namespace ?? parent.LogicalName +
                    (memberDescription != null ? memberDescription.Name : dataType.Name);
                dataType.FullNamespace = (parent?.FullNamespace ?? Enumerable.Empty<string>())
                    .Concat(dataType.Namespace).ToList();
                dataType.Enumeration = _optionBuilderService.BuildOptions(type, null, requestGraph);
                dataType.Id = description.Id;
            }
            dataType.SampleValue = GetSimpleTypeSampleValue(dataType);
        }

        private string GetSimpleTypeSampleValue(DataType type)
        {
            if (type.Enumeration != null)
            {
                return type.Enumeration.Options.FirstOrDefault()?.Value;
            }

            switch (type.Name)
            {
                case Xml.UnsignedLongType:
                case Xml.LongType:
                case Xml.UnsignedIntType:
                case Xml.IntType:
                case Xml.UnsignedShortType:
                case Xml.ShortType:
                case Xml.ByteType:
                case Xml.UnsignedByteType:
                    return typeof(int).GetSampleValue(_configuration);
                case Xml.FloatType:
                case Xml.DoubleType:
                case Xml.DecimalType:
                    return typeof(decimal).GetSampleValue(_configuration);
                case Xml.BooleanType:
                    return typeof(bool).GetSampleValue(_configuration);
                case Xml.DateTimeType:
                    return typeof(DateTime).GetSampleValue(_configuration);
                case Xml.DurationType:
                    return typeof(TimeSpan).GetSampleValue(_configuration);
                case Xml.UuidType:
                    return typeof(Guid).GetSampleValue(_configuration);
                case Xml.AnyUriType:
                    return typeof(Uri).GetSampleValue(_configuration);
                default:
                    return _configuration.SampleStringValue;
            }
        }

        private void BuildDictionary(
            string name,
            DataType dataType,
            Type type,
            TypeDescription description,
            bool requestGraph,
            IApiDescription endpoint,
            IEnumerable<Type> ancestors,
            MemberDescription memberDescription,
            DataType parent = null,
            string logicalName = null,
            string @namespace = null)
        {
            dataType.Name = name ?? memberDescription?.Name ?? description.Name;
            var types = type.GetGenericDictionaryTypes();
            dataType.IsDictionary = true;
            dataType.Comments = memberDescription?.Comments ?? dataType.Comments;
            dataType.DictionaryEntry = new DictionaryEntry
            {
                KeyName = memberDescription?.DictionaryEntry.KeyName ??  
                    description?.DictionaryEntry.KeyName,
                KeyComments = memberDescription?.DictionaryEntry.KeyComments ??
                    description?.DictionaryEntry.KeyComments,
                KeyType = BuildGraph(types.Key, requestGraph, endpoint, dataType, ancestors),
                ValueComments = memberDescription?.DictionaryEntry.ValueComments ??
                    description?.DictionaryEntry.ValueComments,
                ValueType = BuildGraph(types.Value, requestGraph, 
                    endpoint, parent ?? dataType, ancestors, memberDescription,
                    logicalName: logicalName, @namespace: @namespace)
            };
        }

        private void BuildArray(
            string name,
            DataType dataType,
            Type type,
            TypeDescription description,
            bool requestGraph,
            IApiDescription endpoint,
            IEnumerable<Type> ancestors,
            MemberDescription memberDescription,
            DataType parent = null,
            string logicalName = null,
            string @namespace = null)
        {
            dataType.Name = name ?? memberDescription?.Name ?? description.Name;
            dataType.IsArray = true;
            dataType.Comments = memberDescription?.Comments ?? dataType.Comments;
            var itemType = BuildGraph(type.GetEnumerableElementType(), requestGraph, 
                endpoint, parent ?? dataType, ancestors, memberDescription,
                logicalName: logicalName, @namespace: @namespace);
            dataType.ArrayItem = new ArrayItem
            {
                Name = memberDescription?.ArrayItem.Name ?? 
                    description?.ArrayItem.Name ?? 
                    itemType.Name,
                Comments = memberDescription?.ArrayItem.Comments ?? 
                    description.ArrayItem?.Comments,
                Type = itemType
            };
        }

        private void BuildComplexType(
            DataType parent,
            string name,
            TypeDescription description,
            DataType dataType,
            Type type,
            bool requestGraph,
            IApiDescription endpoint,
            IEnumerable<Type> ancestors,
            string logicalName,
            string @namespace)
        {
            dataType.Id = description.Id;
            dataType.Name = name ?? description.Name;
            dataType.IsComplex = true;
            dataType.LogicalName = logicalName ?? dataType.Name;
            dataType.Namespace = @namespace ?? dataType.LogicalName;
            dataType.FullNamespace = (parent?.FullNamespace ?? Enumerable.Empty<string>())
                .Concat(dataType.Namespace).ToList();
            dataType.Members = type.GetProperties()
                .Select(x => new
                {
                    Property = x,
                    Ancestors = ancestors.Concat(type),
                    Type = x.PropertyType,
                    UnwrappedType = x.PropertyType.UnwrapType(),
                    Description = _memberConvention.GetDescription(x)
                })
                .Where(x => x.Ancestors.All(y => y != x.UnwrappedType) && !x.Description.Hidden)
                .Select(x => _configuration.MemberOverrides.Apply(new MemberOverrideContext
                {
                    Description = x.Description,
                    Property = x.Property,
                    ApiDescription = endpoint,
                    IsRequest = requestGraph,
                    Member = new Member
                    {
                        Name = x.Description?.Name,
                        Comments = x.Description?.Comments,
                        DefaultValue = requestGraph ? x.Description?.DefaultValue : null,
                        SampleValue = x.Description?.SampleValue,
                        Optional = requestGraph && 
                            (x.Description?.Optional ?? 
                            _configuration.DefaultOptionalScope)
                                .IsOptional(endpoint.HttpMethod),
                        Deprecated = x.Description.Deprecated,
                        DeprecationMessage = x.Description.DeprecationMessage,
                        Type = BuildGraph(x.Type, requestGraph, endpoint,
                            dataType, x.Ancestors, x.Description),
                        MaxLength = x.Description?.MaxLength,
                        Encoding = x.Description?.Encoding?.ToString()
                    },

                }).Member).ToList();
        }
    }

    public static class TypeGraphServiceExtensions
    {
        public static bool IsOptional(this OptionalScope optional, HttpMethod method)
        {
            return optional == OptionalScope.All ||
                (optional == OptionalScope.Post && method == HttpMethod.Post) ||
                (optional == OptionalScope.Put && method == HttpMethod.Put) ||
                (optional == OptionalScope.AllButPost && method != HttpMethod.Post) ||
                (optional == OptionalScope.AllButPut && method != HttpMethod.Put);
        }
    }
}