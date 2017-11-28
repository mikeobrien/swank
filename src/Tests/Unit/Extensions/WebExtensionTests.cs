using System.Web.Http.Description;
using System.Web.Http.Routing;
using NUnit.Framework;
using Should;
using Swank.Extensions;
using Tests.Common;

namespace Tests.Unit.Extensions
{
    [TestFixture]
    public class WebExtensionTests
    {
        [Test]
        [TestCase("{Yada}", "/")]
        [TestCase("{Yada}/books/{Id}/categories/{CategoryId}/classification/{ClassId}", 
            "/books/categories/classification")]
        public void should_get_resource_identifier_from_pattern(
            string template, string resourceIdentifier)
        {
            template.GetRouteResourceIdentifier().ShouldEqual(resourceIdentifier);
        }

        [Test]
        [TestCase("fark", "~/fark")]
        [TestCase("/fark", "/fark")]
        [TestCase("~/fark", "~/fark")]
        public void should_root_url(string url, string expected)
        {
            url.EnsureRooted().ShouldEqual(expected);
        }

        [Test]
        public void should_normalize_slashes()
        {
            "/fark\\farker".NormalizeUrlSlashes().ShouldEqual("/fark/farker");
        }

        [Test]
        [TestCase("", null)]
        [TestCase("fark", null, "//fark//")]
        [TestCase("fark", "//fark//")]
        [TestCase("fark/farker/fark", "//fark//", "//farker/fark")]
        public void should_combine_urls(string expected, string url, params string[] urls)
        {
            url.CombineUrls(urls).ShouldEqual(expected);
        }

        [Test]
        [TestCase(null, "f.txt", "f.txt")]
        [TestCase("", "f.txt", "f.txt")]
        [TestCase("", "n4/f.txt", "n4/f.txt")]
        [TestCase("", @"n4\f.txt", @"n4/f.txt")]
        [TestCase("n1", "f.txt", "f.txt")]
        [TestCase("n1/n2", "n1/n2/n3/n4/f.txt", "n3/n4/f.txt")]
        [TestCase("n1/n2/", "n1/n2/n3/n4/f.txt", "n3/n4/f.txt")]
        [TestCase("/n1/n2", "n1/n2/n3/n4/f.txt", "n3/n4/f.txt")]
        [TestCase("/n1/n2/", "/n1/n2/n3/n4/f.txt", "n3/n4/f.txt")]
        [TestCase(@"c:\p1\p2\p3", @"c:\p1\p2\p3\p4\f.txt", "p4/f.txt")]
        [TestCase(@"c:\p1\p2\p3", @"c:\p1\p2\p3\p4\f.txt", "p4/f.txt")]
        [TestCase(@"c:\p1\p2\p3", @"c:\p1\p2\p3\p4\f.txt", "p4/f.txt")]
        public void should_make_relative_url(string root, string url, string expected)
        {
            url.MakeRelative(root).ShouldEqual(expected);
        }

        [Test]
        public void should_generate_namespace_from_route_template()
        {
            "level1/{param1}/level2/{param2}"
                .GetNamespaceFromRoute().ShouldOnlyContain("level1", "level2");
        }

        [Test]
        [TestCase(null, null)]
        [TestCase("", "")]
        [TestCase("Oh <b>hai</b> there!", "Oh hai there!")]
        public void should_remove_html(string html, string expected)
        {
            html.StripHtml().ShouldEqual(expected);
        }

        [Test]
        [TestCase(null, null)]
        [TestCase("", "")]
        [TestCase("&#39;fark&#39;", "'fark'")]
        [TestCase("&quot;fark&quot;", "\"fark\"")]
        public void should_remove_html_entities(string html, string expected)
        {
            html.HtmlDecode().ShouldEqual(expected);
        }

        [Test]
        [TestCase("localhost", "localhost")]
        [TestCase("fark.com", "fark")]
        [TestCase("www.fark.com", "www")]
        public void should_get_subdomain(string url, string expected)
        {
            $"http://{url}".ParseUri().GetSubdomain().ShouldEqual(expected);
        }

        [Test]
        [TestCase("localhost", "localhost")]
        [TestCase("fark.com", "fark.com")]
        [TestCase("www.fark.com", "fark.com")]
        public void should_get_root_domain(string url, string expected)
        {
            $"http://{url}".ParseUri().GetRootDomain().ShouldEqual(expected);
        }

        [Test]
        [TestCase("fark/{farker}", ApiParameterSource.FromUri, true)]
        [TestCase("fark/{*farker}", ApiParameterSource.FromUri, true)]
        [TestCase("fark/{farker}", ApiParameterSource.FromBody, false)]
        [TestCase("fark/farker", ApiParameterSource.FromUri, false)]
        public void should_indicate_if_a_parameter_is_a_url_parameter(string url, ApiParameterSource source, bool expected)
        {
            new ApiParameterDescription { Name = "farker", Source = source }
                .IsUrlParameter(new ApiDescription { Route = new HttpRoute(url) }).ShouldEqual(expected);
        }

        [Test]
        [TestCase("fark/farker", ApiParameterSource.FromUri, true)]
        [TestCase("fark/{farker}", ApiParameterSource.FromUri, false)]
        [TestCase("fark/{*farker}", ApiParameterSource.FromUri, false)]
        [TestCase("fark/{farker}", ApiParameterSource.FromBody, false)]
        public void should_indicate_if_a_parameter_is_a_querystring(string url, ApiParameterSource source, bool expected)
        {
            new ApiParameterDescription { Name = "farker", Source = source }
                .IsQuerystring(new ApiDescription { Route = new HttpRoute(url) }).ShouldEqual(expected);
        }
    }
}
