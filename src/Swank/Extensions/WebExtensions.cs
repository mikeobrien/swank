using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Swank.Extensions
{
    public static class WebExtensions
    {
        public static Task<HttpResponseMessage> CreateTextResponseTask(this string content)
        {
            return content.CreateResponseTask("text/plain");
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
            this string content, string contentType)
        {
            return new HttpResponseMessage
            {
                Content = new StringContent(content, Encoding.UTF8, contentType)
            }.ToTaskCompletionSource();
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
            this T source) where T : class
        {
            return new HttpResponseMessage
            {
                Content = new StringContent(source.SerializeJson(), 
                    Encoding.UTF8, Mime.ApplicationJson)
            }.ToTaskCompletionSource();
        }

        public static Task<HttpResponseMessage> CreateResponseTask(
            this HttpRequestMessage request, HttpStatusCode statusCode)
        {
            return request.CreateResponse(statusCode)
                .ToTaskCompletionSource();
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

        public static List<string> GetNamespaceFromRoute(this string routeTemplate)
        {
            routeTemplate = routeTemplate.Split('?').First();
            return routeTemplate.Split(new [] { '/' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(x => !x.Contains("{") && !x.Contains("}")).ToList();
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

        public static string GetSubdomain(this Uri url)
        {
            return url.Host.Split(".").First();
        }

        public static string GetRootDomain(this Uri url)
        {
            var parts = url.Host.Split(".");
            return parts.Count() < 3 ? url.Host : 
                parts.Skip(parts.Length - 2).Join(".");
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

        public static string SerializeJson<T>(this T source) where T : class
        {
            if (source == null) return null;
            return JsonConvert.SerializeObject(source, 
                Formatting.Indented, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }
            );
        }

        public static bool IsGet(this HttpMethod method)
        {
            return method == HttpMethod.Get;
        }

        public static bool IsPost(this HttpMethod method)
        {
            return method == HttpMethod.Post;
        }

        public static bool IsPut(this HttpMethod method)
        {
            return method == HttpMethod.Put;
        }

        public static bool IsPostOrPut(this HttpMethod method)
        {
            return method.IsPost() || method.IsPut();
        }

        public static bool IsDelete(this HttpMethod method)
        {
            return method == HttpMethod.Delete;
        }
        
        private static readonly Regex HtmlEntityRegex = new Regex("&(#)?([a-zA-Z0-9]*);");

        public static string HtmlDecode(this string html)
        {
            if (html.IsNullOrEmpty()) return html;
            return HtmlEntityRegex.Replace(html, x => x.Groups[1].Value == "#"
                ? ((char)int.Parse(x.Groups[2].Value)).ToString()
                : HttpUtility.HtmlDecode(x.Groups[0].Value));
        }

        public static string StripHtml(this string html)
        {
            return html.IsNotNullOrEmpty() ? Regex.Replace(html, "<.*?>", "") : html;
        }
    }
}
