using System.Linq;
using Swank.Extensions;

namespace Swank.Description
{
    public class ResourceConvention : IDescriptionConvention<IApiDescription, ResourceDescription>
    {
        private readonly MarkerConvention<ResourceDescription> _descriptions;
        private readonly XmlComments _xmlComments;

        public ResourceConvention(XmlComments xmlComments,
            MarkerConvention<ResourceDescription> descriptions)
        {
            _descriptions = descriptions;
            _xmlComments = xmlComments;
        }

        public virtual ResourceDescription GetDescription(IApiDescription endpoint)
        {
            if (endpoint.HasControllerAttribute<ResourceAttribute>())
            {
                var resource = endpoint.GetControllerAttribute<ResourceAttribute>();
                return new ResourceDescription
                {
                    Name = resource.Name,
                    Comments = resource.Comments ??
                        endpoint.ControllerType.Assembly.FindResourceNamed(
                            endpoint.ControllerType.FullName.AddMarkdownExtension())
                };
            }

            var xmlComments = _xmlComments.GetType(endpoint.ControllerType);

            if (xmlComments != null && xmlComments.Summary.IsNotNullOrEmpty())
            {
                return new ResourceDescription
                {
                    Name = xmlComments.Summary,
                    Comments = xmlComments.Remarks ??
                        endpoint.ControllerType.Assembly.FindResourceNamed(
                            endpoint.ControllerType.FullName.AddMarkdownExtension())
                };
            }

            return _descriptions.GetDescriptions(endpoint.ControllerType.Assembly)
                .Where(x => endpoint.ControllerType.IsInNamespace(x))
                .OrderByDescending(x => x.GetType().Namespace.Length)
                .FirstOrDefault();
        }
    }
}