using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Description;
using Swank.Configuration;
using Swank.Description;
using Swank.Extensions;
using Swank.Web.Handlers;
using Swank.Web.Handlers.App;
using Swank.Web.Handlers.Templates;

namespace Swank
{
    public static class Bootstrap
    {
        public static void Swank(this HttpConfiguration httpConfiguration,
            Action<ConfigurationDsl> configure = null)
        {
            var configuration = new Configuration
                .Configuration(Assembly.GetCallingAssembly());
            configure?.Invoke(new ConfigurationDsl(configuration));

            var container = MicroContainer.Create(x => x
                .RegisterFactory(httpConfiguration.GetInstance)
                .Register(configuration)
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
                    OptionDescription>, OptionConvention>());

            configuration.Assets.ForEach(x =>
            {
                var url = x.GetUrl(configuration);
                httpConfiguration.Routes.MapHttpRoute(
                    name: url, routeTemplate: url.TrimStart('/'),
                    defaults: null, constraints: null,
                    handler: new AssetHandler(x));
            });

            configuration.Templates.ForEach(x =>
            {
                var url = x.GetUrl(configuration);
                httpConfiguration.Routes.MapHttpRoute(
                    name: url, routeTemplate: url.TrimStart('/'),
                    defaults: null, constraints: null,
                    handler: container.CreateInstance<TemplateHandler>(x));
            });

            httpConfiguration.Routes.MapHttpRoute(
                name: configuration.AppUrl,
                routeTemplate: configuration.AppUrl.TrimStart('/'),
                defaults: null, constraints: null,
                handler: container.GetInstance<AppHandler>());

            httpConfiguration.Routes.MapHttpRoute(
                name: configuration.AppUrl.CombineUrls("resources"),
                routeTemplate: configuration.AppUrl.CombineUrls("resources").TrimStart('/'),
                defaults: null, constraints: null,
                handler: container.GetInstance<AppResourceHandler>());

            httpConfiguration.Routes.MapHttpRoute(
                name: configuration.SpecificationUrl,
                routeTemplate: configuration.SpecificationUrl.TrimStart('/'),
                defaults: null, constraints: null,
                handler: container.GetInstance<SpecificationHandler>());

            if (configuration.IgnoreFolders)
                IgnorePaths.Initialize(configuration.AppUrl, configuration.SpecificationUrl);
        }
    }
}
