using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Swank.Extensions;
using Swank.Specification;
using Swank.Web.Templates;

namespace Swank.Web.Handlers.Templates
{ 
    public class TemplateModel
    {
        public List<Module> Specification { get; set; }
        public List<NamespaceModel> Namespaces { get; set; }
        public Dictionary<string, string> Values { get; set; }
    }

    public class TemplateHandler : HandlerBase
    {
        private readonly WebTemplate _template;
        private readonly SpecificationService _specification;
        private readonly NamespaceDescriptionService _namespaceDescriptionFactory;

        public TemplateHandler(WebTemplate template, 
            SpecificationService specification,
            NamespaceDescriptionService namespaceDescriptionFactory)
        {
            _template = template;
            _specification = specification;
            _namespaceDescriptionFactory = namespaceDescriptionFactory;
        }

        protected override Task<HttpResponseMessage> Send(HttpRequestMessage request)
        {
            var specification = _specification.Generate();
            return _template.Render(new TemplateModel
            {
                Specification = specification,
                Namespaces = _namespaceDescriptionFactory.Create(specification),
                Values = request.GetQueryNameValuePairs()
                    .ToDictionary(x => x.Key, x => x.Value, 
                        StringComparer.OrdinalIgnoreCase)
            }).CreateResponseTask(_template.MimeType);
        }
    }
}
