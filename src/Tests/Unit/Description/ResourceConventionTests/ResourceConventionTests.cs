using System.Reflection;
using System.Web.Http.Description;
using Swank.Description;
using NUnit.Framework;
using Should;
using Swank.Extensions;
using Tests.Common;

namespace Tests.Unit.Description.ResourceConventionTests
{
    [TestFixture]
    public class ResourceConventionTests
    {
        private IDescriptionConvention<ApiDescription, ResourceDescription> _resourceConvention;
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
            _resourceConvention = new ResourceConvention(_comments,
                new MarkerConvention<ResourceDescription>());
        }

        [Test]
        public void should_use_attribute_description_over_embedded_description()
        {
            var resource = _resourceConvention.GetDescription(
                ApiDescription<ResourceCommentsPriority.Controller>
                    .ForAction(x => x.Get(null)));

            resource.Name.ShouldEqual("Some Description");
            resource.Comments.ShouldEqual("Some comments.");

            Assembly.GetExecutingAssembly()
                .FindResourceNamed(typeof(ResourceCommentsPriority.Resource)
                    .FullName.AddMarkdownExtension())
                .ShouldEqual("**This is a resource**");
        }

        [Test]
        public void should_not_find_resource_description_when_none_is_specified()
        {
            _resourceConvention.GetDescription(
                ApiDescription<OrphanedAction.Controller>
                    .ForAction(x => x.Get(null))).ShouldBeNull();
        }

        [Test]
        public void should_set_default_description_when_no_marker_is_defined()
        {
            var resource = _resourceConvention.GetDescription(
                ApiDescription<ResourceDescriptions.NoDescription.Controller>
                    .ForAction(x => x.Get(null)));

            resource.Name.ShouldBeNull();
            resource.Comments.ShouldBeNull();
        }

        [Test]
        public void should_find_resource_marker_description()
        {
            var resource = _resourceConvention.GetDescription(
                ApiDescription<ResourceDescriptions.Description.Controller>
                    .ForAction(x => x.Get(null)));

            resource.Name.ShouldEqual("Some Resource");
            resource.Comments.ShouldEqual("Some comments.");
        }

        [Test]
        public void should_find_resource_marker_description_and_markdown_embedded_comments()
        {
            var resource = _resourceConvention.GetDescription(
                ApiDescription<ResourceDescriptions.EmbeddedMarkdownComments.Controller>
                    .ForAction(x => x.Get(null)));

            resource.Name.ShouldEqual("Some Markdown Resource");
            resource.Comments.ShouldEqual("**Some markdown comments**");
        }

        [Test]
        public void should_find_resource_attribute_description()
        {
            var resource = _resourceConvention.GetDescription(
                ApiDescription<AttributeResource.Controller>
                    .ForAction(x => x.Get(null)));
            resource.ShouldNotBeNull();
            resource.Name.ShouldEqual("Some Resource");
            resource.Comments.ShouldEqual("Some resource description");
        }

        [Test]
        public void should_find_resource_attribute_markdown_description()
        {
            var resource = _resourceConvention.GetDescription(
                ApiDescription<AttributeResource.EmbeddedMarkdownController>
                    .ForAction(x => x.Get(null)));
            resource.ShouldNotBeNull();
            resource.Name.ShouldEqual("Some Markdown Resource");
            resource.Comments.ShouldEqual("**This is a resource**");
        }

        [Test]
        public void should_find_xml_comments_description()
        {
            var resource = _resourceConvention.GetDescription(
                ApiDescription<XmlCommentsResource.Controller>
                    .ForAction(x => x.Get(null)));
            resource.ShouldNotBeNull();
            resource.Name.ShouldEqual("summary");
            resource.Comments.ShouldEqual("remarks");
        }

        [Test]
        public void should_find_xml_comments_description_and_embedded_markdown_comments()
        {
            var resource = _resourceConvention.GetDescription(
                ApiDescription<XmlCommentsResource.MarkdownCommentsController>
                    .ForAction(x => x.Get(null)));
            resource.ShouldNotBeNull();
            resource.Name.ShouldEqual("summary");
            resource.Comments.ShouldEqual("**Some markdown comments**");
        }

        [Test]
        public void should_not_find_xml_comments_description_when_summary_is_missing()
        {
            _resourceConvention.GetDescription(
                ApiDescription<XmlCommentsResource.MissingSummaryController>
                    .ForAction(x => x.Get(null))).ShouldBeNull();
        }

        [Test]
        public void should_find_parent_resource_description()
        {
            var resource = _resourceConvention.GetDescription(
                ApiDescription<ChildResources.ChildNamespace.Controller>
                    .ForAction(x => x.Get(null)));
            resource.ShouldNotBeNull();
            resource.Name.ShouldEqual("Some Resource");
            resource.Comments.ShouldBeNull();
        }

        [Test]
        public void should_find_closest_parent_resource_description()
        {
            var resource = _resourceConvention.GetDescription(
                ApiDescription<NestedResources.ChildNamespace.Controller>
                    .ForAction(x => x.Get(null)));
            resource.ShouldNotBeNull();
            resource.Name.ShouldEqual("Another Resource");
            resource.Comments.ShouldBeNull();
        }
    }
}