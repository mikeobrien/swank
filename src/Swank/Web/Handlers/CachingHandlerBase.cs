using System;
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

        protected virtual object GetData(HttpRequestMessage request)
        {
            return null;
        }

        protected virtual string GetCacheKey(HttpRequestMessage request, object data)
        {
            return "";
        }

        protected virtual byte[] CreateResponse(HttpRequestMessage request)
        {
            throw new NotImplementedException();
        }

        protected virtual byte[] CreateResponse(HttpRequestMessage request, object data)
        {
            return CreateResponse(request);
        }

        protected override Task<HttpResponseMessage> Send(HttpRequestMessage request)
        {
            var data = GetData(request);
            var cacheKey = GetCacheKey(request, data) ?? "";
            cacheKey += (cacheKey != "" ? "-" : "") + 
                _configuration.CacheKey(request);
            byte[] response;
            if (!Cache.ContainsKey(cacheKey))
            {
                response = CreateResponse(request, data);
                Cache.TryAdd(cacheKey, response);
            }
            else response = Cache[cacheKey];

            return response == null
                ? request.CreateResponseTask(HttpStatusCode.NotFound)
                : response.CreateResponseTask(_mimeType);
        }
    }
}