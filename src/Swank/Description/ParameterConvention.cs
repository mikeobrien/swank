﻿using System;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using System.Xml.Serialization;
using Swank.Configuration;
using Swank.Extensions;

namespace Swank.Description
{
    public class ParameterConvention : IDescriptionConvention<ApiParameterDescription, ParameterDescription>
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

        public virtual ParameterDescription GetDescription(ApiParameterDescription parameter)
        {
            var description = parameter.GetAttribute<DescriptionAttribute>();
            var type = parameter.ParameterDescriptor.ParameterType;
            var xmlComments = _xmlComments.GetMethod(parameter.GetMethodInfo());

            return new ParameterDescription
            {
                Name = description.WhenNotNull(x => x.Name).Otherwise(parameter.Name),
                Type = (type.GetListElementType() ?? type).GetXmlName(
                    _configuration.EnumFormat == EnumFormat.AsString),
                Comments = description.WhenNotNull(x => x.Comments).OtherwiseDefault() ??
                    parameter.GetAttribute<CommentsAttribute>().WhenNotNull(x => x.Comments)
                        .OtherwiseDefault() ?? parameter.Documentation ??
                        xmlComments?.Parameters.TryGetValue(parameter.Name),
                DefaultValue = (parameter.GetAttribute<DefaultValueAttribute>()?.Value ??
                        GetDefaultValue(parameter.ParameterDescriptor))
                    .WhenNotNull(x => x.ToSampleValueString(_configuration))
                    .OtherwiseDefault(),
                SampleValue = parameter.GetAttribute<SampleValueAttribute>()
                    .WhenNotNull(x => x.Value.ToSampleValueString(_configuration))
                    .Otherwise(parameter.ParameterDescriptor.ParameterType
                    .GetSampleValue(_configuration)),
                Optional = IsOptional(parameter),
                Hidden = parameter.HasAttribute<HideAttribute>() ||
                    parameter.HasAttribute<XmlIgnoreAttribute>(),
                MultipleAllowed = parameter.HasAttribute<MultipleAttribute>() || type.IsListType()
            };
        }

        private static string GetDefaultValue(HttpParameterDescriptor parameter)
        {
            var value = parameter.DefaultValue;
            if (value == null) return null;
            var type = parameter.ParameterType;
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

        public bool IsOptional(ApiParameterDescription parameter)
        {
            return !parameter.HasAttribute<RequiredAttribute>() && 
                parameter.ParameterDescriptor.IsOptional;
        }
    }
}