using System;

namespace Swank.Description
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public class SampleValueAttribute : Attribute
    {
        public SampleValueAttribute(object value)
        {
            Value = value;
        }

        public object Value { get; private set; }
    }
}