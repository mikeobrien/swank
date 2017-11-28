using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Swank.Extensions;

namespace Swank.Web.Handlers
{
    public abstract class HandlerBase : HttpMessageHandler
    {
        private readonly bool _forceTrailingSlash;

        protected HandlerBase(bool forceTrailingSlash = false)
        {
            _forceTrailingSlash = forceTrailingSlash;
        }

        protected abstract Task<HttpResponseMessage> Send(HttpRequestMessage request);

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (_forceTrailingSlash)
            {
                var url = request.RequestUri.OriginalString;
                if (!url.EndsWith("/"))
                    return request.CreateRedirectResponseTask(url + "/");
            }

            return Send(request);
        }
    }
}