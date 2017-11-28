using System.Collections.Generic;
using System.Linq;

namespace Swank.Description
{
    public class StatusCodeConvention : IDescriptionConvention<IApiDescription, List<StatusCodeDescription>>
    {
        public virtual List<StatusCodeDescription> GetDescription(IApiDescription endpoint)
        {
            return endpoint.GetControllerAndActionAttributes<StatusCodeAttribute>()
                .Select(x => new StatusCodeDescription
                {
                    Code = x.Code,
                    Name = x.Name,
                    Comments = x.Comments
                }).OrderBy(x => x.Code).ToList();
        }
    }
}