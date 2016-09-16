using System.Web.Http.Routing;
using NSubstitute;
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
            var route = Substitute.For<IHttpRoute>();
            route.RouteTemplate.Returns(template);
            route.GetRouteResourceIdentifier().ShouldEqual(resourceIdentifier);
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
            new HttpRoute("level1/{param1}/level2/{param2}")
                .GetNamespaceFromRoute().ShouldOnlyContain("level1", "level2");
        }
    }
}
