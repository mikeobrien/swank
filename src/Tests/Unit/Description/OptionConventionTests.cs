using Swank.Description;
using NUnit.Framework;
using Should;
using Tests.Common;

namespace Tests.Unit.Description
{
    [TestFixture]
    public class OptionConventionTests
    {
        private OptionConvention _optionConvention;
        private XmlComments _comments;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _comments = new XmlComments(new Swank.Configuration
                .Configuration().Configure(x => x.AddXmlComments()));
        }

        [SetUp]
        public void Setup()
        {
            _optionConvention = new OptionConvention(_comments);
        }

        public enum Options
        {
            [Swank.Description.Description("Option 2", "This is option 2.")]
            Option2,
            Option1,
            [Comments("This is option 3!")]
            Option3,
            [Hide]
            HiddenOption,
            /// <summary>summary</summary>
            XmlSummary,
            /// <summary>remarks</summary>
            XmlRemarks
        }

        [Test]
        public void should_return_default_description_of_option()
        {
            var description = _optionConvention.GetDescription(
                typeof(Options).GetField("Option1"));
            description.Name.ShouldEqual("Option1");
            description.Comments.ShouldBeNull();
        }

        [Test]
        public void should_return_attribute_description_of_option()
        {
            var description = _optionConvention.GetDescription(
                typeof(Options).GetField("Option2"));
            description.Name.ShouldEqual("Option 2");
            description.Comments.ShouldEqual("This is option 2.");
        }

        [Test]
        public void should_return_attribute_comments_of_option()
        {
            var description = _optionConvention.GetDescription(
                typeof(Options).GetField("Option3"));
            description.Name.ShouldEqual("Option3");
            description.Comments.ShouldEqual("This is option 3!");
        }

        [Test]
        public void should_set_xml_summary_comments()
        {
            var description = _optionConvention.GetDescription(
                typeof(Options).GetField("XmlSummary"));
            description.Name.ShouldEqual("XmlSummary");
            description.Comments.ShouldEqual("summary");
        }

        [Test]
        public void should_set_xml_remarks_comments()
        {
            var description = _optionConvention.GetDescription(
                typeof(Options).GetField("XmlRemarks"));
            description.Name.ShouldEqual("XmlRemarks");
            description.Comments.ShouldEqual("remarks");
        }

        [Test]
        public void should_return_not_hidden_when_not_configured()
        {
            _optionConvention.GetDescription(typeof(Options)
                .GetField("Option1")).Hidden.ShouldBeFalse();
        }

        [Test]
        public void should_return_hidden_when_configured()
        {
            _optionConvention.GetDescription(typeof(Options)
                .GetField("HiddenOption")).Hidden.ShouldBeTrue();
        }
    }
}