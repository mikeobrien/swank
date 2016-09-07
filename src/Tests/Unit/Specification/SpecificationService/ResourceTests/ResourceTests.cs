using System.Linq;
using Swank.Description;
using Swank.Specification;
using NUnit.Framework;
using Should;
using Swank.Configuration;
using Tests.Common;

namespace Tests.Unit.Specification.SpecificationService.ResourceTests
{
    [TestFixture]
    public class ResourceTests
    {
        [Test]
        public void should_set_default_description_when_no_marker_is_defined()
        {
            var spec = Builder.BuildSpec<ResourceDescriptions.NoDescription.Controller>();

            var resource = spec[0].Resources[0];

            resource.Name.ShouldBeNull();
            resource.Comments.ShouldBeNull();
        }

        [Test]
        public void should_set_description_when_marker_is_defined()
        {
            var spec = Builder.BuildSpec<ResourceDescriptions.Description.Controller>();

            var resource = spec[0].Resources[0];

            resource.Name.ShouldEqual("Some Resource");
            resource.Comments.ShouldEqual("<p>Some <strong>comments</strong>.</p>");
        }

        [Test]
        public void should_set_description_and_markdown_embedded_resource_comments_when_marker_is_defined()
        {
            var spec = Builder.BuildSpec<ResourceDescriptions
                .EmbeddedMarkdownComments.Controller>();

            var resource = spec[0].Resources[0];

            resource.Name.ShouldEqual("Some Markdown Resource");
            resource.Comments.ShouldEqual("<p><strong>Some markdown comments</strong></p>");
        }

        [Test]
        public void should_set_markdown_embedded_resource_comments_when_resource_file_is_defined()
        {
            var spec = Builder.BuildSpec<ResourceDescriptions
                .OrphanedEmbeddedMarkdown.Controller>();

            var resource = spec[0].Resources[0];

            resource.Name.ShouldEqualNameUrl<ResourceDescriptions
                .OrphanedEmbeddedMarkdown.Controller>(x => x.Get(null));
            resource.Comments.ShouldEqual("<p><strong>Some markdown comments</strong></p>");
        }

        [Test]
        public void should_set_description_when_attribute_is_applied()
        {
            var spec = Builder.BuildSpec<AttributeResource.Attribute.Controller>();

            var resource = spec[0].Resources[0];

            resource.Name.ShouldEqual("Some Resource");
            resource.Comments.ShouldEqual("<p>Some <strong>resource</strong> description</p>");
        }

        [Test]
        public void should_set_description_and_markdown_embedded_resource_comments_when_attribute_is_applied()
        {
            var spec = Builder.BuildSpec<AttributeResource
                .EmbeddedMarkdownComments.Controller>();

            var resource = spec[0].Resources[0];

            resource.Name.ShouldEqual("Some Markdown Resource");
            resource.Comments.ShouldEqual("<p><strong>This is a resource</strong></p>");
        }

        [Test]
        public void should_throw_an_exception_for_orphaned_actions()
        {
            NUnit.Framework.Assert.Throws<OrphanedResourceActionException>(() =>
                Builder.BuildSpec<OrphanedAction.Controller>(x => x
                    .WhenResourceOrphaned(OrphanedEndpoints.Fail)));
        }

        [Test]
        public void should_not_throw_an_exception_when_there_are_no_orphaned_actions()
        {
            NUnit.Framework.Assert.DoesNotThrow(() => Builder
                .BuildSpec<NotOrphanedAction.Controller>(x => x
                    .WhenResourceOrphaned(OrphanedEndpoints.Fail)));
        }

        [Test]
        public void should_group_all_actions_in_the_same_namespace_into_the_same_resource()
        {
            var spec = Builder.BuildSpec<SameNamespace.Controller>();

            spec[0].Resources.Count.ShouldEqual(1);

            var resource = spec[0].Resources[0];
            resource.Endpoints.Count.ShouldEqual(2);
            resource.Name.ShouldEqual("Some Resource");
            resource.Endpoints[0].UrlTemplate.ShouldEqualUrl<SameNamespace
                .ChildNamespace.Controller>(x => x.Get(null));
            resource.Endpoints[1].UrlTemplate.ShouldEqualUrl<SameNamespace
                .Controller>(x => x.Get(null));
        }

        [Test]
        public void should_group_all_actions_in_child_namespaces_into_the_same_resource()
        {
            var spec = Builder.BuildSpec<ChildResources.Controller>();

            spec[0].Resources.Count.ShouldEqual(1);

            var resource = spec[0].Resources[0];
            resource.Endpoints.Count.ShouldEqual(2);
            resource.Name.ShouldEqual("Some Resource");
            resource.Endpoints[0].UrlTemplate.ShouldEqualUrl<ChildResources
                .ChildNamespace.Controller>(x => x.Get(null));
            resource.Endpoints[1].UrlTemplate.ShouldEqualUrl<ChildResources
                .Controller>(x => x.Get(null));
        }

        [Test]
        public void should_group_actions_into_the_closest_parent_resources()
        {
            var spec = Builder.BuildSpec<NestedResources.Controller>();

            spec[0].Resources.Count.ShouldEqual(2);

            var resource = spec[0].Resources[0];
            resource.Endpoints.Count.ShouldEqual(1);
            resource.Name.ShouldEqual("Another Resource");
            resource.Endpoints[0].UrlTemplate.ShouldEqualUrl<NestedResources
                .ChildNamespace.Controller>(x => x.Get(null));

            resource = spec[0].Resources[1];
            resource.Endpoints.Count.ShouldEqual(1);
            resource.Name.ShouldEqual("Some Resource");
            resource.Endpoints[0].UrlTemplate.ShouldEqualUrl<NestedResources
                .Controller>(x => x.Get(null));
        }

        [Test]
        public void should_group_orphaned_actions_into_default_resources()
        {
            var spec = Builder.BuildSpec<OrphanedResources.Controller>();

            spec[0].Resources.Count.ShouldEqual(2);

            var resource = spec[0].Resources[0];
            resource.Endpoints.Count.ShouldEqual(1);
            resource.Name.ShouldEqualNameUrl<OrphanedResources
                .ChildNamespace.Controller>(x => x.Get(null));
            resource.Endpoints[0].UrlTemplate.ShouldEqualUrl<OrphanedResources
                .ChildNamespace.Controller>(x => x.Get(null));

            resource = spec[0].Resources[1];
            resource.Endpoints.Count.ShouldEqual(1);
            resource.Name.ShouldEqualNameUrl<OrphanedResources
                .Controller>(x => x.Get(null));
            resource.Endpoints[0].UrlTemplate.ShouldEqualUrl<OrphanedResources
                .Controller>(x => x.Get(null));
        }

        [Test]
        public void should_group_orphaned_actions_into_the_specified_default_resource()
        {
            var spec = Builder.BuildSpec<OrphanedResources.Controller>(
                x => x.WithDefaultResource(y => new ResourceDescription
                {
                    Name = "Endpoints"
                }));

            spec[0].Resources.Count.ShouldEqual(1);

            var resource = spec[0].Resources[0];
            resource.Endpoints.Count.ShouldEqual(2);
            resource.Name.ShouldEqual("Endpoints");
            resource.Endpoints[0].UrlTemplate.ShouldEqualUrl<OrphanedResources
                .ChildNamespace.Controller>(x => x.Get(null));
            resource.Endpoints[1].UrlTemplate.ShouldEqualUrl
                <OrphanedResources.Controller>(x => x.Get(null));
        }

        [Test]
        public void should_ignore_orphaned_actions()
        {
            var spec = Builder.BuildSpec<OrphanedNestedResources.Controller>(x => x
                .WhenResourceOrphaned(OrphanedEndpoints.Exclude));

            spec[0].Resources.Count.ShouldEqual(1);

            var resource = spec[0].Resources[0];
            resource.Endpoints.Count.ShouldEqual(1);
            resource.Name.ShouldEqual("Another Resource");
            resource.Endpoints[0].UrlTemplate.ShouldEqualUrl
                <OrphanedNestedResources.ChildNamespace
                    .Controller>(x => x.Get(null));
        }
    }
}