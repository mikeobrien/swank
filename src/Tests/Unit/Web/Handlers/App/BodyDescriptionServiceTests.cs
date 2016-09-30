using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Description;
using System.Xml.Serialization;
using NUnit.Framework;
using Should;
using Swank.Configuration;
using Swank.Description;
using Swank.Extensions;
using Swank.Specification;
using Swank.Web.Handlers.App;
using Tests.Common;
using Tests.Unit.Specification;

namespace Tests.Unit.Web.Handlers.App
{
    [TestFixture]
    public class BodyDescriptionServiceTests
    {
        public List<BodyDefinitionModel> BuildDescription(Type type,
            Action<Swank.Configuration.Configuration> configure = null, bool requestGraph = false)
        {
            var configuration = new Swank.Configuration.Configuration();
            configure?.Invoke(configuration);
            return new BodyDescriptionService(configuration)
                .Create(Builder.BuildTypeGraphService(configuration: configuration)
                    .BuildForMessage(requestGraph, type, new EndpointDescription { MethodName = "Get" }, 
                        new ApiDescription()));
        }

        public List<BodyDefinitionModel> BuildDescription<T>(
            Action<Swank.Configuration.Configuration> configure = null, bool requestGraph = false)
        {
            return BuildDescription(typeof(T), configure, requestGraph);
        }

        // Complex types

        [Comments("Complex type comments")]
        public class ComplexTypeWithNoMembers { }

        [Test]
        public void should_create_complex_type()
        {
            var description = BuildDescription<ComplexTypeWithNoMembers>();

            description.ShouldBeIndexed().ShouldTotal(2);

            description[0].ShouldBeComplexType("ComplexTypeWithNoMembers", 0,
                x => x.First().Opening().Comments("Complex type comments")
                    .Namespace("Get").FullNamespace("Get").LogicalName("GetResponse"));

            description[1].ShouldBeComplexType("ComplexTypeWithNoMembers", 0,
                x => x.Last().Closing());
        }

        public class ComplexTypeWithSimpleMembers
        {
            public string StringMember { get; set; }
            public bool BooleanMember { get; set; }
            public DateTime DateTimeMember { get; set; }
            public TimeSpan DurationMember { get; set; }
            public Guid UuidMember { get; set; }
            public int NumericMember { get; set; }
        }

        [Test]
        public void should_create_complex_type_with_simple_type_members()
        {
            var description = BuildDescription<ComplexTypeWithSimpleMembers>();

            description.ShouldBeIndexed().ShouldTotal(8);

            description[0].ShouldBeComplexType("ComplexTypeWithSimpleMembers", 0,
                x => x.First().Opening().Namespace("Get").FullNamespace("Get")
                    .LogicalName("GetResponse"));

            description[1].ShouldBeSimpleTypeMember("StringMember",
                "string", 1, "", x => x.IsString(), x => x.Required());
            description[2].ShouldBeSimpleTypeMember("BooleanMember",
                "boolean", 1, "false", x => x.IsBoolean(), x => x.Required());
            description[3].ShouldBeSimpleTypeMember("DateTimeMember",
                "dateTime", 1, DateTime.Now.ToString("g"), x => x.IsDateTime(), x => x.Required());
            description[4].ShouldBeSimpleTypeMember("DurationMember",
                "duration", 1, "0:00:00", x => x.IsDuration(), x => x.Required());
            description[5].ShouldBeSimpleTypeMember("UuidMember",
                "uuid", 1, "00000000-0000-0000-0000-000000000000", x => x.IsGuid(), x => x.Required());
            description[6].ShouldBeSimpleTypeMember("NumericMember",
                "int", 1, "0", x => x.IsNumeric(), x => x.IsLastMember().Required());

            description[7].ShouldBeComplexType("ComplexTypeWithSimpleMembers", 0,
                x => x.Last().Closing());
        }

        public enum Options
        {
            Option,
            [Comments("This is an option.")]
            OptionWithComments
        }

        public class ComplexTypeWithSimpleOptionMember
        {
            public Options OptionMember { get; set; }
        }

        [Test]
        [TestCase(EnumFormat.AsString, "String", "Option", "OptionWithComments")]
        [TestCase(EnumFormat.AsNumber, "Int", "0", "1")]
        public void should_create_complex_type_with_simple_option_member(
            EnumFormat format, string dataTypeName, string value1, string value2)
        {
            var description = BuildDescription<ComplexTypeWithSimpleOptionMember>(
                x => x.EnumFormat = format);

            description.ShouldBeIndexed().ShouldTotal(3);

            description[0].ShouldBeComplexType("ComplexTypeWithSimpleOptionMember", 0,
                x => x.First().Opening().Namespace("Get").FullNamespace("Get")
                    .LogicalName("GetResponse"));

            description[1].ShouldBeSimpleTypeMember("OptionMember", 
                dataTypeName.ToLower(), 1, value1,
                x => x.IsNumeric(format == EnumFormat.AsNumber)
                    .IsString(format == EnumFormat.AsString)
                    .Namespace("GetResponseOptionMember")
                    .FullNamespace("Get", "GetResponseOptionMember")
                    .Options("Options")
                        .WithOption("Option", value1)
                        .WithOptionAndComments("OptionWithComments", value2, "This is an option."),
                x => x.IsLastMember().Required());

            description[2].ShouldBeComplexType("ComplexTypeWithSimpleOptionMember", 0,
                x => x.Last().Closing());
        }

        public class ComplexTypeWithOptionalMember
        {
            public string OptionalMember { get; set; }
            public int RequiredMember { get; set; }
        }
        
        [Test]
        public void should_create_complex_type_with_optional_members()
        {
            var description = BuildDescription<ComplexTypeWithOptionalMember>(requestGraph: true);

            description.ShouldBeIndexed().ShouldTotal(4);

            description[0].ShouldBeComplexType("ComplexTypeWithOptionalMember", 0,
                x => x.First().Opening().Namespace("Get").FullNamespace("Get")
                    .LogicalName("GetRequest"));

            description[1].ShouldBeSimpleTypeMember("OptionalMember",
                "string", 1, "", x => x.IsString(),
                x => x.Optional());

            description[2].ShouldBeSimpleTypeMember("RequiredMember",
                "int", 1, "0", x => x.IsNumeric(),
                x => x.Optional().IsLastMember());

            description[3].ShouldBeComplexType("ComplexTypeWithOptionalMember", 0,
                x => x.Last().Closing());
        }

        public class ComplexTypeWithNullableMember
        {
            public int NonNullableMember { get; set; }
            public int? NullableMember { get; set; }
        }

        [Test]
        public void should_create_complex_type_with_nullable_members()
        {
            var description = BuildDescription<ComplexTypeWithNullableMember>(requestGraph: true);

            description.ShouldBeIndexed().ShouldTotal(4);

            description[0].ShouldBeComplexType("ComplexTypeWithNullableMember", 0,
                x => x.First().Opening().Namespace("Get").FullNamespace("Get")
                    .LogicalName("GetRequest"));

            description[1].ShouldBeSimpleTypeMember("NonNullableMember",
                "int", 1, "0", x => x.IsNumeric(), x => x.Optional());

            description[2].ShouldBeSimpleTypeMember("NullableMember",
                "int", 1, "0", x => x.IsNumeric().IsNullable(),
                x => x.Optional().IsLastMember());

            description[3].ShouldBeComplexType("ComplexTypeWithNullableMember", 0,
                x => x.Last().Closing());
        }

        public class ComplexTypeWithDeprecatedMember
        {
            [Obsolete("Why u no use different one??")]
            public string DeprecatedMember { get; set; }
        }

        [Test]
        public void should_create_complex_type_with_deprecated_members()
        {
            var description = BuildDescription<ComplexTypeWithDeprecatedMember>();

            description.ShouldBeIndexed().ShouldTotal(3);

            description[0].ShouldBeComplexType("ComplexTypeWithDeprecatedMember", 0,
                x => x.First().Opening().Namespace("Get").FullNamespace("Get")
                    .LogicalName("GetResponse"));

            description[1].ShouldBeSimpleTypeMember("DeprecatedMember",
                "string", 1, "", x => x.IsString(), x => x.IsLastMember().Required()
                .IsDeprecated("Why u no use different one??"));

            description[2].ShouldBeComplexType("ComplexTypeWithDeprecatedMember", 0,
                x => x.Last().Closing());
        }

        public class ComplexTypeWithDefaultValueMember
        {
            [DefaultValue("zero")]
            public string DefaultValueMember { get; set; }
        }

        [Test]
        public void should_create_complex_type_with_default_value_members()
        {
            var description = BuildDescription<ComplexTypeWithDefaultValueMember>(
                requestGraph: true);

            description.ShouldBeIndexed().ShouldTotal(3);

            description[0].ShouldBeComplexType("ComplexTypeWithDefaultValueMember", 0,
                x => x.First().Opening().Namespace("Get").FullNamespace("Get")
                    .LogicalName("GetRequest"));

            description[1].ShouldBeSimpleTypeMember("DefaultValueMember",
                "string", 1, "", x => x.IsString(),
                x => x.Default("zero").Optional().IsLastMember());

            description[2].ShouldBeComplexType("ComplexTypeWithDefaultValueMember", 0,
                x => x.Last().Closing());
        }

        public class ComplexTypeWithSampleValueMember
        {
            [SampleValue("zero")]
            public string SampleValueMember { get; set; }
        }

        [Test]
        public void should_create_complex_type_with_sample_value_members()
        {
            var description = BuildDescription<ComplexTypeWithSampleValueMember>();

            description.ShouldBeIndexed().ShouldTotal(3);

            description[0].ShouldBeComplexType("ComplexTypeWithSampleValueMember", 0,
                x => x.First().Opening().Namespace("Get").FullNamespace("Get")
                    .LogicalName("GetResponse"));

            description[1].ShouldBeSimpleTypeMember("SampleValueMember", "string", 1, 
                "zero", x => x.IsString(), x => x.IsLastMember().Required());

            description[2].ShouldBeComplexType("ComplexTypeWithSampleValueMember", 0,
                x => x.Last().Closing());
        }

        public class ComplexTypeWithArrayMembers
        {
            public List<string> ArrayMember { get; set; }
        }

        public class ComplexTypeWithArrayMembersWithCustomItemName
        {
            [XmlArrayItem("Item")]
            public List<string> ArrayMember { get; set; }
        }

        [Test]
        [TestCase(typeof(ComplexTypeWithArrayMembers), "string")]
        [TestCase(typeof(ComplexTypeWithArrayMembersWithCustomItemName), "Item")]
        public void should_create_complex_type_with_array_members(
            Type type, string itemName)
        {
            var description = BuildDescription(type);

            description.ShouldBeIndexed().ShouldTotal(5);
            description[0].ShouldBeComplexType(type.Name, 0, x =>
                x.First().Opening().Namespace("Get").FullNamespace("Get")
                    .LogicalName("GetResponse"));

            description[1].ShouldBeArrayMember("ArrayMember", 1,
                x => x.Opening().TypeName("string"), 
                x => x.IsLastMember().Required().SampleValue(""));

            description[2].ShouldBeSimpleType(itemName,
                "string", 2, "", x => x.IsString());

            description[3].ShouldBeArrayMember("ArrayMember", 1,
                x => x.Closing(), x => x.IsLastMember());

            description[4].ShouldBeComplexType(type.Name, 0,
                x => x.Last().Closing());
        }

        public class ComplexTypeWithDictionaryMember
        {
            [DictionaryDescription("Entries", "This is a dictionary.",
                "KeyName", "This is a dictionary key.",
                "This is a dictionary value.")]
            public Dictionary<string, string> DictionaryMember { get; set; }
        }

        [Test]
        public void should_create_complex_type_with_dictionary_members()
        {
            var description = BuildDescription<ComplexTypeWithDictionaryMember>();

            description.ShouldBeIndexed().ShouldTotal(5);

            description[0].ShouldBeComplexType("ComplexTypeWithDictionaryMember", 0,
                x => x.First().Opening().Namespace("Get").FullNamespace("Get")
                    .LogicalName("GetResponse"));

            description[1].ShouldBeDictionaryMember("Entries", 1, 
                x => x.Opening().TypeName("string"),
                x => x.Comments("This is a dictionary.").IsLastMember()
                    .SampleValue("").Required());

            description[2].ShouldBeSimpleTypeDictionaryEntry(
                "KeyName", "string", "string", 2, "",
                x => x.IsString().Comments("This is a dictionary value."),
                x => x.KeyComments("This is a dictionary key."));

            description[3].ShouldBeDictionaryMember("Entries", 1, x => x.Closing(),
                x => x.IsLastMember());

            description[4].ShouldBeComplexType("ComplexTypeWithDictionaryMember", 0,
                x => x.Last().Closing());
        }

        // Arrays

        [ArrayDescription("Items", "This is an array",
            "Item", "This is an array item.")]
        public class ArrayType : List<string> { }

        [Test]
        public void should_create_an_array_with_a_description()
        {
            var description = BuildDescription<ArrayType>();

            description.ShouldBeIndexed().ShouldTotal(3);

            description[0].ShouldBeArray("Items", 0,
                x => x.Comments("This is an array").First()
                    .Opening().TypeName("string"));

            description[1].ShouldBeSimpleType("Item", "string", 1, "", x => x
                .Comments("This is an array item.").IsString());

            description[2].ShouldBeArray("Items", 0,
                x => x.Last().Closing());
        }

        public enum ArrayOptions { Option1, Option2 }

        [Test]
        [TestCase(EnumFormat.AsString, "String", "Option1", "Option2")]
        [TestCase(EnumFormat.AsNumber, "Int", "0", "1")]
        public void should_create_an_array_of_options(EnumFormat format, 
            string dataTypeName, string value1, string value2)
        {
            var description = BuildDescription<List<ArrayOptions>>(
                x => x.EnumFormat = format);

            description.ShouldBeIndexed().ShouldTotal(3);

            description[0].ShouldBeArray($"ArrayOf{dataTypeName}", 0,
                x => x.First().Opening().Namespace("Get")
                    .FullNamespace("Get").TypeName(dataTypeName.ToLower())
                    .Options("ArrayOptions")
                        .WithOption("Option1", value1)
                        .WithOption("Option2", value2));

            description[1].ShouldBeSimpleType(dataTypeName.ToLower(), 
                dataTypeName.ToLower(), 1, value1,
                x => x.IsNumeric(format == EnumFormat.AsNumber)
                    .IsString(format == EnumFormat.AsString)
                    .Namespace("Get").FullNamespace("Get")
                    .Options("ArrayOptions")
                        .WithOption("Option1", value1)
                        .WithOption("Option2", value2));

            description[2].ShouldBeArray($"ArrayOf{dataTypeName}", 0,
                x => x.Last().Closing());
        }

        public class ArrayComplexType { public string Member { get; set; } }

        [Test]
        public void should_create_an_array_of_complex_types()
        {
            var description = BuildDescription<List<ArrayComplexType>>();

            description.ShouldBeIndexed().ShouldTotal(5);

            description[0].ShouldBeArray("ArrayOfArrayComplexType", 0,
                x => x.First().Opening().LogicalName("GetResponse")
                    .Namespace("Get").FullNamespace("Get"));

            description[1].ShouldBeComplexType("ArrayComplexType", 
                1, x => x.Opening().Namespace("Get").FullNamespace("Get")
                    .LogicalName("GetResponse"));

            description[2].ShouldBeSimpleTypeMember("Member", "string", 2, "",
                x => x.IsString(), x => x.IsLastMember().Required());

            description[3].ShouldBeComplexType("ArrayComplexType", 1,
                x => x.Closing());

            description[4].ShouldBeArray("ArrayOfArrayComplexType", 0,
                x => x.Last().Closing());
        }

        [Test]
        public void should_create_an_array_of_arrays()
        {
            var description = BuildDescription<List<List<string>>>();

            description.ShouldBeIndexed().ShouldTotal(5);

            description[0].ShouldBeArray("ArrayOfArrayOfString", 0,
                x => x.First().Opening());

            description[1].ShouldBeArray("ArrayOfString", 1, 
                x => x.Opening().TypeName("string"));

            description[2].ShouldBeSimpleType("string", 
                "string", 2, "", x => x.IsString());

            description[3].ShouldBeArray("ArrayOfString", 1, x => x.Closing());

            description[4].ShouldBeArray("ArrayOfArrayOfString", 
                0, x => x.Last().Closing());
        }

        [Test]
        public void should_create_an_array_of_dictionaries()
        {
            var description = BuildDescription<List<Dictionary<string, int>>>();

            description.ShouldBeIndexed().ShouldTotal(5);

            description[0].ShouldBeArray("ArrayOfDictionaryOfInt", 0,
                x => x.First().Opening());

            description[1].ShouldBeDictionary("DictionaryOfInt", 1,
                x => x.Opening().TypeName("int"));

            description[2].ShouldBeSimpleTypeDictionaryEntry(
                "key", "string", "int", 2, "0", x => x.IsNumeric());

            description[3].ShouldBeDictionary("DictionaryOfInt", 
                1, x => x.Closing());

            description[4].ShouldBeArray("ArrayOfDictionaryOfInt", 
                0, x => x.Last().Closing());
        }

        // Dictionaries

        [DictionaryDescription("Entries", "This is a dictionary.",
            "KeyName", "This is a dictionary key.",
            "This is a dictionary value.")]
        public class DictionaryType : Dictionary<string, int> { }

        [Test]
        public void should_create_a_dictionary_with_a_description()
        {
            var description = BuildDescription<DictionaryType>();

            description.ShouldBeIndexed().ShouldTotal(3);

            description[0].ShouldBeDictionary("Entries", 0, x => x
                .Comments("This is a dictionary.")
                .First().Opening().TypeName("int"));

            description[1].ShouldBeSimpleTypeDictionaryEntry(
                "KeyName", "string", "int", 1, "0",
                x => x.IsNumeric().Comments("This is a dictionary value."),
                x => x.KeyComments("This is a dictionary key."));

            description[2].ShouldBeDictionary("Entries", 
                0, x => x.Last().Closing());
        }

        public enum DictionaryKeyOptions { KeyOption1, KeyOption2 }
        public enum DictionaryValueOptions { ValueOption1, ValueOption2 }

        [Test]
        [TestCase(EnumFormat.AsString, "String", "KeyOption1", 
            "KeyOption2", "ValueOption1", "ValueOption2")]
        [TestCase(EnumFormat.AsNumber, "Int", "0", "1", "0", "1")]
        public void should_create_an_dictionary_of_string_options(
            EnumFormat format, string dataTypeName, string key1, 
            string key2, string value1, string value2)
        {
            var description = BuildDescription<Dictionary<DictionaryKeyOptions, 
                DictionaryValueOptions>>(x => x.EnumFormat = format);

            description.ShouldBeIndexed().ShouldTotal(3);

            description[0].ShouldBeDictionary($"DictionaryOf{dataTypeName}", 0,
                x => x.First().Opening().Namespace("Get")
                    .FullNamespace("Get").TypeName(dataTypeName.ToLower())
                    .Options("DictionaryValueOptions")
                        .WithOption("ValueOption1", value1)
                        .WithOption("ValueOption2", value2));

            description[1].ShouldBeSimpleTypeDictionaryEntry(
                "key", dataTypeName.ToLower(),
                dataTypeName.ToLower(), 1, value1,
                x => x.IsNumeric(format == EnumFormat.AsNumber)
                    .IsString(format == EnumFormat.AsString)
                    .Namespace("Get").FullNamespace("Get")
                    .Options("DictionaryValueOptions")
                        .WithOption("ValueOption1", value1)
                        .WithOption("ValueOption2", value2),
                x => x.KeyOptions("DictionaryKeyOptions")
                    .WithOption("KeyOption1", key1)
                    .WithOption("KeyOption2", key2));

            description[2].ShouldBeDictionary($"DictionaryOf{dataTypeName}", 
                0, x => x.Last().Closing());
        }

        public class DictionaryComplexType { public string Member { get; set; } }

        [Test]
        public void should_create_a_dictionary_of_complex_types()
        {
            var description = BuildDescription<Dictionary<string, DictionaryComplexType>>();

            description.ShouldBeIndexed().ShouldTotal(5);

            description[0].ShouldBeDictionary("DictionaryOfDictionaryComplexType", 0,
                x => x.First().Opening().LogicalName("GetResponse")
                    .Namespace("Get").FullNamespace("Get"));

            description[1].ShouldBeOpeningComplexTypeDictionaryEntry(
                "key", "string", 1, x => x.Namespace("Get")
                    .FullNamespace("Get").LogicalName("GetResponse"));

            description[2].ShouldBeSimpleTypeMember("Member", "string", 2, "",
                x => x.IsString(), x => x.IsLastMember().Required());

            description[3].ShouldBeClosingComplexTypeDictionaryEntry("key", 1);

            description[4].ShouldBeDictionary("DictionaryOfDictionaryComplexType", 
                0, x => x.Last().Closing());
        }

        [Test]
        public void should_create_a_dictionary_of_arrays()
        {
            var description = BuildDescription<Dictionary<string, List<int>>>();

            description.ShouldBeIndexed().ShouldTotal(5);

            description[0].ShouldBeDictionary("DictionaryOfArrayOfInt", 0,
                x => x.First().Opening());

            description[1].ShouldBeOpeningArrayDictionaryEntry(
                "key", "string", 1, x => x.TypeName("int"));

            description[2].ShouldBeSimpleType("int", "int", 2, "0", x => x.IsNumeric());

            description[3].ShouldBeClosingArrayDictionaryEntry("key", 1);

            description[4].ShouldBeDictionary("DictionaryOfArrayOfInt", 
                0, x => x.Last().Closing());
        }

        [Test]
        public void should_create_a_dictionary_of_dictionaries()
        {
            var description = BuildDescription<Dictionary<string, Dictionary<string, int>>>();

            description.ShouldBeIndexed().ShouldTotal(5);

            description[0].ShouldBeDictionary("DictionaryOfDictionaryOfInt", 0,
                x => x.First().Opening());

            description[1].ShouldBeOpeningDictionaryDictionaryEntry(
                "key", "string", 1, x => x.TypeName("int"));

            description[2].ShouldBeSimpleTypeDictionaryEntry(
                "key", "string", "int", 2, "0", x => x.IsNumeric());

            description[3].ShouldBeClosingDictionaryDictionaryEntry("key", 1);

            description[4].ShouldBeDictionary("DictionaryOfDictionaryOfInt", 
                0, x => x.Last().Closing());
        }
    }

    public static class LineItemDescriptionAssertions
    {
        // Simple type assertions

        public static void ShouldBeSimpleType(this BodyDefinitionModel source,
            string name, string typeName, int level, string sampleValue,
            Action<SimpleTypeDsl> simpleTypeProperties)
        {
            source.ShouldMatchLineItem(CreateSimpleType(name, typeName,
                level, sampleValue, simpleTypeProperties));
        }

        public static void ShouldBeSimpleTypeMember(this BodyDefinitionModel source,
            string name, string typeName, int level, string sampleValue,
            Action<SimpleTypeDsl> simpleTypeProperties,
            Action<MemberDsl> memberProperties = null)
        {
            var compare = CreateSimpleType(name, typeName,
                level, sampleValue, simpleTypeProperties);
            compare.IsMember = true;
            memberProperties?.Invoke(new MemberDsl(compare));
            source.ShouldMatchLineItem(compare);
        }

        public static void ShouldBeSimpleTypeDictionaryEntry(this BodyDefinitionModel source,
            string name, string keyTypeName, string valueTypeName, int level, 
            string sampleValue, Action<SimpleTypeDsl> simpleTypeProperties,
            Action<DictionaryKeyDsl> dictionaryEntryProperties = null)
        {
            var compare = CreateSimpleType(name, valueTypeName,
                level, sampleValue, simpleTypeProperties);
            compare.IsDictionaryEntry = true;
            compare.DictionaryKey = new KeyModel { TypeName = keyTypeName };
            dictionaryEntryProperties?.Invoke(new DictionaryKeyDsl(compare));
            source.ShouldMatchLineItem(compare);
        }

        private static BodyDefinitionModel CreateSimpleType(
            string name, string typeName, int level, string sampleValue,
            Action<SimpleTypeDsl> simpleTypeProperties)
        {
            var simpleType = new BodyDefinitionModel
            {
                Name = name,
                TypeName = typeName,
                IsSimpleType = true,
                SampleValue = sampleValue,
                Whitespace = BodyDescriptionService.Whitespace.Repeat(level)
            };
            simpleTypeProperties(new SimpleTypeDsl(simpleType));
            return simpleType;
        }

        public class SimpleTypeDsl
        {
            private readonly BodyDefinitionModel _body;
            public SimpleTypeDsl(BodyDefinitionModel body) { _body = body; }

            public SimpleTypeDsl Comments(string comments)
            {
                _body.Comments = comments; return this;
            }

            public SimpleTypeDsl IsNullable() { _body.Nullable = true; return this; }

            public SimpleTypeDsl IsString() { _body.IsString = true; return this; }

            public SimpleTypeDsl IsString(bool isString)
            {
                if (isString) _body.IsString = true; return this;
            }

            public SimpleTypeDsl IsBoolean() { _body.IsBoolean = true; return this; }
            public SimpleTypeDsl IsNumeric() { _body.IsNumeric = true; return this; }

            public SimpleTypeDsl IsNumeric(bool isNumeric)
            {
                if (isNumeric) _body.IsNumeric = true; return this;
            }

            public SimpleTypeDsl IsDateTime() { _body.IsDateTime = true; return this; }
            public SimpleTypeDsl IsDuration() { _body.IsDuration = true; return this; }
            public SimpleTypeDsl IsGuid() { _body.IsGuid = true; return this; }

            public OptionDsl Options(string name)
            {
                _body.Enumeration = _body.Enumeration ?? new Enumeration();
                _body.Enumeration.Name = name;
                return new OptionDsl(_body.Enumeration);
            }

            public SimpleTypeDsl LogicalName(string logicalName)
            {
                _body.LogicalName = logicalName; return this;
            }

            public SimpleTypeDsl Namespace(string @namespace)
            {
                _body.Namespace = @namespace; return this;
            }

            public SimpleTypeDsl FullNamespace(params string[] @namespace)
            {
                _body.FullNamespace = @namespace.ToList(); return this;
            }
        }

        // Array assertions

        public static void ShouldBeArray(
            this BodyDefinitionModel source, string name, int level,
            Action<ArrayDsl> properties)
        {
            source.ShouldMatchLineItem(CreateArray(name, level, properties));
        }

        public static void ShouldBeArrayMember(
            this BodyDefinitionModel source, string name, int level,
            Action<ArrayDsl> arrayProperties,
            Action<MemberDsl> memberProperties = null)
        {
            var compare = CreateArray(name, level, arrayProperties);
            compare.IsMember = true;
            memberProperties?.Invoke(new MemberDsl(compare));
            source.ShouldMatchLineItem(compare);
        }

        public static void ShouldBeOpeningArrayDictionaryEntry(
            this BodyDefinitionModel source, string name, string keyTypeName, int level,
            Action<ArrayDsl> arrayProperties = null,
            Action<DictionaryKeyDsl> dictionaryKeyProperties = null)
        {
            var compare = CreateArray(name, level, arrayProperties);
            compare.IsOpening = true;
            compare.IsDictionaryEntry = true;
            compare.DictionaryKey = new KeyModel { TypeName = keyTypeName };
            dictionaryKeyProperties?.Invoke(new DictionaryKeyDsl(compare));
            source.ShouldMatchLineItem(compare);
        }

        public static void ShouldBeClosingArrayDictionaryEntry(
            this BodyDefinitionModel source, string name, int level,
            Action<ArrayDsl> arrayProperties = null)
        {
            var compare = CreateArray(name, level, arrayProperties);
            compare.IsClosing = true;
            compare.IsDictionaryEntry = true;
            source.ShouldMatchLineItem(compare);
        }

        private static BodyDefinitionModel CreateArray(
            string name, int level, Action<ArrayDsl> properties)
        {
            var arrayType = new BodyDefinitionModel
            {
                Name = name,
                IsArray = true,
                Whitespace = BodyDescriptionService.Whitespace.Repeat(level)
            };
            properties?.Invoke(new ArrayDsl(arrayType));
            return arrayType;
        }

        public class ArrayDsl
        {
            private readonly BodyDefinitionModel _body;
            public ArrayDsl(BodyDefinitionModel body) { _body = body; }

            public ArrayDsl Comments(string comments)
            {
                _body.Comments = comments; return this;
            }

            public ArrayDsl Opening() { _body.IsOpening = true; return this; }
            public ArrayDsl Closing() { _body.IsClosing = true; return this; }
            public ArrayDsl First() { _body.IsFirst = true; return this; }
            public ArrayDsl Last() { _body.IsLast = true; return this; }

            public OptionDsl Options(string name)
            {
                _body.Enumeration = _body.Enumeration ?? new Enumeration();
                _body.Enumeration.Name = name;
                return new OptionDsl(_body.Enumeration);
            }

            public ArrayDsl TypeName(string typeName)
            {
                _body.TypeName = typeName; return this;
            }

            public ArrayDsl LogicalName(string logicalName)
            {
                _body.LogicalName = logicalName; return this;
            }

            public ArrayDsl Namespace(string @namespace)
            {
                _body.Namespace = @namespace; return this;
            }

            public ArrayDsl FullNamespace(params string[] @namespace)
            {
                _body.FullNamespace = @namespace.ToList(); return this;
            }
        }

        // Dictionary assertions

        public static void ShouldBeDictionary(
            this BodyDefinitionModel source, string name, int level,
            Action<DictionaryDsl> properties)
        {
            source.ShouldMatchLineItem(CreateDictionary(name, level, properties));
        }

        public static void ShouldBeDictionaryMember(
            this BodyDefinitionModel source, string name, int level,
            Action<DictionaryDsl> dictionaryProperties,
            Action<MemberDsl> memberProperties = null)
        {
            var compare = CreateDictionary(name, level, dictionaryProperties);
            compare.IsMember = true;
            memberProperties?.Invoke(new MemberDsl(compare));
            source.ShouldMatchLineItem(compare);
        }

        public static void ShouldBeDictionaryDictionaryEntry(
            this BodyDefinitionModel source, string name, string keyTypeName, int level,
            Action<DictionaryDsl> dictionaryProperties,
            Action<DictionaryKeyDsl> dictionaryKeyProperties = null)
        {
            var compare = CreateDictionary(name, level, dictionaryProperties);
            compare.IsDictionaryEntry = true;
            compare.DictionaryKey = new KeyModel { TypeName = keyTypeName };
            dictionaryKeyProperties?.Invoke(new DictionaryKeyDsl(compare));
            source.ShouldMatchLineItem(compare);
        }

        public static void ShouldBeOpeningDictionaryDictionaryEntry(
            this BodyDefinitionModel source, string name, string keyTypeName, int level,
            Action<DictionaryDsl> dictionaryProperties = null,
            Action<DictionaryKeyDsl> dictionaryKeyProperties = null)
        {
            var compare = CreateDictionary(name, level, dictionaryProperties);
            compare.IsOpening = true;
            compare.IsDictionaryEntry = true;
            compare.DictionaryKey = new KeyModel { TypeName = keyTypeName };
            dictionaryKeyProperties?.Invoke(new DictionaryKeyDsl(compare));
            source.ShouldMatchLineItem(compare);
        }

        public static void ShouldBeClosingDictionaryDictionaryEntry(
            this BodyDefinitionModel source, string name, int level,
            Action<DictionaryDsl> dictionaryKeyProperties = null)
        {
            var compare = CreateDictionary(name, level, dictionaryKeyProperties);
            compare.IsClosing = true;
            compare.IsDictionaryEntry = true;
            source.ShouldMatchLineItem(compare);
        }

        private static BodyDefinitionModel CreateDictionary(
            string name, int level, Action<DictionaryDsl> properties)
        {
            var dictionaryType = new BodyDefinitionModel
            {
                Name = name,
                IsDictionary = true,
                Whitespace = BodyDescriptionService.Whitespace.Repeat(level)
            };
            properties?.Invoke(new DictionaryDsl(dictionaryType));
            return dictionaryType;
        }

        public class DictionaryDsl
        {
            private readonly BodyDefinitionModel _body;
            public DictionaryDsl(BodyDefinitionModel body) { _body = body; }

            public DictionaryDsl Comments(string comments)
            {
                _body.Comments = comments; return this;
            }

            public DictionaryDsl Opening() { _body.IsOpening = true; return this; }
            public DictionaryDsl Closing() { _body.IsClosing = true; return this; }
            public DictionaryDsl First() { _body.IsFirst = true; return this; }
            public DictionaryDsl Last() { _body.IsLast = true; return this; }

            public OptionDsl Options(string name)
            {
                _body.Enumeration = _body.Enumeration ?? new Enumeration();
                _body.Enumeration.Name = name;
                return new OptionDsl(_body.Enumeration);
            }

            public DictionaryDsl TypeName(string typeName)
            {
                _body.TypeName = typeName; return this;
            }

            public DictionaryDsl LogicalName(string logicalName)
            {
                _body.LogicalName = logicalName; return this;
            }

            public DictionaryDsl Namespace(string @namespace)
            {
                _body.Namespace = @namespace; return this;
            }

            public DictionaryDsl FullNamespace(params string[] @namespace)
            {
                _body.FullNamespace = @namespace.ToList(); return this;
            }
        }

        // Complex type assertions

        public static void ShouldBeComplexType(this BodyDefinitionModel source,
            string name, int level, Action<ComplexTypeDsl> properties)
        {
            source.ShouldMatchLineItem(CreateComplexType(name, level, properties));
        }

        public static void ShouldBeComplexTypeMember(
            this BodyDefinitionModel source, string name, int level,
            Action<ComplexTypeDsl> complexTypeProperties = null,
            Action<MemberDsl> memberProperties = null)
        {
            var compare = CreateComplexType(name, level, complexTypeProperties);
            compare.IsMember = true;
            memberProperties?.Invoke(new MemberDsl(compare));
            source.ShouldMatchLineItem(compare);
        }

        public static void ShouldBeOpeningComplexTypeDictionaryEntry(
            this BodyDefinitionModel source, string name, string keyTypeName, int level,
            Action<ComplexTypeDsl> complexTypeProperties = null,
            Action<DictionaryKeyDsl> dictionaryKeyProperties = null)
        {
            var compare = CreateComplexType(name, level, complexTypeProperties);
            compare.IsOpening = true;
            compare.IsDictionaryEntry = true;
            compare.DictionaryKey = new KeyModel { TypeName = keyTypeName };
            dictionaryKeyProperties?.Invoke(new DictionaryKeyDsl(compare));
            source.ShouldMatchLineItem(compare);
        }

        public static void ShouldBeClosingComplexTypeDictionaryEntry(
            this BodyDefinitionModel source, string name, int level,
            Action<ComplexTypeDsl> complexTypeProperties = null)
        {
            var compare = CreateComplexType(name, level, complexTypeProperties);
            compare.IsClosing = true;
            compare.IsDictionaryEntry = true;
            source.ShouldMatchLineItem(compare);
        }

        private static BodyDefinitionModel CreateComplexType(
            string name, int level, Action<ComplexTypeDsl> properties = null)
        {
            var complexType = new BodyDefinitionModel
            {
                Name = name,
                IsComplexType = true,
                Whitespace = BodyDescriptionService.Whitespace.Repeat(level)
            };
            properties?.Invoke(new ComplexTypeDsl(complexType));
            return complexType;
        }

        public class ComplexTypeDsl
        {
            private readonly BodyDefinitionModel _body;
            public ComplexTypeDsl(BodyDefinitionModel body) { _body = body; }

            public ComplexTypeDsl Comments(string comments)
            {
                _body.Comments = comments; return this;
            }

            public ComplexTypeDsl Opening() { _body.IsOpening = true; return this; }
            public ComplexTypeDsl Closing() { _body.IsClosing = true; return this; }
            public ComplexTypeDsl First() { _body.IsFirst = true; return this; }
            public ComplexTypeDsl Last() { _body.IsLast = true; return this; }

            public ComplexTypeDsl LogicalName(string logicalName)
            {
                _body.LogicalName = logicalName; return this;
            }

            public ComplexTypeDsl Namespace(string @namespace)
            {
                _body.Namespace = @namespace; return this;
            }

            public ComplexTypeDsl FullNamespace(params string[] @namespace)
            {
                _body.FullNamespace = @namespace.ToList(); return this;
            }
        }

        // Common assertion DSLs

        public class DictionaryKeyDsl
        {
            private readonly KeyModel _key;

            public DictionaryKeyDsl(BodyDefinitionModel body)
            {
                _key = body.DictionaryKey;
            }

            public DictionaryKeyDsl KeyComments(string comments)
            {
                _key.Comments = comments; return this;
            }

            public OptionDsl KeyOptions(string name)
            {
                _key.Enumeration = _key.Enumeration ?? new Enumeration();
                _key.Enumeration.Name = name;
                return new OptionDsl(_key.Enumeration);
            }
        }

        public class MemberDsl
        {
            private readonly BodyDefinitionModel _body;
            public MemberDsl(BodyDefinitionModel body) { _body = body; }

            public MemberDsl Comments(string comments)
            {
                _body.Comments = comments; return this;
            }

            public MemberDsl SampleValue(string sampleValue)
            {
                _body.SampleValue = sampleValue; return this;
            }

            public MemberDsl Default(string value) { _body.DefaultValue = value; return this; }
            public MemberDsl Required() { _body.Optional = false; return this; }
            public MemberDsl Optional() { _body.Optional = true; return this; }
            public MemberDsl IsLastMember() { _body.IsLastMember = true; return this; }

            public MemberDsl IsDeprecated(string message = null)
            {
                _body.IsDeprecated = true;
                _body.DeprecationMessage = message;
                return this;
            }
        }

        public class OptionDsl
        {
            private readonly Enumeration _options;
            public OptionDsl(Enumeration options) { _options = options; }

            public OptionDsl WithOption(string value)
            {
                return WithOption(new Option { Value = value });
            }

            public OptionDsl WithOption(string name, string value)
            {
                return WithOption(new Option { Name = name, Value = value });
            }

            public OptionDsl WithOptionAndComments(string value, string comments)
            {
                return WithOption(new Option { Value = value, Comments = comments });
            }

            public OptionDsl WithOptionAndComments(string name, string value, string comments)
            {
                return WithOption(new Option { Name = name, Value = value, Comments = comments });
            }

            private OptionDsl WithOption(Option option)
            {
                if (_options.Options == null) _options.Options = new List<Option>();
                _options.Options.Add(option); return this;
            }
        }

        // Common assertions

        public static List<BodyDefinitionModel> ShouldBeIndexed(this List<BodyDefinitionModel> source)
        {
            Enumerable.Range(0, source.Count).ForEach(x => source[x].Index.ShouldEqual(x + 1));
            return source;
        }

        private static void ShouldMatchLineItem(this BodyDefinitionModel source, BodyDefinitionModel compare)
        {
            source.Name.ShouldEqual(compare.Name);
            source.Comments.ShouldEqual(compare.Comments);
            source.LogicalName.ShouldEqual(compare.LogicalName);
            source.Namespace.ShouldEqual(compare.Namespace);
            source.FullNamespace.ShouldOnlyContain(compare.FullNamespace?.ToArray());
            source.IsFirst.ShouldEqual(compare.IsFirst);
            source.IsLast.ShouldEqual(compare.IsLast);
            source.TypeName.ShouldEqual(compare.TypeName);
            source.SampleValue.ShouldEqual(compare.SampleValue);
            source.DefaultValue.ShouldEqual(compare.DefaultValue);
            source.Optional.ShouldEqual(compare.Optional);
            source.Nullable.ShouldEqual(compare.Nullable);
            source.Whitespace.ShouldEqual(compare.Whitespace);
            source.IsDeprecated.ShouldEqual(compare.IsDeprecated);
            source.DeprecationMessage.ShouldEqual(compare.DeprecationMessage);

            source.IsOpening.ShouldEqual(compare.IsOpening);
            source.IsClosing.ShouldEqual(compare.IsClosing);

            source.IsMember.ShouldEqual(compare.IsMember);
            source.IsLastMember.ShouldEqual(compare.IsLastMember);

            source.IsSimpleType.ShouldEqual(compare.IsSimpleType);
            source.IsString.ShouldEqual(compare.IsString);
            source.IsBoolean.ShouldEqual(compare.IsBoolean);
            source.IsNumeric.ShouldEqual(compare.IsNumeric);
            source.IsDateTime.ShouldEqual(compare.IsDateTime);
            source.IsDuration.ShouldEqual(compare.IsDuration);
            source.IsGuid.ShouldEqual(compare.IsGuid);
            source.Enumeration.ShouldEqualOptions(compare.Enumeration);

            source.IsComplexType.ShouldEqual(compare.IsComplexType);

            source.IsArray.ShouldEqual(compare.IsArray);

            source.IsDictionary.ShouldEqual(compare.IsDictionary);
            source.IsDictionaryEntry.ShouldEqual(compare.IsDictionaryEntry);

            if (compare.DictionaryKey == null) source.DictionaryKey.ShouldBeNull();
            else
            {
                source.DictionaryKey.TypeName.ShouldEqual(compare.DictionaryKey.TypeName);
                source.DictionaryKey.Comments.ShouldEqual(compare.DictionaryKey.Comments);
                source.DictionaryKey.Enumeration.ShouldEqualOptions(compare.DictionaryKey.Enumeration);
            }
        }

        private static void ShouldEqualOptions(this Enumeration source, Enumeration compare)
        {
            if (compare == null) source.ShouldBeNull();
            else
            {
                source.Name.ShouldEqual(compare.Name);
                source.Comments.ShouldEqual(compare.Comments);
                source.Options.ShouldTotal(compare.Options.Count);
                foreach (var option in source.Options.Zip(compare.Options,
                    (s, c) => new { Source = s, Compare = c }))
                {
                    option.Source.Name.ShouldEqual(option.Compare.Name);
                    option.Source.Comments.ShouldEqual(option.Compare.Comments);
                    option.Source.Value.ShouldEqual(option.Compare.Value);
                }
            }
        }
    }
}
