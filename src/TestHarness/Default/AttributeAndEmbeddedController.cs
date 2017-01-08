using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web.Http;
using Swank.Description;

namespace TestHarness.Default
{
    public class AttributeAndEmbeddedController : ApiController
    {
        [Comments("Type comments. Raw denim aesthetic synth nesciunt. :trollface:")]
        public class Model
        {
            [Comments("Member comments. Raw denim aesthetic synth nesciunt. :trollface:")]
            public string Value { get; set; }
            [Comments("Member comments. Raw denim aesthetic synth nesciunt. :trollface:")]
            [DefaultValue("fark")]
            public string DefaultValue { get; set; }
            [Comments("Member comments. Raw denim aesthetic synth nesciunt. :trollface:")]
            public Options OptionValue { get; set; }
            [Comments("Member comments. Raw denim aesthetic synth nesciunt. :trollface:")]
            [Obsolete("Obsolete comments. Raw denim aesthetic synth nesciunt. :trollface:")]
            public string DepricatedValue { get; set; }
            [Comments("Member comments. Raw denim aesthetic synth nesciunt. :trollface:")]
            public int NumericValue { get; set; }
            [Comments("Member comments. Raw denim aesthetic synth nesciunt. :trollface:")]
            public bool BooleanValue { get; set; }
            [Comments("Member comments. Raw denim aesthetic synth nesciunt. :trollface:")]
            public DateTime DateValue { get; set; }
            [Comments("Member comments. Raw denim aesthetic synth nesciunt. :trollface:")]
            public TimeSpan DurationValue { get; set; }
            [Comments("Member comments. Raw denim aesthetic synth nesciunt. :trollface:")]
            public Guid GuidValue { get; set; }
            [Comments("Member comments. Raw denim aesthetic synth nesciunt. :trollface:")]
            public ChildModel ComplexValue { get; set; }
            [Comments("Member comments. Raw denim aesthetic synth nesciunt. :trollface:")]
            [ArrayDescription(comments: "", itemName: "", itemComments: "")]
            public List<int> SimpleList { get; set; }
            [Comments("Member comments. Raw denim aesthetic synth nesciunt. :trollface:")]
            public List<ChildModel> ComplexList { get; set; }
            [Comments("Member comments. Raw denim aesthetic synth nesciunt. :trollface:")]
            public List<List<ChildModel>> ListOfList { get; set; }
            [Comments("Member comments. Raw denim aesthetic synth nesciunt. :trollface:")]
            public List<Dictionary<string, ChildModel>> ListOfDictionary { get; set; }
            [Comments("Member comments. Raw denim aesthetic synth nesciunt. :trollface:")]
            [DictionaryDescription(comments: "Dictionary comments. Raw denim aesthetic synth nesciunt. :trollface:",
                keyName: "key-name", keyComments: "Key comments. Raw denim aesthetic synth nesciunt. :trollface:", 
                valueComments: "Value comments. Raw denim aesthetic synth nesciunt. :trollface:")]
            public Dictionary<string, int> SimpleTypeDictionary { get; set; }
            [Comments("Member comments. Raw denim aesthetic synth nesciunt. :trollface:")]
            [DictionaryDescription(comments: "Dictionary comments. Raw denim aesthetic synth nesciunt. :trollface:",
                keyName: "key-name", keyComments: "Key comments. Raw denim aesthetic synth nesciunt. :trollface:",
                valueComments: "Value comments. Raw denim aesthetic synth nesciunt. :trollface:")]
            public Dictionary<string, ChildModel> ComplexTypeDictionary { get; set; }
            [Comments("Member comments. Raw denim aesthetic synth nesciunt. :trollface:")]
            [DictionaryDescription(comments: "Dictionary comments. Raw denim aesthetic synth nesciunt. :trollface:",
                keyName: "key-name", keyComments: "Key comments. Raw denim aesthetic synth nesciunt. :trollface:",
                valueComments: "Value comments. Raw denim aesthetic synth nesciunt. :trollface:")]
            public Dictionary<Options, Options> OptionsDictionary { get; set; }
            [Comments("Member comments. Raw denim aesthetic synth nesciunt. :trollface:")]
            [DictionaryDescription(comments: "Dictionary comments. Raw denim aesthetic synth nesciunt. :trollface:",
                keyName: "key-name", keyComments: "Key comments. Raw denim aesthetic synth nesciunt. :trollface:",
                valueComments: "Value comments. Raw denim aesthetic synth nesciunt. :trollface:")]
            public Dictionary<Options, List<ChildModel>> DictionaryOfLists { get; set; }
            [Comments("Member comments. Raw denim aesthetic synth nesciunt. :trollface:")]
            [DictionaryDescription(comments: "Dictionary comments. Raw denim aesthetic synth nesciunt. :trollface:",
                keyName: "key-name", keyComments: "Key comments. Raw denim aesthetic synth nesciunt. :trollface:",
                valueComments: "Value comments. Raw denim aesthetic synth nesciunt. :trollface:")]
            public Dictionary<Options, Dictionary<string, ChildModel>> DictionaryOfDictionaries { get; set; }
        }

        [Comments("Type comments. Raw denim aesthetic synth nesciunt. :trollface:")]
        public class ChildModel
        {
            [Comments("Member comments. Raw denim aesthetic synth nesciunt. :trollface:")]
            public string Value { get; set; }
            [Comments("Member comments. Raw denim aesthetic synth nesciunt. :trollface:")]
            public int NumericValue { get; set; }
            [Comments("Member comments. Raw denim aesthetic synth nesciunt. :trollface:")]
            public bool BooleanValue { get; set; }
            [Comments("Member comments. Raw denim aesthetic synth nesciunt. :trollface:")]
            public DateTime DateValue { get; set; }
            [Comments("Member comments. Raw denim aesthetic synth nesciunt. :trollface:")]
            public TimeSpan DurationValue { get; set; }
            [Comments("Member comments. Raw denim aesthetic synth nesciunt. :trollface:")]
            public Guid GuidValue { get; set; }
        }

        [Comments("Enum comments. Raw denim aesthetic synth nesciunt. :trollface:")]
        public enum Options
        {
            [Comments("Enum option 1 comments. Raw denim aesthetic synth nesciunt. :trollface:")]
            Option1,
            [Comments("Enum option 2 comments. Raw denim aesthetic synth nesciunt. :trollface:")]
            Option2
        }

        [Secure]
        [Route("resource/comments/{id}/{option}")]
        [Description("Endpoint Name", "Endpoint remarks. Anim pariatur cliche reprehenderit. :trollface:")]

        [RequestHeader("request-header-1", "Request header 1. You probably haven't heard of them. :trollface:")]
        [RequestHeader("request-header-2", "Request header 2. Leggings occaecat craft beer farm-to-table. :trollface:", optional: true)]

        [ResponseHeader("response-header-1", "Response header 1. Vegan excepteur butcher vice lomo. :trollface:")]
        [ResponseHeader("response-header-2", "Response header 2. Raw denim aesthetic synth nesciunt. :trollface:")]

        [StatusCode(HttpStatusCode.Created, "Status code 1. Vegan excepteur butcher vice lomo. :trollface:")]
        [StatusCode(HttpStatusCode.BadGateway, "Status code 2. Raw denim aesthetic synth nesciunt. :trollface:")]

        [ResponseComments("Response description. Raw denim aesthetic synth nesciunt. :trollface:")]
        public Model Get(
            [Comments("Request parameter. Vegan excepteur butcher vice lomo. :trollface:")] Model model,
            [Comments("Url parameter 1. Enim eiusmod high life accusamus. :trollface:")] Guid id,
            [Comments("Option url parameter 2. 3 wolf moon officia aute sunt aliqua. :trollface:")] Options option,
            [Comments("Required, options query string 1. Leggings occaecat craft beer farm-to-table. :trollface:"), Required] Options requiredOption,
            [Comments("Required, multiple querystring 2. Leggings occaecat craft beer farm-to-table. :trollface:"), Required, Multiple] int requiredMultiple,
            [Comments("Optional, default querystring 3. Raw denim aesthetic synth nesciunt. :trollface:"), DefaultValue(5)] int? optionalDefault = null)
        {
            return null;
        }

        [Secure]
        [Route("resource/comments/{id}/{option}")]
        [Description("Endpoint Name", "Endpoint remarks. Anim pariatur cliche reprehenderit. :trollface:")]

        [RequestHeader("request-header-1", "Request header 1. You probably haven't heard of them. :trollface:")]
        [RequestHeader("request-header-2", "Request header 2. Leggings occaecat craft beer farm-to-table. :trollface:", true)]

        [ResponseHeader("response-header-1", "Response header 1. Vegan excepteur butcher vice lomo. :trollface:")]
        [ResponseHeader("response-header-2", "Response header 2. Raw denim aesthetic synth nesciunt. :trollface:")]

        [StatusCode(HttpStatusCode.Created, "Status code 1. Vegan excepteur butcher vice lomo. :trollface:")]
        [StatusCode(HttpStatusCode.BadGateway, "Status code 2. Raw denim aesthetic synth nesciunt. :trollface:")]

        [ResponseComments("Response description. Raw denim aesthetic synth nesciunt. :trollface:")]
        public Model Post(
            [Comments("Request parameter. Vegan excepteur butcher vice lomo. :trollface:")] Model model,
            [Comments("Url parameter 1. Enim eiusmod high life accusamus. :trollface:")] Guid id,
            [Comments("Option url parameter 2. 3 wolf moon officia aute sunt aliqua. :trollface:")] Options option,
            [Comments("Required, options query string 1. Leggings occaecat craft beer farm-to-table. :trollface:"), Required] Options requiredOption,
            [Comments("Required, multiple querystring 2. Leggings occaecat craft beer farm-to-table. :trollface:"), Required, Multiple] int requiredMultiple,
            [Comments("Optional, default querystring 3. Raw denim aesthetic synth nesciunt. :trollface:"), DefaultValue(5)]
            int? optionalDefault = null)
        {
            return null;
        }

        [Secure]
        [Route("resource/comments/{id}/{option}")]
        [Description("Endpoint Name", "Endpoint remarks. Anim pariatur cliche reprehenderit. :trollface:")]

        [RequestHeader("request-header-1", "Request header 1. You probably haven't heard of them. :trollface:")]
        [RequestHeader("request-header-2", "Request header 2. Leggings occaecat craft beer farm-to-table. :trollface:", true)]

        [ResponseHeader("response-header-1", "Response header 1. Vegan excepteur butcher vice lomo. :trollface:")]
        [ResponseHeader("response-header-2", "Response header 2. Raw denim aesthetic synth nesciunt. :trollface:")]

        [StatusCode(HttpStatusCode.Created, "Status code 1. Vegan excepteur butcher vice lomo. :trollface:")]
        [StatusCode(HttpStatusCode.BadGateway, "Status code 2. Raw denim aesthetic synth nesciunt. :trollface:")]

        [ResponseComments("Response description. Raw denim aesthetic synth nesciunt. :trollface:")]
        public Model Put(
            [Comments("Request parameter. Vegan excepteur butcher vice lomo. :trollface:")] Model model,
            [Comments("Url parameter 1. Enim eiusmod high life accusamus. :trollface:")] Guid id,
            [Comments("Option url parameter 2. 3 wolf moon officia aute sunt aliqua. :trollface:")] Options option,
            [Comments("Required, options query string 1. Leggings occaecat craft beer farm-to-table. :trollface:"), Required] Options requiredOption,
            [Comments("Required, multiple querystring 2. Leggings occaecat craft beer farm-to-table. :trollface:"), Required, Multiple] int requiredMultiple,
            [Comments("Optional, default querystring 3. Raw denim aesthetic synth nesciunt. :trollface:"), DefaultValue(5)] int? optionalDefault = null)
        {
            return null;
        }

        [Secure]
        [Route("resource/comments/{id}/{option}")]
        [Description("Endpoint Name", "Endpoint remarks. Anim pariatur cliche reprehenderit. :trollface:")]

        [RequestHeader("request-header-1", "Request header 1. You probably haven't heard of them. :trollface:")]
        [RequestHeader("request-header-2", "Request header 2. Leggings occaecat craft beer farm-to-table. :trollface:", true)]

        [ResponseHeader("response-header-1", "Response header 1. Vegan excepteur butcher vice lomo. :trollface:")]
        [ResponseHeader("response-header-2", "Response header 2. Raw denim aesthetic synth nesciunt. :trollface:")]

        [StatusCode(HttpStatusCode.Created, "Status code 1. Vegan excepteur butcher vice lomo. :trollface:")]
        [StatusCode(HttpStatusCode.BadGateway, "Status code 2. Raw denim aesthetic synth nesciunt. :trollface:")]

        [ResponseComments("Response description. Raw denim aesthetic synth nesciunt. :trollface:")]
        public void Delete(
            [Comments("Request parameter. Vegan excepteur butcher vice lomo. :trollface:")] Model model,
            [Comments("Url parameter 1. Enim eiusmod high life accusamus. :trollface:")] Guid id,
            [Comments("Option url parameter 2. 3 wolf moon officia aute sunt aliqua. :trollface:")] Options option,
            [Comments("Required, options query string 1. Leggings occaecat craft beer farm-to-table. :trollface:"), Required] Options requiredOption,
            [Comments("Required, multiple querystring 2. Leggings occaecat craft beer farm-to-table. :trollface:"), Required, Multiple] int requiredMultiple,
            [Comments("Optional, default querystring 3. Raw denim aesthetic synth nesciunt. :trollface:"), DefaultValue(5)] int? optionalDefault = null)
        {
        }

        [BinaryRequest, BinaryResponse]
        [Route("module/resource/file")]
        public Stream PostFile(Stream stream)
        {
            return null;
        }
    }
}