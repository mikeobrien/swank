namespace Tests.Unit.Description.MarkerConventionTests
{
    namespace MarkerDescriptions
    {
        namespace NoDescription
        {
            public class Description : Swank.Description.Description { }
        }

        namespace Description
        {
            public class Description : Swank.Description.Description
            {
                public Description()
                {
                    Name = "Some Description";
                    Comments = "Some comments.";
                }
            }
        }

        namespace EmbeddedMarkdownComments
        {
            public class Description : Swank.Description.Description
            {
                public Description()
                {
                    Name = "Some Markdown Description";
                }
            }
        }
    }

    namespace MarkerOrder
    {
        namespace AFirstMarker
        {
            public class LastDescription : Swank.Description.Description
            {
                public LastDescription()
                {
                    Name = "Last Description";
                }
            }

            public class FirstDescription : Swank.Description.Description
            {
                public FirstDescription()
                {
                    Name = "First Description";
                }
            }
        }

        namespace ZeeLastMarker
        {
            public class LastDescription : Swank.Description.Description
            {
                public LastDescription()
                {
                    Name = "Last Description";
                }
            }

            public class FirstDescription : Swank.Description.Description
            {
                public FirstDescription()
                {
                    Name = "First Description";
                }
            }
        }
    }

    namespace MarkerCommentsPriority
    {
        public class Description : Swank.Description.Description
        {
            public Description()
            {
                Name = "Some Description";
                Comments = "Some comments.";
            }
        }
    }
}