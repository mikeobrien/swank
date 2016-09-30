using System;
using System.Linq;
using System.Net;
using NUnit.Framework;
using Should;
using Swank.Web.Handlers.App;
using Tests.Common;

namespace Tests.Acceptance
{
    [TestFixture]
    public class UITests
    {
        [Test]
        public void Should_return_app_page()
        {
            var result = WebClient.GetHtml("api");
            result.Status.ShouldEqual(HttpStatusCode.OK, result.Error);

            var data = result.Data;

            try
            {
                data.ShouldContain("<link rel=\"icon\" href=\"/img/favicon.png\">");

                data.ShouldContain("<title>Setec Astronomy</title>");

                data.ShouldContain("<link href=\"/api/css/bootstrap.css/\" rel=\"stylesheet\">");
                data.ShouldContain("<link href=\"/api/css/ie10-viewport-bug-workaround.css/\" rel=\"stylesheet\">");
                data.ShouldContain("<link href=\"/api/css/highlight/github-gist.css/\" rel=\"stylesheet\">");
                data.ShouldContain("<link href=\"/api/css/swank.css/\" rel=\"stylesheet\">");

                data.ShouldContain("<script src=\"/api/js/html5shiv.js/\"></script>");
                data.ShouldContain("<script src=\"/api/js/respond.js/\"></script>");

                data.ShouldContain("overview: '\\u003ch1");
                data.ShouldContain("'Module': '\\u003ch1");

                data.ShouldContain("<img class=\"swank-logo\" src=\"/img/logo.png\" />");
                data.ShouldContain("<a class=\"navbar-brand\" href=\"#\">Setec Astronomy</a>");
                data.ShouldContain("<a href=\"/api/spec\"");

                data.ShouldContain("<a href=\"#authentication\">Authentication</a>");
                data.ShouldContain("<a href=\"#bindings\">Bindings</a>");
                data.ShouldContain("<a href=\"#data-types\">Data Types</a>");

                data.ShouldContain("<a href=\"#ModulePanel-0\" data-toggle=\"collapse\"> Module </a>");
                data.ShouldContain("<a href=\"#modules/Module\">Overview</a>");
                data.ShouldContain("<a href=\"#resources/module/resource\">module/resource</a>");
                data.ShouldContain("<a href=\"#ModulePanel-1\" data-toggle=\"collapse\"> Resources </a>");
                data.ShouldContain("<a href=\"#resources/resource/comments\">resource/comments</a>");
                data.ShouldContain("<a href=\"#resources/xml/comments\">xml/comments</a>");

                data.ShouldContain("<script src=\"/api/js/jquery.js/\"></script>");
                data.ShouldContain("<script src=\"/api/js/bootstrap.js/\"></script>");
                data.ShouldContain("<script src=\"/api/js/respond.js/\"></script>");
                data.ShouldContain("<script src=\"/api/js/ie10-viewport-bug-workaround.js/\"></script>");
                data.ShouldContain("<script src=\"/api/js/sticky-headers.js/\"></script>");
                data.ShouldContain("<script src=\"/api/js/column-tab-panes.js/\"></script>");
                data.ShouldContain("<script src=\"/api/js/highlight.js/\"></script>");
                data.ShouldContain("<script src=\"/api/js/clipboard.js/\"></script>");
                data.ShouldContain("<script src=\"/api/js/cookie.js/\"></script>");
                data.ShouldContain("<script src=\"/api/js/app.js/\"></script>");
        
                data.ShouldContain($"<footer class=\"swank-footer\">Copyright &copy; {DateTime.Now.Year} Setec Astronomy</footer>");
            }
            catch
            {
                Console.WriteLine(data);
                throw;
            }
        }

        [Test]
        public void Should_return_resource()
        {
            var result = WebClient.PostJson<Request, ResourceModel>("api/resources/", 
                new Request { Name = "resource/comments" });
            result.Status.ShouldEqual(HttpStatusCode.OK);

            var data = result.Data;

            data.Name.ShouldEqual("/resource/comments");
            data.Overview.ShouldStartWith("<p>Embedded resource description.");

            data.Endpoints.Count.ShouldEqual(4);

            var endpoint = data.Endpoints.First();

            endpoint.Id.ShouldNotBeEmpty();
            endpoint.Name.ShouldEqual("Endpoint Name");
            endpoint.Comments.ShouldStartWith("<p>Endpoint remarks.");
            endpoint.Method.ShouldEqual("get");
            endpoint.Secure.ShouldBeTrue();
            endpoint.UrlTemplate.ShouldEqual("resource/comments/{id}/{option}?requiredOption={requiredOption}&requiredMultiple={requiredMultiple}&optionalDefault={optionalDefault}");

            endpoint.CodeExamples.Count.ShouldEqual(2);

            var codeExample = endpoint.CodeExamples.First();

            codeExample.Index.ShouldEqual(0);
            codeExample.Name.ShouldEqual("Curl");
            codeExample.Language.ShouldEqual("bash");
            codeExample.Example.ShouldStartWith("curl -X GET -v \\");

            endpoint.QuerystringParameters.Count.ShouldEqual(3);

            var querystring = endpoint.QuerystringParameters.First();

            querystring.Name.ShouldEqual("requiredOption");
            querystring.Comments.ShouldStartWith("Required, options query string 1.");
            querystring.DefaultValue.ShouldBeNull();
            querystring.MultipleAllowed.ShouldBeFalse();
            querystring.Required.ShouldBeTrue();
            querystring.SampleValue.ShouldEqual("Option1");
            querystring.Type.Name.ShouldEqual("string");

            querystring.Type.Enumeration.Name.ShouldEqual("Options");
            querystring.Type.Enumeration.Comments.ShouldStartWith("Enum comments.");
            querystring.Type.Enumeration.Options.Count.ShouldEqual(2);

            var option = querystring.Type.Enumeration.Options.First();

            option.Name.ShouldEqual("Option1");
            option.Comments.ShouldStartWith("Enum option 1 comments.");
            option.Value.StartsWith("Option1");

            endpoint.QuerystringParameters.Skip(1).First().MultipleAllowed.ShouldBeTrue();

            querystring = endpoint.QuerystringParameters.Skip(2).First();
            querystring.DefaultValue.ShouldEqual("5");
            querystring.SampleValue.ShouldEqual("0");
            querystring.Type.Name.ShouldEqual("int");

            endpoint.UrlParameters.Count.ShouldEqual(2);

            var urlParameter = endpoint.UrlParameters.Skip(1).First();

            urlParameter.Name.ShouldEqual("option");
            urlParameter.Comments.ShouldStartWith("Option url parameter 2.");
            urlParameter.SampleValue.ShouldEqual("Option1");
            urlParameter.Type.Name.ShouldEqual("string");

            urlParameter.Type.Enumeration.Name.ShouldEqual("Options");
            urlParameter.Type.Enumeration.Comments.ShouldStartWith("Enum comments.");
            urlParameter.Type.Enumeration.Options.Count.ShouldEqual(2);

            option = urlParameter.Type.Enumeration.Options.First();

            option.Name.ShouldEqual("Option1");
            option.Comments.ShouldStartWith("Enum option 1 comments.");
            option.Value.StartsWith("Option1");

            endpoint.StatusCodes.Count.ShouldEqual(2);

            var statusCode = endpoint.StatusCodes.First();

            statusCode.Name.ShouldEqual("Created");
            statusCode.Code.ShouldEqual(201);
            statusCode.Comments.ShouldStartWith("Status code 1.");

            endpoint.Request.IsBinary.ShouldBeFalse();

            endpoint.Request.Headers.Count.ShouldEqual(2);

            var header = endpoint.Request.Headers.First();
            header.Comments.ShouldStartWith("Request header 1.");
            header.IsAccept.ShouldBeFalse();
            header.IsContentType.ShouldBeFalse();
            header.Name.ShouldEqual("request-header-1");
            header.Optional.ShouldBeFalse();
            header.Required.ShouldBeTrue();
            endpoint.Response.IsBinary.ShouldBeFalse();

            endpoint.Response.Headers.Count.ShouldEqual(2);

            header = endpoint.Response.Headers.First();
            header.Comments.ShouldStartWith("Response header 1.");
            header.IsAccept.ShouldBeFalse();
            header.IsContentType.ShouldBeFalse();
            header.Name.ShouldEqual("response-header-1");
            header.Optional.ShouldBeFalse();
            header.Required.ShouldBeFalse();

            endpoint.Response.Body.Count.ShouldEqual(96);

            var bodyLine = endpoint.Response.Body.First();
            bodyLine.Index.ShouldEqual(1);
            bodyLine.Name.ShouldEqual("Model");
            bodyLine.Comments.StartsWith("Type comments.");
            bodyLine.IsComplexType.ShouldEqual(true);
            bodyLine.IsFirst.ShouldEqual(true);
            bodyLine.IsOpening.ShouldEqual(true);
            bodyLine.LogicalName.ShouldBeNull();
            bodyLine.Namespace.ShouldEqual("Get");
            bodyLine.FullNamespace.ShouldOnlyContain("Get");
            bodyLine.Whitespace.ShouldBeEmpty();
        }
    }
}
