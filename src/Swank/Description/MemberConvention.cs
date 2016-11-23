using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Swank.Extensions;

namespace Swank.Description
{
    public class MemberConvention : IDescriptionConvention<PropertyInfo, MemberDescription>
    {
        private readonly Configuration.Configuration _configuration;
        private readonly XmlComments _xmlComments;

        public MemberConvention(Configuration.Configuration configuration,
            XmlComments xmlComments)
        {
            _configuration = configuration;
            _xmlComments = xmlComments;
        }

        public virtual MemberDescription GetDescription(PropertyInfo property)
        {
            var arrayDescription = property.GetCustomAttribute<ArrayDescriptionAttribute>();
            var dictionaryDescription = property.GetCustomAttribute<DictionaryDescriptionAttribute>();
            var description = property.GetCustomAttribute<DescriptionAttribute>();
            var obsolete = property.GetCustomAttribute<ObsoleteAttribute>();
            var xmlComments = _xmlComments.GetProperty(property);

            return new MemberDescription {
                Name = description.WhenNotNull(x => x.Name).OtherwiseDefault() ??
                    property.GetCustomAttribute<XmlElementAttribute>()
                        .WhenNotNull(x => x.ElementName).OtherwiseDefault() ??
                    property.GetCustomAttribute<DataMemberAttribute>()
                        .WhenNotNull(x => x.Name).OtherwiseDefault() ??
                    property.Name,
                Comments = description.WhenNotNull(x => x.Comments).OtherwiseDefault() ??
                    property.GetCustomAttribute<CommentsAttribute>()
                        .WhenNotNull(x => x.Comments).OtherwiseDefault() ??
                    xmlComments?.Summary ?? xmlComments?.Remarks,
                DefaultValue = property.GetCustomAttribute<DefaultValueAttribute>()
                    .WhenNotNull(x => x.Value.ToSampleValueString(_configuration)).OtherwiseDefault(),
                SampleValue = property.GetCustomAttribute<SampleValueAttribute>()
                    .WhenNotNull(x => x.Value.ToSampleValueString(_configuration))
                    .Otherwise(property.PropertyType.GetSampleValue(_configuration)),
                Optional = property.GetOptionalScope() ?? OptionalScope.All,
                Hidden = property.PropertyType.HasAttribute<HideAttribute>() ||
                    property.HasAttribute<HideAttribute>() ||
                    property.HasAttribute<XmlIgnoreAttribute>(),
                Encoding = GetEncoding(property),
                MaxLength = property.GetCustomAttribute<MaxLengthAttribute>()?.MaxLength,
                Deprecated = obsolete != null,
                DeprecationMessage = obsolete.WhenNotNull(x => x.Message).OtherwiseDefault(),
                ArrayItem = new Description
                {
                    Name = property.GetCustomAttribute<XmlArrayItemAttribute>()
                            .WhenNotNull(x => x.ElementName).OtherwiseDefault() ??
                        arrayDescription.WhenNotNull(x => x.ItemName).OtherwiseDefault(),
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

        private Encoding? GetEncoding(PropertyInfo property)
        {
            if (property.HasAttribute<AsciiEncodingAttribute>()) return Encoding.ASCII;
            if (property.HasAttribute<UnicodeEncodingAttribute>()) return Encoding.Unicode;
            if (property.HasAttribute<ISO8601EncodingAttribute>()) return Encoding.ISO8601;
            return null;
        }
    }

    public static class MemberConventionExtensions
    {
        public static OptionalScope? GetOptionalScope(this PropertyInfo property)
        {
            if (property.HasAttribute<RequiredAttribute>()) return OptionalScope.None;
            if (property.HasAttribute<OptionalForPostAttribute>()) return OptionalScope.Post;
            if (property.HasAttribute<OptionalForPutAttribute>()) return OptionalScope.Put;
            if (property.HasAttribute<RequiredForPutAttribute>()) return OptionalScope.AllButPut;
            if (property.HasAttribute<RequiredForPostAttribute>()) return OptionalScope.AllButPost;
            return OptionalScope.All;
        }
    }
}