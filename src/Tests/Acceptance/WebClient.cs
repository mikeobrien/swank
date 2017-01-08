using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using Bender;
using Swank.Extensions;

namespace Tests.Acceptance
{
    public static class WebClient
    {
        public class Result<T>
        {
            public Result(HttpStatusCode status, string error)
            {
                Status = status;
                Error = error;
            }

            public Result(HttpStatusCode status, T data)
            {
                Status = status;
                Data = data;
            }

            public string Error { get; }
            public HttpStatusCode Status { get; }
            public T Data { get; }
        }

        public static Result<string> GetText(string relativeUrl)
        {
            return Get(relativeUrl, "text/plain", 
                x => new StreamReader(x).ReadToEnd());
        }

        public static Result<string> GetHtml(string relativeUrl)
        {
            return Get(relativeUrl, "text/html",
                x => new StreamReader(x).ReadToEnd());
        }

        public static Result<T> GetJson<T>(string relativeUrl)
        {
            return Get(relativeUrl, "application/json", x => Deserialize.Json<T>(x, y => y.UseCamelCaseNaming()));
        }

        private static Result<T> Get<T>(string relativeUrl, 
            string accept, Func<Stream, T> deserialize)
        {
            var request = (HttpWebRequest)WebRequest.Create(BuildUrl(relativeUrl));
            request.Method = "GET";
            request.Accept = accept;
            using (var response = GetResponse(request))
            {
                using (var responseStream = response.GetResponseStream())
                {
                if ((int)response.StatusCode >= 300)
                    return new Result<T>(response.StatusCode, responseStream.ReadAllText());
                    return new Result<T>(response.StatusCode, deserialize(responseStream));
                }
            }
        }

        public static Result<TResponse> PostJson<TRequest, TResponse>(string relativeUrl, TRequest data)
        {
            var request = (HttpWebRequest)WebRequest.Create(BuildUrl(relativeUrl));
            request.Method = "POST";
            request.ContentType = request.Accept = "application/json";
            using (var requestStream = request.GetRequestStream())
                Serialize.JsonStream(data, requestStream);
            using (var response = GetResponse(request))
            {
                using (var responseStream = response.GetResponseStream())
                {
                    if ((int)response.StatusCode >= 300)
                        return new Result<TResponse>(response.StatusCode, responseStream.ReadAllText());
                    return new Result<TResponse>(response.StatusCode, 
                        Deserialize.Json<TResponse>(responseStream, 
                            x => x.Deserialization(y => y.IgnoreNameCase())));
                }
            }
        }

        private static HttpWebResponse GetResponse(HttpWebRequest request)
        {
            try
            {
                return (HttpWebResponse)request.GetResponse();
            }
            catch (WebException e)
            {
                var response = e.Response as HttpWebResponse;
                if (response == null) throw;
                return response;
            }
        }

        private static string BuildUrl(string relativeUrl)
        {
            // Fiddler can't hook into localhost so when its running 
            // you can use localhost.fiddler
            var host = Process.GetProcessesByName("Fiddler").Any() ?
                    "localhost.fiddler" : "localhost";
            return $"http://{host}:61960/{relativeUrl}";
        }
    }
}
