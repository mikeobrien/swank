using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Swank.Extensions;
using Swank.Specification;
using System.Net;
using Swank.Description.CodeExamples;

namespace Swank.Web.Handlers
{
    public class Request
    {
        public string Name { get; set; }
    }

    public class AppResourceHandler : HandlerBase
    {
        private readonly Configuration.Configuration _configuration;
        private readonly SpecificationService _specification;
        private readonly BodyDescriptionFactory _bodyDescriptionFactory;

        public AppResourceHandler(
            Configuration.Configuration configuration,
            SpecificationService specification,
            BodyDescriptionFactory bodyDescriptionFactory)
        {
            _configuration = configuration;
            _specification = specification;
            _bodyDescriptionFactory = bodyDescriptionFactory;
        }

        protected override Task<HttpResponseMessage> Send(HttpRequestMessage request)
        {
            var url = _configuration.ApiUrl?.ParseUri() ?? request.RequestUri;
            var resource = JsonConvert.DeserializeObject<Request>(
                request.Content.ReadAsStringAsync().Result);

            var endpoint = _specification.Generate()
                .SelectMany(x => x.Resources)
                .Where(x => x.Name.TrimStart('/') == resource.Name)
                .Select(x => new
                {
                    x.Name,
                    Overview = x.Comments,
                    Endpoints = x.Endpoints.Select(e => new EndpointModel
                    {
                        Id = e.Id,
                        Name = e.Name,
                        Comments = e.Comments,
                        Method = e.Method.ToLower(),
                        UrlTemplate = e.UrlTemplate,
                        Secure = e.Secure,
                        UrlParameters = e.UrlParameters,
                        QuerystringParameters = e.QuerystringParameters,
                        StatusCodes = e.StatusCodes,
                        Request = new MessageModel
                        {
                            Comments = e.Request.Comments,
                            IsBinary = e.Request.IsBinary,
                            Headers = e.Request.Headers,
                            Body = e.Request.Type.WhenNotNull(y =>
                                _bodyDescriptionFactory.Create(y))
                                    .OtherwiseDefault()
                        },
                        Response = new MessageModel
                        {
                            Comments = e.Response.Comments,
                            IsBinary = e.Response.IsBinary,
                            Headers = e.Response.Headers,
                            Body = e.Response.Type.WhenNotNull(y =>
                                _bodyDescriptionFactory.Create(y))
                                    .OtherwiseDefault()
                        }
                    }).ToList()
                })
                .Select(x => new ResourceModel
                {
                    Name = x.Name,
                    Overview = x.Overview,
                    Endpoints = x.Endpoints.Select(e => new EndpointModel
                    {
                        Id = e.Id,
                        Name = e.Name,
                        Comments = e.Comments,
                        Method = e.Method,
                        UrlTemplate = e.UrlTemplate,
                        Secure = e.Secure,
                        UrlParameters = e.UrlParameters,
                        QuerystringParameters = e.QuerystringParameters,
                        StatusCodes = e.StatusCodes,
                        Request = e.Request,
                        Response = e.Response,
                        CodeExamples = _configuration.CodeExamples
                            .Select((c, i) => new CodeExampleModel
                            {
                                Index = i,
                                Name = c.Name,
                                Language = c.Language,
                                Comments = c.Comments,
                                Example = c.Render(new TemplateModel
                                {
                                    Name = e.Name,
                                    Comments = e.Comments,
                                    Method = e.Method,
                                    Host = url.Host,
                                    Port = url.Port.ToString(),
                                    Url = url.GetLeftPart(UriPartial.Authority)
                                        .CombineUrls(e.UrlTemplate),
                                    UrlTemplate = e.UrlTemplate,
                                    Secure = e.Secure,
                                    UrlParameters = e.UrlParameters,
                                    QuerystringParameters = e.QuerystringParameters,
                                    StatusCodes = e.StatusCodes,
                                    Request = e.Request,
                                    Response = e.Response
                                })?.Trim()
                            }).ToList()
                    }).ToList()
                }).FirstOrDefault();

            if (endpoint == null) return request
                .CreateErrorResponseTask(HttpStatusCode.NotFound);

            return endpoint.CreateJsonResponseTask();
        }
    }
}
