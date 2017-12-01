using Swank.Specification;
using NUnit.Framework;
using Should;
using Swank.Configuration;
using Tests.Common;
using Config = Swank.Configuration.Configuration;

namespace Tests.Unit.Specification.SpecificationService.ModuleTests
{
    [TestFixture]
    public class ModuleTests
    {
        [Test]
        public void should_set_description_to_default_when_none_is_specified()
        {
            var spec = Builder.BuildSpec<ModuleDescriptions
                .NoDescription.Controller>();

            var module = spec[0];

            module.Name.ShouldEqual(new Config().DefaultModuleName);
            module.Comments.ShouldBeNull();
        }

        [Test]
        public void should_set_description_when_one_is_specified()
        {
            var spec = Builder.BuildSpec<ModuleDescriptions
                .Description.Controller>();

            var module = spec[0];

            module.Name.ShouldEqual("Some Module");
            module.Comments.ShouldEqual("<p>Some <strong>comments</strong>.</p>");
        }

        [Test]
        public void should_set_description_and_markdown_embedded_resource_comments_when_specified()
        {
            var spec = Builder.BuildSpec<ModuleDescriptions
                .EmbeddedMarkdownComments.Controller>();

            var module = spec[0];

            module.Name.ShouldEqual("Some Markdown Module");
            module.Comments.ShouldEqual("<p><strong>Some markdown comments</strong></p>");
        }

        [Test]
        public void should_add_actions_to_closest_parent_module()
        {
            var spec = Builder.BuildSpec<NestedModules.Controller>();

            spec.Count.ShouldEqual(2);
            spec.HasModuleByName(Config.DefaultDefaultModuleName).ShouldBeFalse();

            var module = spec[0];
            module.Name.ShouldEqual("Nested Module");
            module.Resources.Count.ShouldEqual(1);
            module.Resources[0].Endpoints.Count.ShouldEqual(1);
            module.Resources[0].Endpoints[0].UrlTemplate.ShouldEqualUrl
                <NestedModules.NestedModule.Controller>(x => x.Get(null));

            module = spec[1];
            module.Name.ShouldEqual("Root Module");
            module.Resources.Count.ShouldEqual(2);
            module.Resources[0].Endpoints.Count.ShouldEqual(1);
            module.Resources[0].Endpoints[0].UrlTemplate.ShouldEqualUrl
                <NestedModules.Controller>(x => x.Get(null));
            module.Resources[1].Endpoints.Count.ShouldEqual(1);
            module.Resources[1].Endpoints[0].UrlTemplate.ShouldEqualUrl
                <NestedModules.NoModule.Controller>(x => x.Get(null));
        }

        [Test]
        public void should_return_actions_in_root_resources_when_there_are_no_modules_defined()
        {
            var spec = Builder.BuildSpec<NoModules.Controller>();

            spec.Count.ShouldEqual(1);
            spec.HasModuleByName(Config.DefaultDefaultModuleName).ShouldBeTrue();
            spec.GetModuleByName(Config.DefaultDefaultModuleName).Resources[0]
                .Endpoints.Count.ShouldEqual(1);
        }

        [Test]
        public void should_automatically_add_orphaned_actions_to_root_resources_when_modules_are_defined()
        {
            var spec = Builder.BuildSpec<OneModuleAndOrphanedAction.Controller>();

            spec.Count.ShouldEqual(2);
            spec.HasModuleByName(Config.DefaultDefaultModuleName).ShouldBeTrue();

            var module = spec[1];
            module.Name.ShouldEqual("Some Module");
            module.Resources.Count.ShouldEqual(1);
            module.Resources[0].Endpoints.Count.ShouldEqual(1);
            module.Resources[0].Endpoints[0].UrlTemplate
                .ShouldEqualUrl<OneModuleAndOrphanedAction
                .WithModule.Controller>(x => x.Get(null));
                
            var resource = spec.GetModuleByName(Config.DefaultDefaultModuleName).Resources[0];
            resource.Endpoints.Count.ShouldEqual(1);
            resource.Endpoints[0].UrlTemplate.ShouldEqualUrl<OneModuleAndOrphanedAction
                .Controller>(x => x.Get(null));
        }

        [Test]
        public void should_automatically_add_orphaned_actions_to_the_specified_default_module()
        {
            var spec = Builder.BuildSpec<OneModuleAndOrphanedAction
                .Controller>(x => x.WithDefaultModuleName("Default Module"));

            spec.Count.ShouldEqual(2);
            spec.HasModuleByName(Config.DefaultDefaultModuleName).ShouldBeFalse();

            var module = spec[0];
            module.Name.ShouldEqual("Default Module");
            module.Resources.Count.ShouldEqual(1);
            module.Resources[0].Endpoints.Count.ShouldEqual(1);
            module.Resources[0].Endpoints[0].UrlTemplate
                .ShouldEqualUrl<OneModuleAndOrphanedAction
                    .Controller>(x => x.Get(null));

            module = spec[1];
            module.Name.ShouldEqual("Some Module");
            module.Resources.Count.ShouldEqual(1);
            module.Resources[0].Endpoints.Count.ShouldEqual(1);
            module.Resources[0].Endpoints[0].UrlTemplate.ShouldEqualUrl
                <OneModuleAndOrphanedAction.WithModule
                    .Controller>(x => x.Get(null));
        }

        [Test]
        public void should_ignore_orphaned_actions()
        {
            var spec = Builder.BuildSpec<OneModuleAndOrphanedAction.Controller>(x => x
                    .WhenModuleOrphaned(OrphanedEndpoints.Exclude));

            spec.Count.ShouldEqual(1);
            spec.HasModuleByName(Config.DefaultDefaultModuleName).ShouldBeFalse();

            var module = spec[0];
            module.Name.ShouldEqual("Some Module");
            module.Resources.Count.ShouldEqual(1);
            module.Resources[0].Endpoints.Count.ShouldEqual(1);
            module.Resources[0].Endpoints[0].UrlTemplate
                .ShouldEqualUrl<OneModuleAndOrphanedAction
                    .WithModule.Controller>(x => x.Get(null));
        }

        [Test]
        public void should_throw_an_exception_for_orphaned_actions()
        {
            Assert.Throws<OrphanedModuleActionException>(() =>
                Builder.BuildSpec<NoModules.Controller>(x => x
                    .WhenModuleOrphaned(OrphanedEndpoints.Fail)));
        }

        [Test]
        public void should_not_throw_an_exception_when_there_are_no_orphaned_actions()
        {
            Assert.DoesNotThrow(() => Builder.BuildSpec<ModuleDescriptions
                .Description.Controller>(x => x.WhenModuleOrphaned(OrphanedEndpoints.Fail)));
        }
    }
}