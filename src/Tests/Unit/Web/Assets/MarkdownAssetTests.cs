using System.IO;
using System.Reflection;
using NUnit.Framework;
using Should;
using Swank.Extensions;
using Swank.Web.Assets;

namespace Tests.Unit.Web.Assets
{
    [TestFixture]
    public class MarkdownAssetTests
    {
        private const string Html = "<p><em>fark</em></p>";
        private string _path;

        [SetUp]
        public void Setup()
        {
            _path = Path.GetTempFileName();
            File.WriteAllText(_path, "*fark*");
        }

        [TearDown]
        public void TearDown()
        {
            File.Delete(_path);
        }

        [Test]
        public void should_read_string_from_virtual_path()
        {
            MarkdownAsset.FromVirtualPath(_path)
                .ReadString().ShouldEqual(Html);
        }

        [Test]
        public void should_read_bytes_from_virtual_path()
        {
            MarkdownAsset.FromVirtualPath(_path)
                .ReadBytes().ShouldEqual(Html.ToBytes());
        }

        [Test]
        public void should_read_string_from_resource()
        {
            MarkdownAsset.FromResource("MarkdownAssetTests", 
                Assembly.GetExecutingAssembly().AsList())
                .ReadString().ShouldEqual(Html);
        }

        [Test]
        public void should_read_bytes_from_resource()
        {
            MarkdownAsset.FromResource("MarkdownAssetTests",
                Assembly.GetExecutingAssembly().AsList())
                .ReadBytes().ShouldEqual(Html.ToBytes());
        }

        [Test]
        public void should_read_string_from_string()
        {
            MarkdownAsset.FromString("*fark*")
                .ReadString().ShouldEqual(Html);
        }

        [Test]
        public void should_read_bytes_from_string()
        {
            MarkdownAsset.FromString("*fark*")
                .ReadBytes().ShouldEqual(Html.ToBytes());
        }
    }
}
