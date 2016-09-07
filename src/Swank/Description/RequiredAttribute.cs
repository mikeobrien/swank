using System;

namespace Swank.Description
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public class RequiredAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public class RequiredForPostAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public class RequiredForPutAttribute : Attribute { }
}