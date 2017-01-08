using System.Net;
using Swank.Description;

namespace Tests.Unit.Specification.OverrideTests
{
    namespace Controllers
    {
        public class Module : ModuleDescription
        {
            public Module() : base("module", "module comments") { }
        }

        public class Resource : ResourceDescription
        {
            public Resource() : base("resource", "resource comments") { }
        }

        public enum Enum
        {
            [Description("option", "option comments")]
            Option
        }

        [Description("data", "data comments")]
        public class Model
        {
            [Description("member", "member comments")]
            public Enum Member { get; set; }
        }

        public class Controller
        {
            [Description("endpoint", "endpoint comments")]
            [RequestComments("request comments")]
            [ResponseComments("response comments")]
            [StatusCode(HttpStatusCode.InternalServerError, "status code comments")]
            [RequestHeader("request header", "request header comments")]
            [ResponseHeader("response header", "response header comments")]
            public Model Post(Model model, 
                [Comments("url param comments")] int urlParam, 
                [Comments("querystring comments")] string querystring) { return null; }
        }
    }
}