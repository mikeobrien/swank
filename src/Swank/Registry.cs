using System;
using System.Collections.Generic;
using System.Reflection;
using Swank.Description;

namespace Swank
{
    public static class Registry
    {
        public static MicroContainer CreateContainer(Configuration
            .Configuration configuration, Func<Type, object> factory)
        {
            return MicroContainer.Create(x => x
                .RegisterFactory(factory)
                .Register(configuration)
                .Register<IApiExplorer>(configuration.ApiExplorer)
                .Register<IDescriptionConvention<IApiDescription, 
                    ModuleDescription>, ModuleConvention>()
                .Register<IDescriptionConvention<IApiDescription,
                    ResourceDescription>, ResourceConvention>()
                .Register<IDescriptionConvention<IApiDescription,
                    EndpointDescription>, EndpointConvention>()
                .Register<IDescriptionConvention<Type,
                    TypeDescription>, TypeConvention>()
                .Register<IDescriptionConvention<PropertyInfo,
                    MemberDescription>, MemberConvention>()
                .Register<IDescriptionConvention<IParameterDescription,
                    ParameterDescription>, ParameterConvention>()
                .Register<IDescriptionConvention<IApiDescription,
                    List<StatusCodeDescription>>, StatusCodeConvention>()
                .Register<IDescriptionConvention<IApiDescription,
                    List<HeaderDescription>>, HeaderConvention>()
                .Register<IDescriptionConvention<Type,
                    EnumDescription>, EnumConvention>()
                .Register<IDescriptionConvention<FieldInfo,
                    OptionDescription>, OptionConvention>());
        }
    }
}
