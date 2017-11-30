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
            var logicalName = @namespace + (requestGraph ? "Request" : "Response");
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
                Name = name ?? (!type.IsSimpleType() && memberDescription != null ? 
                    memberDescription.Name : description.Name),
                Comments = description.Comments
            };

            if (type.IsDictionary())
                BuildDictionary(dataType, type, description, requestGraph, 
                    endpoint, ancestors, memberDescription, parent,
                    logicalName, @namespace);
            else if (type.IsArray || type.IsList())
                BuildArray(dataType, type, description, requestGraph, 
                    endpoint, ancestors, memberDescription, parent, 
                    logicalName, @namespace);
            else if (type.IsSimpleType()) BuildSimpleType(parent, dataType, 
                description, type, requestGraph, @namespace, memberDescription);
            else BuildComplexType(parent, dataType, type, requestGraph, 
                endpoint, ancestors, logicalName, @namespace);

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
            DataType dataType, 
            TypeDescription description, 
            Type type, 
            bool requestGraph,
            string @namespace,
            MemberDescription memberDescription)
        {
            dataType.IsSimple = true;
            dataType.IsNullable = description.Nullable;
            if (type.GetNullableUnderlyingType().IsEnum)
            {
                dataType.Namespace = @namespace ?? parent.LogicalName +
                    (memberDescription != null ? memberDescription.Name : dataType.Name);
                dataType.FullNamespace = (parent?.FullNamespace ?? Enumerable.Empty<string>())
                    .Concat(dataType.Namespace).ToList();
                dataType.Enumeration = _optionBuilderService.BuildOptions(type, null, requestGraph);
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
            DataType dataType,
            Type type,
            TypeDescription typeDescription,
            bool requestGraph,
            IApiDescription endpoint,
            IEnumerable<Type> ancestors,
            MemberDescription memberDescription,
            DataType parent = null,
            string logicalName = null,
            string @namespace = null)
        {
            var types = type.GetGenericDictionaryTypes();
            dataType.IsDictionary = true;
            dataType.Comments = memberDescription.WhenNotNull(x => x.Comments)
                .OtherwiseDefault() ?? dataType.Comments;
            dataType.DictionaryEntry = new DictionaryEntry
            {
                KeyName = memberDescription.WhenNotNull(x => x.DictionaryEntry.KeyName)
                    .OtherwiseDefault() ??  typeDescription.WhenNotNull(x => x
                        .DictionaryEntry.KeyName).OtherwiseDefault(),
                KeyComments = memberDescription
                    .WhenNotNull(x => x.DictionaryEntry.KeyComments).OtherwiseDefault() ??
                    typeDescription.WhenNotNull(x => x.DictionaryEntry.KeyComments)
                        .OtherwiseDefault(),
                KeyType = BuildGraph(types.Key, requestGraph, endpoint, dataType, ancestors),
                ValueComments = memberDescription
                    .WhenNotNull(x => x.DictionaryEntry.ValueComments).OtherwiseDefault() ??
                    typeDescription.WhenNotNull(x => x.DictionaryEntry.ValueComments)
                        .OtherwiseDefault(),
                ValueType = BuildGraph(types.Value, requestGraph, 
                    endpoint, parent ?? dataType, ancestors, memberDescription,
                    logicalName: logicalName, @namespace: @namespace)
            };
        }

        private void BuildArray(
            DataType dataType,
            Type type,
            TypeDescription typeDescription,
            bool requestGraph,
            IApiDescription endpoint,
            IEnumerable<Type> ancestors,
            MemberDescription memberDescription,
            DataType parent = null,
            string logicalName = null,
            string @namespace = null)
        {
            dataType.IsArray = true;
            dataType.Comments = memberDescription.WhenNotNull(x => x.Comments)
                .OtherwiseDefault() ?? dataType.Comments;
            var itemType = BuildGraph(type.GetListElementType(), requestGraph, 
                endpoint, parent ?? dataType, ancestors, memberDescription,
                logicalName: logicalName, @namespace: @namespace);
            dataType.ArrayItem = new ArrayItem
            {
                Name = memberDescription
                    .WhenNotNull(x => x.ArrayItem.Name).OtherwiseDefault() ?? 
                    typeDescription.WhenNotNull(x => x.ArrayItem.Name).OtherwiseDefault() ?? 
                    itemType.Name,
                Comments = memberDescription
                    .WhenNotNull(x => x.ArrayItem.Comments).OtherwiseDefault() ?? 
                    typeDescription.ArrayItem.WhenNotNull(x => x.Comments).OtherwiseDefault(),
                Type = itemType
            };
        }

        private void BuildComplexType(
            DataType parent,
            DataType dataType,
            Type type,
            bool requestGraph,
            IApiDescription endpoint,
            IEnumerable<Type> ancestors,
            string logicalName,
            string @namespace)
        {
            dataType.IsComplex = true;
            dataType.LogicalName = logicalName ?? dataType.Name;
            dataType.Namespace = @namespace ?? parent.LogicalName + dataType.Name;
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
                        Name = x.Description.WhenNotNull(y => y.Name).OtherwiseDefault(),
                        Comments = x.Description.WhenNotNull(y => y.Comments).OtherwiseDefault(),
                        DefaultValue = requestGraph ? x.Description
                            .WhenNotNull(y => y.DefaultValue)
                            .OtherwiseDefault() : null,
                        SampleValue = x.Description
                            .WhenNotNull(y => y.SampleValue)
                                .OtherwiseDefault(),
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