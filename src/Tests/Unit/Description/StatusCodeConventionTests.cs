using System.Net;
using Swank.Description;
using NUnit.Framework;
using Should;
using Tests.Common;

namespace Tests.Unit.Description
{
    [TestFixture]
    public class StatusCodeConventionTests
    {
        private StatusCodeConvention _statusCodeConvention;

        [SetUp]
        public void Setup()
        {
            _statusCodeConvention = new StatusCodeConvention();
        }

        [StatusCode(HttpStatusCode.LengthRequired)]
        [StatusCode(410, "410 error on controller", "410 error on action description")]
        public class StatusController
        {
            [StatusCode(HttpStatusCode.RequestEntityTooLarge)]
            [StatusCode(412, "412 error on action", "412 error on action description")]
            public object Get(object request) { return null; }
        }

        [Test]
        public void should_set_endpoint_status_codes_on_controllers_and_actions()
        {
            var descriptions = _statusCodeConvention.GetDescription(
                ApiDescription<StatusController>
                .ForAction(x => x.Get(null)));

            descriptions.Count.ShouldEqual(4);

            var statusCode = descriptions[0];
            statusCode.Code.ShouldEqual(410);
            statusCode.Name.ShouldEqual("410 error on controller");
            statusCode.Comments.ShouldEqual("410 error on action description");

            statusCode = descriptions[1];
            statusCode.Code.ShouldEqual(411);
            statusCode.Name.ShouldEqual("Length Required");
            statusCode.Comments.ShouldBeNull();

            statusCode = descriptions[2];
            statusCode.Code.ShouldEqual(412);
            statusCode.Name.ShouldEqual("412 error on action");
            statusCode.Comments.ShouldEqual("412 error on action description");

            statusCode = descriptions[3];
            statusCode.Code.ShouldEqual(413);
            statusCode.Name.ShouldEqual("Request Entity Too Large");
            statusCode.Comments.ShouldBeNull();
        }

        public class NoStatusCodesController
        {
            public object Get(object request) { return null; }
        }

        [Test]
        public void should_not_set_endpoint_status_codes_when_none_are_set_on_controllers_or_actions()
        {
            _statusCodeConvention.GetDescription(
                ApiDescription<NoStatusCodesController>
                .ForAction(x => x.Get(null))).Count.ShouldEqual(0);
        }
    }
}