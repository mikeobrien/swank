using Swank.Description;

namespace Tests.Unit.Description.EndpointConventionTests
{
    namespace EndpointDescriptions
    {
        public class NoDescriptionController
        {
            public object Get(object request)
            {
                return null;
            }
        }

        namespace ActionDescription
        {
            public class XmlCommentController
            {
                /// <summary>summary</summary>
                /// <remarks>remarks</remarks>
                public object Get(object request)
                {
                    return null;
                }
            }

            public class EmbeddedDescriptionController
            {
                public object Get(object request)
                {
                    return null;
                }
            }

            public class AttributeCommentsController
            {
                [Comments("action comments")]
                public object Get(object request)
                {
                    return null;
                }
            }

            public class Controller
            {
                [Description("action name", "action description")]
                public object Get(object request)
                {
                    return null;
                }
            }
        }

        namespace RequestDescription
        {
            public class EmbeddedDescriptionController
            {
                public object Get(object request)
                {
                    return null;
                }
            }

            public class XmlCommentsController
            {
                /// <param name="request">request description</param>
                public object Get(object request)
                {
                    return null;
                }
            }

            public class AttributeController
            {
                [RequestComments("request description")]
                public object Get(object request)
                {
                    return null;
                }
            }
        }

        namespace ResponseDescription
        {
            public class EmbeddedDescriptionController
            {
                public object Get(object request)
                {
                    return null;
                }
            }

            public class XmlCommentsController
            {
                /// <returns>response description</returns>
                public object Get(object request)
                {
                    return null;
                }
            }

            public class AttributeController
            {
                [ResponseComments("response description")]
                public object Get(object request)
                {
                    return null;
                }
            }
        }

        namespace BinaryDescription
        {
            public class BinaryResponseController
            {
                [BinaryResponse]
                public object Get(object request)
                {
                    return null;
                }
            }

            public class BinaryRequestController
            {
                [BinaryRequest]
                public object Get(object request)
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
    }

    namespace AttributePriority
    {
        public class Controller
        {
            [Description("action name", "action description")]
            public object Get(object request)
            {
                return null;
            }
        }
    }
}