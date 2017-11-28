using System.Net;
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
            var asset = _asset.Load();
            return asset == null
                ? request.CreateResponseTask(HttpStatusCode.NotFound)
                : asset.CreateResponseTask(_asset.MimeType);
        }
    }
}
