using NUnit.Framework;
using Should;

namespace Tests.Unit.Specification.OverrideTests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void should_override_module()
        {
            var spec = Builder.BuildSpec<Controllers.Controller>(x => x
                .OverrideModules(y => y.Name = y.Name + "1")
                .OverrideModules(y => y.Comments = y.Comments + "2")
                .OverrideModulesWhen(y => y.Comments = y.Comments + "3", 
                    y => y.Comments.EndsWith("2"))
                .OverrideModulesWhen(y => y.Comments = y.Comments + "4",
                    y => y.Comments.EndsWith("2"))
                .OverrideModules(y => y.Comments = y.Comments + "*fark*"));

            spec[0].Name.ShouldEqual("module1");
            spec[0].Comments.ShouldEqual("<p>module comments23<em>fark</em></p>");
        }

        [Test]
        public void should_override_resource()
        {
            var spec = Builder.BuildSpec<Controllers.Controller>(x => x
                .OverrideResources(y => y.Name = y.Name + "1")
                .OverrideResources(y => y.Comments = y.Comments + "2")
                .OverrideResourcesWhen(y => y.Comments = y.Comments + "3", 
                    y => y.Comments.EndsWith("2"))
                .OverrideResourcesWhen(y => y.Comments = y.Comments + "4", 
                    y => y.Comments.EndsWith("2"))
                .OverrideResources(y => y.Comments = y.Comments + "*fark*"));

            spec[0].Resources[0].Name.ShouldEqual("resource1");
            spec[0].Resources[0].Comments
                .ShouldEqual("<p>resource comments23<em>fark</em></p>");
        }

        [Test]
        public void should_override_endpoint()
        {
            var spec = Builder.BuildSpec<Controllers.Controller>(x => x
                .OverrideEndpoints((y, z) => z.Name = z.Name + "1")
                .OverrideEndpoints((y, z) => z.Comments = z.Comments + "2")
                .OverrideEndpointsWhen((y, z) => z.Comments = z.Comments + "3", 
                    (y, z) => z.Comments.EndsWith("2"))
                .OverrideEndpointsWhen((y, z) => z.Comments = z.Comments + "4", 
                    (y, z) => z.Comments.EndsWith("2"))
                .OverrideEndpoints((y, z) => z.Comments = z.Comments + "*fark*"));

            spec[0].Resources[0].Endpoints[0].Name.ShouldEqual("endpoint1");
            spec[0].Resources[0].Endpoints[0].Comments
                .ShouldEqual("<p>endpoint comments23<em>fark</em></p>");
        }

        [Test]
        public void should_override_type()
        {
            var spec = Builder.BuildSpec<Controllers.Controller>(x => x
                .OverrideTypes((y, z) => z.Name = z.Name + "1")
                .OverrideTypes((y, z) => z.Comments = z.Comments + "2")
                .OverrideTypesWhen((y, z) => z.Comments = z.Comments + "3", 
                    (y, z) => z.Comments.EndsWith("2"))
                .OverrideTypesWhen((y, z) => z.Comments = z.Comments + "4", 
                    (y, z) => z.Comments.EndsWith("2"))
                .OverrideTypes((y, z) => z.Comments = z.Comments + "*fark*"));

            var type = spec[0].Resources[0].Endpoints[0].Request.Type;
            type.Name.ShouldEqual("data1");
            type.Comments.ShouldEqual("data comments23<em>fark</em>");
        }

        [Test]
        public void should_override_member()
        {
            var spec = Builder.BuildSpec<Controllers.Controller>(x => x
                .OverrideMembers((y, z) => z.Name = z.Name + "1")
                .OverrideMembers((y, z) => z.Comments = z.Comments + "2")
                .OverrideMembersWhen((y, z) => z.Comments = z.Comments + "3", 
                    (y, z) => z.Comments.EndsWith("2"))
                .OverrideMembersWhen((y, z) => z.Comments = z.Comments + "4", 
                    (y, z) => z.Comments.EndsWith("2"))
                .OverrideMembers((y, z) => z.Comments = z.Comments + "*fark*"));

            var member = spec[0].Resources[0].Endpoints[0]
                .Request.Type.Members[0];
            member.Name.ShouldEqual("member1");
            member.Comments.ShouldEqual("member comments23<em>fark</em>");
        }

        [Test]
        public void should_override_option()
        {
            var spec = Builder.BuildSpec<Controllers.Controller>(x => x
                .OverrideOptions((y, z) => z.Name = z.Name + "1")
                .OverrideOptions((y, z) => z.Comments = z.Comments + "2")
                .OverrideOptionsWhen((y, z) => z.Comments = z.Comments + "3", 
                    (y, z) => z.Comments.EndsWith("2"))
                .OverrideOptionsWhen((y, z) => z.Comments = z.Comments + "4", 
                    (y, z) => z.Comments.EndsWith("2"))
                .OverrideOptions((y, z) => z.Comments = z.Comments + "*fark*"));

            var options = spec[0].Resources[0].Endpoints[0]
                .Request.Type.Members[0].Type.Options;
            options.Options[0].Name.ShouldEqual("option1");
            options.Options[0].Comments.ShouldEqual("option comments23<em>fark</em>");
        }

        [Test]
        public void should_override_parameters()
        {
            var spec = Builder.BuildSpec<Controllers.Controller>(x => x
                .OverrideParameters((a, b) => b.Name = b.Name + "1")
                .OverrideParameters((a, b) => b.Comments = b.Comments + "2")
                .OverrideParametersWhen((a, b) => b.Comments = b.Comments + "3",
                    (a, b) => b.Comments.EndsWith("2"))
                .OverrideParametersWhen((a, b) => b.Comments = b.Comments + "4",
                    (a, b) => b.Comments.EndsWith("2"))
                .OverrideParameters((a, b) => b.Comments = b.Comments + "*fark*"));

            spec[0].Resources[0].Endpoints[0]
                .UrlParameters[0].Name.ShouldEqual("urlParam1");
            spec[0].Resources[0].Endpoints[0].UrlParameters[0]
                .Comments.ShouldEqual("url param comments23<em>fark</em>");

            spec[0].Resources[0].Endpoints[0].QuerystringParameters[0]
                .Name.ShouldEqual("querystring1");
            spec[0].Resources[0].Endpoints[0].QuerystringParameters[0]
                .Comments.ShouldEqual("querystring comments23<em>fark</em>");
        }

        [Test]
        public void should_override_url_parameters()
        {
            var spec = Builder.BuildSpec<Controllers.Controller>(x => x
                .OverrideUrlParameters((a, b, c) => c.Name = c.Name + "1")
                .OverrideUrlParameters((a, b, c) => c.Comments = c.Comments + "2")
                .OverrideUrlParametersWhen((a, b, c) => c.Comments = c.Comments + "3", 
                    (a, b, c) => c.Comments.EndsWith("2"))
                .OverrideUrlParametersWhen((a, b, c) => c.Comments = c.Comments + "4", 
                    (a, b, c) => c.Comments.EndsWith("2"))
                .OverrideUrlParameters((a, b, c) => c.Comments = c.Comments + "*fark*"));

            spec[0].Resources[0].Endpoints[0]
                .UrlParameters[0].Name.ShouldEqual("urlParam1");
            spec[0].Resources[0].Endpoints[0].UrlParameters[0]
                .Comments.ShouldEqual("url param comments23<em>fark</em>");
        }

        [Test]
        public void should_override_querystring()
        {
            var spec = Builder.BuildSpec<Controllers.Controller>(x => x
                .OverrideQuerystring((a, b, c) => c.Name = c.Name + "1")
                .OverrideQuerystring((a, b, c) => c.Comments = c.Comments + "2")
                .OverrideQuerystringWhen((a, b, c) => c.Comments = c.Comments + "3", 
                    (a, b, c) => c.Comments.EndsWith("2"))
                .OverrideQuerystringWhen((a, b, c) => c.Comments = c.Comments + "4", 
                    (a, b, c) => c.Comments.EndsWith("2"))
                .OverrideQuerystring((a, b, c) => c.Comments = c.Comments + "*fark*"));

            spec[0].Resources[0].Endpoints[0].QuerystringParameters[0]
                .Name.ShouldEqual("querystring1");
            spec[0].Resources[0].Endpoints[0].QuerystringParameters[0]
                .Comments.ShouldEqual("querystring comments23<em>fark</em>");
        }

        [Test]
        public void should_override_message()
        {
            var spec = Builder.BuildSpec<Controllers.Controller>(x => x
                .OverrideMessage((a, b) => b.Comments += "2")
                .OverrideMessageWhen((a, b) => b.Comments += "3",
                    (a, b) => b.Comments.EndsWith("2"))
                .OverrideMessageWhen((a, b) => b.Comments += "4",
                    (a, b) => b.Comments.EndsWith("2"))
                .OverrideMessage((y, z) => z.Comments = z.Comments + "*fark*"));

            spec[0].Resources[0].Endpoints[0].Request
                .Comments.ShouldEqual("<p>request comments23<em>fark</em></p>");
            spec[0].Resources[0].Endpoints[0].Response
                .Comments.ShouldEqual("<p>response comments23<em>fark</em></p>");
        }

        [Test]
        public void should_override_request()
        {
            var spec = Builder.BuildSpec<Controllers.Controller>(x => x
                .OverrideRequest((a, b) => b.Comments += "2")
                .OverrideRequestWhen((a, b) => b.Comments += "3",
                    (a, b) => b.Comments.EndsWith("2"))
                .OverrideRequestWhen((a, b) => b.Comments += "4",
                    (a, b) => b.Comments.EndsWith("2"))
                .OverrideRequest((y, z) => z.Comments = z.Comments + "*fark*"));

            spec[0].Resources[0].Endpoints[0].Request
                .Comments.ShouldEqual("<p>request comments23<em>fark</em></p>");
        }

        [Test]
        public void should_override_response()
        {
            var spec = Builder.BuildSpec<Controllers.Controller>(x => x
                .OverrideResponse((a, b) => b.Comments += "2")
                .OverrideResponseWhen((a, b) => b.Comments += "3", 
                    (a, b) => b.Comments.EndsWith("2"))
                .OverrideResponseWhen((a, b) => b.Comments += "4", 
                    (a, b) => b.Comments.EndsWith("2"))
                .OverrideResponse((y, z) => z.Comments = z.Comments + "*fark*"));

            spec[0].Resources[0].Endpoints[0].Response
                .Comments.ShouldEqual("<p>response comments23<em>fark</em></p>");
        }

        [Test]
        public void should_override_status_codes()
        {
            var spec = Builder.BuildSpec<Controllers.Controller>(x => x
                .OverrideStatusCodes((a, b) => b.Name = b.Name + "1")
                .OverrideStatusCodes((a, b) => b.Comments = b.Comments + "2")
                .OverrideStatusCodesWhen((a, b) => b.Comments = b.Comments + "3", 
                    (a, b) => b.Comments.EndsWith("2"))
                .OverrideStatusCodesWhen((a, b) => b.Comments = b.Comments + "4", 
                    (a, b) => b.Comments.EndsWith("2"))
                .OverrideStatusCodes((y, z) => z.Comments = z.Comments + "*fark*"));

            spec[0].Resources[0].Endpoints[0]
                .StatusCodes[0].Name.ShouldEqual("Internal Server Error1");
            spec[0].Resources[0].Endpoints[0].StatusCodes[0]
                .Comments.ShouldEqual("status code comments23<em>fark</em>");
        }

        [Test]
        public void should_override_all_headers()
        {
            var spec = Builder.BuildSpec<Controllers.Controller>(x => x
                .OverrideHeaders((a, b) => b.Name = b.Name + "1")
                .OverrideHeaders((a, b) => b.Comments = b.Comments + "2")
                .OverrideHeadersWhen((a, b) => b.Comments = b.Comments + "3", 
                    (a, b) => b.Comments.EndsWith("2"))
                .OverrideHeadersWhen((a, b) => b.Comments = b.Comments + "4", 
                    (a, b) => b.Comments.EndsWith("2"))
                .OverrideHeaders((y, z) => z.Comments = z.Comments + "*fark*"));

            var header = spec[0].Resources[0].Endpoints[0].Request.Headers[0];
            header.Name.ShouldEqual("request header1");
            header.Comments.ShouldEqual("request header comments23<em>fark</em>");

            header = spec[0].Resources[0].Endpoints[0].Response.Headers[0];
            header.Name.ShouldEqual("response header1");
            header.Comments.ShouldEqual("response header comments23<em>fark</em>");
        }

        [Test]
        public void should_override_request_headers()
        {
            var spec = Builder.BuildSpec<Controllers.Controller>(x => x
                .OverrideRequestHeaders((a, b) => b.Name = b.Name + "1")
                .OverrideRequestHeaders((a, b) => b.Comments = b.Comments + "2")
                .OverrideRequestHeadersWhen((a, b) => b.Comments = b.Comments + "3", 
                    (a, b) => b.Comments.EndsWith("2"))
                .OverrideRequestHeadersWhen((a, b) => b.Comments = b.Comments + "4", 
                    (a, b) => b.Comments.EndsWith("2"))
                .OverrideRequestHeaders((y, z) => z.Comments = z.Comments + "*fark*"));

            var header = spec[0].Resources[0].Endpoints[0].Request.Headers[0];
            header.Name.ShouldEqual("request header1");
            header.Comments.ShouldEqual("request header comments23<em>fark</em>");

            header = spec[0].Resources[0].Endpoints[0].Response.Headers[0];
            header.Name.ShouldEqual("response header");
            header.Comments.ShouldEqual("response header comments");
        }

        [Test]
        public void should_override_response_headers()
        {
            var spec = Builder.BuildSpec<Controllers.Controller>(x => x
                .OverrideResponseHeaders((a, b) => b.Name = b.Name + "1")
                .OverrideResponseHeaders((a, b) => b.Comments = b.Comments + "2")
                .OverrideResponseHeadersWhen((a, b) => b.Comments = b.Comments + "3", 
                    (a, b) => b.Comments.EndsWith("2"))
                .OverrideResponseHeadersWhen((a, b) => b.Comments = b.Comments + "4", 
                    (a, b) => b.Comments.EndsWith("2"))
                .OverrideResponseHeaders((y, z) => z.Comments = z.Comments + "*fark*"));

            var header = spec[0].Resources[0].Endpoints[0].Request.Headers[0];
            header.Name.ShouldEqual("request header");
            header.Comments.ShouldEqual("request header comments");

            header = spec[0].Resources[0].Endpoints[0].Response.Headers[0];
            header.Name.ShouldEqual("response header1");
            header.Comments.ShouldEqual("response header comments23<em>fark</em>");
        }
    }
}