using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Http.Description;
using Swank;
using Swank.Configuration;
using Swank.Description;
using Swank.Specification;
using Tests.Common;
using IApiExplorer = Swank.Description.IApiExplorer;

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
                x.Where(y => y.ControllerType.InNamespace<TNamespace>());
            }));
        }

        public static List<Swank.Specification.Module> BuildSpec
            <TNamespace>(Swank.Configuration.Configuration configuration)
        {
            return MicroContainer.Create(x => x
                .Register(configuration)
                .Register<IApiExplorer>(ApiDescription<TNamespace>.Discover())
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
