using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using System.Web.Http.Routing;
using NSubstitute;
using Swank.Extensions;

namespace Tests.Common
{
    public static class ApiDescription<T>
    {
        public static ApiDescription ForAction<TReturn>(
            Expression<Func<T, TReturn>> method,
            Action<ApiDescription> configure = null)
        {
            return ForAction(method.GetMethodInfo(), configure);
        }

        private static readonly string[] ActionMethods = 
            { "Get", "Post", "Put", "Delete" };

        public static IApiExplorer Discover()
        {
            var apiExplorer = Substitute.For<IApiExplorer>();
            apiExplorer.ApiDescriptions.Returns(new Collection<ApiDescription>(
                typeof(T).Assembly.GetTypes()
                    .Where(x => x.IsInNamespace(typeof(T)) && 
                        x.Name.EndsWith("Controller"))
                    .SelectMany(x => x.GetMethods())
                    .Where(x => ActionMethods.Contains(x.Name))
                    .Select(x => ForAction(x)).ToList()));
            return apiExplorer;
        }

        private static ApiDescription ForAction(MethodInfo method, 
            Action<ApiDescription> configure = null)
        {
            var actionDescriptor = new ReflectedHttpActionDescriptor(
                new HttpControllerDescriptor(new HttpConfiguration(),
                    method.DeclaringType.FullName, method.DeclaringType), method);
            // The convention in test data is that the first parameter 
            // is the body, second parameter is a url parameter 
            // and the rest are querystring parameters.
            var parameters = method.GetParameters().Select((x, i) => 
                new ApiParameterDescription
                {
                    Name = x.Name,
                    Source = i == 0 ? 
                        ApiParameterSource.FromBody :
                        ApiParameterSource.FromUri,
                    ParameterDescriptor = new ReflectedHttpParameterDescriptor(
                        actionDescriptor, x)
                }
            ).ToList();
            var urlParameters = parameters
                .Where(x => x.Source == ApiParameterSource.FromUri)
                .Take(1).Select(x => x.Name).ToArray();
            var url = method.ToTestUrl(urlParameters) ;
            var httpMethod = method.Name.Map(ActionMethods)
                .To(HttpMethod.Get, HttpMethod.Post, 
                    HttpMethod.Put, HttpMethod.Delete);
            var description = new ApiDescription
            {
                RelativePath = url,
                HttpMethod = httpMethod,
                ActionDescriptor = actionDescriptor,
                Route = new HttpRoute(url)
            };
            parameters.ForEach((first, x) => description
                .ParameterDescriptions.Add(x));
            if (method.ReturnType != typeof(void))
                description.SetProperty(x => x.ResponseDescription, 
                    new ResponseDescription
                    {
                        ResponseType = method.ReturnType
                    });
            configure?.Invoke(description);
            return description;
        }
    }
}
