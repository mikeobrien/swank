using System.Reflection;
using NUnit.Framework;
using Should;
using Swank.Description;
using Tests.Common;
using Swank.Extensions;
using Tests.Unit.Description.EndpointConventionTests.EndpointDescriptions;

namespace Tests.Unit.Description.EndpointConventionTests
{
    [TestFixture]
    public class EndpointConventionTests
    {
        private EndpointConvention _endpointConvention;
        private XmlComments _comments;
        private Swank.Configuration.Configuration _configuration;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _comments = new XmlComments(new Swank.Configuration
                .Configuration().Configure(x => x.AddXmlComments()));
        }

        [SetUp]
        public void Setup()
        {
            _configuration = new Swank.Configuration.Configuration();
            _endpointConvention = new EndpointConvention(_comments, _configuration);
        }

        [Test]
        public void should_use_attribute_description_over_embedded_description()
        {
            var description = _endpointConvention.GetDescription(
                ApiDescription<AttributePriority.Controller>
                    .ForAction(x => x.Get(null)));

            description.Name.ShouldEqual("action name");
            description.Comments.ShouldEqual("action description");

            Assembly.GetExecutingAssembly().FindResourceNamed(
                typeof(AttributePriority.Controller).FullName + "." +
                nameof(AttributePriority.Controller.Get) + ".md")
                .ShouldEqual("embedded action description");
        }

        [Test]
        public void should_use_defaults_for_endpoint_with_no_description()
        {
            var description = _endpointConvention.GetDescription(
                ApiDescription<EndpointDescriptions.NoDescriptionController>
                    .ForAction(x => x.Get(null)));
            description.Name.ShouldEqual("Get");
            description.Comments.ShouldBeNull();
        }

        [Test]
        public void should_get_endpoint_description_from_endpoint_description_attribute()
        {
            var description = _endpointConvention.GetDescription(
                ApiDescription<EndpointDescriptions.ActionDescription.Controller>
                    .ForAction(x => x.Get(null)));
            description.Name.ShouldEqual("action name");
            description.Comments.ShouldEqual("action description");
        }

        [Test]
        public void should_get_endpoint_comments_from_endpoint_comments_attribute()
        {
            var description = _endpointConvention.GetDescription(
                ApiDescription<EndpointDescriptions.ActionDescription
                    .AttributeCommentsController>
                    .ForAction(x => x.Get(null)));
            description.Name.ShouldEqual("Get");
            description.Comments.ShouldEqual("action comments");
        }

        [Test]
        public void should_get_endpoint_description_from_web_api()
        {
            var description = _endpointConvention.GetDescription(
                ApiDescription<EndpointDescriptions.NoDescriptionController>
                    .ForAction(x => x.Get(null), x => x.Documentation = 
                        "web api documentation."));
            description.Name.ShouldEqual("Get");
            description.Comments.ShouldEqual("web api documentation.");
        }

        [Test]
        public void should_get_embedded_markdown_endpoint_description()
        {
            var description = _endpointConvention.GetDescription(
                ApiDescription<EndpointDescriptions.ActionDescription
                    .EmbeddedDescriptionController>.ForAction(x => x.Get(null)));
            description.Name.ShouldEqual("Get");
            description.Comments.ShouldEqual("**An embedded action markdown description**");
        }

        [Test]
        public void should_get_xml_comment_endpoint_description()
        {
            var description = _endpointConvention.GetDescription(
                ApiDescription<EndpointDescriptions.ActionDescription
                    .XmlCommentController>.ForAction(x => x.Get(null)));
            description.Name.ShouldEqual("summary");
            description.Comments.ShouldEqual("remarks");
        }

        [Test]
        public void should_get_request_description_from_attribute()
        {
            var description = _endpointConvention.GetDescription(
                ApiDescription<EndpointDescriptions.RequestDescription
                    .AttributeController>
                    .ForAction(x => x.Get(null)));
            description.RequestComments.ShouldEqual("request description");
        }

        [Test]
        public void should_get_request_description_from_endpoint_named_embedded_file()
        {
            var description = _endpointConvention.GetDescription(
                ApiDescription<EndpointDescriptions.RequestDescription
                    .EmbeddedDescriptionController>
                    .ForAction(x => x.Get(null)));
            description.RequestComments.ShouldEqual("**An embedded action request markdown description**");
        }

        [Test]
        public void should_get_request_description_from_web_api()
        {
            var description = _endpointConvention.GetDescription(
               ApiDescription<EndpointDescriptions.NoDescriptionController>
                    .ForAction(x => x.Get(null), x => x.GetRequestDescription()
                        .Documentation = "web api documentation."));
            description.RequestComments.ShouldEqual("web api documentation.");
        }

        [Test]
        public void should_get_request_description_from_xml_comments()
        {
            var description = _endpointConvention.GetDescription(
                ApiDescription<EndpointDescriptions.RequestDescription
                    .XmlCommentsController>.ForAction(x => x.Get(null)));
            description.RequestComments.ShouldEqual("request description");
        }

        [Test]
        public void should_get_response_description_from_attribute()
        {
            var description = _endpointConvention.GetDescription(
                ApiDescription<EndpointDescriptions.ResponseDescription.AttributeController>
                    .ForAction(x => x.Get(null)));
            description.ResponseComments.ShouldEqual("response description");
        }

        [Test]
        public void should_get_response_description_from_endpoint_named_embedded_file()
        {
            var description = _endpointConvention.GetDescription(
                ApiDescription<EndpointDescriptions.ResponseDescription
                    .EmbeddedDescriptionController>
                    .ForAction(x => x.Get(null)));
            description.ResponseComments.ShouldEqual("**An embedded action response markdown description**");
        }

        [Test]
        public void should_get_response_description_from_web_api()
        {
            var description = _endpointConvention.GetDescription(
                ApiDescription<EndpointDescriptions.NoDescriptionController>
                    .ForAction(x => x.Get(null), x => x.ResponseDescription
                        .Documentation = "response documentation."));
            description.ResponseComments.ShouldEqual("response documentation.");
        }

        [Test]
        public void should_get_response_description_from_xml_comments()
        {
            var description = _endpointConvention.GetDescription(
                ApiDescription<EndpointDescriptions.ResponseDescription
                    .XmlCommentsController>.ForAction(x => x.Get(null)));
            description.ResponseComments.ShouldEqual("response description");
        }

        [Test]
        public void should_get_actions_to_unsecure_by_default()
        {
            _endpointConvention.GetDescription(ApiDescription<EndpointDescriptions.
                SecureDescription.PublicController>.ForAction(x => x.Get(null)))
                .Secure.ShouldBeFalse();
        }

        [Test]
        public void should_get_actions_to_secure_when_controller_flagged()
        {
            _endpointConvention.GetDescription(ApiDescription<EndpointDescriptions.
                SecureDescription.SecureController>.ForAction(x => x.Get(null)))
                .Secure.ShouldBeTrue();
        }

        [Test]
        public void should_get_actions_to_secure_when_endpoint_flagged()
        {
            _endpointConvention.GetDescription(ApiDescription<EndpointDescriptions
                .SecureDescription.SecureActionController>.ForAction(x => x.Get(null)))
                .Secure.ShouldBeTrue();
        }

        [Test]
        public void should_not_mark_request_or_response_binary_by_default()
        {
            var description = _endpointConvention.GetDescription(ApiDescription<EndpointDescriptions.
                NoDescriptionController>.ForAction(x => x.Get(null)));
            description.BinaryRequest.ShouldBeFalse();
            description.BinaryResponse.ShouldBeFalse();
        }

        [Test]
        public void should_mark_request_as_binary()
        {
            var description = _endpointConvention.GetDescription(ApiDescription<EndpointDescriptions.
                BinaryDescription.BinaryRequestController>.ForAction(x => x.Get(null)));
            description.BinaryRequest.ShouldBeTrue();
            description.BinaryResponse.ShouldBeFalse();
        }

        [Test]
        public void should_mark_response_as_binary()
        {
            var description = _endpointConvention.GetDescription(ApiDescription<EndpointDescriptions.
                BinaryDescription.BinaryResponseController>.ForAction(x => x.Get(null)));
            description.BinaryRequest.ShouldBeFalse();
            description.BinaryResponse.ShouldBeTrue();
        }

        [Test]
        public void should_get_type_namespace()
        {
            var description = _endpointConvention.GetDescription(
                ApiDescription<NoDescriptionController>
                    .ForAction(x => x.Get(null)));

            description.Namespace.ShouldOnlyContain("Tests", "Unit", "Description",
                "EndpointConventionTests", "EndpointDescriptions", "NoDescription", "Get");
        }
    }
}