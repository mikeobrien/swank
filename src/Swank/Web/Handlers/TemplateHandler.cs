using Swank.Specification;
using Swank.Web.Templates;

namespace Swank.Web.Handlers
{ 
    public class TemplateHandler : CachingHandlerBase
    {
        private readonly WebTemplate _template;
        private readonly SpecificationService _specification;

        public TemplateHandler(WebTemplate template, 
            SpecificationService specification) : 
            base(template.MimeType)
        {
            _template = template;
            _specification = specification;
        }

        protected override byte[] CreateResponse()
        {
            return _template.Render(_specification.Generate());
        }
    }
}
