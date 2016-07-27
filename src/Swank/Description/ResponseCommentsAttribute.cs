using System;

namespace Swank.Description
{
    public class ResponseCommentsAttribute : Attribute
    {
        public ResponseCommentsAttribute(string comments)
        {
            Comments = comments;
        }

        public string Comments { get; private set; }
    }
}
