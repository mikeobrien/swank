using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

    public class AppResourceHandler : CachingHandlerBase
    {
        private readonly Configuration.Configuration _configuration;
        private readonly SpecificationService _specification;
        private readonly BodyDescriptionService _bodyDescriptionFactory;

        public AppResourceHandler(
            Configuration.Configuration configuration,
            SpecificationService specification,
            BodyDescriptionService bodyDescriptionFactory) : 
            base(configuration, Mime.ApplicationJson)
        {
            _configuration = configuration;
            _specification = specification;
            _bodyDescriptionFactory = bodyDescriptionFactory;
        }

        protected override byte[] CreateResponse(HttpRequestMessage message)
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
                    Endpoints = x.Endpoints.Select(e => MapEndpoint(
                        _configuration, url, e, _configuration.CodeExamples,
                        _bodyDescriptionFactory)).ToList()
                })
                .FirstOrDefault();

            _configuration.ResourcePreRender?.Invoke(message, resource);

            return resource.SerializeJson().ToBytes();
        }

        public static EndpointModel MapEndpoint(Configuration.Configuration configuration,
            Uri url, Endpoint endpoint, List<CodeExample> codeExamples, 
            BodyDescriptionService bodyDescriptionFactory)
        {
            var request = new MessageModel
            {
                Comments = endpoint.Request.Comments,
                IsBinary = endpoint.Request.IsBinary,
                HasBody = endpoint.Request.HasBody,
                Type = endpoint.Request.Type,
                Body = endpoint.Request.Type != null 
                    ? bodyDescriptionFactory.Create(endpoint.Request.Type)
                    : null,
                Headers = endpoint.Request.Headers
            };

            var response = new MessageModel
            {
                Comments = endpoint.Response.Comments,
                IsBinary = endpoint.Response.IsBinary,
                HasBody = endpoint.Response.HasBody,
                Type = endpoint.Response.Type,
                Body = endpoint.Response.Type != null 
                    ? bodyDescriptionFactory.Create(endpoint.Response.Type)
                    : null,
                Headers = endpoint.Response.Headers
            };

            var endpointModel = new EndpointModel
            {
                Id = endpoint.Id,
                Name = endpoint.Name,
                Comments = endpoint.Comments,
                Method = endpoint.Method.ToLower(),
                UrlTemplate = endpoint.UrlTemplate,
                Secure = endpoint.Secure,
            };

            if (!configuration.HideQueryStringSection) endpointModel
                .QuerystringParameters = endpoint.QuerystringParameters;
            if (!configuration.HideUrlParametersSection) endpointModel
                .UrlParameters = endpoint.UrlParameters;
            if (!configuration.HideStatusCodeSection) endpointModel
                .StatusCodes = endpoint.StatusCodes;

            if (!configuration.HideRequestSection)
            {
                endpointModel.Request = new MessageModel
                {
                    Comments = request.Comments,
                    IsBinary = request.IsBinary,
                };
                if (!configuration.HideRequestBodySection)
                {
                    endpointModel.Request.HasBody = request.HasBody;
                    endpointModel.Request.Type = request.Type;
                    endpointModel.Request.Body = request.Body;
                }
                if (!configuration.HideRequestHeadersSection)
                    endpointModel.Request.Headers = request.Headers;
            }

            if (!configuration.HideResponseSection)
            {
                endpointModel.Response = new MessageModel
                {
                    Comments = response.Comments,
                    IsBinary = response.IsBinary
                };
                if (!configuration.HideResponseBodySection)
                {
                    endpointModel.Response.HasBody = response.HasBody;
                    endpointModel.Response.Type = response.Type;
                    endpointModel.Response.Body = response.Body;
                }
                if (!configuration.HideResponseHeadersSection)
                    endpointModel.Response.Headers = response.Headers;
            }

            if (!configuration.HideCodeExamplesSection)
            {
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
                endpointModel.CodeExamples = codeExamples
                    .Select((c, i) => new CodeExampleModel
                    {
                        Index = i,
                        Name = c.Name,
                        Language = c.Language,
                        Comments = c.Comments,
                        Example = c.Render(codeExampleModel)?.Trim()
                    }).ToList();
            }

            return endpointModel;
        }
    }
}
