using System;
using System.Collections.Generic;
using System.Linq;
using Swank.Extensions;

namespace Swank.Description.WebApi
{
    public class WebApiExplorer : IApiExplorer
    {
        private Lazy<List<IApiDescription>> _descriptions;

        public WebApiExplorer(System.Web.Http.Description.IApiExplorer apiExplorer)
        {
            _descriptions = apiExplorer.ToLazy(x => x.ApiDescriptions
                .Select(d => new WebApiDescription(d))
                .Cast<IApiDescription>()
                .ToList());
        }

        public IEnumerable<IApiDescription> ApiDescriptions => _descriptions.Value;
    }
}
