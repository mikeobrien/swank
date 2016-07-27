using Swank.Description;
using NUnit.Framework;
using Should;
using Tests.Common;

namespace Tests.Unit.Description
{
    [TestFixture]
    public class HeaderConventionTests
    {
        private HeaderConvention _headerConvention;

        [SetUp]
        public void Setup()
        {
            _headerConvention = new HeaderConvention();
        }

        [ResponseHeader("content-type")]
        [RequestHeader("api-key", "This is a controller description.", true)]
        public class HeadersController
        {
            [RequestHeader("accept", "This is an endpoint description.", true)]
            [ResponseHeader("content-length")]
            public object Get(object request) { return null; }
        }

        [Test]
        public void should_get_endpoint_headers_from_controllers_and_actions()
        {
            var headerDescriptions = _headerConvention.GetDescription(
                ApiDescription<HeadersController>.ForAction(x => x.Get(null)));

            headerDescriptions.Count.ShouldEqual(4);

            var header = headerDescriptions[0];
            header.Direction.ShouldEqual(HttpDirection.Request);
            header.Name.ShouldEqual("accept");
            header.Comments.ShouldEqual("This is an endpoint description.");
            header.Optional.ShouldBeTrue();

            header = headerDescriptions[1];
            header.Direction.ShouldEqual(HttpDirection.Request);
            header.Name.ShouldEqual("api-key");
            header.Comments.ShouldEqual("This is a controller description.");
            header.Optional.ShouldBeTrue();

            header = headerDescriptions[2];
            header.Direction.ShouldEqual(HttpDirection.Response);
            header.Name.ShouldEqual("content-length");
            header.Comments.ShouldBeNull();
            header.Optional.ShouldBeFalse();

            header = headerDescriptions[3];
            header.Direction.ShouldEqual(HttpDirection.Response);
            header.Name.ShouldEqual("content-type");
            header.Comments.ShouldBeNull();
            header.Optional.ShouldBeFalse();
        }

        public class NoHeadersController
        {
            public object Get(object request) { return null; }
        }

        [Test]
        public void should_not_set_endpoint_headers_when_none_are_set_on_controllers_or_actions()
        {
            _headerConvention.GetDescription(ApiDescription<NoHeadersController>
                .ForAction(x => x.Get(null)))
                .Count.ShouldEqual(0);
        }
    }
}