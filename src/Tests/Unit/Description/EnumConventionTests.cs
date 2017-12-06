using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Swank.Description;
using NUnit.Framework;
using Should;
using Tests.Common;

namespace Tests.Unit.Description
{
    [TestFixture]
    public class EnumConventionTests
    {
        private XmlComments _comments;

        [OneTimeSetUp]
        public void Setup()
        {
            _comments = new XmlComments(new Swank.Configuration
                .Configuration().Configure(x => x.AddXmlComments()));
        }

        public Swank.Description.Description GetDescription(Type type)
        {
            return new EnumConvention(_comments).GetDescription(type);
        }

        public Swank.Description.Description GetDescription<T>()
        {
            return GetDescription(typeof(T));
        }

        public enum SomeEnum { }

        [Test]
        public void should_return_default_description_of_enum()
        {
            var description = GetDescription<SomeEnum>();
            description.Name.ShouldEqual("SomeEnum");
            description.Comments.ShouldBeNull();
        }

        [Comments("This is an enum with comments.")]
        public enum SomeEnumWithComments { }

        [Test]
        public void should_return_attribute_description_of_enum()
        {
            var description = GetDescription<SomeEnumWithComments>();
            description.Name.ShouldEqual("SomeEnumWithComments");
            description.Comments.ShouldEqual("This is an enum with comments.");
        }

        [Name("SomeEnum")]
        public enum SomeEnumWithNameAttribute { }

        [Test]
        public void should_return_attribute_name_from_name_attribute()
        {
            var description = GetDescription<SomeEnumWithNameAttribute>();
            description.Name.ShouldEqual("SomeEnum");
            description.Comments.ShouldBeNull();
        }

        [XmlType("SomeEnum")]
        public enum SomeEnumWithXmlName { }

        [Test]
        public void should_return_attribute_description_of_enum_and_xml_type_attribute()
        {
            var description = GetDescription<SomeEnumWithXmlName>();
            description.Name.ShouldEqual("SomeEnum");
            description.Comments.ShouldBeNull();
        }

        [XmlRoot("SomeRoot")]
        public enum SomeEnumWithXmlRootName { }

        [Test]
        public void should_return_attribute_description_of_enum_and_xml_root_attribute()
        {
            var description = GetDescription<SomeEnumWithXmlRootName>();
            description.Name.ShouldEqual("SomeRoot");
            description.Comments.ShouldBeNull();
        }

        [DataContract(Name = "SomeEnum")]
        public enum SomeEnumWithDataContractName { }

        [Test]
        public void should_return_data_contract_attribute_name()
        {
            var description = GetDescription<SomeEnumWithDataContractName>();
            description.Name.ShouldEqual("SomeEnum");
            description.Comments.ShouldBeNull();
        }

        /// <summary>enum summary</summary>
        public enum XmlCommentSummaryComments { }

        [Test]
        public void should_use_xml_comments_summary()
        {
            var description = GetDescription<XmlCommentSummaryComments>();
            description.Name.ShouldEqual("XmlCommentSummaryComments");
            description.Comments.ShouldEqual("enum summary");
        }

        /// <remarks>enum remarks</remarks>
        public enum XmlCommentRemarksComments { }

        [Test]
        public void should_use_xml_comments_remarks()
        {
            var description = GetDescription<XmlCommentRemarksComments>();
            description.Name.ShouldEqual("XmlCommentRemarksComments");
            description.Comments.ShouldEqual("enum remarks");
        }
    }
}