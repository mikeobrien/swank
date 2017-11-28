using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;

namespace Swank.Description
{
    public interface IApiDescription
    {
        string Id { get; }
        string Name { get; }
        string Documentation { get; }
        HttpMethod HttpMethod { get; }
        string RouteTemplate { get; }
        string RelativePath { get; }
        MethodInfo ActionMethod { get; }
        Type ControllerType { get; }
        IParameterDescription RequestParameter { get; }
        Type ResponseType { get; }
        string ResponseDocumentation { get; }
        IEnumerable<IParameterDescription> ParameterDescriptions { get; }

        T GetActionAttribute<T>() where T : Attribute;
        T GetControllerAttribute<T>() where T : Attribute;
        bool HasControllerAttribute<T>() where T : Attribute;
        bool HasControllerOrActionAttribute<T>() where T : Attribute;
        IEnumerable<T> GetControllerAndActionAttributes<T>() where T : Attribute;
    }
}
