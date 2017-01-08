using System.Reflection;
using Swank.Extensions;

namespace Swank.Description
{
    public class OptionConvention : IDescriptionConvention<FieldInfo, OptionDescription>
    {
        private readonly XmlComments _xmlComments;

        public OptionConvention(XmlComments xmlComments)
        {
            _xmlComments = xmlComments;
        }

        public virtual OptionDescription GetDescription(FieldInfo field)
        {
            var description = field.GetCustomAttribute<DescriptionAttribute>();
            var xmlComments = _xmlComments.GetField(field);
            return new OptionDescription {
                Name = description.WhenNotNull(x => x.Name).Otherwise(field.Name),
                Comments = description.WhenNotNull(x => x.Comments)
                    .Otherwise(field.GetCustomAttribute<CommentsAttribute>()
                    .WhenNotNull(x => x.Comments).OtherwiseDefault() ??
                    xmlComments?.Summary ?? xmlComments?.Remarks),
                Hidden = field.HasAttribute<HideAttribute>()
            };
        }
    }
}