using System;

namespace Swank.Description
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public class DefaultValueAttribute : Attribute
    {
         public DefaultValueAttribute(object value)
         {
             Value = value;
         }

        public object Value { get; private set; }
    }
}