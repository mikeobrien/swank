using System.Collections.Generic;
using NUnit.Framework;
using Should;
using Tests.Common;

namespace Tests.Unit.Specification.SpecificationService.EndpointTests
{
    public class HeadersTests
    {
        private List<Swank.Specification.Module> _spec;

        [SetUp]
        public void Setup()
        {
            _spec = Builder.BuildSpec<HeaderDescriptions.HeadersController>();
        }

        [Test]
        public void should_set_endpoint_headers_on_Controllers_and_actions()
        {
            var endpoint = _spec.GetEndpoint<HeaderDescriptions
                .HeadersController>(x => x.Get(null));

            endpoint.Request.Headers.Count.ShouldEqual(2);

            var header = endpoint.Request.Headers[0];
            header.Name.ShouldEqual("accept");
            header.Comments.ShouldEqual("This is an <strong>endpoint</strong> description.");
            header.Optional.ShouldBeFalse();
            header.Required.ShouldBeTrue();
            header.IsAccept.ShouldBeTrue();
            header.IsContentType.ShouldBeFalse();

            header = endpoint.Request.Headers[1];
            header.Name.ShouldEqual("api-key");
            header.Comments.ShouldEqual("This is a <strong>Controller</strong> description.");
            header.Optional.ShouldBeTrue();
            header.Required.ShouldBeFalse();
            header.IsAccept.ShouldBeFalse();
            header.IsContentType.ShouldBeFalse();

            endpoint.Response.Headers.Count.ShouldEqual(2);

            header = endpoint.Response.Headers[0];
            header.Name.ShouldEqual("content-length");
            header.Comments.ShouldBeNull();
            header.Optional.ShouldBeFalse();
            header.Required.ShouldBeFalse();
            header.IsAccept.ShouldBeFalse();
            header.IsContentType.ShouldBeFalse();

            header = endpoint.Response.Headers[1];
            header.Name.ShouldEqual("content-type");
            header.Comments.ShouldBeNull();
            header.Optional.ShouldBeFalse();
            header.Required.ShouldBeFalse();
            header.IsAccept.ShouldBeFalse();
            header.IsContentType.ShouldBeTrue();
        }

        [Test]
        public void should_not_set_endpoint_headers_when_none_are_set_on_Controllers_or_actions()
        {
            var endpoint = _spec.GetEndpoint<HeaderDescriptions
                .NoHeadersController>(x => x.Get(null));

            endpoint.Request.Headers.Count.ShouldEqual(0);
            endpoint.Response.Headers.Count.ShouldEqual(0);
        }
    }
}