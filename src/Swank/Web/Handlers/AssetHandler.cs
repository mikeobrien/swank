using System.Net.Http;
using System.Threading.Tasks;
using Swank.Extensions;
using Swank.Web.Assets;

namespace Swank.Web.Handlers
{
    public class AssetHandler : HandlerBase
    {
        private readonly WebAsset _asset;

        public AssetHandler(WebAsset asset)
        {
            _asset = asset;
        }

        protected override Task<HttpResponseMessage> Send(HttpRequestMessage request)
        {
            return _asset.Load().CreateResponseTask(_asset.MimeType);
        }
    }
}
