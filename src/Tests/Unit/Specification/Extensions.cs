using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Http.Description;
using Swank;
using Swank.Configuration;
using Swank.Description;
using Swank.Extensions;
using Swank.Specification;
using Tests.Common;

namespace Tests.Unit.Specification
{
    public static class Builder
    {
        public static Endpoint BuildSpecAndGetEndpoint
            <TController>(Expression<Func<TController, object>> action, 
                Action<ConfigurationDsl> configure = null)
        {
            return BuildSpec<TController>(configure)
                .GetEndpoint(action);
        }

        public static List<Swank.Specification.Module> BuildSpec
            <TNamespace>(Action<ConfigurationDsl> configure = null)
        {
            return BuildSpec<TNamespace>(ConfigurationDsl.CreateConfig(x =>
            {
                configure?.Invoke(x);
                x.Where(y => y.GetControllerType().InNamespace<TNamespace>());
            }));
        }

        public static List<Swank.Specification.Module> BuildSpec
            <TNamespace>(Swank.Configuration.Configuration configuration)
        {
            return MicroContainer.Create(x => x
                .Register(configuration)
                .Register<IApiExplorer>(ApiDescription<TNamespace>.Discover())
                .Register<IDescriptionConvention<ApiDescription,
                    ModuleDescription>, ModuleConvention>()
                .Register<IDescriptionConvention<ApiDescription,
                    ResourceDescription>, ResourceConvention>()
                .Register<IDescriptionConvention<ApiDescription,
                    EndpointDescription>, EndpointConvention>()
                .Register<IDescriptionConvention<Type,
                    TypeDescription>, TypeConvention>()
                .Register<IDescriptionConvention<PropertyInfo,
                    MemberDescription>, MemberConvention>()
                .Register<IDescriptionConvention<ApiParameterDescription,
                    ParameterDescription>, ParameterConvention>()
                .Register<IDescriptionConvention<ApiDescription,
                    List<StatusCodeDescription>>, StatusCodeConvention>()
                .Register<IDescriptionConvention<ApiDescription,
                    List<HeaderDescription>>, HeaderConvention>()
                .Register<IDescriptionConvention<Type,
                    EnumDescription>, EnumConvention>()
                .Register<IDescriptionConvention<FieldInfo,
                    OptionDescription>, OptionConvention>())
                .GetInstance<Swank.Specification.SpecificationService>().Generate();
        }

        public static TypeGraphService BuildTypeGraphService(
            Action<Swank.Configuration.Configuration> configure = null,
            Swank.Configuration.Configuration configuration = null)
        {
            configuration = configuration ?? new Swank.Configuration.Configuration();
            configure?.Invoke(configuration);
            return MicroContainer.Create(x => x
                .Register(configuration)
                .Register<IDescriptionConvention<Type, TypeDescription>, TypeConvention>()
                .Register<IDescriptionConvention<PropertyInfo, MemberDescription>, MemberConvention>()
                .Register<IDescriptionConvention<Type, EnumDescription>, EnumConvention>()
                .Register<IDescriptionConvention<FieldInfo, OptionDescription>, OptionConvention>())
                .GetInstance<TypeGraphService>();
        }
    }
}
