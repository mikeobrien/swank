using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Web.Http.Description;
using Swank.Configuration;
using Swank.Description;
using Swank.Extensions;

namespace Swank.Specification
{
    public class SpecificationService
    {
        private class EndpointMapping
        {
            public ApiDescription Endpoint { get; set; }
            public ModuleDescription Module { get; set; }
            public ResourceDescription Resource { get; set; }
        }

        public static readonly Func<string, int> HttpVerbRank = x =>
        {
            switch (x.IsNullOrEmpty() ? null : x.ToLower())
            {
                case "get": return 0;
                case "post": return 1;
                case "put": return 2;
                case "update": return 3;
                case "delete": return 5;
                default: return 4;
            }
        };

        private readonly Configuration.Configuration _configuration;
        private readonly IApiExplorer _apiExplorer;
        private readonly IDescriptionConvention<ApiDescription, ModuleDescription> _moduleConvention;
        private readonly IDescriptionConvention<ApiDescription, ResourceDescription> _resourceConvention;
        private readonly IDescriptionConvention<ApiDescription, EndpointDescription> _endpointConvention;
        private readonly IDescriptionConvention<ApiParameterDescription, ParameterDescription> _parameterConvention;
        private readonly IDescriptionConvention<ApiDescription, List<StatusCodeDescription>> _statusCodeConvention;
        private readonly IDescriptionConvention<ApiDescription, List<HeaderDescription>> _headerConvention;
        private readonly TypeGraphFactory _typeGraphFactory;
        private readonly OptionFactory _optionFactory;
        private readonly Lazy<List<Module>> _specification;

        public SpecificationService(
            Configuration.Configuration configuration,
            IApiExplorer apiExplorer,
            IDescriptionConvention<ApiDescription, ModuleDescription> moduleConvention,
            IDescriptionConvention<ApiDescription, ResourceDescription> resourceConvention,
            IDescriptionConvention<ApiDescription, EndpointDescription> endpointConvention,
            IDescriptionConvention<ApiParameterDescription, ParameterDescription> parameterConvention,
            IDescriptionConvention<ApiDescription, List<StatusCodeDescription>> statusCodeConvention,
            IDescriptionConvention<ApiDescription, List<HeaderDescription>> headerConvention,
            TypeGraphFactory typeGraphFactory,
            OptionFactory optionFactory)
        {
            _configuration = configuration;
            _apiExplorer = apiExplorer;
            _moduleConvention = moduleConvention;
            _resourceConvention = resourceConvention;
            _endpointConvention = endpointConvention;
            _parameterConvention = parameterConvention;
            _statusCodeConvention = statusCodeConvention;
            _typeGraphFactory = typeGraphFactory;
            _optionFactory = optionFactory;
            _headerConvention = headerConvention;
            _specification = new Lazy<List<Module>>(GenerateSpecification);
        }

        public List<Module> Generate()
        {
            return _specification.Value;
        }

        private List<Module> GenerateSpecification()
        {
            var mappings = GetEndpointMapping(_apiExplorer.ApiDescriptions);
            CheckForOrphanedEndpoints(mappings);
            return GetModules(mappings).ThenDo(Markdown.Apply);
        }

        private List<EndpointMapping> GetEndpointMapping(IEnumerable<ApiDescription> endpoints)
        {
            return endpoints
                .Where(x => 
                    x.GetControllerAssembly() != Assembly.GetExecutingAssembly() &&
                    (!_configuration.AppliesToAssemblies.Any() || 
                        _configuration.AppliesToAssemblies
                            .Any(y => y == x.GetControllerAssembly())) && 
                    !x.HasControllerOrActionAttribute<HideAttribute>() &&
                    _configuration.Filter(x))
                .Select(x => new
                {
                    Endpoint = x,
                    Module = _moduleConvention.GetDescription(x),
                    Resource = _resourceConvention.GetDescription(x)
                })
                .Where(x => x.Module != null || _configuration.OrphanedModuleEndpoint != OrphanedEndpoints.Exclude)
                .Where(x => x.Resource != null || _configuration.OrphanedResourceEndpoint != OrphanedEndpoints.Exclude)
                .Select(x => new EndpointMapping
                {
                    Endpoint = x.Endpoint,
                    Module = _configuration.OrphanedModuleEndpoint == OrphanedEndpoints.UseDefault
                        ? x.Module ?? new ModuleDescription(_configuration.DefaultModuleName)
                        : x.Module,
                    Resource = _configuration.OrphanedResourceEndpoint == OrphanedEndpoints.UseDefault
                        ? x.Resource ?? CreateDefaultResource(x.Endpoint)
                        : x.Resource
                }).ToList();
        }
        
        private ResourceDescription CreateDefaultResource(ApiDescription endpoint)
        {
            return _configuration.DefaultResourceFactory?.Invoke(endpoint) ??
                new ResourceDescription
                {
                    Name = _configuration.DefaultResourceIdentifier(endpoint),
                    Comments = endpoint.GetControllerAssembly().FindResourceNamed(
                        endpoint.GetControllerType().FullName.AddMarkdownExtension())
                };
        }

        private void CheckForOrphanedEndpoints(IList<EndpointMapping> mappings)
        {
            if (_configuration.OrphanedModuleEndpoint == OrphanedEndpoints.Fail)
            {
                var orphanedModuleActions = mappings.Where(x => x.Module == null).ToList();
                if (orphanedModuleActions.Any()) throw new OrphanedModuleActionException(
                    orphanedModuleActions.Select(x => x.Endpoint.GetControllerType()
                        .FullName + "." + x.Endpoint.GetMethodInfo().Name));
            }

            if (_configuration.OrphanedResourceEndpoint == OrphanedEndpoints.Fail)
            {
                var orphanedActions = mappings.Where(x => x.Resource == null).ToList();
                if (orphanedActions.Any()) throw new OrphanedResourceActionException(
                    orphanedActions.Select(x => x.Endpoint.GetControllerType()
                        .FullName + "." + x.Endpoint.GetMethodInfo().Name));
            }
        }

        private List<Module> GetModules(IEnumerable<EndpointMapping> mappings)
        {
            return mappings
                .GroupBy(x => x.Module)
                .Select(x => _configuration.ModuleOverrides.Apply(new ModuleOverrideContext
                {
                    Description = x.Key,
                    Module = new Module
                    {
                        Name = x.Key.Name,
                        Comments = x.Key.Comments,
                        Resources = GetResources(x.Select(y => y).ToList())
                    }
                }).Module)
                .OrderBy(x => x.Name).ToList();
        }

        private List<Resource> GetResources(IEnumerable<EndpointMapping> mappings)
        {
            return mappings
                .GroupBy(x => x.Resource)
                .Select(x => _configuration.ResourceOverrides.Apply(new ResourceOverrideContext
                {
                    Description = x.Key,
                    Resource = new Resource
                    {
                        Name = x.Key.Name,
                        Comments = x.Key.Comments,
                        Endpoints = GetEndpoints(x.Select(y => y.Endpoint))
                    }
                }).Resource)
                .OrderBy(x => x.Name).ToList();
        }

        private List<Endpoint> GetEndpoints(IEnumerable<ApiDescription> endpoints)
        {
            return endpoints
                .Select(endpoint =>
                {
                    var description = _endpointConvention.GetDescription(endpoint);
                    return _configuration.EndpointOverrides.Apply(new EndpointOverrideContext
                    {
                        ApiDescription = endpoint,
                        Description = description,
                        Endpoint = new Endpoint
                        {
                            Id = endpoint.ID.Hash(),
                            Name = description.WhenNotNull(y => y.Name).OtherwiseDefault(),
                            Comments = description.WhenNotNull(y => y.Comments).OtherwiseDefault(),
                            UrlTemplate = endpoint.RelativePath,
                            Method = endpoint.HttpMethod.Method,
                            UrlParameters = GetUrlParameters(endpoint),
                            QuerystringParameters = GetQuerystringParameters(endpoint),
                            Secure = description.Secure,
                            StatusCodes = GetStatusCodes(endpoint),
                            Request = GetRequest(endpoint, description),
                            Response = GetResponse(endpoint, description)
                        }
                    }).Endpoint;
                })
                .OrderBy(x => x.UrlTemplate.Split('?').First())
                .ThenBy(x => HttpVerbRank(x.Method)).ToList();
        }

        private List<UrlParameter> GetUrlParameters(ApiDescription endpoint)
        {
            return endpoint.ParameterDescriptions
                .Where(x => x.IsUrlParameter(endpoint))
                .Select(x =>
                {
                    var description = _parameterConvention.GetDescription(x);
                    return _configuration.UrlParameterOverrides.Apply(new UrlParameterOverrideContext
                    {
                        ApiDescription = endpoint,
                        Description = description,
                        UrlParameter = new UrlParameter
                        {
                            Name = description.WhenNotNull(y => y.Name).OtherwiseDefault(),
                            Comments = description.WhenNotNull(y => y.Comments).OtherwiseDefault(),
                            Type = description.WhenNotNull(y => y.Type).OtherwiseDefault(),
                            Options = _optionFactory.BuildOptions(x.ParameterDescriptor.ParameterType, true),
                            SampleValue = description.WhenNotNull(y => y.SampleValue).OtherwiseDefault()
                        }
                    }).UrlParameter;
                }).ToList();
        }

        private List<QuerystringParameter> GetQuerystringParameters(ApiDescription endpoint)
        {
            return endpoint.ParameterDescriptions
                .Where(x => x.IsQuerystring(endpoint))
                .Select(x => new
                {
                    Parameter = x,
                    Description = _parameterConvention.GetDescription(x)
                })
                .Where(x => !x.Description.Hidden)
                .Select(x =>
                {
                    var type = x.Parameter.ParameterDescriptor.ParameterType;
                    return _configuration.QuerystringOverrides.Apply(new QuerystringOverrideContext
                    {
                        ApiDescription = endpoint,
                        Description = x.Description,
                        Querystring = new QuerystringParameter
                        {
                            Name = x.Description.WhenNotNull(y => y.Name).OtherwiseDefault(),
                            Comments = x.Description.WhenNotNull(y => y.Comments).OtherwiseDefault(),
                            Type = x.Description.WhenNotNull(y => y.Type).OtherwiseDefault(),
                            Options = _optionFactory.BuildOptions(type, true),
                            DefaultValue = x.Description.DefaultValue.WhenNotNull(y => y
                                .ToSampleValueString(_configuration)).OtherwiseDefault(),
                            SampleValue = x.Description.WhenNotNull(y => y.SampleValue).OtherwiseDefault(),
                            MultipleAllowed = x.Description.MultipleAllowed,
                            Required = !x.Description.Optional
                        }
                    }).Querystring;
                }).ToList();
        }

        private Message GetRequest(ApiDescription endpoint, EndpointDescription description)
        {
            var data = new Message();
            var requestDescription = endpoint.GetRequestDescription();

            if (requestDescription != null && 
                (endpoint.HttpMethod == HttpMethod.Post ||
                endpoint.HttpMethod == HttpMethod.Put ||
                endpoint.HttpMethod == HttpMethod.Delete))
            {
                data.Type = _typeGraphFactory.BuildGraph(requestDescription
                    .ParameterDescriptor.ParameterType, true, endpoint.HttpMethod);
            }

            data.Comments = description.RequestComments;
            data.Headers = GetHeaders(endpoint, HttpDirection.Request);
            data.IsBinary = description.BinaryRequest;

            return _configuration.RequestOverrides.Apply(new MessageOverrideContext
            {
                ApiDescription = endpoint,
                Description = description,
                Message = data
            }).Message;
        }

        private Message GetResponse(ApiDescription endpoint, EndpointDescription description)
        {
            var data = new Message();
            var responseType = endpoint.GetResponseType();
            DataType type = null;

            if (responseType != null)
            {
                data.Type = _typeGraphFactory.BuildGraph(
                    responseType, false, endpoint.HttpMethod);
            }

            data.Comments = description.ResponseComments;
            data.Headers = GetHeaders(endpoint, HttpDirection.Response);
            data.IsBinary = description.BinaryResponse;

            return _configuration.ResponseOverrides.Apply(new MessageOverrideContext
            {
                ApiDescription = endpoint,
                Description = description,
                Message = data
            }).Message;
        }

        private List<StatusCode> GetStatusCodes(ApiDescription endpoint)
        {
            return _statusCodeConvention.GetDescription(endpoint)
                .Select(x => _configuration.StatusCodeOverrides.Apply(new StatusCodeOverrideContext
                {
                    ApiDescription = endpoint,
                    Description = x,
                    StatusCode = new StatusCode
                    {
                        Code = x.Code,
                        Name = x.Name,
                        Comments = x.Comments
                    }
                }).StatusCode)
                .OrderBy(x => x.Code).ToList();
        }

        private List<Header> GetHeaders(ApiDescription endpoint, HttpDirection direction)
        {
            var overrides = direction == HttpDirection.Request
                ? _configuration.RequestHeaderOverrides
                : _configuration.ResponseHeaderOverrides;
            return _headerConvention.GetDescription(endpoint)
                .Where(x => x.Direction == direction)
                .Select(x => overrides.Apply(new HeaderOverrideContext
                {
                    ApiDescription = endpoint,
                    Description = x,
                    Header = new Header
                    {
                        Name = x.Name,
                        Comments = x.Comments,
                        Optional = direction == HttpDirection.Request && x.Optional,
                        Required = direction == HttpDirection.Request && !x.Optional,
                        IsAccept = x.Name.Equals("accept", StringComparison.OrdinalIgnoreCase),
                        IsContentType = x.Name.Equals("content-type", StringComparison.OrdinalIgnoreCase)
                    }
                }).Header)
                .OrderBy(x => x.Name).ToList();
        }
    }
}
