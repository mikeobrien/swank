using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Should;
using Swank.Extensions;
using Swank.Web.Assets;

namespace Tests.Unit.Web.Assets
{
    [TestFixture]
    public class FileAssetTests
    {
        private string _path1;
        private string _path2;
        private string _relativePath;

        [SetUp]
        public void Setup()
        {
            _relativePath = Path.Combine(Guid.NewGuid().ToString("N"), "fark");
            var filesPath = Path.Combine(Path.GetTempPath(), _relativePath);
            Directory.CreateDirectory(filesPath);
            _path1 = Path.Combine(filesPath, "fark1.md");
            _path2 = Path.Combine(filesPath, "fark2.md");
            File.WriteAllText(_path1, "fark");
            File.WriteAllText(_path2, "farker");
        }

        [TearDown]
        public void TearDown()
        {
            File.Delete(_path1);
            File.Delete(_path2);
        }

        [Test]
        public void should_read_string()
        {
            new FileAsset(_path1, null).ReadString().ShouldEqual("fark");
        }

        [Test]
        public void should_read_bytes()
        {
            new FileAsset(_path1, null).ReadBytes().ShouldEqual("fark".ToBytes());
        }

        [Test]
        public void should_return_file_in_virtual_path()
        {
            var relativePath = Path.Combine(_relativePath, _path1.GetFileName());
            var result = FileAsset.FromVirtualPath(relativePath);
            
            result.ShouldNotBeNull();
            result.Path.ShouldEqual(_path1);
            result.RelativePath.ShouldEqual("fark1.md");
        }

        [Test]
        public void should_not_return_files_in_virtual_path_if_no_extensions_match()
        {
            FileAsset.InVirtualPath("/", null, "fark").ShouldBeEmpty();
        }

        [Test]
        public void should_return_files_in_virtual_path()
        {
            var results = FileAsset.InVirtualPath("/", null, 
                _path1.GetExtension()).OrderBy(x => x.Path).ToList();

            var result = results.FirstOrDefault(x => x.Path == _path1);
            result.ShouldNotBeNull();
            result.RelativePath.ShouldEqual(Path.Combine(_relativePath, 
                _path1.GetFileName()).NormalizeUrlSlashes());

            result = results.FirstOrDefault(x => x.Path == _path2);
            result.ShouldNotBeNull();
            result.RelativePath.ShouldEqual(Path.Combine(_relativePath, 
                _path2.GetFileName()).NormalizeUrlSlashes());
        }

        [Test]
        public void should_return_files_in_virtual_path_with_specific_name()
        {
            var results = FileAsset.InVirtualPath("/", 
                _path1.GetFileNameWithoutExtension(),
                _path1.GetExtension()).Select(x => x.Path);

            results.Count().ShouldEqual(1);
            results.ShouldContain(_path1);
        }
    }
}
