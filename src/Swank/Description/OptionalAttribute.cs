using System;

namespace Swank.Description
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public class OptionalAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public class OptionalForPostAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public class OptionalForPutAttribute : Attribute { }
}