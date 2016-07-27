using System;
using System.Net.Http;
using System.Threading.Tasks;
using Swank.Extensions;

namespace Swank.Web.Handlers
{
    public abstract class CachingHandlerBase : HandlerBase
    {
        private readonly string _mimeType;
        private readonly Lazy<byte[]> _response;

        protected CachingHandlerBase(string mimeType, 
            bool forceTrailingSlash = false) : 
            base(forceTrailingSlash)
        {
            _mimeType = mimeType;
            _response = new Lazy<byte[]>(CreateResponse);
        }

        protected abstract byte[] CreateResponse();

        protected override Task<HttpResponseMessage> Send(HttpRequestMessage request)
        {
            return _response.Value.CreateResponseTask(_mimeType);
        }
    }
}