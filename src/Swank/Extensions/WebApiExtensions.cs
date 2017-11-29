using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Description;

namespace Swank.Extensions
{
    public static class Extensions
    {
        public static object GetInstance(this HttpConfiguration configuration, Type type)
        {
            return configuration.DependencyResolver.GetService(type) ??
                   (configuration.Services.IsSingleService(type) ? 
                       configuration.Services.GetService(type) : null);
        }

        public static bool HasActionAttribute<T>(this ApiDescription description)
            where T : Attribute
        {
            return description.GetActionAttributes<T>().Any();
        }

        public static T GetActionAttribute<T>(this ApiDescription description)
            where T : Attribute
        {
            return description.GetActionAttributes<T>().FirstOrDefault();
        }

        public static IEnumerable<T> GetActionAttributes<T>(this ApiDescription description)
            where T : Attribute
        {
            return description.ActionDescriptor.GetCustomAttributes<T>(true);
        }

        public static MethodInfo GetMethodInfo(this ApiDescription description)
        {
            return description.ActionDescriptor?.GetMethodInfo();
        }

        public static MethodInfo GetMethodInfo(this HttpActionDescriptor descriptor)
        {
            var reflectedDescriptor = descriptor as ReflectedHttpActionDescriptor;
            if (reflectedDescriptor == null)
                throw new InvalidOperationException("Only supports ReflectedHttpActionDescriptor.");
            return reflectedDescriptor.MethodInfo;
        }

        public static string GetRouteResourceIdentifier(this string routeTemplate)
        {
            return "/" + Regex.Replace(routeTemplate, "/*\\{.*?\\}", "").Trim('/');
        }

        public static bool HasControllerOrActionAttribute<T>(this ApiDescription description)
            where T : Attribute
        {
            return description.GetControllerAndActionAttributes<T>().Any();
        }

        public static T GetControllerOrActionAttribute<T>(this ApiDescription description)
            where T : Attribute
        {
            return description.GetControllerAndActionAttributes<T>().FirstOrDefault();
        }

        public static IEnumerable<T> GetControllerAndActionAttributes<T>(
            this ApiDescription description) where T : Attribute
        {
            return description.ActionDescriptor.GetCustomAttributes<T>(true)
                .Concat(description.ActionDescriptor.ControllerDescriptor
                    .ControllerType.GetCustomAttributes<T>());
        }
        
        public static bool IsUrlParameter(this ApiParameterDescription parameter, ApiDescription endpoint)
        {
            return parameter.Source == ApiParameterSource.FromUri &&
                   (endpoint.Route.RouteTemplate.Contains($"{{{parameter.Name}}}") ||
                    endpoint.Route.RouteTemplate.Contains($"{{*{parameter.Name}}}"));
        }

        public static bool IsQuerystring(this ApiParameterDescription parameter, ApiDescription endpoint)
        {
            return parameter.Source == ApiParameterSource.FromUri &&
                   !parameter.IsUrlParameter(endpoint);
        }

        public static Type GetResponseType(this ApiDescription endpoint)
        {
            return endpoint.ResponseDescription.ResponseType ??
                   endpoint.ResponseDescription.DeclaredType;
        }

        public static bool HasControllerAttribute<T>(this ApiDescription description)
            where T : Attribute
        {
            return description.GetControllerAttributes<T>().Any();
        }

        public static T GetControllerAttribute<T>(this ApiDescription description)
            where T : Attribute
        {
            return description.GetControllerAttributes<T>().FirstOrDefault();
        }

        public static IEnumerable<T> GetControllerAttributes<T>(this ApiDescription description)
            where T : Attribute
        {
            return description.ActionDescriptor.ControllerDescriptor
                .ControllerType.GetCustomAttributes<T>();
        }

        public static bool HasAttribute<T>(this ApiParameterDescription description)
            where T : Attribute
        {
            return description.ParameterDescriptor?.GetCustomAttributes<T>().Any() ?? false;
        }

        public static T GetAttribute<T>(this ApiParameterDescription description)
            where T : Attribute
        {
            return description.GetAttributes<T>().FirstOrDefault();
        }

        public static IEnumerable<T> GetAttributes<T>(this ApiParameterDescription description)
            where T : Attribute
        {
            return description.ParameterDescriptor?.GetCustomAttributes<T>();
        }
        
        public static ApiParameterDescription GetRequestDescription(this ApiDescription endpoint)
        {
            return endpoint.ParameterDescriptions.FirstOrDefault(
                x => x.Source == ApiParameterSource.FromBody && 
                     !x.ParameterDescriptor.IsOptional);
        }

        public static Type GetRequestType(this ApiDescription endpoint)
        {
            return endpoint.GetRequestDescription()?.ParameterDescriptor?.ParameterType;
        }
    }
}
