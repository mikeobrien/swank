using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Swank.Extensions;
using Swank.Web.Assets;

namespace Swank.Web.Templates
{
    public class WebTemplate
    {
        private static readonly string[] TemplateExtensions =
        {
            MustacheTemplate.Extension,
            RazorTemplate.Extension
        };

        private readonly string _url;
        private readonly ITemplate _template;

        public WebTemplate(string url, string mimeType, ITemplate template)
        {
            _template = template;
            _url = url;
            MimeType = mimeType;
        }
        
        public string MimeType { get; }

        public string GetUrl(Configuration.Configuration configuration)
        {
            return configuration.AppUrl.CombineUrls(_url);
        }

        public byte[] Render<TModel>(TModel model)
        {
            return _template.RenderBytes(model);
        }

        public static IEnumerable<WebTemplate> FromVirtualPath<TModel>(
            string virtualPath, string url, string mimeType, 
            Configuration.Configuration configuration)
        {
            return Map<TModel>(FileAsset.FromVirtualPath(virtualPath)
                .AsList<IFileAsset>(), url, mimeType, false, configuration);
        }

        public static IEnumerable<WebTemplate> InVirtualPath<TModel>(string virtualPath,
            string url, string mimeType, Configuration.Configuration configuration)
        {
            return Map<TModel>(FileAsset.InVirtualPath(virtualPath, null, TemplateExtensions), 
                url, mimeType, true, configuration);
        }

        public static IEnumerable<WebTemplate> FromResource<TModel>(
            IEnumerable<Assembly> assemblies, string path,
            string url, string mimeType, Configuration.Configuration configuration)
        {
            return Map<TModel>(ResourceAsset.FindSingle(assemblies, path, TemplateExtensions)
                .AsList<IFileAsset>(), url, mimeType, false, configuration);
        }

        public static IEnumerable<WebTemplate> FromResources<TModel>(
            IEnumerable<Assembly> assemblies, string path,
            string url, string mimeType, Configuration.Configuration configuration)
        {
            return Map<TModel>(ResourceAsset.FindUnder(assemblies, path, null, 
               TemplateExtensions), url, mimeType, true, configuration);
        }

        private static IEnumerable<WebTemplate> Map<TModel>(
            List<IFileAsset> assets, string url, string mimeType, 
            bool filename, Configuration.Configuration configuration)
        {
            return assets.Where(x => x != null)
                .Select(x => new WebTemplate(filename ? url.CombineUrls(
                    x.RelativePath.GetPathWithoutExtension().Replace(".", "_")) : url, 
                    mimeType, MapTemplate<TModel>(x, configuration)));
        }

        private static ITemplate MapTemplate<TModel>(IFileAsset asset, Configuration.Configuration configuration)
        {
            return asset.Path.MatchesExtensions(MustacheTemplate.Extension)
                ? (ITemplate) new MustacheTemplate(asset, configuration)
                : new RazorTemplate(asset, configuration).Compile<TModel>();
        }

        public static WebTemplate RazorFromString<TModel>(string url, string mimeType, 
            string template, Configuration.Configuration configuration)
        {
            return new WebTemplate(url, mimeType, RazorTemplate
                .FromString<TModel>(template, configuration));
        }

        public static WebTemplate MustacheFromString(string url, string mimeType, 
            string template, Configuration.Configuration configuration)
        {
            return new WebTemplate(url, mimeType, MustacheTemplate.FromString(template, configuration));
        }
    }
}