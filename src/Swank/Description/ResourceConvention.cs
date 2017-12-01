using System.Linq;
using Swank.Extensions;

namespace Swank.Description
{
    public class ResourceConvention : IDescriptionConvention<IApiDescription, ResourceDescription>
    {
        private readonly MarkerConvention<ResourceDescription> _markerDescriptions;
        private readonly XmlComments _xmlComments;

        public ResourceConvention(XmlComments xmlComments,
            MarkerConvention<ResourceDescription> markerDescriptions)
        {
            _markerDescriptions = markerDescriptions;
            _xmlComments = xmlComments;
        }

        public virtual ResourceDescription GetDescription(IApiDescription endpoint)
        {
            var markerDescription = _markerDescriptions
                .GetDescriptions(endpoint.ControllerType.Assembly)
                .Where(x => endpoint.ControllerType.IsInNamespace(x))
                .OrderByDescending(x => x.GetType().Namespace?.Length)
                .FirstOrDefault();

            var name = markerDescription?.Name;
            var comments = markerDescription?.Comments ??
                endpoint.ControllerType.Assembly.FindResourceNamed(
                    $"{endpoint.ControllerType.Namespace}.Resource".AddMarkdownExtension());

            if (endpoint.HasControllerAttribute<ResourceAttribute>())
            {
                var resource = endpoint.GetControllerAttribute<ResourceAttribute>();
                name = resource.Name ?? name;
                comments = resource.Comments ??
                    endpoint.ControllerType.Assembly.FindResourceNamed(
                        endpoint.ControllerType.FullName.AddMarkdownExtension()) ?? 
                    comments;
            }

            var xmlComments = _xmlComments.GetType(endpoint.ControllerType);

            name = xmlComments?.Summary ?? name;
            comments = xmlComments?.Remarks ?? comments;

            return name.IsNotNullOrEmpty() || comments.IsNotNullOrEmpty()
                ?  new ResourceDescription
                    {
                        Name = name,
                        Comments = comments
                    }
                : null;
        }
    }
}