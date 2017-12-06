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
            return new OptionDescription
            {
                Name = field.GetCustomAttribute<NameAttribute>()?.Name ?? 
                    description?.Name ?? field.Name,
                Comments = description?.Comments ??
                    field.GetCustomAttribute<CommentsAttribute>()?.Comments ??
                    xmlComments?.Summary ?? xmlComments?.Remarks,
                Hidden = field.HasAttribute<HideAttribute>()
            };
        }
    }
}