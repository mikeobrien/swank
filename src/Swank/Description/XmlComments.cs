using Swank.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Swank.Description
{
    public class XmlComments
    {
        public class Comments
        {
            public string Assembly { get; set; }
            public string Name { get; set; }
            public string Summary { get; set; }
            public string Remarks { get; set; }
            public string Returns { get; set; }
            public Dictionary<string, string> Parameters { get; set; }
        }

        private readonly Lazy<List<Comments>> _comments;

        public XmlComments(Configuration.Configuration configuration)
        {
            _comments = new Lazy<List<Comments>>(() => Load(configuration));
        }

        private static List<Comments> Load(Configuration.Configuration configuration)
        {
            return configuration.XmlComments
                .Select(x => XDocument.Parse(x.ReadString()))
                .Select(x =>
                {
                    var assembly = x.XPathSelectElement("/doc/assembly/name").Value.Trim();
                    return x.XPathSelectElements("/doc/members/member")
                        .Select(y => new Comments
                        {
                            Assembly = assembly,
                            Name = y.Attribute("name")?.Value.Trim(),
                            Summary = y.Element("summary")?.Value.Trim(),
                            Remarks = y.Element("remarks")?.Value.Trim(),
                            Returns = y.Element("returns")?.Value.Trim(),
                            Parameters = y.Elements("param").ToDictionary(z => 
                                z.Attribute("name")?.Value.Trim(), z => z.Value)
                        });
                }).SelectMany().ToList();
        }

        public Comments GetType(Type type)
        {
            var assembly = type.Assembly.GetName().Name;
            var name = "T:" + GetTypeFullName(type);
            return _comments.Value.FirstOrDefault(x => 
                x.Assembly == assembly && x.Name == name);
        }

        public Comments GetProperty(PropertyInfo property)
        {
            var assembly = property.DeclaringType.Assembly.GetName().Name;
            var name = "P:" + GetTypeFullName(property.DeclaringType) + "." + property.Name;
            return _comments.Value.FirstOrDefault(x =>
                x.Assembly == assembly && x.Name == name);
        }

        public Comments GetField(FieldInfo field)
        {
            var assembly = field.DeclaringType.Assembly.GetName().Name;
            var name = "F:" + GetTypeFullName(field.DeclaringType) + "." + field.Name;
            return _comments.Value.FirstOrDefault(x =>
                x.Assembly == assembly && x.Name == name);
        }

        public Comments GetMethod(MethodInfo method)
        {
            var assembly = method.DeclaringType.Assembly.GetName().Name;
            var arguments = method.GetGenericArguments().ToList();
            var genericArgumentPrefix = arguments.Any() ? "``" + arguments.Count : "";
            var name = "M:" + GetTypeFullName(method.DeclaringType) + "." + method.Name + 
                genericArgumentPrefix + GetMethodSignature(method, arguments);
            return _comments.Value.FirstOrDefault(x =>
                x.Assembly == assembly && x.Name == name);
        }

        private string GetMethodSignature(MethodInfo method, List<Type> arguments)
        {
            var parameters = method.GetParameters();
            return !parameters.Any() ? "" : "(" + parameters.Select(x => 
                GetTypeName(x.ParameterType, arguments)).Join(",") + ")";
        }

        private string GetTypeName(Type type, List<Type> arguments)
        {
            if (arguments.Contains(type)) return "``" + arguments.IndexOf(type);
            var genericArguments = type.GetGenericArguments();
            return GetParameterTypeFullName(type) + 
                (genericArguments.Length == 0 ? "" : 
                    "{" + genericArguments
                        .Select(x => GetTypeName(x, arguments))
                        .Join(",") + 
                    "}");
        }

        private string GetTypeFullName(Type type)
        {
            return type.FullName.Replace("+", ".").Split('[').First();
        }

        public  string GetParameterTypeFullName(Type type)
        {
            return (type.FullName.IsNotNullOrEmpty()
                ? type.FullName.Replace("+", ".")
                : type.Namespace + "." + type.Name)
                    .Split('`').First();
        }
    }
}
