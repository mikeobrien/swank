using System;
using System.IO;
using System.Web;
using System.Web.Http;
using Swank;
using Swank.Configuration;
using Swank.Extensions;
using Swank.Specification;

namespace DemoSite
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs eventArgs)
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
                .WithDefaultDictionaryKeyName("key")
                .OverrideResources(r => r.Comments = HipsterIpsum.Generate(2))
                .OverrideEndpoints((a, e) =>
                {
                    e.Secure = true;
                    e.Comments = HipsterIpsum.Generate(2);
                    e.Request.Headers.Add(new Header { Name = "authorization", Comments = HipsterIpsum.Generate(2) });
                    e.Response.Headers.Add(new Header { Name = "x-xss-protection", Comments = HipsterIpsum.Generate(2) });
                    e.StatusCodes.Add(new StatusCode { Code = 401, Name = "Unauthorized", Comments = HipsterIpsum.Generate(2) });
                    e.StatusCodes.Add(new StatusCode { Code = 403, Name = "Forbidden", Comments = HipsterIpsum.Generate(2) });
                    e.StatusCodes.Add(new StatusCode { Code = 418, Name = "I'm a teapot", Comments = HipsterIpsum.Generate(2) });
                    e.Response.Comments = HipsterIpsum.Generate(5);
                })
                .OverrideEndpointsWhen((a, e) =>
                {
                    e.Request.Comments = HipsterIpsum.Generate(5);
                }, (a, e) => e.Method != "DELETE")
                .OverrideEndpointsWhen((a, e) =>
                {
                    e.Request.Headers.Add(new Header { Name = "content-type", Comments = HipsterIpsum.Generate(2) });
                    e.Request.Headers.Add(new Header { Name = "accept", Comments = HipsterIpsum.Generate(2) });
                    e.Response.Headers.Add(new Header { Name = "content-type", Comments = HipsterIpsum.Generate(2) });
                    
                }, (a, e) => e.Method != "GET")
                .OverrideEndpointsWhen((a, e) =>
                {
                    e.StatusCodes.Add(new StatusCode { Code = 201, Name = "Created", Comments = HipsterIpsum.Generate(2) });
                }, (a, e) => e.Method == "POST")
                .OverrideUrlParameters((a, p, u) => u.Comments = HipsterIpsum.Generate(2))
                .OverrideQuerystring((a, p, q) => q.Comments = HipsterIpsum.Generate(2))
                .OverrideQuerystringWhen((a, p, q) => q.Required = false, (a, p, q) => q.Name.Contains("optional"))
                .OverrideQuerystringWhen((a, p, q) => q.Required = true, (a, p, q) => !q.Name.Contains("optional"))
                .OverrideQuerystringWhen((a, p, q) => q.MultipleAllowed = true, (a, p, q) => !q.Name.Contains("Multiple"))
                .OverrideQuerystringWhen((a, p, q) => q.DefaultValue = "5", (a, p, q) => !q.Name.Contains("Default"))
                .OverrideTypes((t, d) => d.Comments = HipsterIpsum.Generate(2))
                .OverrideMembers((p, m) => m.Comments = HipsterIpsum.Generate(2))
                .OverrideMembersWhen((p, m) => m.Optional = true, (p, m) => m.Name.Contains("Optional"))
                .OverrideMembersWhen((p, m) => m.DefaultValue = "fark", (p, m) => m.Name.Contains("Default"))
                .OverrideMembersWhen((p, m) =>
                {
                    m.Deprecated = true;
                    m.DeprecationMessage = HipsterIpsum.Generate(2);
                }, (p, m) => m.Name.Contains("Depricated"))
                .OverrideMembersWhen((p, m) =>
                {
                    m.Comments = HipsterIpsum.Generate(2);
                    m.Type.ArrayItem.Comments = HipsterIpsum.Generate(2);
                }, (p, m) => m.Type.IsArray)
                .OverrideMembersWhen((p, m) =>
                {
                    m.Comments = HipsterIpsum.Generate(2);
                    m.Type.DictionaryEntry.KeyComments = HipsterIpsum.Generate(2);
                    m.Type.DictionaryEntry.ValueComments = HipsterIpsum.Generate(2);
                }, (p, m) => m.Type.IsDictionary)
                .OverrideOptions((p, o) => o.Comments = HipsterIpsum.Generate(2))
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