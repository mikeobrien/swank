using System.Collections.Generic;
using Swank.Specification;

namespace Swank.Web.Handlers.App
{
    public class ResourceModel
    {
        public string Name { get; set; }
        public string Overview { get; set; }
        public List<EndpointModel> Endpoints { get; set; }
    }

    public class MessageModel
    {
        public string Comments { get; set; }
        public bool IsBinary { get; set; }
        public bool HasBody { get; set; }
        public List<Header> Headers { get; set; }
        public DataType Type { get; set; }
        public List<BodyDefinitionModel> Body { get; set; }
    }

    public class CodeExampleModel
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public string Language { get; set; }
        public string Comments { get; set; }
        public string Example { get; set; }
    }

    public class EndpointModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Comments { get; set; }
        public List<string> Namespace { get; set; }
        public string Method { get; set; }
        public string UrlTemplate { get; set; }
        public bool Secure { get; set; }
        public List<UrlParameter> UrlParameters { get; set; }
        public List<QuerystringParameter> QuerystringParameters { get; set; }
        public List<StatusCode> StatusCodes { get; set; }
        public MessageModel Request { get; set; }
        public MessageModel Response { get; set; }
        public List<CodeExampleModel> CodeExamples { get; set; }
    }

    public class BodyDefinitionModel
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public List<string> Namespace { get; set; }
        public string Comments { get; set; }
        public bool? IsFirst { get; set; }
        public bool? IsLast { get; set; }
        public string TypeName { get; set; }
        public string SampleValue { get; set; }
        public string DefaultValue { get; set; }
        public bool? Optional { get; set; }
        public bool Nullable { get; set; }
        public string Whitespace { get; set; }
        public bool? IsDeprecated { get; set; }
        public string DeprecationMessage { get; set; }

        public bool? IsOpening { get; set; }
        public bool? IsClosing { get; set; }

        public bool? IsMember { get; set; }
        public bool? IsLastMember { get; set; }

        public bool? IsSimpleType { get; set; }
        public bool? IsString { get; set; }
        public bool? IsBoolean { get; set; }
        public bool? IsNumeric { get; set; }
        public bool? IsDateTime { get; set; }
        public bool? IsDuration { get; set; }
        public bool? IsGuid { get; set; }
        public Enumeration Options { get; set; }

        public bool? IsComplexType { get; set; }

        public bool? IsArray { get; set; }

        public bool? IsDictionary { get; set; }
        public bool? IsDictionaryEntry { get; set; }
        public KeyModel DictionaryKey { get; set; }
    }

    public class KeyModel
    {
        public string Comments { get; set; }
        public string TypeName { get; set; }
        public Enumeration Enumeration { get; set; }
    }
}
