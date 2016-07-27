using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Swank.Specification;
using Module = Swank.Specification.Module;

namespace Tests.Common
{
    public static class SpecExtensions
    {
        public static Endpoint GetEndpoint<T>(this List<Module> modules,
            Expression<Func<T, object>> action)
        {
            var method = action.GetMethodInfo();
            var url = method.ToTestUrl(method.GetParameters()
                .Select(x => x.Name).Skip(1).Take(1).ToArray());
            return modules.SelectMany(x => x.Resources)
                .SelectMany(x => x.Endpoints).FirstOrDefault(x => x.UrlTemplate == url);
        }

        public static Resource GetResource<T>(this List<Module> modules,
            Expression<Func<T, object>> action)
        {
            var endpoint = modules.GetEndpoint(action);
            return modules.SelectMany(x => x.Resources)
                .FirstOrDefault(x => x.Endpoints.Contains(endpoint));
        }

        public static bool HasModuleByName(this IEnumerable<Module> modules, string name)
        {
            return modules.Any(x => x.Name == name);
        }

        public static Module GetModuleByName(this IEnumerable<Module> modules, string name)
        {
            return modules.SingleOrDefault(x => x.Name == name);
        }

        public static UrlParameter GetUrlParameter(this Endpoint endpoint, string name)
        {
            return endpoint.UrlParameters.Single(x => x.Name == name);
        }

        public static QuerystringParameter GetQuerystring(this Endpoint endpoint, string name)
        {
            return endpoint.QuerystringParameters.Single(x => x.Name == name);
        }

        public static string ToTestUrl<T>(this Expression<Func<T, object>> action, 
            params string[] urlParameters)
        {
            return action.GetMethodInfo().ToTestUrl(urlParameters);
        }

        public static string ToTestUrl(this MethodInfo method, params string[] urlParameters)
        {
            var type = method.DeclaringType;
            return urlParameters.Aggregate(type.Namespace
                .Replace("Tests.Unit.", "").Replace(".", "/") +
                    $"/{type.Name}" + $"/{method.Name}",
                 (a, i) => $"{a}/{{{i}}}");
        }
    }
}
