using System;
using System.Reflection;

namespace Swank.Description
{
    public interface IParameterDescription
    {
        string Name { get; }
        string Documentation { get; }
        object DefaultValue { get; }
        Type Type { get; }
        bool IsOptional { get; }
        bool IsUrlParameter { get; }
        bool IsQuerystring { get; }
        MethodInfo ActionMethod { get; }

        T GetAttribute<T>() where T : Attribute;
        bool HasAttribute<T>() where T : Attribute;
    }
}