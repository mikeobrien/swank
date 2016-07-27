using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Should;
using Swank.Extensions;
using Swank.Web.Assets;
using Tests.Common;

namespace Tests.Unit.Web.Assets
{
    [TestFixture]
    public class WebAssetTests
    {
        private readonly Swank.Configuration.Configuration _configuration = 
            new Swank.Configuration.Configuration
        {
            AppUrl = "api"
        };

        [Test]
        public void should_get_web_asset_from_a_string()
        {
            var asset = WebAsset.FromString("url", "mime", "content");

            asset.GetUrl(_configuration).ShouldEqual("api/url/");
            asset.MimeType.ShouldEqual("mime");
            asset.Filename.ShouldEqual("url");
            asset.Load().ShouldEqual("content".ToBytes());
        }

        [Test]
        [TestCase(null, Mime.ApplicationJson)]
        [TestCase("fark/farker", "fark/farker")]
        public void should_get_web_asset_from_a_virtual_path(
            string mimeType, string expectedMimeType)
        {
            SetupFiles((virtualPath, nestedPath) =>
            {
                var asset = WebAsset.FromVirtualPath(Path.Combine(virtualPath, 
                    nestedPath, "File1.json"), "url", mimeType);

                asset.GetUrl(_configuration).ShouldEqual("api/url/File1.json/");
                asset.MimeType.ShouldEqual(expectedMimeType);
                asset.Filename.ShouldEqual("File1.json");
                asset.Load().ShouldEqual("file1".ToBytes());
            });
        }

        [Test]
        [TestCase(null, Mime.ApplicationJson, Mime.TextHtml)]
        [TestCase("fark/farker", "fark/farker", "fark/farker")]
        public void should_get_web_assets_under_a_virtual_path(
            string mimeType, string jsonMimeType, string htmlMimeType)
        {
            SetupFiles((virtualPath, nestedPath) =>
            {
                var assets = WebAsset.InVirtualPath(virtualPath, 
                    "url", mimeType, new [] { ".json", ".html" });

                assets.Count().ShouldEqual(2);

                var asset = assets.FirstOrDefault(x => x
                    .GetUrl(_configuration).EndsWith("File1.json/"));
                asset.GetUrl(_configuration).ShouldEqual(
                    $"api/url/{nestedPath}/File1.json/");
                asset.MimeType.ShouldEqual(jsonMimeType);
                asset.Filename.ShouldEqual("File1.json");
                asset.Load().ShouldEqual("file1".ToBytes());

                asset = assets.FirstOrDefault(x => x
                    .GetUrl(_configuration).EndsWith("File2.html/"));
                asset.GetUrl(_configuration).ShouldEqual(
                    $"api/url/{nestedPath}/File2.html/");
                asset.MimeType.ShouldEqual(htmlMimeType);
                asset.Filename.ShouldEqual("File2.html");
                asset.Load().ShouldEqual("file2".ToBytes());
            });
        }

        private static void SetupFiles(Action<string, string> test)
        {
            var virtualPath = Guid.NewGuid().ToString("n");
            var nestedPath = Guid.NewGuid().ToString("n");
            var path = Path.Combine(Path.GetTempPath(), virtualPath, nestedPath);
            Directory.CreateDirectory(path);

            try
            {
                File.WriteAllText(Path.Combine(path, "File1.json"), "file1");
                File.WriteAllText(Path.Combine(path, "File2.html"), "file2");
                File.WriteAllText(Path.Combine(path, "File3.fark"), "file3");

                test(virtualPath, nestedPath);
            }
            finally
            {
                Directory.Delete(path, true);
            }
        }

        private IEnumerable<Assembly> _thisAssembly = Assembly
            .GetExecutingAssembly().AsList();

        [Test]
        [TestCase(null, Mime.ApplicationJson, 
            "Tests.Unit.Web.Assets.WebAssetTests.Resources.File1.json")]
        [TestCase("fark/farker", "fark/farker", "Resources.File1.json")]
        public void should_get_web_asset_from_a_resource(
            string mimeType, string expectedMimeType, string name)
        {
            var asset = WebAsset.FromResource(_thisAssembly, name, "url", mimeType);

            asset.GetUrl(_configuration).ShouldEqual("api/url/File1.json/");
            asset.MimeType.ShouldEqual(expectedMimeType);
            asset.Filename.ShouldEqual("File1.json");
            asset.Load().StripBom().ShouldEqual("file1".ToBytes());
        }

        [Test]
        [TestCase(null, Mime.ApplicationJson, Mime.TextHtml)]
        [TestCase("fark/farker", "fark/farker", "fark/farker")]
        public void should_get_web_assets_from_a_resource(
            string mimeType, string jsonMimeType, string htmlMimeType)
        {
            var assets = WebAsset.FromResources(_thisAssembly,
                "Tests.Unit.Web.Assets.WebAssetTests",
                "url", mimeType, new[] { ".json", ".html" });

            assets.Count().ShouldEqual(2);

            var asset = assets.FirstOrDefault(x => x
                .GetUrl(_configuration).EndsWith("File1.json/"));
            asset.GetUrl(_configuration).ShouldEqual("api/url/Resources/File1.json/");
            asset.MimeType.ShouldEqual(jsonMimeType);
            asset.Filename.ShouldEqual("File1.json");
            asset.Load().StripBom().ShouldEqual("file1".ToBytes());

            asset = assets.FirstOrDefault(x => x
                .GetUrl(_configuration).EndsWith("File2.html/"));
            asset.GetUrl(_configuration).ShouldEqual("api/url/Resources/File2.html/");
            asset.MimeType.ShouldEqual(htmlMimeType);
            asset.Filename.ShouldEqual("File2.html");
            asset.Load().StripBom().ShouldEqual("file2".ToBytes());
        }

        [Test]
        public void should_create_lazy_url_from_web_asset()
        {
            var configuration = new Swank.Configuration.Configuration { AppUrl = "app/path1" };
            var url = new WebAsset("path2/file.html", "text/html", new StringAsset("data"))
                .ToLazyUrl(configuration);

            url.Filename.ShouldEqual("file.html");
            url.MimeType.ShouldEqual("text/html");
            url.GetUrl().ShouldEqual("/app/path1/path2/file.html/");
        }
    }
}
