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
                Name = property.GetCustomAttribute<NameAttribute>()?.Name ?? description?.Name ??
                    property.GetCustomAttribute<XmlElementAttribute>()?.ElementName ??
                    property.GetCustomAttribute<DataMemberAttribute>()?.Name ??
                    property.Name,
                Comments = description?.Comments ??
                    property.GetCustomAttribute<CommentsAttribute>()?.Comments ??
                    xmlComments?.Summary ?? xmlComments?.Remarks,
                DefaultValue = property.GetCustomAttribute<DefaultValueAttribute>()?
                    .Value.ToSampleValueString(_configuration),
                SampleValue = property.GetCustomAttribute<SampleValueAttribute>()?
                    .Value.ToSampleValueString(_configuration) ??
                    property.PropertyType.GetSampleValue(_configuration),
                Optional = property.GetOptionalScope(),
                Hidden = property.PropertyType.HasAttribute<HideAttribute>() ||
                    property.HasAttribute<HideAttribute>() ||
                    property.HasAttribute<XmlIgnoreAttribute>(),
                Encoding = GetEncoding(property),
                MaxLength = property.GetCustomAttribute<MaxLengthAttribute>()?.MaxLength,
                Deprecated = obsolete != null,
                DeprecationMessage = obsolete?.Message,
                ArrayItem = new Description
                {
                    Name = property.GetCustomAttribute<XmlArrayItemAttribute>()?.ElementName ??
                        arrayDescription?.ItemName,
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
            if (property.HasAttribute<OptionalAttribute>()) return OptionalScope.All;
            if (property.HasAttribute<OptionalForPostAttribute>()) return OptionalScope.Post;
            if (property.HasAttribute<OptionalForPutAttribute>()) return OptionalScope.Put;
            if (property.HasAttribute<RequiredForPutAttribute>()) return OptionalScope.AllButPut;
            if (property.HasAttribute<RequiredForPostAttribute>()) return OptionalScope.AllButPost;
            return property.PropertyType.IsNullable() ? OptionalScope.All : (OptionalScope?)null;
        }
    }
}