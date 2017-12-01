using Swank.Description;

namespace Tests.Unit.Description.ResourceConventionTests
{
    namespace ResourceDescriptions
    {
        namespace NoDescription
        {
            public class Resource : ResourceDescription { }

            public class Controller
            {
                public object Get(object request)
                {
                    return null;
                }
            }
        }

        namespace Description
        {
            public class Resource : ResourceDescription
            {
                public Resource()
                {
                    Name = "Some Resource";
                    Comments = "Some comments.";
                }
            }

            public class Controller
            {
                public object Get(object request)
                {
                    return null;
                }
            }
        }

        namespace EmbeddedTextComments
        {
            public class Resource : ResourceDescription
            {
                public Resource()
                {
                    Name = "Some Text Resource";
                }
            }

            public class Controller
            {
                public object Get(object request)
                {
                    return null;
                }
            }
        }

        namespace EmbeddedMarkdownResourceMarkerComments
        {
            public class Resource : ResourceDescription
            {
                public Resource()
                {
                    Name = "Some Markdown Resource";
                }
            }

            public class Controller
            {
                public object Get(object request)
                {
                    return null;
                }
            }
        }

        namespace EmbeddedMarkdownResourceComments
        {
            public class Controller
            {
                public object Get(object request)
                {
                    return null;
                }
            }
        }
    }

    namespace AttributeResource
    {
        [Resource("Some Resource", "Some resource description")]
        public class Controller
        {
            public object Get(object request)
            {
                return null;
            }
        }

        [Resource("Some Markdown Resource")]
        public class EmbeddedMarkdownController
        {
            public object Get(object request)
            {
                return null;
            }
        }

        [Resource("Some Text Resource")]
        public class EmbeddedTextController
        {
            public object Get(object request)
            {
                return null;
            }
        }
    }

    namespace XmlCommentsResource
    {
        /// <summary>summary</summary>
        /// <remarks>remarks</remarks>
        public class Controller
        {
            public object Get(object request)
            {
                return null;
            }
        }

        /// <summary>summary</summary>
        public class MarkdownCommentsController
        {
            public object Get(object request)
            {
                return null;
            }
        }

        /// <remarks>remarks</remarks>
        public class MissingSummaryController
        {
            public object Get(object request)
            {
                return null;
            }
        }

        public class Resource : ResourceDescription
        {
            public Resource()
            {
                Name = "Some Markdown Resource";
            }
        }
    }

    namespace OrphanedAction
    {
        public class Controller
        {
            public object Get(object request)
            {
                return null;
            }
        }
    }

    namespace ChildResources
    {
        public class Resource : ResourceDescription
        {
            public Resource()
            {
                Name = "Some Resource";
            }
        }

        namespace ChildNamespace
        {
            public class Controller
            {
                public object Get(object request)
                {
                    return null;
                }
            }
        }
    }

    namespace NestedResources
    {
        public class Resource : ResourceDescription
        {
            public Resource()
            {
                Name = "Some Resource";
            }
        }

        namespace ChildNamespace
        {
            public class Resource : ResourceDescription
            {
                public Resource()
                {
                    Name = "Another Resource";
                }
            }

            public class Controller
            {
                public object Get(object request)
                {
                    return null;
                }
            }
        }
    }

    namespace ResourceCommentsPriority
    {
        public class Resource : ResourceDescription
        {
            public Resource()
            {
                Name = "Some Description";
                Comments = "Some comments.";
            }
        }

        public class Controller
        {
            public object Get(object request)
            {
                return null;
            }
        }
    }
}