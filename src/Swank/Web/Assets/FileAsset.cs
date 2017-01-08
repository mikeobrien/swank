using System.Collections.Generic;
using System.IO;
using System.Linq;
using Swank.Extensions;

namespace Swank.Web.Assets
{
    public class FileAsset : IFileAsset
    {
        public FileAsset(string path, string relativePath)
        {
            Path = path;
            RelativePath = relativePath;
        }

        public string Path { get; }
        public string RelativePath { get; }

        public byte[] ReadBytes()
        {
            return File.Exists(Path) ? File.ReadAllBytes(Path) : null;
        }

        public string ReadString()
        {
            return File.Exists(Path) ? File.ReadAllText(Path) : null;
        }

        public static FileAsset FromPath(string path)
        {
            return new FileAsset(path, path.GetFileName());
        }

        public static FileAsset FromVirtualPath(string virtualPath)
        {
            return FromPath(virtualPath.MapPath());
        }

        public static List<IFileAsset> InVirtualPath(string virtualPath, 
            string filename, params string[] extensions)
        {
            var actualPath = virtualPath.MapPath();
            return actualPath.GetFiles($"{filename ?? "*"}.*", SearchOption.AllDirectories)
                .Where(x => x.MatchesExtensions(extensions))
                .Select(x => new FileAsset(x, x.MakeRelative(actualPath)))
                .Cast<IFileAsset>().ToList();
        }
    }
}