using System.Net;
using NUnit.Framework;
using Should;

namespace Tests.Acceptance
{
    [TestFixture]
    public class TemplateTests
    {
        [Test]
        public void should_generate_template()
        {
            var result = WebClient.GetText("api/template");
            result.Status.ShouldEqual(HttpStatusCode.OK);
            result.Data.ShouldContain("GetFile");
            result.Data.ShouldContain("PostFile");
            result.Data.ShouldContain("Endpoint Name");
        }

        [Test]
        public void should_return_404_on_missing_template()
        {
            var result = WebClient.GetText("api/missing/template");
            result.Status.ShouldEqual(HttpStatusCode.NotFound);
        }
    }
}
