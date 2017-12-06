using System;
using System.Reflection;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace Swank.Description
{
    public class EnumConvention : IDescriptionConvention<Type, EnumDescription>
    {
        private readonly XmlComments _xmlComments;

        public EnumConvention(XmlComments xmlComments)
        {
            _xmlComments = xmlComments;
        }

        public virtual EnumDescription GetDescription(Type type)
        {
            var description = type.GetCustomAttribute<DescriptionAttribute>();
            var xmlComments = _xmlComments.GetType(type);

            return new EnumDescription
            {
                Name = type.GetCustomAttribute<XmlRootAttribute>()?.ElementName ??
                    type.GetCustomAttribute<XmlTypeAttribute>()?.TypeName ??
                    type.GetCustomAttribute<DataContractAttribute>()?.Name ??
                        type.GetCustomAttribute<NameAttribute>()?.Name ??
                    description?.Name ?? type.Name,
                Comments = type.GetCustomAttribute<CommentsAttribute>()?.Comments ??
                    description?.Comments ?? xmlComments?.Summary ?? xmlComments?.Remarks
            };
        }
    }
}