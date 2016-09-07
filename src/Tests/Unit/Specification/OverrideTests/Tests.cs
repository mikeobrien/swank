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
                .OverrideModules(y => y.Module.Name = y.Module.Name + "1")
                .OverrideModules(y => y.Module.Comments = y.Module.Comments + "2")
                .OverrideModulesWhen(y => y.Module.Comments = y.Module.Comments + "3", 
                    y => y.Module.Comments.EndsWith("2"))
                .OverrideModulesWhen(y => y.Module.Comments = y.Module.Comments + "4",
                    y => y.Module.Comments.EndsWith("2"))
                .OverrideModules(y => y.Module.Comments = y.Module.Comments + "*fark*"));

            spec[0].Name.ShouldEqual("module1");
            spec[0].Comments.ShouldEqual("<p>module comments23<em>fark</em></p>");
        }

        [Test]
        public void should_override_resource()
        {
            var spec = Builder.BuildSpec<Controllers.Controller>(x => x
                .OverrideResources(y => y.Resource.Name = y.Resource.Name + "1")
                .OverrideResources(y => y.Resource.Comments = y.Resource.Comments + "2")
                .OverrideResourcesWhen(y => y.Resource.Comments = y.Resource.Comments + "3", 
                    y => y.Resource.Comments.EndsWith("2"))
                .OverrideResourcesWhen(y => y.Resource.Comments = y.Resource.Comments + "4", 
                    y => y.Resource.Comments.EndsWith("2"))
                .OverrideResources(y => y.Resource.Comments = y.Resource.Comments + "*fark*"));

            spec[0].Resources[0].Name.ShouldEqual("resource1");
            spec[0].Resources[0].Comments
                .ShouldEqual("<p>resource comments23<em>fark</em></p>");
        }

        [Test]
        public void should_override_endpoint()
        {
            var spec = Builder.BuildSpec<Controllers.Controller>(x => x
                .OverrideEndpoints(z => z.Endpoint.Name = z.Endpoint.Name + "1")
                .OverrideEndpoints(z => z.Endpoint.Comments = z.Endpoint.Comments + "2")
                .OverrideEndpointsWhen(z => z.Endpoint.Comments = z.Endpoint.Comments + "3", 
                    z => z.Endpoint.Comments.EndsWith("2"))
                .OverrideEndpointsWhen(z => z.Endpoint.Comments = z.Endpoint.Comments + "4", 
                    z => z.Endpoint.Comments.EndsWith("2"))
                .OverrideEndpoints(z => z.Endpoint.Comments = z.Endpoint.Comments + "*fark*"));

            spec[0].Resources[0].Endpoints[0].Name.ShouldEqual("endpoint1");
            spec[0].Resources[0].Endpoints[0].Comments
                .ShouldEqual("<p>endpoint comments23<em>fark</em></p>");
        }

        [Test]
        public void should_override_type()
        {
            var spec = Builder.BuildSpec<Controllers.Controller>(x => x
                .OverrideTypes(z => z.DataType.Name = z.DataType.Name + "1")
                .OverrideTypes(z => z.DataType.Comments = z.DataType.Comments + "2")
                .OverrideTypesWhen(z => z.DataType.Comments = z.DataType.Comments + "3", 
                    z => z.DataType.Comments.EndsWith("2"))
                .OverrideTypesWhen(z => z.DataType.Comments = z.DataType.Comments + "4", 
                    z => z.DataType.Comments.EndsWith("2"))
                .OverrideTypes(z => z.DataType.Comments = z.DataType.Comments + "*fark*"));

            var type = spec[0].Resources[0].Endpoints[0].Request.Type;
            type.Name.ShouldEqual("data1");
            type.Comments.ShouldEqual("data comments23<em>fark</em>");
        }

        [Test]
        public void should_override_member()
        {
            var spec = Builder.BuildSpec<Controllers.Controller>(x => x
                .OverrideMembers(z => z.Member.Name = z.Member.Name + "1")
                .OverrideMembers(z => z.Member.Comments = z.Member.Comments + "2")
                .OverrideMembersWhen(z => z.Member.Comments = z.Member.Comments + "3", 
                    z => z.Member.Comments.EndsWith("2"))
                .OverrideMembersWhen(z => z.Member.Comments = z.Member.Comments + "4", 
                    z => z.Member.Comments.EndsWith("2"))
                .OverrideMembers(z => z.Member.Comments = z.Member.Comments + "*fark*"));

            var member = spec[0].Resources[0].Endpoints[0]
                .Request.Type.Members[0];
            member.Name.ShouldEqual("member1");
            member.Comments.ShouldEqual("member comments23<em>fark</em>");
        }

        [Test]
        public void should_override_option()
        {
            var spec = Builder.BuildSpec<Controllers.Controller>(x => x
                .OverrideOptions(z => z.Option.Name = z.Option.Name + "1")
                .OverrideOptions(z => z.Option.Comments = z.Option.Comments + "2")
                .OverrideOptionsWhen(z => z.Option.Comments = z.Option.Comments + "3", 
                    z => z.Option.Comments.EndsWith("2"))
                .OverrideOptionsWhen(z => z.Option.Comments = z.Option.Comments + "4", 
                    z => z.Option.Comments.EndsWith("2"))
                .OverrideOptions(z => z.Option.Comments = z.Option.Comments + "*fark*"));

            var options = spec[0].Resources[0].Endpoints[0]
                .Request.Type.Members[0].Type.Options;
            options.Options[0].Name.ShouldEqual("option1");
            options.Options[0].Comments.ShouldEqual("option comments23<em>fark</em>");
        }

        [Test]
        public void should_override_parameters()
        {
            var spec = Builder.BuildSpec<Controllers.Controller>(x => x
                .OverrideParameters(b => b.Parameter.Name = b.Parameter.Name + "1")
                .OverrideParameters(b => b.Parameter.Comments = b.Parameter.Comments + "2")
                .OverrideParametersWhen(b => b.Parameter.Comments = b.Parameter.Comments + "3",
                    b => b.Parameter.Comments.EndsWith("2"))
                .OverrideParametersWhen(b => b.Parameter.Comments = b.Parameter.Comments + "4",
                    b => b.Parameter.Comments.EndsWith("2"))
                .OverrideParameters(b => b.Parameter.Comments = b.Parameter.Comments + "*fark*"));

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
                .OverrideUrlParameters(c => c.UrlParameter.Name = c.UrlParameter.Name + "1")
                .OverrideUrlParameters(c => c.UrlParameter.Comments = c.UrlParameter.Comments + "2")
                .OverrideUrlParametersWhen(c => c.UrlParameter.Comments = c.UrlParameter.Comments + "3", 
                    c => c.UrlParameter.Comments.EndsWith("2"))
                .OverrideUrlParametersWhen(c => c.UrlParameter.Comments = c.UrlParameter.Comments + "4", 
                    c => c.UrlParameter.Comments.EndsWith("2"))
                .OverrideUrlParameters(c => c.UrlParameter.Comments = c.UrlParameter.Comments + "*fark*"));

            spec[0].Resources[0].Endpoints[0]
                .UrlParameters[0].Name.ShouldEqual("urlParam1");
            spec[0].Resources[0].Endpoints[0].UrlParameters[0]
                .Comments.ShouldEqual("url param comments23<em>fark</em>");
        }

        [Test]
        public void should_override_querystring()
        {
            var spec = Builder.BuildSpec<Controllers.Controller>(x => x
                .OverrideQuerystring(c => c.Querystring.Name = c.Querystring.Name + "1")
                .OverrideQuerystring(c => c.Querystring.Comments = c.Querystring.Comments + "2")
                .OverrideQuerystringWhen(c => c.Querystring.Comments = c.Querystring.Comments + "3", 
                    c => c.Querystring.Comments.EndsWith("2"))
                .OverrideQuerystringWhen(c => c.Querystring.Comments = c.Querystring.Comments + "4", 
                    c => c.Querystring.Comments.EndsWith("2"))
                .OverrideQuerystring(c => c.Querystring.Comments = c.Querystring.Comments + "*fark*"));

            spec[0].Resources[0].Endpoints[0].QuerystringParameters[0]
                .Name.ShouldEqual("querystring1");
            spec[0].Resources[0].Endpoints[0].QuerystringParameters[0]
                .Comments.ShouldEqual("querystring comments23<em>fark</em>");
        }

        [Test]
        public void should_override_message()
        {
            var spec = Builder.BuildSpec<Controllers.Controller>(x => x
                .OverrideMessage(b => b.Message.Comments += "2")
                .OverrideMessageWhen(b => b.Message.Comments += "3",
                    b => b.Message.Comments.EndsWith("2"))
                .OverrideMessageWhen(b => b.Message.Comments += "4",
                    b => b.Message.Comments.EndsWith("2"))
                .OverrideMessage(z => z.Message.Comments = z.Message.Comments + "*fark*"));

            spec[0].Resources[0].Endpoints[0].Request
                .Comments.ShouldEqual("<p>request comments23<em>fark</em></p>");
            spec[0].Resources[0].Endpoints[0].Response
                .Comments.ShouldEqual("<p>response comments23<em>fark</em></p>");
        }

        [Test]
        public void should_override_request()
        {
            var spec = Builder.BuildSpec<Controllers.Controller>(x => x
                .OverrideRequest(b => b.Message.Comments += "2")
                .OverrideRequestWhen(b => b.Message.Comments += "3",
                    b => b.Message.Comments.EndsWith("2"))
                .OverrideRequestWhen(b => b.Message.Comments += "4",
                    b => b.Message.Comments.EndsWith("2"))
                .OverrideRequest(z => z.Message.Comments = z.Message.Comments + "*fark*"));

            spec[0].Resources[0].Endpoints[0].Request
                .Comments.ShouldEqual("<p>request comments23<em>fark</em></p>");
        }

        [Test]
        public void should_override_response()
        {
            var spec = Builder.BuildSpec<Controllers.Controller>(x => x
                .OverrideResponse(b => b.Message.Comments += "2")
                .OverrideResponseWhen(b => b.Message.Comments += "3", 
                    b => b.Message.Comments.EndsWith("2"))
                .OverrideResponseWhen(b => b.Message.Comments += "4", 
                    b => b.Message.Comments.EndsWith("2"))
                .OverrideResponse(z => z.Message.Comments = z.Message.Comments + "*fark*"));

            spec[0].Resources[0].Endpoints[0].Response
                .Comments.ShouldEqual("<p>response comments23<em>fark</em></p>");
        }

        [Test]
        public void should_override_status_codes()
        {
            var spec = Builder.BuildSpec<Controllers.Controller>(x => x
                .OverrideStatusCodes(b => b.StatusCode.Name = b.StatusCode.Name + "1")
                .OverrideStatusCodes(b => b.StatusCode.Comments = b.StatusCode.Comments + "2")
                .OverrideStatusCodesWhen(b => b.StatusCode.Comments = b.StatusCode.Comments + "3", 
                    b => b.StatusCode.Comments.EndsWith("2"))
                .OverrideStatusCodesWhen(b => b.StatusCode.Comments = b.StatusCode.Comments + "4", 
                    b => b.StatusCode.Comments.EndsWith("2"))
                .OverrideStatusCodes(z => z.StatusCode.Comments = z.StatusCode.Comments + "*fark*"));

            spec[0].Resources[0].Endpoints[0]
                .StatusCodes[0].Name.ShouldEqual("Internal Server Error1");
            spec[0].Resources[0].Endpoints[0].StatusCodes[0]
                .Comments.ShouldEqual("status code comments23<em>fark</em>");
        }

        [Test]
        public void should_override_all_headers()
        {
            var spec = Builder.BuildSpec<Controllers.Controller>(x => x
                .OverrideHeaders(b => b.Header.Name = b.Header.Name + "1")
                .OverrideHeaders(b => b.Header.Comments = b.Header.Comments + "2")
                .OverrideHeadersWhen(b => b.Header.Comments = b.Header.Comments + "3", 
                    b => b.Header.Comments.EndsWith("2"))
                .OverrideHeadersWhen(b => b.Header.Comments = b.Header.Comments + "4", 
                    b => b.Header.Comments.EndsWith("2"))
                .OverrideHeaders(z => z.Header.Comments = z.Header.Comments + "*fark*"));

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
                .OverrideRequestHeaders(b => b.Header.Name = b.Header.Name + "1")
                .OverrideRequestHeaders(b => b.Header.Comments = b.Header.Comments + "2")
                .OverrideRequestHeadersWhen(b => b.Header.Comments = b.Header.Comments + "3", 
                    b => b.Header.Comments.EndsWith("2"))
                .OverrideRequestHeadersWhen(b => b.Header.Comments = b.Header.Comments + "4", 
                    b => b.Header.Comments.EndsWith("2"))
                .OverrideRequestHeaders(z => z.Header.Comments = z.Header.Comments + "*fark*"));

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
                .OverrideResponseHeaders(b => b.Header.Name = b.Header.Name + "1")
                .OverrideResponseHeaders(b => b.Header.Comments = b.Header.Comments + "2")
                .OverrideResponseHeadersWhen(b => b.Header.Comments = b.Header.Comments + "3", 
                    b => b.Header.Comments.EndsWith("2"))
                .OverrideResponseHeadersWhen(b => b.Header.Comments = b.Header.Comments + "4", 
                    b => b.Header.Comments.EndsWith("2"))
                .OverrideResponseHeaders(z => z.Header.Comments = z.Header.Comments + "*fark*"));

            var header = spec[0].Resources[0].Endpoints[0].Request.Headers[0];
            header.Name.ShouldEqual("request header");
            header.Comments.ShouldEqual("request header comments");

            header = spec[0].Resources[0].Endpoints[0].Response.Headers[0];
            header.Name.ShouldEqual("response header1");
            header.Comments.ShouldEqual("response header comments23<em>fark</em>");
        }
    }
}