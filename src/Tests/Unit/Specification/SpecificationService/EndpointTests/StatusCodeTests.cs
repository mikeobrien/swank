using System.Collections.Generic;
using NUnit.Framework;
using Should;
using Tests.Common;

namespace Tests.Unit.Specification.SpecificationService.EndpointTests
{
    public class StatusCodeTests
    {
        private List<Swank.Specification.Module> _spec;

        [SetUp]
        public void Setup()
        {
            _spec = Builder.BuildSpec<StatusCodeDescriptions
                .StatusCodesController>();
        }

        [Test]
        public void should_set_endpoint_status_codes_on_Controllers_and_actions()
        {
            var endpoint = _spec.GetEndpoint<StatusCodeDescriptions
                .StatusCodesController>(x => x.Get(null));

            endpoint.StatusCodes.Count.ShouldEqual(4);

            var statusCode = endpoint.StatusCodes[0];
            statusCode.Code.ShouldEqual(410);
            statusCode.Name.ShouldEqual("410 **error** on Controller");
            statusCode.Comments.ShouldEqual("410 error on action description");

            statusCode = endpoint.StatusCodes[1];
            statusCode.Code.ShouldEqual(411);
            statusCode.Name.ShouldEqual("411 **error** on Controller");
            statusCode.Comments.ShouldBeNull();

            statusCode = endpoint.StatusCodes[2];
            statusCode.Code.ShouldEqual(412);
            statusCode.Name.ShouldEqual("412 error **on** action");
            statusCode.Comments.ShouldEqual("412 error on action description");

            statusCode = endpoint.StatusCodes[3];
            statusCode.Code.ShouldEqual(413);
            statusCode.Name.ShouldEqual("413 error **on** action");
            statusCode.Comments.ShouldBeNull();
        }

        [Test]
        public void should_not_set_endpoint_status_codes_when_none_are_set_on_Controllers_or_actions()
        {
            _spec.GetEndpoint<StatusCodeDescriptions
                .NoStatusCodesController>(x => x.Get(null))
                .StatusCodes.Count.ShouldEqual(0);
        }
    }
}