using System.Collections.Generic;
using System.Linq;

namespace Swank.Description
{
    public class HeaderConvention : IDescriptionConvention<IApiDescription, List<HeaderDescription>>
    {
        public virtual List<HeaderDescription> GetDescription(IApiDescription endpoint)
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