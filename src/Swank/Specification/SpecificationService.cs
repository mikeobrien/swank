using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using Swank.Configuration;
using Swank.Description;
using Swank.Extensions;

namespace Swank.Specification
{
    public class SpecificationService
    {
        private class EndpointMapping
        {
            public IApiDescription Endpoint { get; set; }
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
        private readonly IDescriptionConvention<IApiDescription, ModuleDescription> _moduleConvention;
        private readonly IDescriptionConvention<IApiDescription, ResourceDescription> _resourceConvention;
        private readonly IDescriptionConvention<IApiDescription, EndpointDescription> _endpointConvention;
        private readonly IDescriptionConvention<IApiParameterDescription, ParameterDescription> _parameterConvention;
        private readonly IDescriptionConvention<IApiDescription, List<StatusCodeDescription>> _statusCodeConvention;
        private readonly IDescriptionConvention<IApiDescription, List<HeaderDescription>> _headerConvention;
        private readonly TypeGraphService _typeGraphService;
        private readonly Lazy<List<Module>> _specification;

        public SpecificationService(
            Configuration.Configuration configuration,
            IApiExplorer apiExplorer,
            IDescriptionConvention<IApiDescription, ModuleDescription> moduleConvention,
            IDescriptionConvention<IApiDescription, ResourceDescription> resourceConvention,
            IDescriptionConvention<IApiDescription, EndpointDescription> endpointConvention,
            IDescriptionConvention<IApiParameterDescription, ParameterDescription> parameterConvention,
            IDescriptionConvention<IApiDescription, List<StatusCodeDescription>> statusCodeConvention,
            IDescriptionConvention<IApiDescription, List<HeaderDescription>> headerConvention,
            TypeGraphService typeGraphService)
        {
            _configuration = configuration;
            _apiExplorer = apiExplorer;
            _moduleConvention = moduleConvention;
            _resourceConvention = resourceConvention;
            _endpointConvention = endpointConvention;
            _parameterConvention = parameterConvention;
            _statusCodeConvention = statusCodeConvention;
            _typeGraphService = typeGraphService;
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

        private List<EndpointMapping> GetEndpointMapping(IEnumerable<IApiDescription> apiDescriptions)
        {
            return apiDescriptions
                .Where(x => 
                    x.ControllerType.Assembly != Assembly.GetExecutingAssembly() &&
                    (!_configuration.AppliesToAssemblies.Any() || 
                        _configuration.AppliesToAssemblies
                            .Any(y => y == x.ControllerType.Assembly)) && 
                    !x.HasControllerOrActionAttribute<HideAttribute>() &&
                    _configuration.Filter(x))
                .Select(x => new
                {
                    Endpoint = x,
                    Module = _moduleConvention.GetDescription(x),
                    Resource = _resourceConvention.GetDescription(x)
                })
                .Where(x => x.Module?.Name != null || _configuration.OrphanedModuleEndpoint != OrphanedEndpoints.Exclude)
                .Where(x => x.Resource?.Name != null || _configuration.OrphanedResourceEndpoint != OrphanedEndpoints.Exclude)
                .Select(x => new EndpointMapping
                {
                    Endpoint = x.Endpoint,
                    Module = x.Module?.Name == null
                        ? (_configuration.OrphanedModuleEndpoint == OrphanedEndpoints.UseDefault
                            ? new ModuleDescription(_configuration.DefaultModuleName, x.Module?.Comments)
                            : null)
                        : x.Module,
                    Resource = x.Resource?.Name == null
                        ? (_configuration.OrphanedResourceEndpoint == OrphanedEndpoints.UseDefault
                            ? CreateDefaultResource(x.Endpoint, x.Resource?.Comments)
                            : null)
                        : x.Resource
                }).ToList();
        }
        
        private ResourceDescription CreateDefaultResource(IApiDescription apiDescription, string comments)
        {
            return _configuration.DefaultResourceFactory?.Invoke(apiDescription, comments) ??
                new ResourceDescription
                {
                    Name = _configuration.DefaultResourceIdentifier(apiDescription),
                    Comments = comments
                };
        }

        private void CheckForOrphanedEndpoints(IList<EndpointMapping> mappings)
        {
            if (_configuration.OrphanedModuleEndpoint == OrphanedEndpoints.Fail)
            {
                var orphanedModuleActions = mappings.Where(x => x.Module == null).ToList();
                if (orphanedModuleActions.Any()) throw new OrphanedModuleActionException(
                    orphanedModuleActions.Select(x => x.Endpoint.ControllerType
                        .FullName + "." + x.Endpoint.ActionMethod.Name));
            }

            if (_configuration.OrphanedResourceEndpoint == OrphanedEndpoints.Fail)
            {
                var orphanedActions = mappings.Where(x => x.Resource == null).ToList();
                if (orphanedActions.Any()) throw new OrphanedResourceActionException(
                    orphanedActions.Select(x => x.Endpoint.ControllerType
                        .FullName + "." + x.Endpoint.ActionMethod.Name));
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
                .OrderBy(x => x.Name?.Trim('/')).ToList();
        }

        private List<Endpoint> GetEndpoints(IEnumerable<IApiDescription> apiDescriptions)
        {
            return apiDescriptions
                .Select(endpoint =>
                {
                    var description = _endpointConvention.GetDescription(endpoint);
                    return _configuration.EndpointOverrides.Apply(new EndpointOverrideContext
                    {
                        ApiDescription = endpoint,
                        Description = description,
                        Endpoint = new Endpoint
                        {
                            Id = endpoint.Id.Hash(),
                            Name = description.WhenNotNull(y => y.Name).OtherwiseDefault(),
                            Comments = description.WhenNotNull(y => y.Comments).OtherwiseDefault(),
                            Namespace = description.Namespace,
                            MethodName = description.MethodName,
                            UrlTemplate = endpoint.RelativePath,
                            Method = endpoint.HttpMethod.Method,
                            UrlParameters = GetUrlParameters(endpoint, description),
                            QuerystringParameters = GetQuerystringParameters(endpoint, description),
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

        private List<UrlParameter> GetUrlParameters(IApiDescription apiDescription, 
            EndpointDescription endpointDescription)
        {
            return apiDescription.ParameterDescriptions
                .Where(x => x.IsUrlParameter)
                .Select(x =>
                {
                    var description = _parameterConvention.GetDescription(x);
                    var name = description.WhenNotNull(y => y.Name).OtherwiseDefault();
                    return _configuration.UrlParameterOverrides.Apply(new UrlParameterOverrideContext
                    {
                        ApiDescription = apiDescription,
                        Description = description,
                        UrlParameter = new UrlParameter
                        {
                            Name = name,
                            Comments = description.WhenNotNull(y => y.Comments).OtherwiseDefault(),
                            Type = _typeGraphService.BuildForParameter(x.Type, endpointDescription, description, apiDescription),
                            SampleValue = description.WhenNotNull(y => y.SampleValue).OtherwiseDefault(),
                            IsAuth = _configuration.AuthenticationSchemes
                                .SelectMany(y => y.Components)
                                .Any(y => y.Name.EqualsIgnoreCase(name) &&
                                    y.Location == AuthenticationLocation.UrlParameter)
                        }
                    }).UrlParameter;
                }).ToList();
        }

        private List<QuerystringParameter> GetQuerystringParameters(IApiDescription apiDescription, 
            EndpointDescription endpointDescription)
        {
            return apiDescription.ParameterDescriptions
                .Where(x => x.IsQuerystring)
                .Select(x => new
                {
                    Parameter = x,
                    Description = _parameterConvention.GetDescription(x)
                })
                .Where(x => !x.Description.Hidden)
                .Select(x =>
                {
                    var name = x.Description.WhenNotNull(y => y.Name).OtherwiseDefault();
                    return _configuration.QuerystringOverrides.Apply(new QuerystringOverrideContext
                    {
                        ApiDescription = apiDescription,
                        Description = x.Description,
                        Querystring = new QuerystringParameter
                        {
                            Name = name,
                            Comments = x.Description.WhenNotNull(y => y.Comments).OtherwiseDefault(),
                            Type = _typeGraphService.BuildForParameter(x.Parameter.Type, 
                                endpointDescription, x.Description, apiDescription),
                            DefaultValue = x.Description.DefaultValue.WhenNotNull(y => y
                                .ToSampleValueString(_configuration)).OtherwiseDefault(),
                            SampleValue = x.Description.WhenNotNull(y => y.SampleValue).OtherwiseDefault(),
                            MultipleAllowed = x.Description.MultipleAllowed,
                            Required = !x.Description.Optional,
                            IsAuth = _configuration.AuthenticationSchemes
                                .SelectMany(y => y.Components)
                                .Any(y => y.Name.EqualsIgnoreCase(name) &&
                                    y.Location == AuthenticationLocation.Querystring)
                        }
                    }).Querystring;
                }).ToList();
        }

        private Message GetRequest(IApiDescription apiDescription, EndpointDescription description)
        {
            var data = new Message();
            var requestDescription = apiDescription.RequestParameter;

            if (requestDescription != null && 
                (apiDescription.HttpMethod == HttpMethod.Post ||
                apiDescription.HttpMethod == HttpMethod.Put ||
                apiDescription.HttpMethod == HttpMethod.Delete))
            {
                data.Type = _typeGraphService.BuildForMessage(true, 
                    requestDescription.Type, description, apiDescription);
            }

            data.Comments = description.RequestComments;
            data.Headers = GetHeaders(apiDescription, HttpDirection.Request);
            data.IsBinary = description.BinaryRequest;

            return _configuration.RequestOverrides.Apply(new MessageOverrideContext
            {
                ApiDescription = apiDescription,
                Description = description,
                Message = data
            }).Message;
        }

        private Message GetResponse(IApiDescription apiDescription, EndpointDescription description)
        {
            var data = new Message();
            var responseType = apiDescription.ResponseType;

            if (responseType != null)
            {
                data.Type = _typeGraphService.BuildForMessage(
                    false, responseType, description, apiDescription);
            }

            data.Comments = description.ResponseComments;
            data.Headers = GetHeaders(apiDescription, HttpDirection.Response);
            data.IsBinary = description.BinaryResponse;

            return _configuration.ResponseOverrides.Apply(new MessageOverrideContext
            {
                ApiDescription = apiDescription,
                Description = description,
                Message = data
            }).Message;
        }

        private List<StatusCode> GetStatusCodes(IApiDescription apiDescription)
        {
            return _statusCodeConvention.GetDescription(apiDescription)
                .Select(x => _configuration.StatusCodeOverrides.Apply(new StatusCodeOverrideContext
                {
                    ApiDescription = apiDescription,
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

        private List<Header> GetHeaders(IApiDescription apiDescription, HttpDirection direction)
        {
            var overrides = direction == HttpDirection.Request
                ? _configuration.RequestHeaderOverrides
                : _configuration.ResponseHeaderOverrides;
            return _headerConvention.GetDescription(apiDescription)
                .Where(x => x.Direction == direction)
                .Select(x => overrides.Apply(new HeaderOverrideContext
                {
                    ApiDescription = apiDescription,
                    Description = x,
                    Header = new Header
                    {
                        Name = x.Name,
                        Comments = x.Comments,
                        Optional = direction == HttpDirection.Request && x.Optional,
                        Required = direction == HttpDirection.Request && !x.Optional,
                        IsAuth = _configuration.AuthenticationSchemes
                            .SelectMany(y => y.Components)
                            .Any(y => y.Name.EqualsIgnoreCase(x.Name) && 
                                y.Location == AuthenticationLocation.Header)
                    }
                }).Header)
                .OrderBy(x => x.Name).ToList();
        }
    }
}
