using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using System.Web.Http.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Swank.Extensions
{
    public static class WebExtensions
    {
        public static object GetInstance(this HttpConfiguration configuration, Type type)
        {
            return configuration.DependencyResolver.GetService(type) ??
                (configuration.Services.IsSingleService(type) ? 
                    configuration.Services.GetService(type) : null);
        }

        public static MethodInfo GetMethodInfo(this ApiParameterDescription description)
        {
            return description.ParameterDescriptor.ActionDescriptor.GetMethodInfo();
        }

        public static MethodInfo GetMethodInfo(this ApiDescription description)
        {
            return description.ActionDescriptor.GetMethodInfo();
        }

        public static MethodInfo GetMethodInfo(this HttpActionDescriptor descriptor)
        {
            var reflectedDescriptor = descriptor as ReflectedHttpActionDescriptor;
            if (reflectedDescriptor == null)
                throw new InvalidOperationException("Only supports ReflectedHttpActionDescriptor.");
            return reflectedDescriptor.MethodInfo;
        }

        public static bool HasControllerAttribute<T>(this ApiDescription description)
            where T : Attribute
        {
            return description.ActionDescriptor.ControllerDescriptor
                    .ControllerType.GetCustomAttributes<T>().Any();
        }

        public static bool HasControllerOrActionAttribute<T>(this ApiDescription description)
            where T : Attribute
        {
            return description.GetControllerAndActionAttributes<T>().Any();
        }

        public static IEnumerable<T> GetControllerAndActionAttributes<T>(
            this ApiDescription description) where T : Attribute
        {
            return description.ActionDescriptor.GetCustomAttributes<T>(true)
                .Concat(description.ActionDescriptor.ControllerDescriptor
                    .ControllerType.GetCustomAttributes<T>());
        }

        public static T GetActionAttribute<T>(this ApiDescription description)
            where T : Attribute
        {
            return description.ActionDescriptor.GetCustomAttributes<T>(true).FirstOrDefault();
        }

        public static T GetControllerAttribute<T>(this ApiDescription description)
            where T : Attribute
        {
            return description.ActionDescriptor.ControllerDescriptor
                .ControllerType.GetCustomAttributes<T>().FirstOrDefault();
        }

        public static bool HasAttribute<T>(this ApiParameterDescription description)
            where T : Attribute
        {
            return description.ParameterDescriptor?.GetCustomAttributes<T>().Any() ?? false;
        }

        public static T GetAttribute<T>(this ApiParameterDescription description)
            where T : Attribute
        {
            return description.ParameterDescriptor?.GetCustomAttributes<T>().FirstOrDefault();
        }

        public static ApiParameterDescription GetBodyParameter(this ApiDescription endpoint)
        {
            return endpoint.ParameterDescriptions.FirstOrDefault(
                x => x.Source == ApiParameterSource.FromBody);
        }

        public static Type GetControllerType(this ApiDescription description)
        {
            return description.ActionDescriptor.ControllerDescriptor.ControllerType;
        }

        public static Assembly GetControllerAssembly(this ApiDescription description)
        {
            return description.ActionDescriptor.ControllerDescriptor
                .ControllerType.Assembly;
        }

        public static Task<HttpResponseMessage> CreateHtmlResponseTask(this string content)
        {
            return content.CreateResponseTask("text/html");
        }

        public static Task<HttpResponseMessage> CreateJsonResponseTask(this string content)
        {
            return content.CreateResponseTask("application/json");
        }

        public static Task<HttpResponseMessage> CreateResponseTask(
            this byte[] content, string contentType)
        {
            return new HttpResponseMessage
            {
                Content = new ByteArrayContent(content)
                {
                    Headers =
                    {
                        ContentType = new MediaTypeHeaderValue(contentType)
                    }
                }
            }.ToTaskCompletionSource();
        }
        
        public static Task<HttpResponseMessage> CreateJsonResponseTask<T>(
            this T source)
        {
            return new HttpResponseMessage
            {
                Content = new StringContent(source.SerializeJson(), 
                    Encoding.UTF8, Mime.ApplicationJson)
            }.ToTaskCompletionSource();
        }

        public static Task<HttpResponseMessage> CreateResponseTask(
            this string content, string contentType)
        {
            return new HttpResponseMessage
            {
                Content = new StringContent(content, Encoding.UTF8, contentType)
            }.ToTaskCompletionSource();
        }

        public static Task<HttpResponseMessage> CreateErrorResponseTask(
            this HttpRequestMessage request, HttpStatusCode statusCode)
        {
            return request.CreateErrorResponse(statusCode, "")
                .ToTaskCompletionSource();
        }
  
        public static Task<HttpResponseMessage> CreateErrorResponseTask(
            this HttpRequestMessage request, HttpStatusCode statusCode, 
            Exception exception)
        {
            return request.CreateErrorResponse(statusCode, exception).ToTaskCompletionSource();
        }

        public static Task<HttpResponseMessage> CreateRedirectResponseTask(
            this HttpRequestMessage request, string url)
        {
            return new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.MovedPermanently,
                Headers = { Location = new Uri(url) }
            }.ToTaskCompletionSource();
        }

        public static Task<HttpResponseMessage> ToTaskCompletionSource(
            this HttpResponseMessage response)
        {
            var source = new TaskCompletionSource<HttpResponseMessage>();
            source.SetResult(response);
            return source.Task;
        }

        public static string GetRouteResourceIdentifier(this IHttpRoute route)
        {
            return "/" + Regex.Replace(route.RouteTemplate, "/*\\{.*?\\}", "").Trim('/');
        }

        public static Type GetRequestType(this ApiDescription endpoint)
        {
            return endpoint.GetRequestDescription()?.ParameterDescriptor?.ParameterType;
        }

        public static Type GetResponseType(this ApiDescription endpoint)
        {
            return endpoint.ResponseDescription.ResponseType ??
                endpoint.ResponseDescription.DeclaredType;
        }

        public static ApiParameterDescription GetRequestDescription(this ApiDescription endpoint)
        {
            return endpoint.ParameterDescriptions.FirstOrDefault(
                x => x.Source == ApiParameterSource.FromBody && 
                    !x.ParameterDescriptor.IsOptional);
        }

        public static bool IsUrlParameter(this ApiParameterDescription parameter, ApiDescription endpoint)
        {
            return parameter.Source == ApiParameterSource.FromUri &&
                endpoint.Route.RouteTemplate.Contains($"{{{parameter.Name}}}");
        }

        public static bool IsQuerystring(this ApiParameterDescription parameter, ApiDescription endpoint)
        {
            return parameter.Source == ApiParameterSource.FromUri &&
                !parameter.IsUrlParameter(endpoint);
        }

        public static string EnsureRooted(this string url)
        {
            return (!url.StartsWith("/") && 
                !url.StartsWith("~/")
                ? "~/" : "") + url;
        }

        public static string NormalizeUrlSlashes(this string url)
        {
            return url?.Replace("\\", "/").Trim();
        }

        public static string NormalizePathSlashes(this string url)
        {
            return url?.Replace("/", "\\").Trim();
        }

        public static string MapTempPath(this string path, string url)
        {
            return Path.Combine(path, url.NormalizePathSlashes().TrimStart('~', '\\'));
        }

        public static string MapPath(this string url)
        {
            return HostingEnvironment.MapPath(url
                .NormalizeUrlSlashes().EnsureRooted()) ??
                Path.GetTempPath().MapTempPath(url);
        }

        public static string CombineUrls(this string url, params string[] urls)
        {
            return url.AsList().Concat(urls)
                .Where(x => x != null)
                .Select(x => x.NormalizeUrlSlashes().Trim('/'))
                .Join("/");
        }
        
        public static string EnsureLeadingSlash(this string url)
        {
            if (url.IsNullOrEmpty()) return url;
            return "/" + url.TrimStart('/');
        }

        public static string EnsureTrailingSlash(this string url)
        {
            if (url.IsNullOrEmpty()) return url;
            return url.TrimEnd('/') + "/";
        }

        public static string MakeRelative(this string url, string rootUrl)
        {
            if (rootUrl.IsNullOrEmpty()) return url.NormalizeUrlSlashes();
            url = "uri:" + url.NormalizeUrlSlashes().Trim('/');
            rootUrl = "uri:" + rootUrl.NormalizeUrlSlashes().Trim('/') + "/";
            return new Uri(rootUrl).MakeRelativeUri(new Uri(url)).ToString();
        }

        public static Uri ParseUri(this string url)
        {
            return new Uri(url);
        }

        public static string JavaScriptStringEncode(this string value)
        {
            return HttpUtility.JavaScriptStringEncode(value);
        }

        public static object GetUrlParameter(
            this HttpRequestMessage request, string name)
        {
            var route = request.GetRouteData();
            if (route == null) return null;
            return route.Values.ContainsKey(name) ? 
                route.Values[name] : null;
        }

        public static string SerializeJson<T>(this T source)
        {
            return JsonConvert.SerializeObject(source, 
                Formatting.Indented, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }
            );
        }
    }
}
