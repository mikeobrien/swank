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
        IApiParameterDescription RequestParameter { get; }
        Type ResponseType { get; }
        string ResponseDocumentation { get; }
        IEnumerable<IApiParameterDescription> ParameterDescriptions { get; }

        T GetActionAttribute<T>() where T : Attribute;
        T GetControllerAttribute<T>() where T : Attribute;
        T GetControllerOrActionAttribute<T>() where T : Attribute;
        IEnumerable<T> GetActionAttributes<T>() where T : Attribute;
        IEnumerable<T> GetControllerAttributes<T>() where T : Attribute;
        IEnumerable<T> GetControllerAndActionAttributes<T>() where T : Attribute;

        bool HasActionAttribute<T>() where T : Attribute;
        bool HasControllerAttribute<T>() where T : Attribute;
        bool HasControllerOrActionAttribute<T>() where T : Attribute;
    }

    public static class IApiDescriptionExtensions
    {
        public static bool IsPost(this IApiDescription description)
        {
            return description.HttpMethod == HttpMethod.Post;
        }

        public static bool IsPut(this IApiDescription description)
        {
            return description.HttpMethod == HttpMethod.Put;
        }

        public static bool HasRequestOfType<T>(this IApiDescription description)
        {
            return description.RequestParameter?.Type == typeof(T);
        }

        public static bool HasRequest(this IApiDescription description)
        {
            return description.RequestParameter != null;
        }

        public static bool HasResponseOfType<T>(this IApiDescription description)
        {
            return description.ResponseType == typeof(T);
        }

        public static bool HasResponse(this IApiDescription description)
        {
            return description.ResponseType != null;
        }
    }
}
