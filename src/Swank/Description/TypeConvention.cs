using System;
using System.Reflection;
using System.Xml.Serialization;
using Swank.Extensions;
using System.Runtime.Serialization;
using Swank.Configuration;

namespace Swank.Description
{
    public class TypeConvention : IDescriptionConvention<Type, TypeDescription>
    {
        private readonly XmlComments _xmlComments;
        private readonly Configuration.Configuration _configuration;

        public TypeConvention(XmlComments xmlComments,
            Configuration.Configuration configuration)
        {
            _xmlComments = xmlComments;
            _configuration = configuration;
        }

        public virtual TypeDescription GetDescription(Type type)
        {
            var description = type.GetCustomAttribute<DescriptionAttribute>();
            var arrayDescription = type.GetCustomAttribute<ArrayDescriptionAttribute>();
            var dictionaryDescription = type.GetCustomAttribute<DictionaryDescriptionAttribute>();
            var xmlComments = _xmlComments.GetType(type);

            return new TypeDescription
            {
                Id = type.FullName.Hash(),
                Name = type.GetCustomAttribute<XmlRootAttribute>()?.ElementName ??
                    type.GetCustomAttribute<XmlTypeAttribute>()?.TypeName ??
                    type.GetCustomAttribute<DataContractAttribute>()?.Name ??
                    type.GetCustomAttribute<CollectionDataContractAttribute>()?.Name ??
                    type.GetCustomAttribute<NameAttribute>()?.Name ?? description?.Name ??
                    type.GetXmlName(_configuration.EnumFormat == EnumFormat.AsString),
                Comments = type.GetCustomAttribute<CommentsAttribute>()?.Comments ??
                    description?.Comments ??
                    xmlComments?.Summary ?? xmlComments?.Remarks,
                Nullable = type.IsNullable(),
                ArrayItem = new Description
                {
                    Name = arrayDescription?.ItemName,
                    Comments = arrayDescription?.ItemComments
                },
                DictionaryEntry = new DictionaryDescription
                {
                    KeyName = dictionaryDescription?.KeyName,
                    KeyComments = dictionaryDescription?.KeyComments,
                    ValueComments = dictionaryDescription?.ValueComments
                }
            };
        }
    }
}