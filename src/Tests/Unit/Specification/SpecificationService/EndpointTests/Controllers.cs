using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Swank.Description;

namespace Tests.Unit.Specification.SpecificationService.EndpointTests
{
    namespace EndpointDescriptions
    {
        public class NoDescriptionController
        {
            public object Get(object request)
            {
                return null;
            }

            public object Post(object request)
            {
                return null;
            }
        }

        namespace ControllerDescription
        {
            public class EmbeddedDescriptionController
            {
                public object Get(object request)
                {
                    return null;
                }
            }

            public class Controller
            {
                [Description("Some get Controller name", "Some get **Controller** description")]
                public object Get(object request)
                {
                    return null;
                }

                [Description("Some post Controller name", "Some post **Controller** description")]
                public object Post(object request)
                {
                    return null;
                }

                [Description("Some put Controller name", "Some put **Controller** description")]
                public object Put(object request)
                {
                    return null;
                }

                [Description("Some delete Controller name", "Some delete **Controller** description")]
                public object Delete(object request)
                {
                    return null;
                }
            }
        }

        namespace ActionDescription
        {
            public class EmbeddedDescriptionController
            {
                public object Get(object request)
                {
                    return null;
                }
            }

            public class Controller
            {
                [Description("Some get action name", "Some get **action** description")]
                public object Get(object request)
                {
                    return null;
                }

                [Description("Some post action name", "Some post **action** description")]
                public object Post(object request)
                {
                    return null;
                }

                [Description("Some put action name", "Some put **action** description")]
                public object Put(object request)
                {
                    return null;
                }

                [Description("Some delete action name", "Some delete **action** description")]
                public object Delete(object request)
                {
                    return null;
                }
            }
        }

        namespace SecureDescription
        {
            public class PublicController
            {
                public object Get(object request)
                {
                    return null;
                }
            }

            [Secure]
            public class SecureController
            {
                public object Get(object request)
                {
                    return null;
                }
            }

            public class SecureActionController
            {
                [Secure]
                public object Get(object request)
                {
                    return null;
                }
            }
        }

        namespace BinaryDescription
        {
            public class BinaryRequestController
            {
                [BinaryRequest]
                public object Get(object request)
                {
                    return null;
                }
            }

            public class BinaryResponseController
            {
                [BinaryResponse]
                public object Get(object request)
                {
                    return null;
                }
            }
        }
    }

    namespace HiddenEndpointAttributes
    {
        public class HiddenActionController
        {
            [Hide]
            public object Get(object request)
            {
                return null;
            } 
        }

        [Hide]
        public class HiddenController
        {
            public object Get(object request)
            {
                return null;
            }
        }

        public class VisibleController
        {
            public object Get(object request)
            {
                return null;
            }
        }
    }

    namespace ControllerResource
    {
        [Resource("Some Controller")]
        public class Controller
        {
            public object Get(object request)
            {
                return null;
            }
        }
    }

    namespace InputTypeDescriptions
    {
        public class Controller
        {
            [RequestComments("Some get **request** description")]
            public object Get(object request)
            {
                return null;
            }

            [RequestComments("Some post **request** description")]
            public object Post(object request)
            {
                return null;
            }

            [RequestComments("Some put **request** description")]
            public object Put(object request)
            {
                return null;
            }

            [RequestComments("Some delete **request** description")]
            public object Delete(object request)
            {
                return null;
            }
        }

        public class RequestItem {}

        public class CollectionController
        {
            public object Post(List<RequestItem> request)
            {
                return null;
            }
        }

        public class RequestItems : List<RequestItem> { }

        public class InheritedCollectionController
        {
            public object Post(RequestItems request)
            {
                return null;
            }
        }

        [XmlType("NewItemName")]
        public class OverridenRequestItem { }

        public class OverridenRequestController
        {
            public object Post(OverridenRequestItem request)
            {
                return null;
            }
        }

        [XmlType("NewCollectionName")]
        public class OverridenRequestItems : List<OverridenRequestItem> { }

        public class OverridenCollectionController
        {
            public object Post(OverridenRequestItems request)
            {
                return null;
            }
        }
    }

    namespace ResponseDescriptions
    {
        public class Controller
        {
            [ResponseComments("Some get **response** description")]
            public object Get(object request)
            {
                return null;
            }

            [ResponseComments("Some post **response** description")]
            public object Post(object request)
            {
                return null;
            }

            [ResponseComments("Some put **response** description")]
            public object Put(object request)
            {
                return null;
            }

            [ResponseComments("Some delete **response** description")]
            public object Delete(object request)
            {
                return null;
            }
        }

        public class ResponseItem { }

        public class CollectionController
        {
            public List<ResponseItem> Get(object request)
            {
                return null;
            }
        }

        public class ResponseItems : List<ResponseItem> { }

        public class InheritedCollectionController
        {
            public ResponseItems Get(object request)
            {
                return null;
            }
        }

        [XmlType("NewItemName")]
        public class OverridenResponseItem { }

        public class OverridenRequestController
        {
            public OverridenResponseItem Get(object request)
            {
                return null;
            }
        }

        [XmlType("NewCollectionName")]
        public class OverridenResponseItems : List<OverridenResponseItem> { }

        public class OverridenCollectionController
        {
            public OverridenResponseItems Get(object request)
            {
                return null;
            }
        }
    }

    namespace StatusCodeDescriptions
    {
        [StatusCode(411, "411 **error** on Controller")]
        [StatusCode(410, "410 **error** on Controller", "410 error on action description")]
        public class StatusCodesController
        {
            [StatusCode(413, "413 error **on** action")]
            [StatusCode(412, "412 error **on** action", "412 error on action description")]
            public object Get(object request)
            {
                return null;
            }
        }

        public class NoStatusCodesController
        {
            public object Get(object request)
            {
                return null;
            }
        }
    }

    namespace HeaderDescriptions
    {
        [ResponseHeader("content-type")]
        [RequestHeader("api-key", "This is a **Controller** description.", true)]
        public class HeadersController
        {
            [RequestHeader("accept", "This is an **endpoint** description.")]
            [ResponseHeader("content-length")]
            public object Get(object request)
            {
                return null;
            }
        }

        public class NoHeadersController
        {
            public object Get(object request)
            {
                return null;
            }
        }
    }

    namespace Querystrings
    {
        public class Controller
        {
            public object Get(object request, string urlParameter,
                string querystring,
                [Description("description", "description **comments**")]
                string descriptionQuerystring,
                [Comments("querystring **comments**")]
                string commentsQuerystring,
                [Multiple] List<int> multipleQuerystring,
                [Hide]
                string hiddenQuerystring,
                [XmlIgnore]
                string xmlIgnoreQuerystring,
                [DefaultValue(5)]
                int defaultQuerystring,
                [SampleValue(5)]
                int sampleValueQuerystring,
                Guid? optionalQuerystring = null)
            {
                return null;
            }
        }

        public enum Options
        {
            [Hide]
            Option2, 
            Option3, 
            [Description("Option 1", "Option **1** description.")]
            Option1
        }

        public class OptionController
        {
            public object Get(object request, string urlParameter, Options querystring)
            {
                return null;
            }
        }
    }

    namespace UrlParameters
    {
        public class NoCommentsController
        {
            public object Get(object request, Guid urlParameter)
            {
                return null;
            }
        }

        public class CommentsController
        {
            public object Get(object request,
                [Comments("url **parameter** comments")]
                Guid urlParameter)
            {
                return null;
            }
        }

        public enum Options
        {
            [Hide]
            Option2,
            [Description("Option 1", "Option **1** description.")]
            Option1,
            Option3
        }

        public class OptionController
        {
            public object Get(object request, Options urlParameter)
            {
                return null;
            }
        }
    }
}
