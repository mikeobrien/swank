﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Description;
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
        public class TypeWithoutComments { }

        [Test]
        public void should_create_type_without_comments()
        {
            var type = Builder.BuildTypeGraphService().BuildGraph(
                typeof(TypeWithoutComments), HttpMethod.Get, false, null);

            type.Name.ShouldEqual("TypeWithoutComments");
            type.Comments.ShouldBeNull();
        }

        [Comments("This is **a** type.")]
        public class TypeWithComments { }

        [Test]
        public void should_create_type_with_comments()
        {
            var type = Builder.BuildTypeGraphService().BuildGraph(
                typeof(TypeWithComments), HttpMethod.Get, false, null);

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
            })).BuildGraph<TypeWithComments>();

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
            })).BuildGraph<TypeWithOverrodeMember>();

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
        public void should_create_simple_type(Type type, string name)
        {
            should_be_simple_type(Builder.BuildTypeGraphService()
                .BuildGraph(type, HttpMethod.Get, false, null), name);
        }

        public class SimpleTypeMember
        {
            public int Member { get; set; }
        }

        [Test]
        public void should_create_simple_type_member()
        {
            should_be_simple_type(Builder.BuildTypeGraphService().BuildGraph(
                typeof(SimpleTypeMember), HttpMethod.Get, false, null)
                    .Members.Single().Type, "int");
        }

        public void should_be_simple_type(DataType type, string name)
        {
            type.Name.ShouldEqual(name);
            type.Comments.ShouldBeNull();

            type.IsSimple.ShouldBeTrue();
            type.Options.ShouldBeNull();

            type.IsComplex.ShouldBeFalse();
            type.Members.ShouldBeNull();

            type.IsArray.ShouldBeFalse();
            type.ArrayItem.ShouldBeNull();

            type.IsDictionary.ShouldBeFalse();
            type.DictionaryEntry.ShouldBeNull();
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
        public void should_create_simple_type_string_options(
            Type type, EnumFormat format, string dataTypeName, string value1, string value2)
        {
            var dataType = Builder.BuildTypeGraphService(x => x.EnumFormat = format)
                .BuildGraph(type, HttpMethod.Get, false, null);

            dataType.Name.ShouldEqual(dataTypeName);
            dataType.IsSimple.ShouldBeTrue();
            dataType.Options.Options.Count.ShouldEqual(2);

            var option = dataType.Options.Options[0];
            option.Name.ShouldEqual("Option");
            option.Value.ShouldEqual(value1);
            option.Comments.ShouldBeNull();

            option = dataType.Options.Options[1];
            option.Name.ShouldEqual("OptionWithComments");
            option.Value.ShouldEqual(value2);
            option.Comments.ShouldEqual("This is *an* option.");
        }

        [Test]
        public void should_create_simple_type_numeric_options(
            [Values(typeof(Options), typeof(Options?))]Type type)
        {
            var dataType = Builder.BuildTypeGraphService(
                    x => x.EnumFormat = EnumFormat.AsString)
                .BuildGraph(type, HttpMethod.Get, false, null);

            dataType.Name.ShouldEqual("string");
            dataType.IsSimple.ShouldBeTrue();
            dataType.Options.Options.Count.ShouldEqual(2);

            var option = dataType.Options.Options[0];
            option.Name.ShouldEqual("Option");
            option.Value.ShouldEqual("Option");
            option.Comments.ShouldBeNull();

            option = dataType.Options.Options[1];
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
                .BuildGraph(type, HttpMethod.Get, false, null));
        }

        [Comments("This *is* an array.")]
        public class ListWithComments : List<int> { }

        [Test]
        public void should_create_array_with_comments()
        {
            should_be_array_type(Builder.BuildTypeGraphService()
                .BuildGraph<ListWithComments>(),
                    comments: "This *is* an array.");
        }

        [ArrayDescription("ArrayName", "This is *an* array comment.",
            "ItemName", "This is *an* item comment.")]
        public class ListWithArrayDescription : List<int> { }

        [Test]
        public void should_create_array_with_array_description()
        {
            should_be_array_type(Builder.BuildTypeGraphService().
                BuildGraph<ListWithArrayDescription>(),
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
                .BuildGraph<ArrayMember>()
                .Members.Single(x => x.Name == "MemberWithoutComments").Type,
                    "MemberWithoutComments");
        }

        [Test]
        public void should_create_array_member_with_description()
        {
            should_be_array_type(Builder.BuildTypeGraphService()
                .BuildGraph<ArrayMember>()
                .Members.Single(x => x.Name == "ArrayName").Type,
                    "ArrayName", "This *is* an array comment.",
                    "ItemName", "This is *an* item comment.");
        }

        public void should_be_array_type(DataType type, 
            string name = null, string comments = null,
            string itemName = "int", string itemComments = null)
        {
            type.Name.ShouldEqual(name ?? "ArrayOfInt");
            type.Comments.ShouldEqual(comments);

            type.IsArray.ShouldBeTrue();
            type.ArrayItem.ShouldNotBeNull();
            type.ArrayItem.Name.ShouldEqual(itemName);
            type.ArrayItem.Comments.ShouldEqual(itemComments);
            should_be_simple_type(type.ArrayItem.Type, "int");

            type.IsSimple.ShouldBeFalse();
            type.Options.ShouldBeNull();

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
                .BuildGraph(type, HttpMethod.Get, false, null), "DictionaryOfInt");
        }

        [Comments("This is *a* dictionary.")]
        public class DictionaryWithComments : Dictionary<string, int> { }

        [Test]
        public void should_create_dictionary_with_comments()
        {
            should_be_dictionary_type(Builder.BuildTypeGraphService()
                .BuildGraph<DictionaryWithComments>(),
                    "DictionaryOfInt", "This is *a* dictionary.");
        }

        [DictionaryDescription("DictionaryName", "This is *a* dictionary.",
            "KeyName", "This is a *dictionary* key.", "This *is* a dictionary value.")]
        public class DictionaryWithDictionaryComments : Dictionary<string, int> { }

        [Test]
        public void should_create_dictionary_with_dictionary_comments()
        {
            should_be_dictionary_type(Builder.BuildTypeGraphService()
                .BuildGraph<DictionaryWithDictionaryComments>(),
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
                .BuildGraph<DictionaryMember>()
                .Members.Single(x => x.Name == "MemberWithoutComments").Type,
                "MemberWithoutComments");
        }

        [Test]
        public void should_create_dictionary_member_with_description()
        {
            should_be_dictionary_type(Builder.BuildTypeGraphService()
                .BuildGraph<DictionaryMember>()
                .Members.Single(x => x.Name == "DictionaryName").Type,
                "DictionaryName",
                "This *is* a dictionary.",
                "KeyName",
                "This is *a* dictionary key.",
                "This is a *dictionary* value.");
        }

        public void should_be_dictionary_type(DataType type, 
            string name = null, string comments = null,
            string keyName = null, string keyComments = null, 
            string valueComments = null)
        {
            type.Name.ShouldEqual(name);
            type.Comments.ShouldEqual(comments);

            type.IsArray.ShouldBeFalse();
            type.ArrayItem.ShouldBeNull();

            type.IsSimple.ShouldBeFalse();
            type.Options.ShouldBeNull();

            type.IsComplex.ShouldBeFalse();
            type.Members.ShouldBeNull();

            type.IsDictionary.ShouldBeTrue();
            type.DictionaryEntry.ShouldNotBeNull();
            type.DictionaryEntry.KeyName.ShouldEqual(keyName);
            type.DictionaryEntry.KeyComments.ShouldEqual(keyComments);
            should_be_simple_type(type.DictionaryEntry.KeyType, "string");
            type.DictionaryEntry.ValueComments.ShouldEqual(valueComments);
            should_be_simple_type(type.DictionaryEntry.ValueType, "int");
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
                .BuildGraph(typeof(ComplexType), HttpMethod.Get, false, null), 2).Members;

            should_match_member(members[0], "Member1", sampleValue: "",
                type: x => should_be_simple_type(x, "string"));
            should_match_member(members[1], "Member2", sampleValue: "",
                type: x => should_be_simple_type(x, "string"));
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
                .BuildGraph(typeof(ComplexTypeWithMemberComments), 
                    HttpMethod.Get, false, null), 1)
                .Members.Single();

            should_match_member(member, "Member", "This is *a* member.", sampleValue: "",
                type: x => should_be_simple_type(x, "string"));
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
                .BuildGraph(typeof(ComplexTypeWithDefaultValue), 
                    HttpMethod.Get, false, null), 1)
                .Members.Single();

            should_match_member(member, "Member",
                defaultValue: null, sampleValue: "0.00",
                type: x => should_be_simple_type(x, "decimal"));
        }

        [Test]
        public void should_return_complex_type_member_default_value_for_input_type()
        {
            var member = should_be_complex_type(Builder.BuildTypeGraphService()
                .BuildGraph(typeof(ComplexTypeWithDefaultValue), 
                    HttpMethod.Get, true, new ApiDescription()), 1)
                .Members.Single();

            should_match_member(member, "Member",
                defaultValue: "3.14", sampleValue: "0.00", required: true,
                type: x => should_be_simple_type(x, "decimal"));
        }

        [Test]
        public void should_return_complex_type_member_default_value_with_custom_format()
        {
            var member = should_be_complex_type(Builder
                .BuildTypeGraphService(x => x.SampleRealFormat = "0.0")
                .BuildGraph(typeof(ComplexTypeWithDefaultValue), 
                    HttpMethod.Get, true, new ApiDescription()), 1)
                .Members.Single();

            should_match_member(member, "Member",
                defaultValue: "3.1", sampleValue: "0.0", required: true,
                type: x => should_be_simple_type(x, "decimal"));
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
                .BuildGraph(typeof(ComplexTypeWithSampleValue), 
                    HttpMethod.Get, false, null), 1)
                .Members.Single();

            should_match_member(member, "Member",
                sampleValue: "3.14",
                type: x => should_be_simple_type(x, "decimal"));
        }

        [Test]
        public void should_return_complex_type_member_sample_value_for_input_type()
        {
            var member = should_be_complex_type(Builder.BuildTypeGraphService()
                .BuildGraph(typeof(ComplexTypeWithSampleValue), 
                    HttpMethod.Get, true, new ApiDescription()), 1)
                .Members.Single();

            should_match_member(member, "Member",
                sampleValue: "3.14",
                required: true,
                type: x => should_be_simple_type(x, "decimal"));
        }

        [Test]
        public void should_return_complex_type_member_sample_value_with_custom_format()
        {
            var member = should_be_complex_type(Builder
                .BuildTypeGraphService(x => x.SampleRealFormat = "0.0")
                .BuildGraph(typeof(ComplexTypeWithSampleValue), 
                    HttpMethod.Get, true, new ApiDescription()), 1)
                .Members.Single();

            should_match_member(member, "Member",
                sampleValue: "3.1",
                required: true,
                type: x => should_be_simple_type(x, "decimal"));
        }

        public class ComplexTypeWithOptionalMember
        {
            public string OptionalReference { get; set; }
            public int? OptionalNullable { get; set; }
            public int RequiredNonNullable { get; set; }

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
            .Add(nameof(ComplexTypeWithOptionalMember.OptionalReference), true, false, "string", "", null)

            .Add(nameof(ComplexTypeWithOptionalMember.Optional), true, false, "string", "", null)
            .Add(nameof(ComplexTypeWithOptionalMember.Required), false, true, "string", "", null)

            .Add(nameof(ComplexTypeWithOptionalMember.OptionalNullable), true, false, "int", "0", null)
            .Add(nameof(ComplexTypeWithOptionalMember.RequiredNonNullable), false, true, "int", "0", null)

            .Add(nameof(ComplexTypeWithOptionalMember.OptionalForPost), true, false, "string", "", HttpMethod.Post)
            .Add(nameof(ComplexTypeWithOptionalMember.RequiredForPost), false, true, "string", "", HttpMethod.Post)
            .Add(nameof(ComplexTypeWithOptionalMember.OptionalForPost), false, true, "string", "", HttpMethod.Put)
            .Add(nameof(ComplexTypeWithOptionalMember.RequiredForPost), true, false, "string", "", HttpMethod.Put)
            .Add(nameof(ComplexTypeWithOptionalMember.OptionalForPost), false, true, "string", "", null)
            .Add(nameof(ComplexTypeWithOptionalMember.RequiredForPost), true, false, "string", "", null)

            .Add(nameof(ComplexTypeWithOptionalMember.OptionalForPut), true, false, "string", "", HttpMethod.Put)
            .Add(nameof(ComplexTypeWithOptionalMember.RequiredForPut), false, true, "string", "", HttpMethod.Put)
            .Add(nameof(ComplexTypeWithOptionalMember.OptionalForPut), false, true, "string", "", HttpMethod.Post)
            .Add(nameof(ComplexTypeWithOptionalMember.RequiredForPut), true, false, "string", "", HttpMethod.Post)
            .Add(nameof(ComplexTypeWithOptionalMember.OptionalForPut), false, true, "string", "", null)
            .Add(nameof(ComplexTypeWithOptionalMember.RequiredForPut), true, false, "string", "", null));

        [Test]
        [TestCaseSource(nameof(OptionalMemberTestCases))]
        public void should_return_complex_type_optional_member_when_input(
            string property, bool optional, bool required, string dataType, 
                string sampleValue, HttpMethod method)
        {
            var apiDescription = new ApiDescription { HttpMethod = method };
            var members = should_be_complex_type(Builder.BuildTypeGraphService()
                .BuildGraph(typeof(ComplexTypeWithOptionalMember), 
                    HttpMethod.Get, true, apiDescription), 9).Members;

            should_match_member(members.First(x => x.Name == property), property,
                required: required, sampleValue: sampleValue, optional: optional,
                type: x => should_be_simple_type(x, dataType));
        }

        [Test]
        [TestCaseSource(nameof(OptionalMemberTestCases))]
        public void should_not_return_complex_type_optional_member_when_output(
            string property, bool optional, bool required, string dataType,
                string sampleValue, HttpMethod method)
        {
            var members = should_be_complex_type(Builder.BuildTypeGraphService()
                .BuildGraph(typeof(ComplexTypeWithOptionalMember), 
                    HttpMethod.Get, false, null), 9).Members;

            should_match_member(members.First(x => x.Name == property), property,
                required: false, sampleValue: sampleValue, optional: false,
                type: x => should_be_simple_type(x, dataType));
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
                .BuildGraph<CyclicModel>(), 1)
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
                .BuildGraph<CyclicArrayModel>(), 1)
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
                .BuildGraph<CyclicDictionaryModel>(), 1)
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
            Builder.BuildTypeGraphService().BuildGraph(
                typeof(ComplexTypeWithHiddenMember), HttpMethod.Get, false, null)
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
            Builder.BuildTypeGraphService().BuildGraph(
                typeof(ComplexTypeWithHiddenTypeMember), HttpMethod.Get, false, null)
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
                .BuildGraph(typeof(ComplexTypeWithDeprecatedMembers), 
                    HttpMethod.Get, false, null), 2).Members;

            should_match_member(members[0], "DeprecatedMember",
                deprecated: true, sampleValue: "",
                type: x => should_be_simple_type(x, "string"));
        }

        [Test]
        public void should_indicate_if_member_is_deprecated_with_message()
        {
            var members = should_be_complex_type(Builder.BuildTypeGraphService()
                .BuildGraph(typeof(ComplexTypeWithDeprecatedMembers), 
                    HttpMethod.Get, false, null), 2).Members;

            should_match_member(members[1], "DeprecatedMemberWithMessage",
                deprecated: true, sampleValue: "",
                deprecatedMessage: "**DO NOT** seek the treasure!",
                type: x => should_be_simple_type(x, "string"));
        }

        public void should_match_member(Member member, string name,
            string comments = null, string defaultValue = null,
            string sampleValue = null, bool required = false,
            bool optional = false, bool deprecated = false,
            string deprecatedMessage = null, Action<DataType> type = null)
        {
            member.Name.ShouldEqual(name);
            member.Comments.ShouldEqual(comments);
            member.DefaultValue.ShouldEqual(defaultValue);
            member.SampleValue.ShouldEqual(sampleValue);
            member.Required.ShouldEqual(required);
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
            type.Options.ShouldBeNull();

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
        }

        public class NamespacedChild
        {
            public NamespacedChild2 Child2 { get; set; }
        }

        public class NamespacedChild2 { }

        [Test]
        [TestCase("POST", true, "Post", "PostRequest")]
        [TestCase("PUT", false, "Put", "PutResponse")]
        public void should_have_complex_type_namespace(string method, 
            bool request, string @namespace, string logicalName)
            
        {
            var type = Builder.BuildTypeGraphService()
                .BuildGraph<Namespace>(new HttpMethod(method), request);
            type.LogicalName.ShouldEqual(logicalName);
            type.Namespace.ShouldEqual(@namespace);
            type.FullNamespace.ShouldOnlyContain(@namespace);
        }

        [Test]
        public void should_have_complex_type_member_namespace()
        {
            var type = Builder.BuildTypeGraphService()
                .BuildGraph<Namespace>(HttpMethod.Post, true);

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
                .BuildGraph<Namespace>(HttpMethod.Post, true);

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
                .BuildGraph<Namespace>(HttpMethod.Post, true);

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
        public void should_have_list_namespace()
        {
            var type = Builder.BuildTypeGraphService()
                .BuildGraph<List<NamespacedChild>>(HttpMethod.Post, true);

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
        public void should_have_dictionary_namespace()
        {
            var type = Builder.BuildTypeGraphService()
                .BuildGraph<Dictionary<string, NamespacedChild>>(HttpMethod.Post, true);

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
    }

    public static class TypeGraphFactoryExtensions
    {
        public static DataType BuildGraph<T>(this TypeGraphService service, 
            HttpMethod method = null, bool requestGraph = false)
        {
            return service.BuildGraph(typeof(T), method ?? HttpMethod.Get, 
                requestGraph, new ApiDescription());
        }

        public static Member Member(this List<Member> members, string name)
        {
            return members.First(x => x.Name == name);
        }
    }
}