using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Swank.Extensions;

namespace Swank.Web.Assets
{
    public class WebAsset
    {
        private readonly string _url;
        private readonly IAsset _asset;

        public WebAsset(string url, string mimeType, IAsset asset)
        {
            _asset = asset;
            _url = url;
            MimeType = mimeType;
            Filename = url.GetFileName();
        }
        
        public string MimeType { get; }
        public string Filename { get; }

        public string GetUrl(Configuration.Configuration configuration)
        {
            return configuration.AppUrl.CombineUrls(_url)
                .EnsureTrailingSlash();
        }

        public byte[] Load()
        {
            return _asset.ReadBytes();
        }

        public static WebAsset FromVirtualPath(string virtualPath, 
            string url, string mimeType = null)
        {
            return MapSingle(FileAsset.FromVirtualPath(virtualPath), url, mimeType);
        }

        public static IEnumerable<WebAsset> InVirtualPath(string virtualPath, 
            string url, string mimeType, string[] extensions)
        {
            return MapMany(FileAsset.InVirtualPath(virtualPath, 
                null, extensions), url, mimeType);
        }

        public static WebAsset FromResource(IEnumerable<Assembly> assemblies, 
            string path, string url, string mimeType = null)
        {
            return MapSingle(ResourceAsset.FindSingle(assemblies, path), url, mimeType);
        }

        public static IEnumerable<WebAsset> FromResources(
            IEnumerable<Assembly> assemblies, string path, 
            string url, string mimeType, params string[] extensions)
        {
            return MapMany(ResourceAsset.FindUnder(assemblies, 
                path, null, extensions), url, mimeType);
        }

        private static WebAsset MapSingle(IFileAsset asset, string url, string mimeType = null)
        {
            return asset == null ? null : new WebAsset(url.CombineUrls(asset.RelativePath),
                mimeType ?? asset.Path.GetExtension().GetMimeTypeFromExtension(), asset);
        }

        private static IEnumerable<WebAsset> MapMany(
            List<IFileAsset> assets, string url, string mimeType)
        {
            return assets.Select(x => new WebAsset(url.CombineUrls(x.RelativePath),
                mimeType ?? x.Path.GetExtension().GetMimeTypeFromExtension(), x));
        }

        public static WebAsset FromString(string url, string mimeType, string content)
        {
            return new WebAsset(url, mimeType, (StringAsset)content);
        }
    }

    public static class WebAssetExtensions
    {
        public static WebAsset FindByFilename(this IEnumerable<WebAsset> assets, string filename)
        {
            return assets.First(x => x.Filename == filename || 
                x.Filename.GetFileNameWithoutExtension() == filename);
        }

        public static LazyUrl ToLazyUrl(this WebAsset asset, Configuration.Configuration configuration)
        {
            return new LazyUrl(() => asset.GetUrl(configuration).MakeAbsolute(),
                asset.Filename, asset.MimeType);
        }
    }
}