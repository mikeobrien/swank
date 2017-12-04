using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using Bender.Collections;
using Swank;
using Swank.Configuration;
using Swank.Specification;

namespace TestHarness
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            var configuration = GlobalConfiguration.Configuration;
            //configuration.EnableSwank();
            configuration.AddSwankDocs(x => x
                //.WithAppAt("")
                //.CollapseModules()
                .IgnoreFolders("~/")
                .WithFavIconAt("/img/favicon.png")
                .WithPageTitle("Setec Astronomy")
                .WithLogoAt("/img/logo.png")
                .WithHeader("Setec Astronomy")
                .WithApiAt("https://www.setecastronomy.com")
                .WithCopyright("Copyright &copy; {{year}} Setec Astronomy")
                .WithOverviewLink("Authentication")
                .WithOverviewLink("Bindings")
                .WithOverviewLink("Data Types")
                .WithCodeExampleTheme(CodeExampleTheme.GithubGist)
                //.WithResourceIdentifier(r => r.ActionDescriptor.ControllerDescriptor.ControllerType.Namespace)
                //.WithNumericEnum()
                .AddXmlComments()
                .WithBasicAuthenticationScheme()
                .WithTokenAuthenticationScheme()
                //.OverrideResourcesWhen(r => r.Comments = $"This is the {r.Name.ToLower()} resource.", r => r.Comments.IsNullOrEmpty())
                //.HideXmlData()
                //.HideJsonData()
                //.HideTestDriveSection()
                .WithTemplateFromVirtualPath("Templates/Template.cshtml", "template", "text/plain")
                .WithTemplateFromVirtualPath("Templates/missing.cshtml", "missing/template", "text/plain")
                .IsInDebugModeWhenAppIsInDebugMode()
                .OverrideRequest(r => r.Message.Headers.Add(new Header
                {
                    Name = "Authorization",
                    Comments = "Lorem markdownum formam reicerer adstitit capiunt " +
                        "verbaque hac quam erat, vulnere :poop:",
                    IsAuth = true
                }))
                .OverrideRequestWhen(
                    r => { r.Message.Type = null; r.Message.IsBinary = true; }, 
                    r => r.ApiDescription.RequestParameter?.Type == typeof(Stream))
                .OverrideResponseWhen(
                    r => { r.Message.Type = null; r.Message.IsBinary = true; },
                    r => r.ApiDescription.RequestParameter?.Type == typeof(Stream))
                .OverrideRequestWhen(r => {
                    r.Message.Headers.AddItem(new Header { Name = "content-type", Comments = 
                        "Lorem markdownum formam reicerer adstitit capiunt " +
                        "verbaque hac quam erat, vulnere :poop:" });
                    r.Message.Headers.AddItem(new Header { Name = "accept", Comments = 
                        "Lorem markdownum formam reicerer adstitit capiunt " +
                        "verbaque hac quam erat, vulnere :poop:" });
                }, r => !r.Message.Headers.Any())
                .OverrideResponseWhen(r => {
                    r.Message.Headers.AddItem(new Header { Name = "content-type", Comments = 
                        "Lorem markdownum formam reicerer adstitit capiunt " +
                        "verbaque hac quam erat, vulnere :poop:" });
                    r.Message.Headers.AddItem(new Header { Name = "session-id", Comments = 
                        "Lorem markdownum formam reicerer adstitit capiunt " +
                        "verbaque hac quam erat, vulnere :poop:" });
                }, r => !r.Message.Headers.Any()));

            configuration.MapHttpAttributeRoutes();

            //configuration.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);
            configuration.Formatters.XmlFormatter.UseXmlSerializer = true;
            configuration.EnsureInitialized();
        }
    }
}