using System.Collections.Generic;
using System.Linq;
using System.Net;
using NUnit.Framework;
using Should;
using Swank.Specification;

namespace Tests.Acceptance
{
    [TestFixture]
    public class SpecTests
    {
        [Test]
        public void Should_return_schema_at_specified_url()
        {
            var result = WebClient.GetJson<List<Module>>("api/spec");
            result.Status.ShouldEqual(HttpStatusCode.OK);
            //result.Data.Modules.First().Resources.First().Endpoints.Count.ShouldEqual(1);
        }
    }
}
