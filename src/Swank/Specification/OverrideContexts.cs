using System;
using System.Reflection;
using System.Web.Http.Description;
using Swank.Description;

namespace Swank.Specification
{
    public class ModuleOverrideContext
    {
        public Module Module { get; set; }
        public ModuleDescription Description { get; set; }
    }

    public class ResourceOverrideContext
    {
        public Resource Resource { get; set; }
        public ResourceDescription Description { get; set; }
    }

    public class EndpointOverrideContext
    {
        public Endpoint Endpoint { get; set; }
        public ApiDescription ApiDescription { get; set; }
        public EndpointDescription Description { get; set; }
    }

    public interface IParameterOverrideContext
    {
        IDescription Parameter { get; }
        ApiDescription ApiDescription { get; }
        Description.Description Description { get; }
    }

    public class UrlParameterOverrideContext : IParameterOverrideContext
    {
        public UrlParameter UrlParameter { get; set; }
        public ApiDescription ApiDescription { get; set; }
        public ParameterDescription Description { get; set; }
        Description.Description IParameterOverrideContext.Description => Description;
        IDescription IParameterOverrideContext.Parameter => UrlParameter;
    }

    public class QuerystringOverrideContext : IParameterOverrideContext
    {
        public QuerystringParameter Querystring { get; set; }
        public ApiDescription ApiDescription { get; set; }
        public ParameterDescription Description { get; set; }
        Description.Description IParameterOverrideContext.Description => Description;
        IDescription IParameterOverrideContext.Parameter => Querystring;
    }

    public class StatusCodeOverrideContext
    {
        public StatusCode StatusCode { get; set; }
        public ApiDescription ApiDescription { get; set; }
        public StatusCodeDescription Description { get; set; }
    }

    public class HeaderOverrideContext
    {
        public Header Header { get; set; }
        public ApiDescription ApiDescription { get; set; }
        public HeaderDescription Description { get; set; }
    }

    public class MessageOverrideContext
    {
        public Message Message { get; set; }
        public ApiDescription ApiDescription { get; set; }
        public EndpointDescription Description { get; set; }
    }

    public class TypeOverrideContext
    {
        public bool IsRequest { get; set; }
        public DataType DataType { get; set; }
        public Type Type { get; set; }
        public TypeDescription Description { get; set; }
    }

    public class MemberOverrideContext
    {
        public bool IsRequest { get; set; }
        public Member Member { get; set; }
        public PropertyInfo Property { get; set; }
        public MemberDescription Description { get; set; }
    }

    public class OptionOverrideContext
    {
        public bool IsRequest { get; set; }
        public Option Option { get; set; }
        public FieldInfo Field { get; set; }
        public OptionDescription Description { get; set; }
    }
}
