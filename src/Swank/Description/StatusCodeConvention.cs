using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Description;
using Swank.Extensions;

namespace Swank.Description
{
    public class StatusCodeConvention : IDescriptionConvention<ApiDescription, List<StatusCodeDescription>>
    {
        public virtual List<StatusCodeDescription> GetDescription(ApiDescription endpoint)
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