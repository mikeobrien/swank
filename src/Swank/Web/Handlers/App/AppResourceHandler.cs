using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Swank.Description.CodeExamples;
using Swank.Extensions;
using Swank.Specification;

namespace Swank.Web.Handlers.App
{
    public class Request
    {
        public string Name { get; set; }
    }

    public class AppResourceHandler : HandlerBase
    {
        private readonly Configuration.Configuration _configuration;
        private readonly SpecificationService _specification;
        private readonly BodyDescriptionService _bodyDescriptionFactory;

        public AppResourceHandler(
            Configuration.Configuration configuration,
            SpecificationService specification,
            BodyDescriptionService bodyDescriptionFactory)
        {
            _configuration = configuration;
            _specification = specification;
            _bodyDescriptionFactory = bodyDescriptionFactory;
        }

        protected override Task<HttpResponseMessage> Send(HttpRequestMessage message)
        {
            var url = _configuration.ApiUrl?.ParseUri() ?? message.RequestUri;
            var request = JsonConvert.DeserializeObject<Request>(
                message.Content.ReadAsStringAsync().Result);

            var resource = _specification.Generate()
                .SelectMany(x => x.Resources)
                .Where(x => x.Name.TrimStart('/') == request.Name)
                .Select(x => new ResourceModel
                {
                    Name = x.Name,
                    Overview = x.Comments,
                    Endpoints = x.Endpoints.Select(e => MapEndpoint(url, e,
                    _configuration.CodeExamples,
                    _bodyDescriptionFactory)).ToList()
                })
                .FirstOrDefault();

            if (resource == null) return message
                .CreateErrorResponseTask(HttpStatusCode.NotFound);

            return resource.CreateJsonResponseTask();
        }

        public static EndpointModel MapEndpoint(Uri url, Endpoint endpoint,
            List<CodeExample> codeExamples, BodyDescriptionService bodyDescriptionFactory)
        {
            var request = new MessageModel
            {
                Comments = endpoint.Request.Comments,
                IsBinary = endpoint.Request.IsBinary,
                Headers = endpoint.Request.Headers,
                Body = endpoint.Request.Type
                    .WhenNotNull(bodyDescriptionFactory.Create)
                    .OtherwiseDefault()
            };

            var response = new MessageModel
            {
                Comments = endpoint.Response.Comments,
                IsBinary = endpoint.Response.IsBinary,
                Headers = endpoint.Response.Headers,
                Body = endpoint.Response.Type
                    .WhenNotNull(bodyDescriptionFactory.Create)
                    .OtherwiseDefault()
            };

            var codeExampleModel = new Description.CodeExamples.CodeExampleModel
            {
                Name = endpoint.Name,
                Comments = endpoint.Comments,
                Method = endpoint.Method,
                MethodName = endpoint.MethodName,
                Namespace = endpoint.Namespace,
                Host = url.Host,
                Subdomain = url.GetSubdomain(),
                RootDomain = url.GetRootDomain(),
                Port = url.Port,
                Authority = url.GetLeftPart(UriPartial.Authority),
                Url = url.GetLeftPart(UriPartial.Authority)
                    .CombineUrls(endpoint.UrlTemplate),
                UrlTemplate = endpoint.UrlTemplate,
                Secure = endpoint.Secure,
                UrlParameters = endpoint.UrlParameters,
                QuerystringParameters = endpoint.QuerystringParameters,
                StatusCodes = endpoint.StatusCodes,
                Request = request,
                Response = response
            };

            return new EndpointModel
            {
                Id = endpoint.Id,
                Name = endpoint.Name,
                Comments = endpoint.Comments,
                Method = endpoint.Method.ToLower(),
                UrlTemplate = endpoint.UrlTemplate,
                Secure = endpoint.Secure,
                UrlParameters = endpoint.UrlParameters,
                QuerystringParameters = endpoint.QuerystringParameters,
                StatusCodes = endpoint.StatusCodes,
                Request = request,
                Response = response,
                CodeExamples = codeExamples
                    .Select((c, i) => new CodeExampleModel
                    {
                        Index = i,
                        Name = c.Name,
                        Language = c.Language,
                        Comments = c.Comments,
                        Example = c.Render(codeExampleModel)?.Trim()
                    }).ToList()
            };
        }
    }
}
