using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Description;
using Bender.Reflection;
using Swank.Description;
using Swank.Specification;
using NUnit.Framework;
using Should;
using Swank.Configuration;
using Tests.Common;

namespace Tests.Unit.Specification
{
    [TestFixture]
    public class TypeGraphServiceTests
    {
        private readonly EndpointDescription _endpointDescription = 
            new EndpointDescription { MethodName = "Method" };
        public class TypeWithoutComments { }

        [Test]
        public void should_create_type_without_comments()
        {
            var type = Builder.BuildTypeGraphService().BuildForMessage(false, 
                typeof(TypeWithoutComments), _endpointDescription, null);

            type.Name.ShouldEqual("TypeWithoutComments");
            type.Comments.ShouldBeNull();
        }

        [Comments("This is **a** type.")]
        public class TypeWithComments { }

        [Test]
        public void should_create_type_with_comments()
        {
            var type = Builder.BuildTypeGraphService().BuildForMessage(false, 
                typeof(TypeWithComments), _endpointDescription, null);

            type.Name.ShouldEqual("TypeWithComments");
            type.Comments.ShouldEqual("This is **a** type.");
        }

        [Test]
        public void should_override_type()
        {
            var type = Builder.BuildTypeGraphService(x => x.TypeOverrides.Add(t =>
            {
                t.DataType.Name += t.Type.Name;
                t.DataType.Comments += t.Type.Name;
            })).BuildForMessage<TypeWithComments>();

            type.Name.ShouldEqual("TypeWithCommentsTypeWithComments");
            type.Comments.ShouldEqual("This is **a** type.TypeWithComments");
        }

        public class TypeWithOverrodeMember
        {
            [Comments("This is a **member**.")]
            public string Member { get; set; }
        }

        [Test]
        public void should_override_type_members()
        {
            var type = Builder.BuildTypeGraphService(x => x.MemberOverrides.Add(m =>
            {
                m.Member.Name += m.Property.Name;
                m.Member.Comments += m.Property.Name;
            })).BuildForMessage<TypeWithOverrodeMember>();

            var member = type.Members.Single();
            member.Name.ShouldEqual("MemberMember");
            member.Comments.ShouldEqual("This is a **member**.Member");
        }

        // Simple types

        [Test]
        [TestCase(typeof(string), "string")]
        [TestCase(typeof(int), "int")]
        [TestCase(typeof(int?), "int")]
        [TestCase(typeof(Guid), "uuid")]
        [TestCase(typeof(TimeSpan), "duration")]
        [TestCase(typeof(DateTime), "dateTime")]
        [TestCase(typeof(Uri), "anyURI")]
        public void should_create_simple_message_type(Type type, string name)
        {
            should_be_simple_type(Builder.BuildTypeGraphService()
                .BuildForMessage(false, type, _endpointDescription, null), type, name);
        }

        [Test]
        [TestCase(typeof(string), "string")]
        [TestCase(typeof(int), "int")]
        [TestCase(typeof(int?), "int")]
        [TestCase(typeof(Guid), "uuid")]
        [TestCase(typeof(TimeSpan), "duration")]
        [TestCase(typeof(DateTime), "dateTime")]
        [TestCase(typeof(Uri), "anyURI")]
        public void should_create_simple_parameter_type(Type type, string name)
        {
            should_be_simple_type(Builder.BuildTypeGraphService()
                .BuildForParameter(type, "parameter", name), type, name);
        }

        public class SimpleTypeMember
        {
            public int Member { get; set; }
        }

        [Test]
        public void should_create_simple_type_member()
        {
            should_be_simple_type(Builder.BuildTypeGraphService().BuildForMessage(
                false, typeof(SimpleTypeMember), _endpointDescription, null)
                    .Members.Single().Type, typeof(int), "int");
        }

        public void should_be_simple_type(DataType dataType, Type type, string name)
        {
            dataType.Name.ShouldEqual(name);
            dataType.LogicalName.ShouldBeNull();
            dataType.Namespace.ShouldBeNull();
            dataType.FullNamespace.ShouldBeNull();
            dataType.Comments.ShouldBeNull();
            dataType.IsNullable.ShouldEqual(type.IsNullable());

            dataType.IsSimple.ShouldBeTrue();
            dataType.Enumeration.ShouldBeNull();

            dataType.IsComplex.ShouldBeFalse();
            dataType.Members.ShouldBeNull();

            dataType.IsArray.ShouldBeFalse();
            dataType.ArrayItem.ShouldBeNull();

            dataType.IsDictionary.ShouldBeFalse();
            dataType.DictionaryEntry.ShouldBeNull();
        }

        public enum Options
        {
            Option,
            [Comments("This is *an* option.")]
            OptionWithComments
        }

        [Test]
        [TestCase(typeof(Options), EnumFormat.AsString, "string", "Option", "OptionWithComments")]
        [TestCase(typeof(Options?), EnumFormat.AsString, "string", "Option", "OptionWithComments")]
        [TestCase(typeof(Options), EnumFormat.AsNumber, "int", "0", "1")]
        [TestCase(typeof(Options?), EnumFormat.AsNumber, "int", "0", "1")]
        public void should_create_message_simple_type_string_options(
            Type type, EnumFormat format, string dataTypeName, string value1, string value2)
        {
            var dataType = Builder.BuildTypeGraphService(x => x.EnumFormat = format)
                .BuildForMessage(false, type, _endpointDescription, null);

            dataType.Name.ShouldEqual(dataTypeName);
            dataType.IsSimple.ShouldBeTrue();
            dataType.Enumeration.Options.Count.ShouldEqual(2);

            var option = dataType.Enumeration.Options[0];
            option.Name.ShouldEqual("Option");
            option.Value.ShouldEqual(value1);
            option.Comments.ShouldBeNull();

            option = dataType.Enumeration.Options[1];
            option.Name.ShouldEqual("OptionWithComments");
            option.Value.ShouldEqual(value2);
            option.Comments.ShouldEqual("This is *an* option.");
        }

        [Test]
        public void should_create_message_simple_type_numeric_options(
            [Values(typeof(Options), typeof(Options?))]Type type)
        {
            var dataType = Builder.BuildTypeGraphService(
                    x => x.EnumFormat = EnumFormat.AsString)
                .BuildForMessage(false, type, _endpointDescription, null);

            dataType.Name.ShouldEqual("string");
            dataType.IsSimple.ShouldBeTrue();
            dataType.Enumeration.Options.Count.ShouldEqual(2);

            var option = dataType.Enumeration.Options[0];
            option.Name.ShouldEqual("Option");
            option.Value.ShouldEqual("Option");
            option.Comments.ShouldBeNull();

            option = dataType.Enumeration.Options[1];
            option.Name.ShouldEqual("OptionWithComments");
            option.Value.ShouldEqual("OptionWithComments");
            option.Comments.ShouldEqual("This is *an* option.");
        }

        [Test]
        [TestCase(typeof(Options), EnumFormat.AsString, "string", "Option", "OptionWithComments")]
        [TestCase(typeof(Options?), EnumFormat.AsString, "string", "Option", "OptionWithComments")]
        [TestCase(typeof(Options), EnumFormat.AsNumber, "int", "0", "1")]
        [TestCase(typeof(Options?), EnumFormat.AsNumber, "int", "0", "1")]
        public void should_create_parameter_simple_type_string_options(Type type,
            EnumFormat format, string dataTypeName, string value1, string value2)
        {
            var dataType = Builder.BuildTypeGraphService(x => x.EnumFormat = format)
                .BuildForParameter(type, "options", "Get");

            dataType.Name.ShouldEqual(dataTypeName);
            dataType.IsSimple.ShouldBeTrue();
            dataType.Enumeration.Options.Count.ShouldEqual(2);

            var option = dataType.Enumeration.Options[0];
            option.Name.ShouldEqual("Option");
            option.Value.ShouldEqual(value1);
            option.Comments.ShouldBeNull();

            option = dataType.Enumeration.Options[1];
            option.Name.ShouldEqual("OptionWithComments");
            option.Value.ShouldEqual(value2);
            option.Comments.ShouldEqual("This is *an* option.");
        }

        [Test]
        [TestCase(typeof(Options))]
        [TestCase(typeof(Options?))]
        public void should_create_parameter_simple_type_numeric_options(Type type)
        {
            var dataType = Builder.BuildTypeGraphService(
                    x => x.EnumFormat = EnumFormat.AsString)
                .BuildForParameter(type, "options", "Get");

            dataType.Name.ShouldEqual("string");
            dataType.IsSimple.ShouldBeTrue();
            dataType.Enumeration.Options.Count.ShouldEqual(2);

            var option = dataType.Enumeration.Options[0];
            option.Name.ShouldEqual("Option");
            option.Value.ShouldEqual("Option");
            option.Comments.ShouldBeNull();

            option = dataType.Enumeration.Options[1];
            option.Name.ShouldEqual("OptionWithComments");
            option.Value.ShouldEqual("OptionWithComments");
            option.Comments.ShouldEqual("This is *an* option.");
        }

        // Arrays

        [Test]
        [TestCase(typeof(int[]))]
        [TestCase(typeof(int?[]))]
        [TestCase(typeof(IList<int>))]
        [TestCase(typeof(List<int>))]
        public void should_create_array(Type type)
        {
            should_be_array_type(Builder.BuildTypeGraphService()
                .BuildForMessage(false, type, _endpointDescription, null), 
                    type.GetGenericEnumerableType());
        }

        [Comments("This *is* an array.")]
        public class ListWithComments : List<int> { }

        [Test]
        public void should_create_array_with_comments()
        {
            should_be_array_type(Builder.BuildTypeGraphService()
                .BuildForMessage<ListWithComments>(), typeof(int),
                    comments: "This *is* an array.");
        }

        [ArrayDescription("ArrayName", "This is *an* array comment.",
            "ItemName", "This is *an* item comment.")]
        public class ListWithArrayDescription : List<int> { }

        [Test]
        public void should_create_array_with_array_description()
        {
            should_be_array_type(Builder.BuildTypeGraphService().
                BuildForMessage<ListWithArrayDescription>(), typeof(int),
                    "ArrayName", "This is *an* array comment.",
                    "ItemName", "This is *an* item comment.");
        }

        public class ArrayMember
        {
            public List<int> MemberWithoutComments { get; set; }
            [ArrayDescription("ArrayName", "This *is* an array comment.",
                "ItemName", "This is *an* item comment.")]
            public List<int> MemberWithComments { get; set; }
        }

        [Test]
        public void should_create_array_member_without_description()
        {
            should_be_array_type(Builder.BuildTypeGraphService()
                .BuildForMessage<ArrayMember>()
                .Members.Single(x => x.Name == "MemberWithoutComments").Type,
                    typeof(int), "MemberWithoutComments");
        }

        [Test]
        public void should_create_array_member_with_description()
        {
            should_be_array_type(Builder.BuildTypeGraphService()
                .BuildForMessage<ArrayMember>()
                .Members.Single(x => x.Name == "ArrayName").Type,
                    typeof(int), "ArrayName", "This *is* an array comment.",
                    "ItemName", "This is *an* item comment.");
        }

        public void should_be_array_type(DataType type, Type itemType, 
            string name = null, string comments = null,
            string itemName = "int", string itemComments = null)
        {
            type.Name.ShouldEqual(name ?? "ArrayOfInt");
            type.Comments.ShouldEqual(comments);

            type.IsArray.ShouldBeTrue();
            type.ArrayItem.ShouldNotBeNull();
            type.ArrayItem.Name.ShouldEqual(itemName);
            type.ArrayItem.Comments.ShouldEqual(itemComments);
            should_be_simple_type(type.ArrayItem.Type, itemType, "int");

            type.IsSimple.ShouldBeFalse();
            type.Enumeration.ShouldBeNull();

            type.IsComplex.ShouldBeFalse();
            type.Members.ShouldBeNull();

            type.IsDictionary.ShouldBeFalse();
            type.DictionaryEntry.ShouldBeNull();
        }

        // Dictionaries

        [Test]
        [TestCase(typeof(IDictionary<string, int>))]
        [TestCase(typeof(Dictionary<string, int>))]
        public void should_create_dictionary(Type type)
        {
            should_be_dictionary_type(Builder.BuildTypeGraphService()
                .BuildForMessage(false, type, _endpointDescription, null), 
                    typeof(string), typeof(int), "DictionaryOfInt");
        }

        [Comments("This is *a* dictionary.")]
        public class DictionaryWithComments : Dictionary<string, int> { }

        [Test]
        public void should_create_dictionary_with_comments()
        {
            should_be_dictionary_type(Builder.BuildTypeGraphService()
                .BuildForMessage<DictionaryWithComments>(),
                    typeof(string), typeof(int), "DictionaryOfInt", 
                    "This is *a* dictionary.");
        }

        [DictionaryDescription("DictionaryName", "This is *a* dictionary.",
            "KeyName", "This is a *dictionary* key.", "This *is* a dictionary value.")]
        public class DictionaryWithDictionaryComments : Dictionary<string, int> { }

        [Test]
        public void should_create_dictionary_with_dictionary_comments()
        {
            should_be_dictionary_type(Builder.BuildTypeGraphService()
                .BuildForMessage<DictionaryWithDictionaryComments>(),
                    typeof(string), typeof(int), 
                    name: "DictionaryName",
                    comments: "This is *a* dictionary.",
                    keyName: "KeyName",
                    keyComments: "This is a *dictionary* key.",
                    valueComments: "This *is* a dictionary value.");
        }

        public class DictionaryMember
        {
            public Dictionary<string, int> MemberWithoutComments { get; set; }
            [DictionaryDescription("DictionaryName", "This *is* a dictionary.",
                "KeyName", "This is *a* dictionary key.", "This is a *dictionary* value.")]
            public Dictionary<string, int> MemberWithComments { get; set; }
        }

        [Test]
        public void should_create_dictionary_member_without_comments()
        {
            should_be_dictionary_type(Builder.BuildTypeGraphService()
                .BuildForMessage<DictionaryMember>()
                .Members.Single(x => x.Name == "MemberWithoutComments").Type,
                typeof(string), typeof(int), "MemberWithoutComments");
        }

        [Test]
        public void should_create_dictionary_member_with_description()
        {
            should_be_dictionary_type(Builder.BuildTypeGraphService()
                .BuildForMessage<DictionaryMember>()
                .Members.Single(x => x.Name == "DictionaryName").Type,
                typeof(string), typeof(int), 
                "DictionaryName",
                "This *is* a dictionary.",
                "KeyName",
                "This is *a* dictionary key.",
                "This is a *dictionary* value.");
        }

        public void should_be_dictionary_type(DataType type, 
            Type keyType, Type valueType,
            string name = null, string comments = null,
            string keyName = null, string keyComments = null, 
            string valueComments = null)
        {
            type.Name.ShouldEqual(name);
            type.Comments.ShouldEqual(comments);

            type.IsArray.ShouldBeFalse();
            type.ArrayItem.ShouldBeNull();

            type.IsSimple.ShouldBeFalse();
            type.Enumeration.ShouldBeNull();

            type.IsComplex.ShouldBeFalse();
            type.Members.ShouldBeNull();

            type.IsDictionary.ShouldBeTrue();
            type.DictionaryEntry.ShouldNotBeNull();
            type.DictionaryEntry.KeyName.ShouldEqual(keyName);
            type.DictionaryEntry.KeyComments.ShouldEqual(keyComments);
            should_be_simple_type(type.DictionaryEntry.KeyType, keyType, "string");
            type.DictionaryEntry.ValueComments.ShouldEqual(valueComments);
            should_be_simple_type(type.DictionaryEntry.ValueType, valueType, "int");
        }

        // Complex types

        public class ComplexType
        {
            public string Member1 { get; set; }
            public string Member2 { get; set; }
        }

        [Test]
        public void should_create_complex_type_and_members()
        {
            var members = should_be_complex_type(Builder.BuildTypeGraphService()
                .BuildForMessage(false, typeof(ComplexType), _endpointDescription, 
                    null), 2).Members;

            should_match_member(members[0], "Member1", sampleValue: "",
                type: x => should_be_simple_type(x, typeof(string), "string"));
            should_match_member(members[1], "Member2", sampleValue: "",
                type: x => should_be_simple_type(x, typeof(string), "string"));
        }

        public class ComplexTypeWithMemberComments
        {
            [Comments("This is *a* member.")]
            public string Member { get; set; }
        }

        [Test]
        public void should_return_complex_type_member_comments()
        {
            var member = should_be_complex_type(Builder.BuildTypeGraphService()
                .BuildForMessage(false, typeof(ComplexTypeWithMemberComments),
                    _endpointDescription, null), 1)
                .Members.Single();

            should_match_member(member, "Member", "This is *a* member.", sampleValue: "",
                type: x => should_be_simple_type(x, typeof(string), "string"));
        }

        public class ComplexTypeWithDefaultValue
        {
            [DefaultValue(3.14159)]
            public decimal Member { get; set; }
        }

        [Test]
        public void should_not_return_complex_type_member_default_value_for_output_type()
        {
            var member = should_be_complex_type(Builder.BuildTypeGraphService()
                .BuildForMessage(false, typeof(ComplexTypeWithDefaultValue),
                    _endpointDescription, null), 1)
                .Members.Single();

            should_match_member(member, "Member",
                defaultValue: null, sampleValue: "0.00",
                type: x => should_be_simple_type(x, typeof(decimal), "decimal"));
        }

        [Test]
        public void should_return_complex_type_member_default_value_for_input_type()
        {
            var member = should_be_complex_type(Builder.BuildTypeGraphService()
                .BuildForMessage(true, typeof(ComplexTypeWithDefaultValue),
                    _endpointDescription, new ApiDescription()), 1)
                .Members.Single();

            should_match_member(member, "Member",
                defaultValue: "3.14", sampleValue: "0.00", optional: true,
                type: x => should_be_simple_type(x, typeof(decimal), "decimal"));
        }

        [Test]
        public void should_return_complex_type_member_default_value_with_custom_format()
        {
            var member = should_be_complex_type(Builder
                .BuildTypeGraphService(x => x.SampleRealFormat = "0.0")
                .BuildForMessage(true, typeof(ComplexTypeWithDefaultValue),
                    _endpointDescription, new ApiDescription()), 1)
                .Members.Single();

            should_match_member(member, "Member",
                defaultValue: "3.1", sampleValue: "0.0", optional: true,
                type: x => should_be_simple_type(x, typeof(decimal), "decimal"));
        }

        public class ComplexTypeWithSampleValue
        {
            [SampleValue(3.14159)]
            public decimal Member { get; set; }
        }

        [Test]
        public void should_return_complex_type_member_sample_value_for_output_type()
        {
            var member = should_be_complex_type(Builder.BuildTypeGraphService()
                .BuildForMessage(false, typeof(ComplexTypeWithSampleValue),
                    _endpointDescription, null), 1)
                .Members.Single();

            should_match_member(member, "Member",
                sampleValue: "3.14",
                type: x => should_be_simple_type(x, typeof(decimal), "decimal"));
        }

        [Test]
        public void should_return_complex_type_member_sample_value_for_input_type()
        {
            var member = should_be_complex_type(Builder.BuildTypeGraphService()
                .BuildForMessage(true, typeof(ComplexTypeWithSampleValue),
                    _endpointDescription, new ApiDescription()), 1)
                .Members.Single();

            should_match_member(member, "Member",
                sampleValue: "3.14", optional: true,
                type: x => should_be_simple_type(x, typeof(decimal), "decimal"));
        }

        [Test]
        public void should_return_complex_type_member_sample_value_with_custom_format()
        {
            var member = should_be_complex_type(Builder
                .BuildTypeGraphService(x => x.SampleRealFormat = "0.0")
                .BuildForMessage(true, typeof(ComplexTypeWithSampleValue),
                    _endpointDescription, new ApiDescription()), 1)
                .Members.Single();

            should_match_member(member, "Member",
                sampleValue: "3.1", optional: true,
                type: x => should_be_simple_type(x, typeof(decimal), "decimal"));
        }

        public class ComplexTypeWithOptionalMember
        {
            public string OptionalReference { get; set; }
            public int? OptionalNullable { get; set; }
            public int OptionalNonNullable { get; set; }

            [Optional]
            public string Optional { get; set; }

            [Required]
            public string Required { get; set; }

            [OptionalForPost]
            public string OptionalForPost { get; set; }

            [OptionalForPut]
            public string OptionalForPut { get; set; }

            [RequiredForPost]
            public string RequiredForPost { get; set; }

            [RequiredForPut]
            public string RequiredForPut { get; set; }
        }

        private static readonly object[][] OptionalMemberTestCases = TestCaseSource.Create(x => x
            .Add(nameof(ComplexTypeWithOptionalMember.OptionalReference), true, "string", "", null)

            .Add(nameof(ComplexTypeWithOptionalMember.Optional), true, "string", "", null)
            .Add(nameof(ComplexTypeWithOptionalMember.Required), false, "string", "", null)

            .Add(nameof(ComplexTypeWithOptionalMember.OptionalNullable), true, "int", "0", null)
            .Add(nameof(ComplexTypeWithOptionalMember.OptionalNonNullable), true, "int", "0", null)

            .Add(nameof(ComplexTypeWithOptionalMember.OptionalForPost), true, "string", "", HttpMethod.Post)
            .Add(nameof(ComplexTypeWithOptionalMember.RequiredForPost), false, "string", "", HttpMethod.Post)
            .Add(nameof(ComplexTypeWithOptionalMember.OptionalForPost), false, "string", "", HttpMethod.Put)
            .Add(nameof(ComplexTypeWithOptionalMember.RequiredForPost), true, "string", "", HttpMethod.Put)
            .Add(nameof(ComplexTypeWithOptionalMember.OptionalForPost), false, "string", "", null)
            .Add(nameof(ComplexTypeWithOptionalMember.RequiredForPost), true, "string", "", null)

            .Add(nameof(ComplexTypeWithOptionalMember.OptionalForPut), true, "string", "", HttpMethod.Put)
            .Add(nameof(ComplexTypeWithOptionalMember.RequiredForPut), false, "string", "", HttpMethod.Put)
            .Add(nameof(ComplexTypeWithOptionalMember.OptionalForPut), false, "string", "", HttpMethod.Post)
            .Add(nameof(ComplexTypeWithOptionalMember.RequiredForPut), true, "string", "", HttpMethod.Post)
            .Add(nameof(ComplexTypeWithOptionalMember.OptionalForPut), false, "string", "", null)
            .Add(nameof(ComplexTypeWithOptionalMember.RequiredForPut), true, "string", "", null));

        [Test]
        [TestCaseSource(nameof(OptionalMemberTestCases))]
        public void should_return_complex_type_optional_member_when_input(
            string property, bool optional, string dataType, 
                string sampleValue, HttpMethod method)
        {
            var apiDescription = new ApiDescription { HttpMethod = method };
            var members = should_be_complex_type(Builder.BuildTypeGraphService()
                .BuildForMessage(true, typeof(ComplexTypeWithOptionalMember),
                    _endpointDescription, apiDescription), 9).Members;
            var propertyType = typeof(ComplexTypeWithOptionalMember)
                .GetProperty(property).PropertyType;

            should_match_member(members.First(x => x.Name == property), property,
                sampleValue: sampleValue, optional: optional,
                type: x => should_be_simple_type(x, propertyType, dataType));
        }

        [Test]
        [TestCaseSource(nameof(OptionalMemberTestCases))]
        public void should_not_return_complex_type_optional_member_when_output(
            string property, bool optional, string dataType,
                string sampleValue, HttpMethod method)
        {
            var members = should_be_complex_type(Builder.BuildTypeGraphService()
                .BuildForMessage(false, typeof(ComplexTypeWithOptionalMember),
                    _endpointDescription, null), 9).Members;
            var propertyType = typeof(ComplexTypeWithOptionalMember)
                .GetProperty(property).PropertyType;

            should_match_member(members.First(x => x.Name == property), property,
                optional: false, sampleValue: sampleValue,
                type: x => should_be_simple_type(x, propertyType, dataType));
        }

        public class CyclicModel
        {
            public string Member { get; set; }
            public CyclicModel CyclicMember { get; set; }
        }

        [Test]
        public void should_exclude_complex_type_cyclic_members()
        {
            should_be_complex_type(Builder.BuildTypeGraphService()
                .BuildForMessage<CyclicModel>(), 1)
                .Members.Single().Name.ShouldEqual("Member");
        }

        public class CyclicArrayModel
        {
            public string Member { get; set; }
            public List<CyclicArrayModel> CyclicMember { get; set; }
        }

        [Test]
        public void should_exclude_complex_type_cyclic_array_members()
        {
            should_be_complex_type(Builder.BuildTypeGraphService()
                .BuildForMessage<CyclicArrayModel>(), 1)
                .Members.Single().Name.ShouldEqual("Member");
        }

        public class CyclicDictionaryModel
        {
            public string Member { get; set; }
            public List<CyclicDictionaryModel> CyclicMember { get; set; }
        }

        [Test]
        public void should_exclude_complex_type_cyclic_dictionary_members()
        {
            should_be_complex_type(Builder.BuildTypeGraphService()
                .BuildForMessage<CyclicDictionaryModel>(), 1)
                .Members.Single().Name.ShouldEqual("Member");
        }

        public class ComplexTypeWithHiddenMember
        {
            [Hide]
            public string HiddenMember { get; set; }
        }

        [Test]
        public void should_exclude_a_member_if_it_is_hidden()
        {
            Builder.BuildTypeGraphService().BuildForMessage(false, 
                typeof(ComplexTypeWithHiddenMember), _endpointDescription, null)
                .Members.Any(x => x.Name == "HiddenMember").ShouldBeFalse();
        }

        [Hide]
        public class HiddenType { }

        public class ComplexTypeWithHiddenTypeMember
        {
            public HiddenType HiddenTypeMember { get; set; }
        }

        [Test]
        public void should_exclude_a_member_if_its_type_is_hidden()
        {
            Builder.BuildTypeGraphService().BuildForMessage(false, 
                typeof(ComplexTypeWithHiddenTypeMember), _endpointDescription, null)
                .Members.Any(x => x.Name == "HiddenTypeMember").ShouldBeFalse();
        }

        public class ComplexTypeWithDeprecatedMembers
        {
            [Obsolete]
            public string DeprecatedMember { get; set; }

            [Obsolete("**DO NOT** seek the treasure!")]
            public string DeprecatedMemberWithMessage { get; set; }
        }

        [Test]
        public void should_indicate_if_member_is_deprecated()
        {
            var members = should_be_complex_type(Builder.BuildTypeGraphService()
                .BuildForMessage(false, typeof(ComplexTypeWithDeprecatedMembers),
                    _endpointDescription, null), 2).Members;

            should_match_member(members[0], "DeprecatedMember",
                deprecated: true, sampleValue: "",
                type: x => should_be_simple_type(x, typeof(string), "string"));
        }

        [Test]
        public void should_indicate_if_member_is_deprecated_with_message()
        {
            var members = should_be_complex_type(Builder.BuildTypeGraphService()
                .BuildForMessage(false, typeof(ComplexTypeWithDeprecatedMembers),
                    _endpointDescription, null), 2).Members;

            should_match_member(members[1], "DeprecatedMemberWithMessage",
                deprecated: true, sampleValue: "",
                deprecatedMessage: "**DO NOT** seek the treasure!",
                type: x => should_be_simple_type(x, typeof(string), "string"));
        }

        public void should_match_member(Member member, string name,
            string comments = null, string defaultValue = null,
            string sampleValue = null, bool optional = false, 
            bool deprecated = false, string deprecatedMessage = null, 
            Action<DataType> type = null)
        {
            member.Name.ShouldEqual(name);
            member.Comments.ShouldEqual(comments);
            member.DefaultValue.ShouldEqual(defaultValue);
            member.SampleValue.ShouldEqual(sampleValue);
            member.Optional.ShouldEqual(optional);
            member.Type.ShouldNotBeNull();
            member.Deprecated.ShouldEqual(deprecated);
            member.DeprecationMessage.ShouldEqual(deprecatedMessage);
            type?.Invoke(member.Type);
        }

        public DataType should_be_complex_type(DataType type,
            int memberCount, string comments = null)
        {
            type.Name.ShouldEqual(type.Name);
            type.Comments.ShouldEqual(comments);

            type.IsArray.ShouldBeFalse();
            type.ArrayItem.ShouldBeNull();

            type.IsSimple.ShouldBeFalse();
            type.Enumeration.ShouldBeNull();

            type.IsComplex.ShouldBeTrue();
            type.Members.ShouldNotBeNull();
            type.Members.Count.ShouldEqual(memberCount);

            type.IsDictionary.ShouldBeFalse();
            type.DictionaryEntry.ShouldBeNull();

            return type;
        }

        // Namespaces

        public class Namespace
        {
            public NamespacedChild Child { get; set; }
            public List<NamespacedChild> ChildList { get; set; }
            public Dictionary<string, NamespacedChild> ChildHash { get; set; }
            public Options Options { get; set; }
        }

        public class NamespacedChild
        {
            public NamespacedChild2 Child2 { get; set; }
        }

        public class NamespacedChild2 { }

        [Test]
        [TestCase(true, "Post", "PostRequest")]
        [TestCase(false, "Put", "PutResponse")]
        public void should_have_complex_type_namespace( 
            bool request, string @namespace, string logicalName)
            
        {
            var type = Builder.BuildTypeGraphService()
                .BuildForMessage<Namespace>(@namespace, request);
            type.LogicalName.ShouldEqual(logicalName);
            type.Namespace.ShouldEqual(@namespace);
            type.FullNamespace.ShouldOnlyContain(@namespace);
        }

        [Test]
        public void should_have_optional_member_namespace()
        {
            var type = Builder.BuildTypeGraphService()
                .BuildForMessage<Namespace>("Post", true);

            var member = type.Members.Member(nameof(Namespace.Options));

            member.Type.LogicalName.ShouldBeNull();
            member.Type.Namespace.ShouldEqual("PostRequestOptions");
            member.Type.FullNamespace.ShouldOnlyContain("Post", "PostRequestOptions");
        }

        [Test]
        public void should_have_complex_type_member_namespace()
        {
            var type = Builder.BuildTypeGraphService()
                .BuildForMessage<Namespace>("Post", true);

            var member = type.Members.Member(nameof(Namespace.Child));

            member.Type.LogicalName.ShouldEqual("Child");
            member.Type.Namespace.ShouldEqual("PostRequestChild");
            member.Type.FullNamespace.ShouldOnlyContain("Post", "PostRequestChild");

            member = member.Type.Members.Member(nameof(NamespacedChild.Child2));

            member.Type.LogicalName.ShouldEqual("Child2");
            member.Type.Namespace.ShouldEqual("ChildChild2");
            member.Type.FullNamespace.ShouldOnlyContain("Post", "PostRequestChild", "ChildChild2");
        }

        [Test]
        public void should_have_list_member_namespace()
        {
            var type = Builder.BuildTypeGraphService()
                .BuildForMessage<Namespace>("Post", true);

            var member = type.Members.Member(nameof(Namespace.ChildList));

            member.Type.LogicalName.ShouldBeNull();
            member.Type.Namespace.ShouldBeNull();
            member.Type.FullNamespace.ShouldBeNull();

            var item = member.Type.ArrayItem;

            item.Type.LogicalName.ShouldEqual("ChildList");
            item.Type.Namespace.ShouldEqual("PostRequestChildList");
            item.Type.FullNamespace.ShouldOnlyContain("Post", "PostRequestChildList");

            member = item.Type.Members.Member(nameof(NamespacedChild.Child2));

            member.Type.LogicalName.ShouldEqual("Child2");
            member.Type.Namespace.ShouldEqual("ChildListChild2");
            member.Type.FullNamespace.ShouldOnlyContain("Post", "PostRequestChildList", "ChildListChild2");
        }

        [Test]
        public void should_have_dictionary_member_namespace()
        {
            var type = Builder.BuildTypeGraphService()
                .BuildForMessage<Namespace>("Post", true);

            var member = type.Members.Member(nameof(Namespace.ChildHash));

            member.Type.LogicalName.ShouldBeNull();
            member.Type.Namespace.ShouldBeNull();
            member.Type.FullNamespace.ShouldBeNull();

            var item = member.Type.DictionaryEntry;

            item.ValueType.LogicalName.ShouldEqual("ChildHash");
            item.ValueType.Namespace.ShouldEqual("PostRequestChildHash");
            item.ValueType.FullNamespace.ShouldOnlyContain("Post", "PostRequestChildHash");

            member = item.ValueType.Members.Member(nameof(NamespacedChild.Child2));

            member.Type.LogicalName.ShouldEqual("Child2");
            member.Type.Namespace.ShouldEqual("ChildHashChild2");
            member.Type.FullNamespace.ShouldOnlyContain("Post", "PostRequestChildHash", "ChildHashChild2");
        }

        [Test]
        public void should_have_message_list_namespace()
        {
            var type = Builder.BuildTypeGraphService()
                .BuildForMessage<List<NamespacedChild>>("Post", true);

            type.LogicalName.ShouldBeNull();
            type.Namespace.ShouldBeNull();
            type.FullNamespace.ShouldBeNull();

            var item = type.ArrayItem;

            item.Type.LogicalName.ShouldEqual("PostRequest");
            item.Type.Namespace.ShouldEqual("Post");
            item.Type.FullNamespace.ShouldOnlyContain("Post");

            var member = item.Type.Members.Member(nameof(NamespacedChild.Child2));

            member.Type.LogicalName.ShouldEqual("Child2");
            member.Type.Namespace.ShouldEqual("PostRequestChild2");
            member.Type.FullNamespace.ShouldOnlyContain("Post", "PostRequestChild2");
        }

        [Test]
        public void should_have_parameter_list_namespace()
        {
            var type = Builder.BuildTypeGraphService()
                .BuildForParameter<List<int>>("param", "string");

            type.LogicalName.ShouldBeNull();
            type.Namespace.ShouldBeNull();
            type.FullNamespace.ShouldBeNull();

            var item = type.ArrayItem;

            item.Type.Name.ShouldEqual("int");
            item.Type.LogicalName.ShouldBeNull();
            item.Type.Namespace.ShouldBeNull();
            item.Type.FullNamespace.ShouldBeNull();
        }

        [Test]
        public void should_have_message_dictionary_namespace()
        {
            var type = Builder.BuildTypeGraphService()
                .BuildForMessage<Dictionary<string, NamespacedChild>>("Post", true);

            type.LogicalName.ShouldBeNull();
            type.Namespace.ShouldBeNull();
            type.FullNamespace.ShouldBeNull();

            var item = type.DictionaryEntry;

            item.ValueType.LogicalName.ShouldEqual("PostRequest");
            item.ValueType.Namespace.ShouldEqual("Post");
            item.ValueType.FullNamespace.ShouldOnlyContain("Post");

            var member = item.ValueType.Members.Member(nameof(NamespacedChild.Child2));

            member.Type.LogicalName.ShouldEqual("Child2");
            member.Type.Namespace.ShouldEqual("PostRequestChild2");
            member.Type.FullNamespace.ShouldOnlyContain("Post", "PostRequestChild2");
        }

        [Test]
        public void should_have_parameter_dictionary_namespace()
        {
            var type = Builder.BuildTypeGraphService()
                .BuildForParameter<Dictionary<string, string>>("param", "string");

            type.LogicalName.ShouldBeNull();
            type.Namespace.ShouldBeNull();
            type.FullNamespace.ShouldBeNull();

            var item = type.DictionaryEntry;

            item.ValueType.Name.ShouldEqual("string");
            item.ValueType.LogicalName.ShouldBeNull();
            item.ValueType.Namespace.ShouldBeNull();
            item.ValueType.FullNamespace.ShouldBeNull();
        }
    }

    public static class TypeGraphFactoryExtensions
    {
        public static DataType BuildForMessage<T>(this TypeGraphService service, 
            string methodName = "Get", bool requestGraph = false)
        {
            return service.BuildForMessage(requestGraph, typeof(T), new EndpointDescription
                { MethodName = methodName }, new ApiDescription());
        }

        public static DataType BuildForParameter<T>(this TypeGraphService service,
            string parameterName, string parameterType,
            string methodName = "Get")
        {
            return service.BuildForParameter(typeof(T), parameterName, parameterType, methodName);
        }


        public static DataType BuildForParameter(this TypeGraphService service,
            Type type, string parameterName, string parameterType,
            string methodName = "Get")
        {
            return service.BuildForParameter(type, 
                new EndpointDescription { MethodName = methodName },
                new ParameterDescription { Name = parameterName, Type = parameterType }, 
                new ApiDescription());
        }

        public static Member Member(this List<Member> members, string name)
        {
            return members.First(x => x.Name == name);
        }
    }
}
