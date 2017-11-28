using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Web.Http.Description;
using Swank.Extensions;

namespace Swank.Description.WebApi
{
    public class WebApiDescription : IApiDescription
    {
        private readonly ApiDescription _apiDescription;

        public WebApiDescription(ApiDescription apiDescription)
        {
            ActionMethod = apiDescription.GetMethodInfo();
            RequestParameter = GetRequestParameter(apiDescription);
            _apiDescription = apiDescription;
        }

        private static IParameterDescription GetRequestParameter(ApiDescription apiDescription)
        {
            var requestParameter = apiDescription.GetRequestDescription();
            return requestParameter == null 
                ? null 
                : new WebApiParameterDescription(apiDescription, requestParameter);
        }

        public string Id => _apiDescription.ID;
        public string Name => _apiDescription.ActionDescriptor.ActionName;
        public string Documentation => _apiDescription.Documentation;
        public HttpMethod HttpMethod => _apiDescription.HttpMethod;
        public string RouteTemplate => _apiDescription.Route.RouteTemplate;
        public string RelativePath => _apiDescription.RelativePath;
        public MethodInfo ActionMethod { get; }
        public Type ControllerType => _apiDescription.ActionDescriptor
            .ControllerDescriptor.ControllerType;
        public Type ResponseType => _apiDescription.GetResponseType();
        public string ResponseDocumentation => _apiDescription
            .ResponseDescription?.Documentation;
        public IParameterDescription RequestParameter { get; }
        
        public IEnumerable<IParameterDescription> ParameterDescriptions => 
            _apiDescription.ParameterDescriptions.Select(x => 
                new WebApiParameterDescription(_apiDescription, x));

        public T GetActionAttribute<T>() where T : Attribute =>
            _apiDescription.GetActionAttribute<T>();

        public T GetControllerAttribute<T>() where T : Attribute =>
            _apiDescription.GetControllerAttribute<T>();

        public bool HasControllerAttribute<T>() where T : Attribute =>
            _apiDescription.HasControllerAttribute<T>();

        public bool HasControllerOrActionAttribute<T>() where T : Attribute =>
             _apiDescription.HasControllerOrActionAttribute<T>();

        public IEnumerable<T> GetControllerAndActionAttributes<T>() where T : Attribute =>
            _apiDescription.GetControllerAndActionAttributes<T>();
    }
}
