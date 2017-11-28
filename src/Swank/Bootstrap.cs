using System;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using Swank.Configuration;
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

            var container = Registry.CreateContainer(configuration, 
                httpConfiguration.GetInstance);

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
                IgnorePaths.Initialize(Enumerable.Concat(configuration.IgnoreFolderUrls, 
                    new [] { configuration.AppUrl, configuration.SpecificationUrl }));
        }
    }
}
