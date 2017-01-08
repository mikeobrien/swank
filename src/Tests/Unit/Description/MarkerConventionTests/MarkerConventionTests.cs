using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Swank.Description;
using NUnit.Framework;
using Should;
using Swank.Extensions;
using Tests.Common;

namespace Tests.Unit.Description.MarkerConventionTests
{
    [TestFixture]
    public class MarkerConventionTests
    {
        private IList<Swank.Description.Description> _descriptions;

        [SetUp]
        public void Setup()
        {
            _descriptions = new MarkerConvention<Swank.Description.Description>()
                .GetDescriptions(Assembly.GetExecutingAssembly())
                .Where(x => x.GetType().InNamespace<MarkerConventionTests>()).ToList();
        }

        [Test]
        public void should_use_marker_description_over_embedded_description()
        {
            var marker = _descriptions.First(x => x.GetType() == 
                typeof(MarkerCommentsPriority.Description));

            marker.Name.ShouldEqual("Some Description");
            marker.Comments.ShouldEqual("Some comments.");

            Assembly.GetExecutingAssembly()
                .FindResourceNamed(typeof(MarkerCommentsPriority.Description)
                    .FullName.AddMarkdownExtension())
                .ShouldEqual("**This is a marker**");
        }

        [Test]
        public void should_set_description_to_default_when_none_is_specified()
        {
            var marker = _descriptions.First(x => x.GetType() == 
                typeof(MarkerDescriptions.NoDescription.Description));

            marker.Name.ShouldBeNull();
            marker.Comments.ShouldBeNull();
        }

        [Test]
        public void should_set_description_when_one_is_specified()
        {
            var marker = _descriptions.First(x => x.GetType() == 
                typeof(MarkerDescriptions.Description.Description));

            marker.Name.ShouldEqual("Some Description");
            marker.Comments.ShouldEqual("Some comments.");
        }

        [Test]
        public void should_set_description_and_markdown_embedded_resource_comments_when_specified()
        {
            var marker = _descriptions.First(x => x.GetType() == 
                typeof(MarkerDescriptions.EmbeddedMarkdownComments.Description));

            marker.Name.ShouldEqual("Some Markdown Description");
            marker.Comments.ShouldEqual("**Some markdown comments**");
        }

        [Test]
        public void should_be_ordered_by_descending_namespace_and_ascending_name()
        {
            _descriptions[0].ShouldBeType<MarkerOrder.ZeeLastMarker.FirstDescription>();
            _descriptions[1].ShouldBeType<MarkerOrder.ZeeLastMarker.LastDescription>();
            _descriptions[2].ShouldBeType<MarkerOrder.AFirstMarker.FirstDescription>();
            _descriptions[3].ShouldBeType<MarkerOrder.AFirstMarker.LastDescription>();
        }
    }
}