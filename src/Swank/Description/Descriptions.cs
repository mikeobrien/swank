using System.Collections.Generic;

namespace Swank.Description
{
    public class ModuleDescription : Description
    {
        public ModuleDescription() {}
        public ModuleDescription(string name, string 
            comments = null) : base(name, comments) { }
    }

    public class ResourceDescription : Description
    {
        public ResourceDescription() {}
        public ResourceDescription(string name, string 
            comments = null) : base(name, comments) { }
    }

    public class EndpointDescription : Description
    {
        public bool Secure { get; set; }
        public List<string> Namespace { get; set; }
        public string MethodName { get; set; }
        public string RequestComments { get; set; }
        public string ResponseComments { get; set; }
        public bool BinaryRequest { get; set; }
        public bool BinaryResponse { get; set; }
    }

    public enum OptionalScope
    {
        All, None, Post, Put, AllButPost, AllButPut
    }

    public class ParameterDescription : Description
    {
        public string Type { get; set; }
        public string SampleValue { get; set; }
        public string DefaultValue { get; set; }
        public bool Optional { get; set; }
        public bool Hidden { get; set; }
        public bool MultipleAllowed { get; set; }
        public bool Deprecated { get; set; }
        public string DeprecationMessage { get; set; }
    }

    public class MemberDescription : Description
    {
        public string SampleValue { get; set; }
        public string DefaultValue { get; set; }
        public OptionalScope Optional { get; set; }
        public bool Hidden { get; set; }
        public bool Deprecated { get; set; }
        public string DeprecationMessage { get; set; }
        public Description ArrayItem { get; set; }
        public DictionaryDescription DictionaryEntry { get; set; }
    }

    public class DictionaryDescription
    {
        public string KeyName { get; set; }
        public string KeyComments { get; set; }
        public string ValueComments { get; set; }
    }

    public class EnumDescription : Description { }

    public class OptionDescription : Description
    {
        public bool Hidden { get; set; }
    }

    public enum HttpDirection { Request, Response }

    public class HeaderDescription : Description
    {
        public HttpDirection Direction { get; set; }
        public bool Optional { get; set; }
    }

    public class StatusCodeDescription : Description
    {
        public int Code { get; set; }
    }

    public class TypeDescription : Description
    {
        public Description ArrayItem { get; set; }
        public DictionaryDescription DictionaryEntry { get; set; }
    }
}