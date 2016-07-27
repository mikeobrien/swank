using Swank.Extensions;
using Swank.Specification;

namespace Swank.Web.Handlers
{
    public class SpecificationHandler : CachingHandlerBase
    {
        private readonly SpecificationService _specification;

        public SpecificationHandler(
            SpecificationService specification) : 
            base(Mime.ApplicationJson)
        {
            _specification = specification;
        }

        protected override byte[] CreateResponse()
        {
            return _specification.Generate().SerializeJson().ToBytes();
        }
    }
}
