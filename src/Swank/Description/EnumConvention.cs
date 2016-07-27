using System;
using System.Reflection;
using System.Xml.Serialization;
using Swank.Extensions;
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
                Name = type.GetCustomAttribute<XmlRootAttribute>()
                        .WhenNotNull(x => x.ElementName).OtherwiseDefault() ??
                    type.GetCustomAttribute<XmlTypeAttribute>()
                        .WhenNotNull(x => x.TypeName).OtherwiseDefault() ??
                    type.GetCustomAttribute<DataContractAttribute>()
                        .WhenNotNull(x => x.Name).OtherwiseDefault() ??
                    description.WhenNotNull(x => x.Name).OtherwiseDefault() ??
                    type.Name,
                Comments = type.GetCustomAttribute<CommentsAttribute>()
                        .WhenNotNull(x => x.Comments).OtherwiseDefault() ??
                    description.WhenNotNull(x => x.Comments).OtherwiseDefault() ??
                    xmlComments?.Summary ?? xmlComments?.Remarks
            };
        }
    }
}