using System;
using System.Reflection;
using System.Web.Http.Description;
using Swank.Extensions;

namespace Swank.Description.WebApi
{
    public class WebApiParameterDescription : IParameterDescription
    {
        private readonly ApiParameterDescription _parameterDescription;

        public WebApiParameterDescription(ApiDescription apiDescription, 
            ApiParameterDescription parameterDescription)
        {
            _parameterDescription = parameterDescription;
            IsUrlParameter = parameterDescription.IsUrlParameter(apiDescription);
            IsQuerystring = parameterDescription.IsQuerystring(apiDescription);
            ActionMethod = apiDescription.GetMethodInfo();
        }

        public string Name => _parameterDescription.Name;
        public string Documentation => _parameterDescription.Documentation;
        public object DefaultValue => _parameterDescription.ParameterDescriptor.DefaultValue;
        public Type Type => _parameterDescription.ParameterDescriptor.ParameterType;
        public bool IsOptional => _parameterDescription.ParameterDescriptor.IsOptional;
        public bool IsUrlParameter { get; }
        public bool IsQuerystring { get; }
        public MethodInfo ActionMethod { get; }

        public T GetAttribute<T>() where T : Attribute =>
            _parameterDescription.GetAttribute<T>();

        public bool HasAttribute<T>() where T : Attribute =>
            _parameterDescription.HasAttribute<T>();
    }
}