using System.Net.Http;
using Swank.Extensions;
using Swank.Specification;

namespace Swank.Web.Handlers
{
    public class SpecificationHandler : CachingHandlerBase
    {
        private readonly Configuration.Configuration _configuration;
        private readonly SpecificationService _specification;

        public SpecificationHandler(Configuration.Configuration configuration,
            SpecificationService specification) : 
            base(configuration, Mime.ApplicationJson)
        {
            _configuration = configuration;
            _specification = specification;
        }

        protected override byte[] CreateResponse(HttpRequestMessage request)
        {
            var spec = _specification.Generate();
            spec.ForEach(x => _configuration.SpecPreRender?.Invoke(request, x));
            return spec.SerializeJson().ToBytes();
        }
    }
}
