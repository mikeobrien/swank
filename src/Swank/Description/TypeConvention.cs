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

            return new TypeDescription {
                Name = type.GetCustomAttribute<XmlRootAttribute>()
                        .WhenNotNull(x => x.ElementName).OtherwiseDefault() ??
                    type.GetCustomAttribute<XmlTypeAttribute>()
                        .WhenNotNull(x => x.TypeName).OtherwiseDefault() ??
                    type.GetCustomAttribute<DataContractAttribute>()
                        .WhenNotNull(x => x.Name).OtherwiseDefault() ??
                    type.GetCustomAttribute<CollectionDataContractAttribute>()
                        .WhenNotNull(x => x.Name).OtherwiseDefault() ??
                    description.WhenNotNull(x => x.Name).OtherwiseDefault() ??
                    type.GetXmlName(_configuration.EnumFormat == EnumFormat.AsString),
                Comments = type.GetCustomAttribute<CommentsAttribute>()
                        .WhenNotNull(x => x.Comments).OtherwiseDefault() ??
                    description.WhenNotNull(x => x.Comments).OtherwiseDefault() ??
                    xmlComments?.Summary ?? xmlComments?.Remarks,
                Namespace = _configuration.TypeNamespace(type),
                ArrayItem = new Description
                {
                    Name = arrayDescription.WhenNotNull(x => x.ItemName).OtherwiseDefault(),
                    Comments = arrayDescription.WhenNotNull(x => x.ItemComments).OtherwiseDefault()
                },
                DictionaryEntry = new DictionaryDescription
                {
                    KeyName = dictionaryDescription.WhenNotNull(x => x.KeyName).OtherwiseDefault(),
                    KeyComments = dictionaryDescription.WhenNotNull(x => x.KeyComments).OtherwiseDefault(),
                    ValueComments = dictionaryDescription.WhenNotNull(x => x.ValueComments).OtherwiseDefault()
                }
            };
        }
    }
}