using System.Collections.Generic;
using Swank.Specification;
using Swank.Web.Handlers.App;

namespace Swank.Description.CodeExamples
{
    public class CodeExampleModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Comments { get; set; }
        public string Method { get; set; }
        public string MethodName { get; set; }
        public List<string> Namespace { get; set; }
        public string Host { get; set; }
        public string Subdomain { get; set; }
        public string RootDomain { get; set; }
        public int Port { get; set; }
        public string Url { get; set; }
        public string Authority { get; set; }
        public string UrlTemplate { get; set; }
        public bool Secure { get; set; }
        public List<UrlParameter> UrlParameters { get; set; }
        public List<QuerystringParameter> QuerystringParameters { get; set; }
        public List<StatusCode> StatusCodes { get; set; }
        public MessageModel Request { get; set; }
        public MessageModel Response { get; set; }
    }
}
