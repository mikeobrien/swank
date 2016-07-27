using System.Web.Http.Description;
using Swank.Description;
using NUnit.Framework;
using Should;
using Tests.Common;

namespace Tests.Unit.Description.ModuleConventionTests
{
    [TestFixture]
    public class ModuleConventionTests
    {
        public const string SchedulesModuleComments = 
            "<p><strong>These are schedules yo!</strong></p>";
        private IDescriptionConvention<ApiDescription, ModuleDescription> _moduleConvention;

        [SetUp]
        public void Setup()
        {
            _moduleConvention = new ModuleConvention(
                new MarkerConvention<ModuleDescription>());
        }

        [Test]
        public void should_set_description_to_default_when_none_is_specified()
        {
            var module = _moduleConvention.GetDescription(
                ApiDescription<ModuleDescriptions.NoDescription.Controller>
                    .ForAction(x => x.Get(null)));

            module.Name.ShouldBeNull();
            module.Comments.ShouldBeNull();
        }

        [Test]
        public void should_set_description_when_one_is_specified()
        {
            var module = _moduleConvention.GetDescription(
                ApiDescription<ModuleDescriptions.Description.Controller>
                    .ForAction(x => x.Get(null)));

            module.Name.ShouldEqual("Some Module");
            module.Comments.ShouldEqual("Some comments.");
        }

        [Test]
        public void should_set_description_and_markdown_embedded_resource_comments_when_specified()
        {
            var module = _moduleConvention.GetDescription(
                ApiDescription<ModuleDescriptions.EmbeddedMarkdownComments.Controller>
                    .ForAction(x => x.Get(null)));

            module.Name.ShouldEqual("Some Markdown Module");
            module.Comments.ShouldEqual("**Some markdown comments**");
        }

        [Test]
        public void should_set_description_to_parent()
        {
            var module = _moduleConvention.GetDescription(
                ApiDescription<NestedModules.NoModules.Controller>
                    .ForAction(x => x.Get(null)));

            module.Name.ShouldEqual("Root Module");
            module.Comments.ShouldBeNull();
        }

        [Test]
        public void should_set_description_to_closest_parent()
        {
            var module = _moduleConvention.GetDescription(
                ApiDescription<NestedModules.NestedModule.Controller>
                    .ForAction(x => x.Get(null)));

            module.Name.ShouldEqual("Nested Module");
            module.Comments.ShouldBeNull();
        }
    }
}