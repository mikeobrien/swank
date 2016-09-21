using System;
using System.Collections.Generic;
using System.Linq;

namespace Swank.Description
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ActionNameAttribute : Attribute
    {
        public ActionNameAttribute(string name, params string[] @namespace)
        {
            Name = name;
            if (@namespace.Any())
                Namespace = @namespace.ToList();
        }

        public string Name { get; }
        public List<string> Namespace { get; set; }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ActionNamespaceAttribute : Attribute
    {
        public ActionNamespaceAttribute(params string[] @namespace)
        {
            if (@namespace.Any())
                Namespace = @namespace.ToList();
        }

        public List<string> Namespace { get; set; }
    }
}
