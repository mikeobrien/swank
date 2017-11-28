using System.Collections.Generic;

namespace Swank.Description
{
    public interface IApiExplorer
    {
        IEnumerable<IApiDescription> ApiDescriptions { get; }
    }
}