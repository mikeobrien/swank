using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Swank.Extensions;

namespace Swank.Web.Handlers
{
    public abstract class CachingHandlerBase : HandlerBase
    {
        private readonly string _mimeType;
        private readonly Configuration.Configuration _configuration;
        private static readonly ConcurrentDictionary<string, byte[]> Cache = 
            new ConcurrentDictionary<string, byte[]>();

        protected CachingHandlerBase(Configuration.Configuration configuration,
             string mimeType, bool forceTrailingSlash = false) : 
            base(forceTrailingSlash)
        {
            _configuration = configuration;
            _mimeType = mimeType;
        }

        protected abstract byte[] CreateResponse(HttpRequestMessage request);

        protected override Task<HttpResponseMessage> Send(HttpRequestMessage request)
        {
            var cacheKey = _configuration.CacheKey(request);
            byte[] response;
            if (!Cache.ContainsKey(cacheKey))
            {
                response = CreateResponse(request);
                Cache.TryAdd(cacheKey, response);
            }
            else response = Cache[cacheKey];

            return response == null
                ? request.CreateResponseTask(HttpStatusCode.NotFound)
                : response.CreateResponseTask(_mimeType);
        }
    }
}