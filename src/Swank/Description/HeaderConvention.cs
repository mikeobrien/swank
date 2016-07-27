using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Description;
using Swank.Extensions;

namespace Swank.Description
{
    public class HeaderConvention : IDescriptionConvention<ApiDescription, List<HeaderDescription>>
    {
        public virtual List<HeaderDescription> GetDescription(ApiDescription endpoint)
        {
            return endpoint.GetControllerAndActionAttributes<HeaderAttribute>()
                .Select(x => new HeaderDescription
                {
                    Direction = x is RequestHeaderAttribute ? 
                        HttpDirection.Request : HttpDirection.Response,
                    Name = x.Name,
                    Comments = x.Comments,
                    Optional = x.Optional
                }).OrderBy(x => x.Direction).ThenBy(x => x.Name).ToList();
        }
    }
}