using System;
using System.Xml.Serialization;
using Swank.Configuration;
using Swank.Extensions;

namespace Swank.Description
{
    public class ParameterConvention : IDescriptionConvention<IApiParameterDescription, ParameterDescription>
    {
        private readonly Configuration.Configuration _configuration;
        private readonly XmlComments _xmlComments;

        public ParameterConvention(
            Configuration.Configuration configuration,
            XmlComments xmlComments)
        {
            _configuration = configuration;
            _xmlComments = xmlComments;
        }

        public virtual ParameterDescription GetDescription(IApiParameterDescription parameter)
        {
            var description = parameter.GetAttribute<DescriptionAttribute>();
            var type = parameter.Type;
            var xmlComments = _xmlComments.GetMethod(parameter.ActionMethod);

            return new ParameterDescription
            {
                Name = parameter.GetAttribute<NameAttribute>()?.Name ?? 
                    description?.Name ?? parameter.Name,
                Type = (type.GetListElementType() ?? type).GetXmlName(
                    _configuration.EnumFormat == EnumFormat.AsString),
                Comments = description?.Comments ??
                    parameter.GetAttribute<CommentsAttribute>()?.Comments ?? 
                    parameter.Documentation ??
                    xmlComments?.Parameters.TryGetValue(parameter.Name),
                DefaultValue = (parameter.GetAttribute<DefaultValueAttribute>()?.Value ??
                        GetDefaultValue(parameter))?
                        .ToSampleValueString(_configuration),
                SampleValue = parameter.GetAttribute<SampleValueAttribute>()?
                        .Value.ToSampleValueString(_configuration) ??
                    parameter.Type.GetSampleValue(_configuration),
                Optional = IsOptional(parameter),
                Hidden = parameter.HasAttribute<HideAttribute>() ||
                    parameter.HasAttribute<XmlIgnoreAttribute>(),
                MultipleAllowed = parameter.HasAttribute<MultipleAttribute>() || type.IsListType()
            };
        }

        private static string GetDefaultValue(IApiParameterDescription parameter)
        {
            var value = parameter.DefaultValue;
            if (value == null) return null;
            var type = parameter.Type;
            // Web API sets the default values of nullable enums 
            // to a numeric value instead of the enum value as it 
            // does with a non nullable. So this normalizes enum 
            // default values so they are the enum value for both 
            // nullable and non nullable types.
            var underlyingType = type.GetNullableUnderlyingType();
            if (underlyingType.IsEnum && type.IsNullable() &&
                underlyingType != value.GetType())
            {
                return Enum.ToObject(underlyingType, value).ToString();
            }
            return value.ToString();
        }

        public bool IsOptional(IApiParameterDescription parameter)
        {
            return !parameter.HasAttribute<RequiredAttribute>() && 
                parameter.IsOptional;
        }
    }
}