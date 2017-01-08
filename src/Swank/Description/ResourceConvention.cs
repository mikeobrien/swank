using System.Linq;
using System.Web.Http.Description;
using Swank.Extensions;

namespace Swank.Description
{
    public class ResourceConvention : IDescriptionConvention<ApiDescription, ResourceDescription>
    {
        private readonly MarkerConvention<ResourceDescription> _descriptions;
        private readonly XmlComments _xmlComments;

        public ResourceConvention(XmlComments xmlComments,
            MarkerConvention<ResourceDescription> descriptions)
        {
            _descriptions = descriptions;
            _xmlComments = xmlComments;
        }

        public virtual ResourceDescription GetDescription(ApiDescription endpoint)
        {
            if (endpoint.HasControllerAttribute<ResourceAttribute>())
            {
                var resource = endpoint.GetControllerAttribute<ResourceAttribute>();
                return new ResourceDescription
                {
                    Name = resource.Name,
                    Comments = resource.Comments ??
                        endpoint.GetControllerAssembly().FindResourceNamed(
                            endpoint.GetControllerType().FullName.AddMarkdownExtension())
                };
            }

            var xmlComments = _xmlComments.GetType(endpoint.GetControllerType());

            if (xmlComments != null && xmlComments.Summary.IsNotNullOrEmpty())
            {
                return new ResourceDescription
                {
                    Name = xmlComments.Summary,
                    Comments = xmlComments.Remarks ??
                        endpoint.GetControllerAssembly().FindResourceNamed(
                            endpoint.GetControllerType().FullName.AddMarkdownExtension())
                };
            }

            return _descriptions.GetDescriptions(endpoint.GetControllerAssembly())
                .Where(x => endpoint.GetControllerType().IsInNamespace(x))
                .OrderByDescending(x => x.GetType().Namespace.Length)
                .FirstOrDefault();
        }
    }
}