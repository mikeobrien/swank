using System;
using Swank.Extensions;
using Swank.Web.Assets;

namespace Swank.Web.Templates
{
    public class MustacheTemplate : ITemplate
    {
        public const string Extension = ".mustache";

        private readonly IAsset _asset;
        private readonly Configuration.Configuration _configuration;

        public MustacheTemplate(IAsset asset, Configuration.Configuration configuration)
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
                return _asset?.ReadString()?
                        .RemoveWhitespace()
                        .ConvertNbspHtmlEntityToSpaces()
                        .ConvertBrHtmlTagsToLineBreaks()
                        .RenderMustache(model);
            }
            catch (Exception exception)
            {
                return _configuration.DebugMode ? exception.ToString() : "Error rendering template. Enable debug mode to see more information.";
            }
        }

        public static MustacheTemplate FromString(string template, 
            Configuration.Configuration configuration)
        {
            return new MustacheTemplate((StringAsset)template, configuration);
        }
    }
}