using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http.Description;
using Swank.Description;
using Swank.Description.CodeExamples;
using Swank.Extensions;
using Swank.Specification;
using Swank.Web.Assets;
using Swank.Web.Templates;
using Module = Swank.Specification.Module;

namespace Swank.Configuration
{
    public enum OrphanedEndpoints { Exclude, Fail, UseDefault }
    public enum EnumFormat { AsNumber, AsString }

    public class Configuration
    {
        public const string AppStylesheet = "swank.css";
        public const string DefaultCodeExampleTheme = "github-gist.css";
        public const string DefaultDefaultModuleName = "Resources"; // yo dawg I heard you like defaults...

        public static readonly Func<ApiDescription, List<string>> DefaultActionNamespace = x => 
            x.GetActionAttribute<ActionNamespaceAttribute>()?.Namespace ??
            x.GetActionAttribute<ActionNameAttribute>()?.Namespace ??
            x.Route.GetNamespaceFromRoute().DistinctSiblings().ToList();
        public static readonly Func<ApiDescription, string> DefaultActionName = x =>
            x.GetActionAttribute<ActionNameAttribute>()?.Name ?? x.GetMethodInfo().Name;

        public class OverviewLink
        {
            public string Name { get; set; }
            public string FragmentId { get; set; }
        }

        public class Service<T>
        {
            public Type Type { get; set; }
            public object Config { get; set; }
        }

        public Configuration() : this(Assembly.GetCallingAssembly()) { }

        public Configuration(Assembly assembly)
        {
            AppUrl = "api";
            SpecificationUrl = "api/spec";
            AppliesToAssemblies = new List<Assembly> { assembly };
            AppTemplate = RazorTemplate.FromResourceInThisAssembly<List<Module>>("App.cshtml", this);
            Overview = new LazyAsset(() => MarkdownAsset
                .FromVirtualPath("~/Overview.md"));
            XmlComments = new List<IAsset>();
            Copyright = $"Copyright &copy; {DateTime.Now.Year}";
            Templates = new List<WebTemplate>();
            TemplateNamespaceIncludesModule = false;
            Assets = new List<WebAsset>(WebAsset.FromResources(
                Assembly.GetExecutingAssembly().AsList(),
                "Swank.Web.Content", null, null, ".js", ".css", ".eot", 
                    ".svg", ".ttf", ".woff", ".woff2", ".png", ".jpg"));

            Stylesheets = new List<LazyUrl>
            {
                Assets.FindByFilename("bootstrap.css").ToLazyUrl(this),
                Assets.FindByFilename("ie10-viewport-bug-workaround.css").ToLazyUrl(this),
                Assets.FindByFilename(DefaultCodeExampleTheme).ToLazyUrl(this),
                Assets.FindByFilename("emoji.css").ToLazyUrl(this),
                Assets.FindByFilename("github-markdown.css").ToLazyUrl(this),
                Assets.FindByFilename(AppStylesheet).ToLazyUrl(this)
            };

            Scripts = new List<LazyUrl>
            {
                Assets.FindByFilename("jquery.js").ToLazyUrl(this),
                Assets.FindByFilename("bootstrap.js").ToLazyUrl(this),
                Assets.FindByFilename("respond.js").ToLazyUrl(this),
                Assets.FindByFilename("ie10-viewport-bug-workaround.js").ToLazyUrl(this),
                Assets.FindByFilename("sticky-headers.js").ToLazyUrl(this),
                Assets.FindByFilename("column-tab-panes.js").ToLazyUrl(this),
                Assets.FindByFilename("highlight.js").ToLazyUrl(this),
                Assets.FindByFilename("clipboard.js").ToLazyUrl(this),
                Assets.FindByFilename("cookie.js").ToLazyUrl(this),
                Assets.FindByFilename("handlebars.js").ToLazyUrl(this),
                Assets.FindByFilename("app.js").ToLazyUrl(this)
            };

            IEPolyfills = new List<LazyUrl>
            {
                Assets.FindByFilename("html5shiv.js").ToLazyUrl(this),
                Assets.FindByFilename("respond.js").ToLazyUrl(this)
            };

            DisplayJsonData = true;
            DisplayXmlData = true;
            Filter = x => true;
            DefaultModuleName = DefaultDefaultModuleName;
            OrphanedModuleEndpoint = OrphanedEndpoints.UseDefault;
            DefaultResourceIdentifier = x => x.Route.GetRouteResourceIdentifier();
            OrphanedResourceEndpoint = OrphanedEndpoints.UseDefault;
            DefaultDictionaryKeyName = "key";
            EnumFormat = EnumFormat.AsString;
            OverviewLinks = new List<OverviewLink>();
            
            ActionNamespace = DefaultActionNamespace;
            ActionName = DefaultActionName;

            CodeExamples = new List<CodeExample>
            {
                new CodeExample("Curl", "bash", null, RazorTemplate
                    .FromResourceInThisAssembly<CodeExampleModel>("curl.cshtml", this)),
                new CodeExample("Node.js", "javascript", null, RazorTemplate
                    .FromResourceInThisAssembly<CodeExampleModel>("node.cshtml", this))
            };

            SampleDateTimeFormat = "g";
            SampleIntegerFormat = "0";
            SampleRealFormat = "0.00";
            SampleTimeSpanFormat = "g";
            SampleGuidFormat = "D";

            SampleStringValue = "";
            SampleBoolValue = false;
            SampleDateTimeValue = DateTime.Now;
            SampleIntegerValue = 0;
            SampleRealValue = 0;
            SampleTimeSpanValue = TimeSpan.FromHours(0);
            SampleGuidValue = Guid.Empty;

            ModuleConvention = new Service<IDescriptionConvention<ApiDescription, 
                ModuleDescription>> { Type = typeof(ModuleConvention) };
            ResourceConvention = new Service<IDescriptionConvention<ApiDescription, 
                ResourceDescription>> { Type = typeof(ResourceConvention) };
            EndpointConvention = new Service<IDescriptionConvention<ApiDescription, 
                EndpointDescription>> { Type = typeof(EndpointConvention) };
            MemberConvention = new Service<IDescriptionConvention<PropertyInfo, 
                MemberDescription>> { Type = typeof(MemberConvention) };
            EnumConvention = new Service<IDescriptionConvention<Type, 
                EnumDescription>> { Type = typeof(EnumConvention) };
            EnumOptionConvention = new Service<IDescriptionConvention<FieldInfo, 
                OptionDescription>> { Type = typeof(OptionConvention) };
            StatusCodeConvention = new Service<IDescriptionConvention<ApiDescription, 
                List<StatusCodeDescription>>> { Type = typeof(StatusCodeConvention) };
            HeaderConvention = new Service<IDescriptionConvention<ApiDescription, 
                List<HeaderDescription>>> { Type = typeof(HeaderConvention) };
            TypeConvention = new Service<IDescriptionConvention<Type, 
                TypeDescription>> { Type = typeof(TypeConvention) };

            ModuleOverrides = new List<Action<ModuleOverrideContext>>();
            ResourceOverrides = new List<Action<ResourceOverrideContext>>();
            EndpointOverrides = new List<Action<EndpointOverrideContext>>();
            UrlParameterOverrides = new List<Action<UrlParameterOverrideContext>>();
            QuerystringOverrides = new List<Action<QuerystringOverrideContext>>();
            StatusCodeOverrides = new List<Action<StatusCodeOverrideContext>>();
            RequestHeaderOverrides = new List<Action<HeaderOverrideContext>>();
            ResponseHeaderOverrides = new List<Action<HeaderOverrideContext>>();
            RequestOverrides = new List<Action<MessageOverrideContext>>();
            ResponseOverrides = new List<Action<MessageOverrideContext>>();
            TypeOverrides = new List<Action<TypeOverrideContext>>();
            MemberOverrides = new List<Action<MemberOverrideContext>>();
            OptionOverrides = new List<Action<OptionOverrideContext>>();
        }

        public bool DebugMode { get; set; }
        public bool IgnoreFolders { get; set; }
        public string AppUrl { get; set; }
        public string SpecificationUrl { get; set; }
        public RazorTemplate AppTemplate { get; set; }
        public string FavIconUrl { get; set; }
        public string ApiUrl { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string LogoUrl { get; set; }
        public List<IAsset> XmlComments { get; set; }
        public IAsset Overview { get; set; }
        public List<OverviewLink> OverviewLinks { get; set; }
        public string Copyright { get; set; }
        public List<WebAsset> Assets { get; }
        public List<WebTemplate> Templates { get; }
        public bool TemplateNamespaceIncludesModule { get; set; }
        public List<LazyUrl> Scripts { get; }
        public List<LazyUrl> Stylesheets { get; }
        public List<LazyUrl> IEPolyfills { get; }
        public bool DisplayJsonData { get; set; }
        public bool DisplayXmlData { get; set; }
        public List<Assembly> AppliesToAssemblies { get; }
        public Func<ApiDescription, bool> Filter { get; set; }
        public Func<ApiDescription, List<string>> ActionNamespace { get; set; }
        public Func<ApiDescription, string> ActionName { get; set; }

        public OrphanedEndpoints OrphanedModuleEndpoint { get; set; }
        public OrphanedEndpoints OrphanedResourceEndpoint { get; set; }
        public string DefaultModuleName { get; set; }
        public Func<ApiDescription, ResourceDescription> DefaultResourceFactory { get; set; }
        public Func<ApiDescription, string> DefaultResourceIdentifier { get; set; }

        public string DefaultDictionaryKeyName { get; set; }

        public EnumFormat EnumFormat { get; set; }

        public List<CodeExample> CodeExamples { get; }

        public string SampleDateTimeFormat { get; set; }
        public string SampleIntegerFormat { get; set; }
        public string SampleRealFormat { get; set; }
        public string SampleTimeSpanFormat { get; set; }
        public string SampleGuidFormat { get; set; }

        public string SampleStringValue { get; set; }
        public bool SampleBoolValue { get; set; }
        public DateTime SampleDateTimeValue { get; set; }
        public int SampleIntegerValue { get; set; }
        public decimal SampleRealValue { get; set; }
        public TimeSpan SampleTimeSpanValue { get; set; }
        public Guid SampleGuidValue { get; set; }

        public Service<IDescriptionConvention<ApiDescription, 
            ModuleDescription>> ModuleConvention { get; }
        public Service<IDescriptionConvention<ApiDescription, 
            ResourceDescription>> ResourceConvention { get; }
        public Service<IDescriptionConvention<ApiDescription, 
            EndpointDescription>> EndpointConvention { get; }
        public Service<IDescriptionConvention<ApiDescription, 
            List<StatusCodeDescription>>> StatusCodeConvention { get; }
        public Service<IDescriptionConvention<ApiDescription, 
            List<HeaderDescription>>> HeaderConvention { get; }
        public Service<IDescriptionConvention<Type, TypeDescription>> 
            TypeConvention { get; }
        public Service<IDescriptionConvention<PropertyInfo, 
            MemberDescription>> MemberConvention { get; }
        public Service<IDescriptionConvention<Type, EnumDescription>> 
            EnumConvention { get; }
        public Service<IDescriptionConvention<FieldInfo, 
            OptionDescription>> EnumOptionConvention { get; }

        public List<Action<ModuleOverrideContext>> ModuleOverrides { get; }
        public List<Action<ResourceOverrideContext>> ResourceOverrides { get; }
        public List<Action<EndpointOverrideContext>> EndpointOverrides { get; }
        public List<Action<UrlParameterOverrideContext>> UrlParameterOverrides { get; }
        public List<Action<QuerystringOverrideContext>> QuerystringOverrides { get; }
        public List<Action<StatusCodeOverrideContext>> StatusCodeOverrides { get; }
        public List<Action<HeaderOverrideContext>> RequestHeaderOverrides { get; }
        public List<Action<HeaderOverrideContext>> ResponseHeaderOverrides { get; }
        public List<Action<MessageOverrideContext>> RequestOverrides { get; }
        public List<Action<MessageOverrideContext>> ResponseOverrides { get; }
        public List<Action<TypeOverrideContext>> TypeOverrides { get; }
        public List<Action<MemberOverrideContext>> MemberOverrides { get; }
        public List<Action<OptionOverrideContext>> OptionOverrides { get; }
    }
}
