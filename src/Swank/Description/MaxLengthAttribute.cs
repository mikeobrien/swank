using System;

namespace Swank.Description
{
    public class MaxLengthAttribute : Attribute
    {
        public MaxLengthAttribute(int maxLength)
        {
            MaxLength = maxLength;
        }

        public int MaxLength { get; }
    }
}
