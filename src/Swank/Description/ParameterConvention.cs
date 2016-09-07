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
                Type = (type.GetListElementType() ?? type).GetXmlName(
                    _configuration.EnumFormat == EnumFormat.AsString),
                Name = description.WhenNotNull(x => x.Name).Otherwise(parameter.Name),
                Comments = description.WhenNotNull(x => x.Comments).OtherwiseDefault() ??
                    parameter.GetAttribute<CommentsAttribute>().WhenNotNull(x => x.Comments)
                        .OtherwiseDefault() ?? parameter.Documentation ??
                        xmlComments?.Parameters.TryGetValue(parameter.Name),
                DefaultValue = (parameter.GetAttribute<DefaultValueAttribute>()?.Value ?? 
                        parameter.ParameterDescriptor.DefaultValue)
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

        private bool IsOptional(ApiParameterDescription parameter)
        {
            if (parameter.HasAttribute<RequiredAttribute>()) return false;
            if (parameter.HasAttribute<OptionalAttribute>()) return true;
            return parameter.ParameterDescriptor.IsOptional;
        }
    }
}