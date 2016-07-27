using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Should;
using Swank.Web.Assets;

namespace Tests.Unit.Web.Assets
{
    [TestFixture]
    public class LazyUrlTests
    {
        [Test]
        [TestCase(null, "image/png")]
        [TestCase("fark/farker", "fark/farker")]
        public void should_create_lazy_url_from_string(
            string mimeType, string expectedMimeType)
        {
            var url = "path/file.png".ToLazyUrl(mimeType);

            url.Filename.ShouldEqual("file.png");
            url.MimeType.ShouldEqual(expectedMimeType);
            url.GetUrl().ShouldEqual("path/file.png");
        }

        [Test]
        public void should_remove_by_filename(
            [Values("content.html", "content")] string filename)
        {
            var urls = new List<LazyUrl>
            {
                (LazyUrl) "path/content.html",
                (LazyUrl) "path/image.png"
            };

            urls.RemoveByFilename(filename);

            urls.Count.ShouldEqual(1);
            urls.First().Filename.ShouldEqual("image.png");
        }

        [Test]
        public void should_prepend_url_when_target_exists(
            [Values("image.png", "image")] string filename)
        {
            var urls = new List<LazyUrl>
            {
                (LazyUrl) "path/content.html",
                (LazyUrl) "path/image.png"
            };

            urls.PrependOrAdd((LazyUrl)"path/font.woff", filename);

            urls.Count.ShouldEqual(3);
            urls.First().Filename.ShouldEqual("content.html");
            urls.Skip(1).First().Filename.ShouldEqual("font.woff");
            urls.Skip(2).First().Filename.ShouldEqual("image.png");
        }

        [Test]
        public void should_append_url_when_target_does_not_exist()
        {
            var urls = new List<LazyUrl>
            {
                (LazyUrl) "path/content.html",
                (LazyUrl) "path/image.png"
            };

            urls.PrependOrAdd((LazyUrl)"path/font.woff", "farker");

            urls.Count.ShouldEqual(3);
            urls.First().Filename.ShouldEqual("content.html");
            urls.Skip(1).First().Filename.ShouldEqual("image.png");
            urls.Skip(2).First().Filename.ShouldEqual("font.woff");
        }
    }
}
