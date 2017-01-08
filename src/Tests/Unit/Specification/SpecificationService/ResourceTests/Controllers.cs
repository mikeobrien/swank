using Swank.Description;

namespace Tests.Unit.Specification.SpecificationService.ResourceTests
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
                    Comments = "Some **comments**.";
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

        namespace EmbeddedMarkdownComments
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

        namespace OrphanedEmbeddedMarkdown
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
        namespace Attribute
        {
            [Resource("Some Resource", "Some **resource** description")]
            public class Controller
            {
                public object Get(object request)
                {
                    return null;
                }
            }
        }

        namespace EmbeddedMarkdownComments
        {
            [Resource("Some Markdown Resource")]
            public class Controller
            {
                public object Get(object request)
                {
                    return null;
                }
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

    namespace NotOrphanedAction
    {
        public class Resource : ResourceDescription
        {
            public Resource()
            {
                Name = "Some Resource";
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

    namespace SameNamespace
    {
        public class Resource : ResourceDescription
        {
            public Resource()
            {
                Name = "Some Resource";
            }
        }

        public class Controller
        {
            public object Get(object request)
            {
                return null;
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

    namespace ChildResources
    {
        public class Resource : ResourceDescription
        {
            public Resource()
            {
                Name = "Some Resource";
            }
        }

        public class Controller
        {
            public object Get(object request)
            {
                return null;
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

        public class Controller
        {
            public object Get(object request)
            {
                return null;
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

    namespace OrphanedResources
    {
        public class Controller
        {
            public object Get(object request)
            {
                return null;
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

    namespace OrphanedNestedResources
    {
        public class Controller
        {
            public object Get(object request)
            {
                return null;
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
}