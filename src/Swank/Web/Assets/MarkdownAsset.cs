using System.Collections.Generic;
using System.Reflection;
using Swank.Extensions;

namespace Swank.Web.Assets
{
    public class MarkdownAsset : IAsset
    {
        public const string Extension = ".md";

        private readonly IAsset _asset;

        public MarkdownAsset(IAsset asset)
        {
            _asset = asset;
        }

        public byte[] ReadBytes()
        {
            return ReadString().ToBytes();
        }

        public string ReadString()
        {
            return _asset?.ReadString()?.TransformMarkdownBlock();
        }

        public static MarkdownAsset FromVirtualPath(string virtualPath)
        {
            return new MarkdownAsset(FileAsset.FromVirtualPath(virtualPath));
        }

        public static MarkdownAsset FromResource(string name,
            IEnumerable<Assembly> assemblies)
        {
            return new MarkdownAsset(ResourceAsset.FindSingle(assemblies, name, Extension));
        }

        public static MarkdownAsset FromString(string source)
        {
            return new MarkdownAsset((StringAsset)source);
        }
    }
}