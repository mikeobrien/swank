using System.Collections.Generic;
using Swank.Specification;
using Swank.Web.Templates;

namespace Swank.Web.Handlers.Templates
{ 
    public class TemplateModel
    {
        public List<Module> Specification { get; set; }
        public List<NamespaceModel> Namespaces { get; set; }
    }

    public class TemplateHandler : CachingHandlerBase
    {
        private readonly WebTemplate _template;
        private readonly SpecificationService _specification;
        private readonly NamespaceDescriptionService _namespaceDescriptionFactory;

        public TemplateHandler(WebTemplate template, 
            SpecificationService specification,
            NamespaceDescriptionService namespaceDescriptionFactory) : 
            base(template.MimeType)
        {
            _template = template;
            _specification = specification;
            _namespaceDescriptionFactory = namespaceDescriptionFactory;
        }

        protected override byte[] CreateResponse()
        {
            var specification = _specification.Generate();
            return _template.Render(new TemplateModel
            {
                Specification = specification,
                Namespaces = _namespaceDescriptionFactory.Create(specification)
            });
        }
    }
}
