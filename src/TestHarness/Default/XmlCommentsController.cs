using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web.Http;
using Swank.Description;

namespace TestHarness.Default
{
    /// <summary>/xml/comments</summary>
    /// <remarks>
    /// Resource description. Raw denim aesthetic synth nesciunt. :trollface:
    /// </remarks>
    public class XmlCommentsController : ApiController
    {
        /// <summary>   
        /// Type summary. You probably haven't heard of them. :trollface:
        /// </summary>
        /// <remarks>
        /// Type remarks. Raw denim aesthetic synth nesciunt. :trollface:
        /// </remarks>
        public class Model
        {
            /// <summary>
            /// Member summary. Raw denim aesthetic synth nesciunt. :trollface:
            /// </summary>
            /// <remarks>
            /// Member remarks. Raw denim aesthetic synth nesciunt. :trollface:
            /// </remarks>
            public string Value { get; set; }
            /// <summary>Member summary. Raw denim aesthetic synth nesciunt. :trollface:</summary>
            [Optional]
            public string OptionalValue { get; set; }
            /// <summary>Member summary. Raw denim aesthetic synth nesciunt. :trollface:</summary>
            [DefaultValue("fark")]
            public string DefaultValue { get; set; }
            /// <summary>Member summary. Raw denim aesthetic synth nesciunt. :trollface:</summary>
            public Options OptionValue { get; set; }
            /// <summary>Member summary. Raw denim aesthetic synth nesciunt. :trollface:</summary>
            [Obsolete("Obsolete comments. Raw denim aesthetic synth nesciunt. :trollface:")]
            public string DepricatedValue { get; set; }
            /// <summary>Member summary. Raw denim aesthetic synth nesciunt. :trollface:</summary>
            public int NumericValue { get; set; }
            /// <summary>Member summary. Raw denim aesthetic synth nesciunt. :trollface:</summary>
            public bool BooleanValue { get; set; }
            /// <summary>Member summary. Raw denim aesthetic synth nesciunt. :trollface:</summary>
            public DateTime DateValue { get; set; }
            /// <summary>Member summary. Raw denim aesthetic synth nesciunt. :trollface:</summary>
            public TimeSpan DurationValue { get; set; }
            /// <summary>Member summary. Raw denim aesthetic synth nesciunt. :trollface:</summary>
            public Guid GuidValue { get; set; }
            /// <summary>Member summary. Raw denim aesthetic synth nesciunt. :trollface:</summary>
            public ChildModel ComplexValue { get; set; }
            /// <summary>Member summary. Raw denim aesthetic synth nesciunt. :trollface:</summary>
            public List<int> SimpleList { get; set; }
            /// <summary>Member summary. Raw denim aesthetic synth nesciunt. :trollface:</summary>
            public List<ChildModel> ComplexList { get; set; }
            /// <summary>Member summary. Raw denim aesthetic synth nesciunt. :trollface:</summary>
            public List<List<ChildModel>> ListOfList { get; set; }
            /// <summary>Member summary. Raw denim aesthetic synth nesciunt. :trollface:</summary>
            public List<Dictionary<string, ChildModel>> ListOfDictionary { get; set; }
            /// <summary>Member summary. Raw denim aesthetic synth nesciunt. :trollface:</summary>
            [DictionaryDescription(comments: "Dictionary comments. Raw denim aesthetic synth nesciunt. :trollface:",
                keyName: "key-name", keyComments: "Key comments. Raw denim aesthetic synth nesciunt. :trollface:", 
                valueComments: "Value comments. Raw denim aesthetic synth nesciunt. :trollface:")]
            public Dictionary<string, int> SimpleTypeDictionary { get; set; }
            /// <summary>Member summary. Raw denim aesthetic synth nesciunt. :trollface:</summary>
            [DictionaryDescription(comments: "Dictionary comments. Raw denim aesthetic synth nesciunt. :trollface:",
                keyName: "key-name", keyComments: "Key comments. Raw denim aesthetic synth nesciunt. :trollface:",
                valueComments: "Value comments. Raw denim aesthetic synth nesciunt. :trollface:")]
            public Dictionary<string, ChildModel> ComplexTypeDictionary { get; set; }
            /// <summary>Member summary. Raw denim aesthetic synth nesciunt. :trollface:</summary>
            [DictionaryDescription(comments: "Dictionary comments. Raw denim aesthetic synth nesciunt. :trollface:",
                keyName: "key-name", keyComments: "Key comments. Raw denim aesthetic synth nesciunt. :trollface:",
                valueComments: "Value comments. Raw denim aesthetic synth nesciunt. :trollface:")]
            public Dictionary<Options, Options> OptionsDictionary { get; set; }
            /// <summary>Member summary. Raw denim aesthetic synth nesciunt. :trollface:</summary>
            [DictionaryDescription(comments: "Dictionary comments. Raw denim aesthetic synth nesciunt. :trollface:",
                keyName: "key-name", keyComments: "Key comments. Raw denim aesthetic synth nesciunt. :trollface:",
                valueComments: "Value comments. Raw denim aesthetic synth nesciunt. :trollface:")]
            public Dictionary<Options, List<ChildModel>> DictionaryOfLists { get; set; }
            /// <summary>Member summary. Raw denim aesthetic synth nesciunt. :trollface:</summary>
            [DictionaryDescription(comments: "Dictionary comments. Raw denim aesthetic synth nesciunt. :trollface:",
                keyName: "key-name", keyComments: "Key comments. Raw denim aesthetic synth nesciunt. :trollface:",
                valueComments: "Value comments. Raw denim aesthetic synth nesciunt. :trollface:")]
            public Dictionary<Options, Dictionary<string, ChildModel>> DictionaryOfDictionaries { get; set; }
        }

        /// <remarks>
        /// Type remarks. Raw denim aesthetic synth nesciunt. :trollface:
        /// </remarks>
        public class ChildModel
        {
            /// <summary>Member summary. Raw denim aesthetic synth nesciunt. :trollface:</summary>
            public string Value { get; set; }
            /// <summary>Member summary. Raw denim aesthetic synth nesciunt. :trollface:</summary>
            public int NumericValue { get; set; }
            /// <summary>Member summary. Raw denim aesthetic synth nesciunt. :trollface:</summary>
            public bool BooleanValue { get; set; }
            /// <summary>Member summary. Raw denim aesthetic synth nesciunt. :trollface:</summary>
            public DateTime DateValue { get; set; }
            /// <summary>Member summary. Raw denim aesthetic synth nesciunt. :trollface:</summary>
            public TimeSpan DurationValue { get; set; }
            /// <summary>Member summary. Raw denim aesthetic synth nesciunt. :trollface:</summary>
            public Guid GuidValue { get; set; }
        }

        /// <summary>   
        /// Enum summary. You probably haven't heard of them. :trollface:
        /// </summary>
        /// <remarks>
        /// Enum remarks. Raw denim aesthetic synth nesciunt. :trollface:
        /// </remarks>
        public enum Options
        {
            /// <summary>
            /// Enum option 1 summary. You probably haven't heard of them. :trollface:
            /// </summary>
            /// <remarks>
            /// Enum option 1 remarks. Raw denim aesthetic synth nesciunt. :trollface:
            /// </remarks>
            Option1,
            /// <summary>
            /// Enum option 2 summary. Vegan excepteur butcher vice lomo. :trollface:
            /// </summary>
            /// <remarks>
            /// Enum option 2 remarks. Raw denim aesthetic synth nesciunt. :trollface:
            /// </remarks>
            Option2
        }

        /// <summary>
        /// Endpoint Name
        /// </summary>
        /// <remarks>Endpoint remarks. Anim pariatur cliche reprehenderit. :trollface:</remarks>
        /// <param name="model">Request parameter. Vegan excepteur butcher vice lomo. :trollface:</param>
        /// <param name="id">Url parameter 1. Enim eiusmod high life accusamus. :trollface:</param>
        /// <param name="option">Option url parameter 2. 3 wolf moon officia aute sunt aliqua. :trollface:</param>
        /// <param name="requiredOption">Required, options query string 1. Leggings occaecat craft beer farm-to-table. :trollface:</param>
        /// <param name="requiredMultiple">Required, multiple querystring 2. Leggings occaecat craft beer farm-to-table. :trollface:</param>
        /// <param name="optionalDefault">Optional, default querystring 3. Raw denim aesthetic synth nesciunt. :trollface:</param>
        /// <returns>Response description. Raw denim aesthetic synth nesciunt. :trollface:</returns>
        [Secure]
        [Route("xml/comments/{id}/{option}")] 

        [RequestHeader("request-header-1", "Request header 1. You probably haven't heard of them. :trollface:")]
        [RequestHeader("request-header-2", "Request header 2. Leggings occaecat craft beer farm-to-table. :trollface:", true)]

        [ResponseHeader("response-header-1", "Response header 1. Vegan excepteur butcher vice lomo. :trollface:")]
        [ResponseHeader("response-header-2", "Response header 2. Raw denim aesthetic synth nesciunt. :trollface:")]

        [StatusCode(HttpStatusCode.Created, "Status code 1. Vegan excepteur butcher vice lomo. :trollface:")]
        [StatusCode(HttpStatusCode.BadGateway, "Status code 2. Raw denim aesthetic synth nesciunt. :trollface:")]

        public Model Get(Model model, 
            Guid id,
            Options option,
            [Required] Options requiredOption,
            [Required, Multiple] int requiredMultiple,
            [DefaultValue(5)] int? optionalDefault = null)
        {
            return null;
        }

        /// <summary>
        /// Endpoint Name
        /// </summary>
        /// <remarks>Endpoint remarks. Anim pariatur cliche reprehenderit. :trollface:</remarks>
        /// <param name="model">Request parameter. Vegan excepteur butcher vice lomo. :trollface:</param>
        /// <param name="id">Url parameter 1. Enim eiusmod high life accusamus. :trollface:</param>
        /// <param name="option">Option url parameter 2. 3 wolf moon officia aute sunt aliqua. :trollface:</param>
        /// <param name="requiredOption">Required, options query string 1. Leggings occaecat craft beer farm-to-table. :trollface:</param>
        /// <param name="requiredMultiple">Required, multiple querystring 2. Leggings occaecat craft beer farm-to-table. :trollface:</param>
        /// <param name="optionalDefault">Optional, default querystring 3. Raw denim aesthetic synth nesciunt. :trollface:</param>
        /// <returns>Response description. Raw denim aesthetic synth nesciunt. :trollface:</returns>
        [Secure]
        [Route("xml/comments/{id}/{option}")]

        [RequestHeader("request-header-1", "Request header 1. You probably haven't heard of them. :trollface:")]
        [RequestHeader("request-header-2", "Request header 2. Leggings occaecat craft beer farm-to-table. :trollface:", true)]

        [ResponseHeader("response-header-1", "Response header 1. Vegan excepteur butcher vice lomo. :trollface:")]
        [ResponseHeader("response-header-2", "Response header 2. Raw denim aesthetic synth nesciunt. :trollface:")]

        [StatusCode(HttpStatusCode.Created, "Status code 1. Vegan excepteur butcher vice lomo. :trollface:")]
        [StatusCode(HttpStatusCode.BadGateway, "Status code 2. Raw denim aesthetic synth nesciunt. :trollface:")]

        public Model Post(Model model,
            Guid id,
            Options option,
            [Required] Options requiredOption,
            [Required, Multiple] int requiredMultiple,
            [DefaultValue(5)] int? optionalDefault = null)
        {
            return null;
        }

        /// <summary>
        /// Endpoint Name
        /// </summary>
        /// <remarks>Endpoint remarks. Anim pariatur cliche reprehenderit. :trollface:</remarks>
        /// <param name="model">Request parameter. Vegan excepteur butcher vice lomo. :trollface:</param>
        /// <param name="id">Url parameter 1. Enim eiusmod high life accusamus. :trollface:</param>
        /// <param name="option">Option url parameter 2. 3 wolf moon officia aute sunt aliqua. :trollface:</param>
        /// <param name="requiredOption">Required, options query string 1. Leggings occaecat craft beer farm-to-table. :trollface:</param>
        /// <param name="requiredMultiple">Required, multiple querystring 2. Leggings occaecat craft beer farm-to-table. :trollface:</param>
        /// <param name="optionalDefault">Optional, default querystring 3. Raw denim aesthetic synth nesciunt. :trollface:</param>
        /// <returns>Response description. Raw denim aesthetic synth nesciunt. :trollface:</returns>
        [Secure]
        [Route("xml/comments/{id}/{option}")]

        [RequestHeader("request-header-1", "Request header 1. You probably haven't heard of them. :trollface:")]
        [RequestHeader("request-header-2", "Request header 2. Leggings occaecat craft beer farm-to-table. :trollface:", true)]

        [ResponseHeader("response-header-1", "Response header 1. Vegan excepteur butcher vice lomo. :trollface:")]
        [ResponseHeader("response-header-2", "Response header 2. Raw denim aesthetic synth nesciunt. :trollface:")]

        [StatusCode(HttpStatusCode.Created, "Status code 1. Vegan excepteur butcher vice lomo. :trollface:")]
        [StatusCode(HttpStatusCode.BadGateway, "Status code 2. Raw denim aesthetic synth nesciunt. :trollface:")]

        public Model Put(Model model,
            Guid id,
            Options option,
            [Required] Options requiredOption,
            [Required, Multiple] int requiredMultiple,
            [DefaultValue(5)] int? optionalDefault = null)
        {
            return null;
        }

        /// <summary>
        /// Endpoint Name
        /// </summary>
        /// <remarks>Endpoint remarks. Anim pariatur cliche reprehenderit. :trollface:</remarks>
        /// <param name="model">Request parameter. Vegan excepteur butcher vice lomo. :trollface:</param>
        /// <param name="id">Url parameter 1. Enim eiusmod high life accusamus. :trollface:</param>
        /// <param name="option">Option url parameter 2. 3 wolf moon officia aute sunt aliqua. :trollface:</param>
        /// <param name="requiredOption">Required, options query string 1. Leggings occaecat craft beer farm-to-table. :trollface:</param>
        /// <param name="requiredMultiple">Required, multiple querystring 2. Leggings occaecat craft beer farm-to-table. :trollface:</param>
        /// <param name="optionalDefault">Optional, default querystring 3. Raw denim aesthetic synth nesciunt. :trollface:</param>
        /// <returns>Response description. Raw denim aesthetic synth nesciunt. :trollface:</returns>
        [Secure]
        [Route("xml/comments/{id}/{option}")]

        [RequestHeader("request-header-1", "Request header 1. You probably haven't heard of them. :trollface:")]
        [RequestHeader("request-header-2", "Request header 2. Leggings occaecat craft beer farm-to-table. :trollface:", true)]

        [ResponseHeader("response-header-1", "Response header 1. Vegan excepteur butcher vice lomo. :trollface:")]
        [ResponseHeader("response-header-2", "Response header 2. Raw denim aesthetic synth nesciunt. :trollface:")]

        [StatusCode(HttpStatusCode.Created, "Status code 1. Vegan excepteur butcher vice lomo. :trollface:")]
        [StatusCode(HttpStatusCode.BadGateway, "Status code 2. Raw denim aesthetic synth nesciunt. :trollface:")]

        public void Delete(Model model,
            Guid id,
            Options option,
            [Required] Options requiredOption,
            [Required, Multiple] int requiredMultiple,
            [DefaultValue(5)] int? optionalDefault = null)
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