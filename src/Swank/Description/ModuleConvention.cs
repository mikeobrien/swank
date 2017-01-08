using System.Linq;
using System.Web.Http.Description;
using Swank.Extensions;

namespace Swank.Description
{
    public class ModuleConvention : IDescriptionConvention<ApiDescription, ModuleDescription>
    {
        private readonly MarkerConvention<ModuleDescription> _descriptions;

        public ModuleConvention(MarkerConvention<ModuleDescription> descriptions)
        {
            _descriptions = descriptions;
        }

        public virtual ModuleDescription GetDescription(ApiDescription endpoint)
        {
            return _descriptions.GetDescriptions(endpoint.GetControllerAssembly())
                .FirstOrDefault(x => endpoint.GetControllerType().IsInNamespace(x));
        }
    }
}