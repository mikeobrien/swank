using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Http.Description;
using System.Xml.Serialization;
using Swank.Description;
using NUnit.Framework;
using Should;
using Tests.Common;

namespace Tests.Unit.Description
{
    [TestFixture]
    public class ParameterConventionTests
    {
        private XmlComments _comments;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _comments = new XmlComments(new Swank.Configuration
                .Configuration().Configure(x => x.AddXmlComments()));
        }

        public ParameterDescription GetDescription<TReturn>(
            Expression<Func<Controller, TReturn>> method,
            Action<ApiParameterDescription> configure = null)
        {
            return new ParameterConvention(new Swank.Configuration
                    .Configuration(), _comments).GetDescription(
                ApiDescription<Controller>.ForAction(method, 
                    x => configure?.Invoke(x .ParameterDescriptions.First()))
                    .ParameterDescriptions.First());
        }

        public class Controller
        {
            public object Value(int value) { return  null; }

            public object DescriptionValue([Swank.Description
                .Description("name", "comments")] int value) { return null; }
            public object CommentsValue([Comments("comments")] int value) { return null; }
            /// <param name="value1">value 1 comments</param>            
            /// <param name="value2">value 2 comments</param>
            public object XmlCommentsValue(int value1, int value2) { return null; }
            public object DefaultValue([DefaultValue(5)] int value) { return null; }
            public object DefaultForOptionalValue(int? value = 8) { return null; }
            public object SampleValue([SampleValue(5)] int value) { return null; }
            public object OptionalNullableValue(int? value = null) { return null; }
            public object OptionalValue([Optional] int value) { return null; }
            public object RequiredValue([Required] int? value = null) { return null; }
            public object HiddenValue([Hide] int value) { return null; }
            public object XmlIgnoreValue([XmlIgnore] int value) { return null; }
            public object MultipleValue([Multiple] int value) { return null; }
            public object ListValue(List<int> value) { return null; }
        }

        [Test]
        public void should_not_specify_type()
        {
            GetDescription(x => x.Value(0)).Type.ShouldEqual("int");
        }

        [Test]
        public void should_specify_list_item_type()
        {
            GetDescription(x => x.ListValue(null)).Type.ShouldEqual("int");
        }

        [Test]
        public void should_not_specify_description_by_default()
        {
            var description = GetDescription(x => x.Value(0));
            description.Name.ShouldEqual("value");
            description.Comments.ShouldBeNull();
        }

        [Test]
        public void should_specify_description_if_description_attribute_applied()
        {
            var description = GetDescription(x => x.DescriptionValue(0));
            description.Name.ShouldEqual("name");
            description.Comments.ShouldEqual("comments");
        }

        [Test]
        public void should_not_specify_comments_by_default()
        {
            GetDescription(x => x.Value(0)).Comments.ShouldBeNull();
        }

        [Test]
        public void should_specify_comments_if_comments_attribute_applied()
        {
            GetDescription(x => x.CommentsValue(0))
                .Comments.ShouldEqual("comments");
        }

        [Test]
        public void should_set_xml_summary_comments()
        {
            GetDescription(x => x.XmlCommentsValue(0, 0))
                .Comments.ShouldEqual("value 1 comments");
        }

        [Test]
        public void should_not_specify_default_value_by_default()
        {
            GetDescription(x => x.Value(0)).DefaultValue.ShouldBeNull();
        }

        [Test]
        public void should_specify_default_value_if_default_value_specified()
        {
            GetDescription(x => x.DefaultForOptionalValue(0)).DefaultValue.ShouldEqual("8");
        }

        [Test]
        public void should_specify_default_value_if_default_value_attribute_applied()
        {
            GetDescription(x => x.DefaultValue(0)).DefaultValue.ShouldEqual("5");
        }

        [Test]
        public void should_specify_default_sample_value_by_default()
        {
            GetDescription(x => x.Value(0)).SampleValue.ShouldEqual("0");
        }

        [Test]
        public void should_specify_sample_value_if_sample_value_attribute_applied()
        {
            GetDescription(x => x.SampleValue(0)).SampleValue.ShouldEqual("5");
        }

        [Test]
        public void should_not_be_optional_by_default()
        {
            GetDescription(x => x.Value(0)).Optional.ShouldBeFalse();
        }

        [Test]
        public void should_be_optional_if_optional_if_nullable()
        {
            GetDescription(x => x.OptionalNullableValue(0)).Optional.ShouldBeTrue();
        }

        [Test]
        public void should_be_optional_if_optional_attribute_applied()
        {
            GetDescription(x => x.OptionalValue(0)).Optional.ShouldBeTrue();
        }

        [Test]
        public void should_not_be_optional_if_required_attribute_applied()
        {
            GetDescription(x => x.RequiredValue(0)).Optional.ShouldBeFalse();
        }

        [Test]
        public void should_not_be_hidden_by_default()
        {
            GetDescription(x => x.Value(0)).Hidden.ShouldBeFalse();
        }

        [Test]
        public void should_be_hidden_if_hidden_attribute_applied()
        {
            GetDescription(x => x.HiddenValue(0)).Hidden.ShouldBeTrue();
        }

        [Test]
        public void should_be_hidden_if_xml_ignore_attribute_applied()
        {
            GetDescription(x => x.XmlIgnoreValue(0)).Hidden.ShouldBeTrue();
        }

        [Test]
        public void should_not_allow_multiple_by_default()
        {
            GetDescription(x => x.Value(0)).MultipleAllowed.ShouldBeFalse();
        }

        [Test]
        public void should_allow_multiple_if_multiple_attribute_applied()
        {
            GetDescription(x => x.MultipleValue(0)).MultipleAllowed.ShouldBeTrue();
        }

        [Test]
        public void should_allow_multiple_if_list_type()
        {
            GetDescription(x => x.ListValue(null)).MultipleAllowed.ShouldBeTrue();
        }
    }
}