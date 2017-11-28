using System.Linq;
using Swank.Extensions;

namespace Swank.Description
{
    public class ModuleConvention : IDescriptionConvention<IApiDescription, ModuleDescription>
    {
        private readonly MarkerConvention<ModuleDescription> _descriptions;

        public ModuleConvention(MarkerConvention<ModuleDescription> descriptions)
        {
            _descriptions = descriptions;
        }

        public virtual ModuleDescription GetDescription(IApiDescription endpoint)
        {
            return _descriptions.GetDescriptions(endpoint.ControllerType.Assembly)
                .FirstOrDefault(x => endpoint.ControllerType.IsInNamespace(x));
        }
    }
}