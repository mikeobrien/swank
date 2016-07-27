﻿using System;
using System.IO;
using System.Web;
using System.Web.Http;
using Swank;
using Swank.Configuration;
using Swank.Extensions;

namespace TestHarness
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            var configuration = GlobalConfiguration.Configuration;
            //configuration.EnableSwank();
            configuration.EnableSwank(x => x
                //.WithAppAt("")
                .WithFavIconAt("/img/favicon.png")
                .WithPageTitle("Setec Astronomy")
                .WithLogoAt("/img/logo.png")
                .WithHeader("Setec Astronomy")
                .WithApiAt("https://www.setecastronomy.com")
                .WithCopyright("Copyright &copy; {year} Setec Astronomy")
                .WithOverviewLink("Authentication")
                .WithOverviewLink("Bindings")
                .WithOverviewLink("Data Types")
                .WithCodeExampleTheme(CodeExampleTheme.GithubGist)
                //.WithResourceIdentifier(r => r.ActionDescriptor.ControllerDescriptor.ControllerType.Namespace)
                //.WithNumericEnum()
                .AddXmlComments()
                //.OverrideResourcesWhen(r => r.Comments = $"This is the {r.Name.ToLower()} resource.", r => r.Comments.IsNullOrEmpty())
                //.HideXmlData()
                //.HideJsonData()
                .IsInDebugModeWhenAppIsInDebugMode()
                .OverrideRequestWhen(
                    (a, d) => { d.Type = null; d.IsBinary = true; }, 
                    (a, d) => a.GetRequestType() == typeof(Stream))
                .OverrideResponseWhen(
                    (a, d) => { d.Type = null; d.IsBinary = true; },
                    (a, d) => a.GetResponseType() == typeof(Stream)));

            configuration.MapHttpAttributeRoutes();

            //configuration.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

            configuration.EnsureInitialized();
        }
    }
}