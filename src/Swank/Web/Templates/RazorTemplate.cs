using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Swank.Extensions;
using Swank.Web.Assets;

namespace Swank.Web.Templates
{
    public class RazorTemplate : ITemplate
    {
        public const string Extension = ".cshtml";

        private readonly IAsset _asset;
        private readonly Configuration.Configuration _configuration;

        public RazorTemplate(IAsset asset, Configuration.Configuration configuration)
        {
            _asset = asset;
            _configuration = configuration;
        }

        public byte[] RenderBytes<TModel>(TModel model)
        {
            return RenderString(model).ToBytes();
        }

        public string RenderString<TModel>(TModel model)
        {
            try
            {
                return _asset.WhenNotNull(x => x.ReadString())
                    .WhenNotNull(x => x.RenderRazor(model))
                    .OtherwiseDefault();
            }
            catch (Exception exception)
            {
                return _configuration.DebugMode ? exception.Message
                    .Split("List of loaded Assemblies").First() : "";
            }
        }

        public RazorTemplate Compile<TModel>()
        {
            _asset.ReadString().CompileRazor<TModel>();
            return this;
        }

        public static RazorTemplate FromResourceInThisAssembly<TModel>(
            string name, Configuration.Configuration configuration)
        {
            return FromResource<TModel>(name, Assembly
                .GetCallingAssembly().AsList(), configuration);
        }

        public static RazorTemplate FromResource<T, TModel>(string name, 
            Configuration.Configuration configuration)
        {
            return FromResource<TModel>(name, typeof(T).Assembly.AsList(), configuration);
        }

        public static RazorTemplate FromResource<TModel>(string name,
            IEnumerable<Assembly> assemblies, 
            Configuration.Configuration configuration)
        {
            return new RazorTemplate(ResourceAsset.FindSingle(assemblies, 
                name, Extension), configuration).Compile<TModel>();
        }

        public static RazorTemplate FromVirtualPath<TModel>(string virtualPath, 
            Configuration.Configuration configuration)
        {
            return new RazorTemplate(FileAsset.FromVirtualPath(virtualPath), 
                configuration).Compile<TModel>();
        }

        public static RazorTemplate FromString<TModel>(string template,
            Configuration.Configuration configuration)
        {
            return new RazorTemplate((StringAsset)template, configuration).Compile<TModel>();
        }
    }
}