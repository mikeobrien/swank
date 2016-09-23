using System;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Swank.Description;
using NUnit.Framework;
using Should;
using Tests.Common;

namespace Tests.Unit.Description
{
    [TestFixture]
    public class MemberConventionTests
    {
        private XmlComments _comments;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _comments = new XmlComments(new Swank.Configuration
                .Configuration().Configure(x => x.AddXmlComments()));
        }

        public MemberDescription GetDescription(Expression<Func<Model, object>> property)
        {
            return GetDescription(property.GetPropertyName());
        }

        public MemberDescription GetDescription(string property)
        {
            return new MemberConvention(new Swank.Configuration.Configuration(),
                _comments).GetDescription(typeof(Model).GetProperty(property));
        }

        [Hide]
        public class HiddenType { }

        public class Model
        {
            public string NoDescription { get; set; }

            [XmlElement("NewName")]
            public string CustomXmlElementName { get; set; }

            [DataMember(Name = "NewName")]
            public string CustomDataMemberName { get; set; }

            [Comments("This is a comment.")]
            public string WithComments { get; set; }

            /// <summary>This is a comment.</summary>
            public string WithXmlSummary { get; set; }

            /// <summary>This is a comment.</summary>
            public string WithXmlRemarks { get; set; }

            [DefaultValue("This is a default.")]
            public string WithDefaultValue { get; set; }

            [SampleValue("This is a sample.")]
            public string WithSampleValue { get; set; }

            public string OptionalReference { get; set; }

            public int? OptionalNullable { get; set; }
            
            public int RequiredNonNullable { get; set; }

            public string Optional { get; set; }

            [Required]
            public string Required { get; set; }

            [OptionalForPost]
            public string OptionalForPost { get; set; }

            [RequiredForPost]
            public string RequiredForPost { get; set; }

            [OptionalForPut]
            public string OptionalForPut { get; set; }

            [RequiredForPut]
            public string RequiredForPut { get; set; }

            public int? Nullable { get; set; }

            [XmlArrayItem("ItemName")]
            public string WithArrayItemName { get; set; }

            [ArrayDescription]
            public string WithEmptyArrayComments { get; set; }

            [ArrayDescription("NewName", "This is an array comment.", "ItemName", "This is an item comment.")]
            public string WithArrayDescription { get; set; }

            [DictionaryDescription]
            public string WithEmptyDictionaryDescription { get; set; }

            [DictionaryDescription("NewName", "This is a dictionary comment.", "KeyName",
                "This is a key comment.", "This is a value comment.")]
            public string WithDictionaryDescription { get; set; }

            [XmlIgnore]
            public string XmlIgnored { get; set; }

            [Hide]
            public string Hidden { get; set; }

            public HiddenType HiddenType { get; set; }

            [Swank.Description.Description("NewName", "This is a comment.")]
            public string WithDescription { get; set; }

            [Obsolete]
            public string Deprecated { get; set; }

            [Obsolete("DO NOT seek the treasure!")]
            public string DeprecatedWithMessage { get; set; }
        }

        [Test]
        public void should_return_visible_if_not_specified()
        {
            GetDescription(x => x.NoDescription).Hidden.ShouldBeFalse();
        }

        [Test]
        public void should_return_hidden_if_specified(
            [Values(nameof(Model.XmlIgnored), nameof(Model.Hidden),
                nameof(Model.HiddenType))] string property)
        {
            GetDescription(property).Hidden.ShouldBeTrue();
        }

        [Test]
        public void should_return_default_name()
        {
            GetDescription(x => x.NoDescription).Name.ShouldEqual("NoDescription");
        }

        [Test]
        public void should_return_custom_name(
            [Values(nameof(Model.CustomXmlElementName), nameof(Model.CustomDataMemberName),
                nameof(Model.WithDescription), nameof(Model.WithArrayDescription),
                nameof(Model.WithDictionaryDescription))] string property)
        {
            GetDescription(property).Name.ShouldEqual("NewName");
        }

        [Test]
        public void should_return_null_comments_if_not_specified()
        {
            GetDescription(x => x.NoDescription).Comments.ShouldBeNull();
        }

        [Test]
        public void should_return_comments_if_specified(
            [Values(nameof(Model.WithComments), 
            nameof(Model.WithDescription),
            nameof(Model.WithXmlRemarks),
            nameof(Model.WithXmlSummary))] string property)
        {
            GetDescription(property).Comments.ShouldEqual("This is a comment.");
        }

        [Test]
        public void should_return_null_default_value_if_not_specified()
        {
            GetDescription(x => x.NoDescription).DefaultValue.ShouldBeNull();
        }

        [Test]
        public void should_return_default_value_if_specified()
        {
            GetDescription(x => x.WithDefaultValue)
                .DefaultValue.ShouldEqual("This is a default.");
        }

        [Test]
        public void should_return_default_sample_value_if_not_specified()
        {
            GetDescription(x => x.NoDescription).SampleValue.ShouldEqual("");
        }

        [Test]
        public void should_return_sample_value_if_specified()
        {
            GetDescription(x => x.WithSampleValue)
                .SampleValue.ShouldEqual("This is a sample.");
        }

        [Test]
        public void should_return_optional_if_not_specified_and_reference()
        {
            GetDescription(x => x.OptionalReference).Optional.ShouldEqual(OptionalScope.All);
        }

        [Test]
        public void should_return_optional_if_not_specified()
        {
            GetDescription(x => x.OptionalNullable).Optional.ShouldEqual(OptionalScope.All);
        }

        [Test]
        public void should_return_optional_if_specified()
        {
            GetDescription(x => x.Optional).Optional.ShouldEqual(OptionalScope.All);
        }

        [Test]
        public void should_return_required_if_specified()
        {
            GetDescription(x => x.Required).Optional.ShouldEqual(OptionalScope.None);
        }

        [Test]
        public void should_return_optional_for_post_if_specified()
        {
            GetDescription(x => x.OptionalForPost).Optional.ShouldEqual(OptionalScope.Post);
        }

        [Test]
        public void should_return_required_for_post_if_specified()
        {
            GetDescription(x => x.RequiredForPost).Optional.ShouldEqual(OptionalScope.AllButPost);
        }

        [Test]
        public void should_return_optional_for_put_if_specified()
        {
            GetDescription(x => x.OptionalForPut).Optional.ShouldEqual(OptionalScope.Put);
        }

        [Test]
        public void should_return_required_for_put_if_specified()
        {
            GetDescription(x => x.RequiredForPut).Optional.ShouldEqual(OptionalScope.AllButPut);
        }

        [Test]
        public void should_return_not_deprecated()
        {
            var description = GetDescription(x => x.NoDescription);
            description.Deprecated.ShouldBeFalse();
            description.DeprecationMessage.ShouldBeNull();
        }

        [Test]
        public void should_return_deprecated_without_message()
        {
            var description = GetDescription(x => x.Deprecated);
            description.Deprecated.ShouldBeTrue();
            description.DeprecationMessage.ShouldBeNull();
        }

        [Test]
        public void should_return_deprecated_with_message()
        {
            var description = GetDescription(x => x.DeprecatedWithMessage);
            description.Deprecated.ShouldBeTrue();
            description.DeprecationMessage.ShouldEqual("DO NOT seek the treasure!");
        }

        [Test]
        public void should_return_null_array_comments_if_not_specified(
            [Values(nameof(Model.NoDescription), 
            nameof(Model.WithEmptyArrayComments))] string property)
        {
            GetDescription(property).Comments.ShouldBeNull();
        }

        [Test]
        public void should_return_array_comments_if_specified()
        {
            GetDescription(x => x.WithArrayDescription)
                .Comments.ShouldEqual("This is an array comment.");
        }

        [Test]
        public void should_return_null_array_item_name_if_not_specified()
        {
            GetDescription(x => x.NoDescription).ArrayItem.Name.ShouldBeNull();
        }

        [Test]
        public void should_return_array_item_name_if_specified(
            [Values(nameof(Model.WithArrayItemName), 
            nameof(Model.WithArrayDescription))] string property)
        {
            GetDescription(property).ArrayItem.Name.ShouldEqual("ItemName");
        }

        [Test]
        public void should_return_null_array_item_comments_if_not_specified(
            [Values("NoDescription", "WithEmptyArrayComments")] string property)
        {
            GetDescription(property).ArrayItem.Comments.ShouldBeNull();
        }

        [Test]
        public void should_return_array_item_comments_if_specified()
        {
            GetDescription(x => x.WithArrayDescription).ArrayItem
                .Comments.ShouldEqual("This is an item comment.");
        }

        [Test]
        public void should_return_null_dictionary_comments_if_not_specified(
            [Values(nameof(Model.NoDescription), 
            nameof(Model.WithEmptyDictionaryDescription))] string property)
        {
            GetDescription(property).Comments.ShouldBeNull();
        }

        [Test]
        public void should_return_dictionary_comments_if_specified()
        {
            GetDescription(x => x.WithDictionaryDescription)
                .Comments.ShouldEqual("This is a dictionary comment.");
        }

        [Test]
        public void should_return_null_dictionary_key_name_if_not_specified(
            [Values(nameof(Model.NoDescription), 
            nameof(Model.WithEmptyDictionaryDescription))] string property)
        {
            GetDescription(property).DictionaryEntry.KeyName.ShouldBeNull();
        }

        [Test]
        public void should_return_dictionary_key_name_if_specified()
        {
            GetDescription(x => x.WithDictionaryDescription).DictionaryEntry
                .KeyName.ShouldEqual("KeyName");
        }

        [Test]
        public void should_return_null_dictionary_key_comments_if_not_specified(
            [Values(nameof(Model.NoDescription), 
            nameof(Model.WithEmptyDictionaryDescription))] string property)
        {
            GetDescription(property).DictionaryEntry.KeyComments.ShouldBeNull();
        }

        [Test]
        public void should_return_dictionary_key_comments_if_specified()
        {
            GetDescription(x => x.WithDictionaryDescription).DictionaryEntry
                .KeyComments.ShouldEqual("This is a key comment.");
        }

        [Test]
        public void should_return_null_dictionary_value_comments_if_not_specified(
            [Values(nameof(Model.NoDescription), 
            nameof(Model.WithEmptyDictionaryDescription))] string property)
        {
            GetDescription(property).DictionaryEntry.ValueComments.ShouldBeNull();
        }

        [Test]
        public void should_return_dictionary_value_comments_if_specified()
        {
            GetDescription(x => x.WithDictionaryDescription).DictionaryEntry
                .ValueComments.ShouldEqual("This is a value comment.");
        }
    }
}