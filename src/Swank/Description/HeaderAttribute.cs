using System;

namespace Swank.Description
{
    public class RequestHeaderAttribute : HeaderAttribute
    {
        public RequestHeaderAttribute(string name,
            string comments = null, bool optional = false)
            : base(name, comments, optional) { }
    }

    public class ResponseHeaderAttribute : HeaderAttribute
    {
        public ResponseHeaderAttribute(string name,
            string comments = null) : base(name, comments) { }
    }

    [AttributeUsage(AttributeTargets.Method | 
        AttributeTargets.Class, AllowMultiple = true)]
    public abstract class HeaderAttribute : Attribute
    {
        protected HeaderAttribute(string name, 
            string comments, bool optional = false)
        {
            Name = name;
            Comments = comments;
            Optional = optional;
        }
        
        public string Name { get; private set; }
        public string Comments { get; private set; }
        public bool Optional { get; set; }
    }
}