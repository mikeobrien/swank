using System;
using System.Linq;
using System.Reflection;
using Swank.Configuration;
using Swank.Description;
using Swank.Extensions;

namespace Swank.Specification
{
    public class OptionBuilderService
    {
        private readonly Configuration.Configuration _configuration;
        private readonly IDescriptionConvention<Type, EnumDescription> _enumConvention;
        private readonly IDescriptionConvention<FieldInfo, OptionDescription> _optionConvention;

        public OptionBuilderService(
            Configuration.Configuration configuration,
            IDescriptionConvention<Type, EnumDescription> enumConvention,
            IDescriptionConvention<FieldInfo, OptionDescription> optionConvention)
        {
            _configuration = configuration;
            _enumConvention = enumConvention;
            _optionConvention = optionConvention;
        }

        public Enumeration BuildOptions(Type type, IApiDescription endpoint, bool request)
        {
            type = type.GetNullableUnderlyingType();
            if (!type.IsEnum) return null;
            var description = _enumConvention.GetDescription(type);
            return new Enumeration
            {
                Name = description?.Name ?? type.Name,
                Comments = description?.Comments,
                Options = type.GetEnumOptions()
                    .Select(x => new
                    {
                        Option = x,
                        Description = _optionConvention.GetDescription(x)
                    })
                    .Where(x => !x.Description.Hidden)
                    .Select(x =>
                        _configuration.OptionOverrides.Apply(new OptionOverrideContext
                        {
                            IsRequest = request,
                            Description = x.Description,
                            ApiDescription = endpoint,
                            Field = x.Option,
                            Option = new Option
                            {
                                Name = x.Description?.Name,
                                Comments = x.Description?.Comments,
                                Value = _configuration.EnumFormat == EnumFormat.AsString ? 
                                    x.Option.Name : x.Option.GetRawConstantValue().ToString()
                            }
                        }).Option)
                    .OrderBy(x => x.Name).ToList()
            };
        }
    }
}