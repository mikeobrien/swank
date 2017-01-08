using System;
using System.Net;
using Swank.Extensions;

namespace Swank.Description
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class StatusCodeAttribute : Attribute
    {
        public StatusCodeAttribute(HttpStatusCode code, string comments = null) : 
            this((int)code, code.ToTitleFromPascalCasing(), comments) { }

        public StatusCodeAttribute(int code, string name, string comments = null)
        {
            Code = code;
            Name = name;
            Comments = comments;
        }
        
        public string Name { get; private set; }
        public string Comments { get; private set; }
        public int Code { get; private set; }
    }
}