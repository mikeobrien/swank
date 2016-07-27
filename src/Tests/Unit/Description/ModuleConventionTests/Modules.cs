using Swank.Description;

namespace Tests.Unit.Description.ModuleConventionTests
{
    namespace ModuleDescriptions
    {
        namespace NoDescription
        {
            public class Module : ModuleDescription { }

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
            public class Module : ModuleDescription
            {
                public Module()
                {
                    Name = "Some Module";
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

        namespace EmbeddedMarkdownComments
        {
            public class Module : ModuleDescription
            {
                public Module()
                {
                    Name = "Some Markdown Module";
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

    namespace NestedModules
    {
        namespace NoModules
        {
            public class Controller
            {
                public object Get(object request)
                {
                    return null;
                }
            }
        }

        public class RootModule : ModuleDescription
        {
            public RootModule()
            {
                Name = "Root Module";
            }
        }

        namespace NestedModule
        {
            public class NestedModule : ModuleDescription
            {
                public NestedModule()
                {
                    Name = "Nested Module";
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