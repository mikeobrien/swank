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
    public class ResourceAssetTests
    {
        private readonly Assembly _assembly = Assembly.GetExecutingAssembly();
        private const string ResourcePath = "Tests\\Unit\\Web\\Assets\\ResourceAssertResources";
        private const string ResourceNamespace = "Tests.Unit.Web.Assets.ResourceAssertResources";
        private const string ResourceFilenameMd = "ResourceAssetTests.md";
        private const string ResourceFilenameTxt = "ResourceAssetTests.txt";
        private const string RelativePathMd = "Resources\\" + ResourceFilenameMd;
        private const string RelativePathTxt = "Resources\\" + ResourceFilenameTxt;

        [Test]
        public void should_generate_path([Values(ResourcePath, 
            ResourceNamespace)] string path)
        {
            ResourceAsset.FindSingle(_assembly.AsList(), ResourceFilenameMd).Path
                .ShouldEqual(Path.Combine(ResourcePath, RelativePathMd));
        }

        [Test]
        public void should_read_string()
        {
            ResourceAsset.FindSingle(_assembly.AsList(), ResourceFilenameMd)
                .ReadString().ShouldEqual("md");
        }

        [Test]
        public void should_read_bytes()
        {
            ResourceAsset.FindSingle(_assembly
                .AsList(), ResourceFilenameTxt)
                .ReadBytes().StripBom().ShouldEqual("txt".ToBytes());
        }

        [Test]
        public void should_find_a_resource_by_the_full_path()
        {
            ResourceAsset.FindSingle(_assembly
                .AsList(), Path.Combine(ResourcePath, RelativePathMd))
                .ReadString().ShouldEqual("md");
        }

        [Test]
        public void should_find_a_resource_by_the_filename_with_an_extension()
        {
            ResourceAsset.FindSingle(_assembly
                .AsList(), "ResourceAssetTests.md")
                .ReadString().ShouldEqual("md");
        }

        [Test]
        public void should_find_a_resource_by_the_filename_with_an_extension_filtered_by_extension()
        {
            ResourceAsset.FindSingle(_assembly
                .AsList(), "ResourceAssetTests.md", ".md")
                .ReadString().ShouldEqual("md");
        }

        [Test]
        public void should_find_a_resource_by_the_filename_without_an_extension_filtered_by_extension()
        {
            ResourceAsset.FindSingle(_assembly
                .AsList(), "ResourceAssetTests", ".md")
                .ReadString().ShouldEqual("md");
        }
        
        [Test]
        public void should_multiple_resources_by_the_filename_with_an_extension()
        {
            var results = ResourceAsset.FindMany(_assembly
                .AsList(), "ResourceAssetTests.md");
            results.Count.ShouldEqual(1);
            results.First().ReadString().ShouldEqual("md");
        }
        [Test]
        public void should_multiple_resources_by_the_filename_without_an_extension_filtered_by_extension()
        {
            var results = ResourceAsset.FindMany(_assembly
                .AsList(), "ResourceAssetTests", ".txt");
            results.Count.ShouldEqual(1);
            results.First().ReadString().ShouldEqual("txt");
        }

        [Test]
        public void should_not_return_resources_if_no_extensions_match()
        {
            ResourceAsset.FindUnder(_assembly.AsList(), 
                "/", null, "fark").ShouldBeEmpty();
        }

        [Test]
        public void should_return_resources([Values(ResourcePath,
            ResourceNamespace)] string path)
        {
            var results = ResourceAsset.FindUnder(_assembly.AsList(),
                path, null, ".md", ".txt");

            var result = results.FirstOrDefault(x => x.Path == 
                Path.Combine(ResourcePath, RelativePathMd));
            result.ShouldNotBeNull();
            result.RelativePath.ShouldEqual(RelativePathMd.NormalizeUrlSlashes());

            result = results.FirstOrDefault(x => x.Path ==
                Path.Combine(ResourcePath, RelativePathTxt));
            result.ShouldNotBeNull();
            result.RelativePath.ShouldEqual(RelativePathTxt.NormalizeUrlSlashes());
        }

        [Test]
        public void should_return_resources_with_specific_name_under_namespace(
            [Values(ResourcePath, ResourceNamespace)] string path)
        {
            var results = ResourceAsset.FindUnder(_assembly.AsList(), 
                Path.Combine(path, "Resources"),
                ResourceFilenameMd.GetFileNameWithoutExtension(),
                ResourceFilenameMd.GetExtension()).Select(x => x.Path);

            results.Count().ShouldEqual(1);
            results.ShouldContain(Path.Combine(ResourcePath, RelativePathMd));
        }
    }
}
