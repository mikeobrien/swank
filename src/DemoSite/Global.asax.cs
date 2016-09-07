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
            configuration.Swank(x => x
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
                .OverrideResources(r => r.Resource.Comments = HipsterIpsum.Generate(2))
                .OverrideEndpoints(e =>
                {
                    e.Endpoint.Secure = true;
                    e.Endpoint.Comments = HipsterIpsum.Generate(2);
                    e.Endpoint.Request.Headers.Add(new Header { Name = "authorization", Comments = HipsterIpsum.Generate(2) });
                    e.Endpoint.Response.Headers.Add(new Header { Name = "x-xss-protection", Comments = HipsterIpsum.Generate(2) });
                    e.Endpoint.StatusCodes.Add(new StatusCode { Code = 401, Name = "Unauthorized", Comments = HipsterIpsum.Generate(2) });
                    e.Endpoint.StatusCodes.Add(new StatusCode { Code = 403, Name = "Forbidden", Comments = HipsterIpsum.Generate(2) });
                    e.Endpoint.StatusCodes.Add(new StatusCode { Code = 418, Name = "I'm a teapot", Comments = HipsterIpsum.Generate(2) });
                    e.Endpoint.Response.Comments = HipsterIpsum.Generate(5);
                })
                .OverrideEndpointsWhen(e =>
                {
                    e.Endpoint.Request.Comments = HipsterIpsum.Generate(5);
                }, e => e.Endpoint.Method != "DELETE")
                .OverrideEndpointsWhen(e =>
                {
                    e.Endpoint.Request.Headers.Add(new Header { Name = "content-type", Comments = HipsterIpsum.Generate(2) });
                    e.Endpoint.Request.Headers.Add(new Header { Name = "accept", Comments = HipsterIpsum.Generate(2) });
                    e.Endpoint.Response.Headers.Add(new Header { Name = "content-type", Comments = HipsterIpsum.Generate(2) });
                    
                }, e => e.Endpoint.Method != "GET")
                .OverrideEndpointsWhen(e =>
                {
                    e.Endpoint.StatusCodes.Add(new StatusCode { Code = 201, Name = "Created", Comments = HipsterIpsum.Generate(2) });
                }, e => e.Endpoint.Method == "POST")
                .OverrideUrlParameters(u => u.UrlParameter.Comments = HipsterIpsum.Generate(2))
                .OverrideQuerystring(q => q.Querystring.Comments = HipsterIpsum.Generate(2))
                .OverrideQuerystringWhen(q => q.Querystring.Required = false, q => q.Querystring.Name.Contains("optional"))
                .OverrideQuerystringWhen(q => q.Querystring.Required = true, q => !q.Querystring.Name.Contains("optional"))
                .OverrideQuerystringWhen(q => q.Querystring.MultipleAllowed = true, q => !q.Querystring.Name.Contains("Multiple"))
                .OverrideQuerystringWhen(q => q.Querystring.DefaultValue = "5", q => !q.Querystring.Name.Contains("Default"))
                .OverrideTypes(d => d.DataType.Comments = HipsterIpsum.Generate(2))
                .OverrideMembers(m => m.Member.Comments = HipsterIpsum.Generate(2))
                .OverrideMembersWhen(m => m.Member.Optional = true, m => m.Member.Name.Contains("Optional"))
                .OverrideMembersWhen(m => m.Member.DefaultValue = "fark", m => m.Member.Name.Contains("Default"))
                .OverrideMembersWhen(m =>
                {
                    m.Member.Deprecated = true;
                    m.Member.DeprecationMessage = HipsterIpsum.Generate(2);
                }, m => m.Member.Name.Contains("Depricated"))
                .OverrideMembersWhen(m =>
                {
                    m.Member.Comments = HipsterIpsum.Generate(2);
                    m.Member.Type.ArrayItem.Comments = HipsterIpsum.Generate(2);
                }, m => m.Member.Type.IsArray)
                .OverrideMembersWhen(m =>
                {
                    m.Member.Comments = HipsterIpsum.Generate(2);
                    m.Member.Type.DictionaryEntry.KeyComments = HipsterIpsum.Generate(2);
                    m.Member.Type.DictionaryEntry.ValueComments = HipsterIpsum.Generate(2);
                }, m => m.Member.Type.IsDictionary)
                .OverrideOptions(o => o.Option.Comments = HipsterIpsum.Generate(2))
                //.HideXmlData()
                //.HideJsonData()
                .IsInDebugModeWhenAppIsInDebugMode()
                .OverrideRequestWhen(
                    r => { r.Message.Type = null; r.Message.IsBinary = true; }, 
                    r => r.ApiDescription.GetRequestType() == typeof(Stream))
                .OverrideResponseWhen(
                    r => { r.Message.Type = null; r.Message.IsBinary = true; },
                    r => r.ApiDescription.GetResponseType() == typeof(Stream)));

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