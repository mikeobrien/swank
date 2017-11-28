using System;

namespace Swank.Description
{
    public class RequestCommentsAttribute : Attribute
    {
        public RequestCommentsAttribute(string comments)
        {
            Comments = comments;
        }

        public string Comments { get; }
    }
}
