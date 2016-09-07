using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Web.Http.Description;
using Swank.Description;
using Swank.Extensions;

namespace Swank.Specification
{
    public class TypeGraphFactory
    {
        private readonly Configuration.Configuration _configuration;
        private readonly IDescriptionConvention<PropertyInfo, MemberDescription> _memberConvention;
        private readonly OptionFactory _optionFactory;
        private readonly IDescriptionConvention<Type, TypeDescription> _typeConvention;

        public TypeGraphFactory(
            Configuration.Configuration configuration,
            IDescriptionConvention<Type, TypeDescription> typeConvention,
            IDescriptionConvention<PropertyInfo, MemberDescription> memberConvention,
            OptionFactory optionFactory)
        {
            _configuration = configuration;
            _memberConvention = memberConvention;
            _optionFactory = optionFactory;
            _typeConvention = typeConvention;
        }

        public DataType BuildGraph(Type type, bool requestGraph, ApiDescription endpoint)
        {
            var dataType = BuildGraph(null, type, requestGraph, endpoint, null);
            GenerateShortNamespaces(dataType);
            return dataType;
        }

        private DataType BuildGraph(
            DataType parent,
            Type type,
            bool requestGraph,
            ApiDescription endpoint,
            IEnumerable<Type> ancestors,
            MemberDescription memberDescription = null)
        {
            var description = _typeConvention.GetDescription(type);

            var dataType = new DataType
            {
                Name = !type.IsSimpleType() && memberDescription != null ?
                    memberDescription.Name : description.Name,
                LongNamespace = parent.MapOrDefault(x => x.LongNamespace
                    .Concat(x.Name).ToList(), new List<string>()),
                ShortNamespace = new List<string>(),
                Comments = description.Comments
            };

            if (type.IsDictionary())
                BuildDictionary(dataType, type, description, 
                    requestGraph, endpoint, ancestors, memberDescription);
            else if (type.IsArray || type.IsList())
                BuildArray(dataType, type, description, requestGraph, 
                    endpoint, ancestors, memberDescription);
            else if (type.IsSimpleType()) BuildSimpleType(dataType, type, requestGraph);
            else BuildComplexType(dataType, type, requestGraph, endpoint, ancestors);

            return _configuration.TypeOverrides.Apply(new TypeOverrideContext
            {
                Type = type,
                DataType = dataType,
                ApiDescription = endpoint,
                Description = description,
                IsRequest = requestGraph
            }).DataType;
        }

        private void BuildDictionary(
            DataType dataType,
            Type type,
            TypeDescription typeDescription,
            bool requestGraph,
            ApiDescription endpoint,
            IEnumerable<Type> ancestors,
            MemberDescription memberDescription)
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
                    typeDescription.WhenNotNull(x => x.DictionaryEntry.KeyComments).OtherwiseDefault(),
                KeyType = BuildGraph(dataType, types.Key, requestGraph, endpoint, ancestors),
                ValueComments = memberDescription
                    .WhenNotNull(x => x.DictionaryEntry.ValueComments).OtherwiseDefault() ??
                    typeDescription.WhenNotNull(x => x.DictionaryEntry.ValueComments).OtherwiseDefault(),
                ValueType = BuildGraph(dataType, types.Value, requestGraph, endpoint, ancestors)
            };
        }

        private void BuildArray(
            DataType dataType,
            Type type,
            TypeDescription typeDescription,
            bool requestGraph,
            ApiDescription endpoint,
            IEnumerable<Type> ancestors,
            MemberDescription memberDescription)
        {
            dataType.IsArray = true;
            dataType.Comments = memberDescription.WhenNotNull(x => x.Comments)
                .OtherwiseDefault() ?? dataType.Comments;
            var itemType = BuildGraph(dataType, type.GetListElementType(), 
                requestGraph, endpoint, ancestors);
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

        private void BuildSimpleType(DataType dataType, Type type, bool requestGraph)
        {
            dataType.IsSimple = true;
            dataType.LongNamespace.Clear();
            dataType.ShortNamespace.Clear();
            if (type.GetNullableUnderlyingType().IsEnum)
                dataType.Options = _optionFactory.BuildOptions(type, null, requestGraph);
        }

        private void BuildComplexType(
            DataType dataType,
            Type type,
            bool requestGraph,
            ApiDescription endpoint,
            IEnumerable<Type> ancestors)
        {
            dataType.IsComplex = true;
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
                        Required = requestGraph && x.Description
                            .WhenNotNull(y => y.Optional.IsRequired(endpoint.HttpMethod))
                            .OtherwiseDefault(),
                        Optional = requestGraph && x.Description
                            .WhenNotNull(y => y.Optional.IsOptional(endpoint.HttpMethod))
                            .OtherwiseDefault(),
                        Deprecated = x.Description.Deprecated,
                        DeprecationMessage = x.Description.DeprecationMessage,
                        Type = BuildGraph(dataType, x.Type, requestGraph, endpoint,
                            x.Ancestors, x.Description)
                    }
                }).Member).ToList();
        }

        private static void GenerateShortNamespaces(DataType type)
        {
            type.TraverseMany(GetTypeChildTypes)
                .GroupBy(x => x.Name)
                .Where(x => x.Count() > 1)
                .ForEach(x => x.ShrinkMultipartKeyRight(y => y.LongNamespace, (t, k) =>
                    t.ShortNamespace = k.EndsWith(t.Name) ? k.Shorten(1).ToList() : k));
        }

        private static IEnumerable<DataType> GetTypeChildTypes(DataType type)
        {
            if (type.Members != null)
                foreach (var childType in type.Members.Select(y => y.Type))
                    yield return childType;
            if (type.ArrayItem != null) yield return type.ArrayItem.Type;
            if (type.DictionaryEntry != null)
            {
                yield return type.DictionaryEntry.KeyType;
                yield return type.DictionaryEntry.ValueType;
            }
        }
    }

    public static class TypeGraphFactoryExtensions
    {
        public static bool IsRequired(this OptionalScope optional, HttpMethod method)
        {
            return optional == OptionalScope.None ||
                (optional == OptionalScope.Post && method != HttpMethod.Post) ||
                (optional == OptionalScope.Put && method != HttpMethod.Put) ||
                (optional == OptionalScope.AllButPost && method == HttpMethod.Post) ||
                (optional == OptionalScope.AllButPut && method == HttpMethod.Put);
        }

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