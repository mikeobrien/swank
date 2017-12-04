using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using Swank.Description;
using Swank.Description.CodeExamples;
using Swank.Description.WebApi;
using Swank.Extensions;
using Swank.Specification;
using Swank.Web.Assets;
using Swank.Web.Handlers.App;
using Swank.Web.Templates;
using CodeExampleModel = Swank.Description.CodeExamples.CodeExampleModel;
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

        public static readonly Func<IApiDescription, List<string>> DefaultActionNamespace = x => 
            x.GetActionAttribute<ActionNamespaceAttribute>()?.Namespace ??
            x.GetActionAttribute<ActionNameAttribute>()?.Namespace ??
            x.RouteTemplate.GetNamespaceFromRoute().DistinctSiblings().ToList();
        public static readonly Func<IApiDescription, string> DefaultActionName = x =>
            x.GetActionAttribute<ActionNameAttribute>()?.Name ?? x.ActionMethod.Name;

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
            AppliesToAssemblies = new List<Assembly> { assembly };
            AppTemplate = RazorTemplate.FromResourceInThisAssembly<List<Module>>("App.cshtml", this);

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
                Assets.FindByFilename("base64.js").ToLazyUrl(this),
                Assets.FindByFilename("vkbeautify.js").ToLazyUrl(this),
                Assets.FindByFilename("swank.js").ToLazyUrl(this)
            };

            IEPolyfills = new List<LazyUrl>
            {
                Assets.FindByFilename("html5shiv.js").ToLazyUrl(this),
                Assets.FindByFilename("respond.js").ToLazyUrl(this)
            };

            CodeExamples = new List<CodeExample>
            {
                new CodeExample("Curl", "bash", null, RazorTemplate
                    .FromResourceInThisAssembly<CodeExampleModel>("curl.cshtml", this)),
                new CodeExample("Node.js", "javascript", null, RazorTemplate
                    .FromResourceInThisAssembly<CodeExampleModel>("node.cshtml", this))
            };
        }
        
        public Type ApiExplorer { get; set; } = typeof(WebApiExplorer);
        public bool DebugMode { get; set; }
        public bool IgnoreFolders { get; set; }
        public string[] IgnoreFolderUrls { get; set; }
        public string AppUrl { get; set; } = "api";

        private string _specificationUrl;

        public string SpecificationUrl
        {
            get => _specificationUrl.IsNullOrEmpty()
                ? $"{AppUrl.TrimEnd('/')}/spec"
                : _specificationUrl;
            set => _specificationUrl = value;
        }

        public RazorTemplate AppTemplate { get; set; }
        public string FavIconUrl { get; set; }
        public string ApiUrl { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string LogoUrl { get; set; }
        public bool CollapseModules { get; set; }
        public Func<HttpRequestMessage, string> CacheKey { get; set; } = x => x.RequestUri.PathAndQuery;
        public Action<HttpRequestMessage, AppModel> AppPreRender { get; set; }
        public Action<HttpRequestMessage, ResourceModel> ResourcePreRender { get; set; }
        public Action<HttpRequestMessage, Module> SpecPreRender { get; set; }
        public List<IAsset> XmlComments { get; set; } = new List<IAsset>();
        public IAsset Overview { get; set; } = new LazyAsset(() => MarkdownAsset
            .FromVirtualPath("~/Overview.md"));
        public List<OverviewLink> OverviewLinks { get; set; } = new List<OverviewLink>();
        public string Copyright { get; set; } = $"Copyright &copy; {DateTime.Now.Year}";
        public List<AuthenticationScheme> AuthenticationSchemes { get; set; } = new List<AuthenticationScheme>();
        public List<WebAsset> Assets { get; } = new List<WebAsset>(WebAsset.FromResources(
            Assembly.GetExecutingAssembly().AsList(),
            "Swank.Web.Content", null, null, ".js", ".css", ".eot", 
            ".svg", ".ttf", ".woff", ".woff2", ".png", ".jpg", ".gif"));
        public List<WebTemplate> Templates { get; } = new List<WebTemplate>();
        public bool TemplateNamespaceIncludesModule { get; set; }
        public List<LazyUrl> Scripts { get; }
        public List<LazyUrl> Stylesheets { get; }
        public List<LazyUrl> IEPolyfills { get; }
        public bool DisplayJsonData { get; set; } = true;
        public bool DisplayXmlData { get; set; } = true;
        public List<Assembly> AppliesToAssemblies { get; }
        public Func<IApiDescription, bool> Filter { get; set; } = x => true;
        public Func<IApiDescription, List<string>> ActionNamespace { get; set; } = DefaultActionNamespace;
        public Func<IApiDescription, string> ActionName { get; set; } = DefaultActionName;

        public bool HideStatusCodeSection { get; set; }
        public bool HideCodeExamplesSection { get; set; }
        public bool HideTestDriveSection { get; set; }
        public bool HideQueryStringSection { get; set; }
        public bool HideUrlParametersSection { get; set; }

        public bool HideRequestSection { get; set; }
        public bool HideRequestHeadersSection { get; set; }
        public bool HideRequestBodySection { get; set; }

        public bool HideResponseSection { get; set; }
        public bool HideResponseHeadersSection { get; set; }
        public bool HideResponseBodySection { get; set; }

        public OrphanedEndpoints OrphanedModuleEndpoint { get; set; } = OrphanedEndpoints.UseDefault;
        public OrphanedEndpoints OrphanedResourceEndpoint { get; set; } = OrphanedEndpoints.UseDefault;
        public string DefaultModuleName { get; set; } = DefaultDefaultModuleName;
        public Func<IApiDescription, string, ResourceDescription> DefaultResourceFactory { get; set; }
        public Func<IApiDescription, string> DefaultResourceIdentifier { get; set; } = x => x.RouteTemplate.GetRouteResourceIdentifier();

        public string DefaultDictionaryKeyName { get; set; } = "key";

        public EnumFormat EnumFormat { get; set; } = EnumFormat.AsString;
        public OptionalScope DefaultOptionalScope { get; set; } = OptionalScope.None;

        public List<CodeExample> CodeExamples { get; }

        public string SampleDateTimeFormat { get; set; } = "g";
        public string SampleIntegerFormat { get; set; } = "0";
        public string SampleRealFormat { get; set; } = "0.00";
        public string SampleTimeSpanFormat { get; set; } = "g";
        public string SampleGuidFormat { get; set; } = "D";

        public string SampleStringValue { get; set; } = "";
        public bool SampleBoolValue { get; set; } = false;
        public DateTime SampleDateTimeValue { get; set; } = DateTime.Now;
        public int SampleIntegerValue { get; set; } = 0;
        public decimal SampleRealValue { get; set; } = 0;
        public TimeSpan SampleTimeSpanValue { get; set; } = TimeSpan.FromHours(0);
        public Guid SampleGuidValue { get; set; } = Guid.Empty;
        public Uri SampleUriValue { get; set; } = new Uri("http://www.google.com");

        public Service<IDescriptionConvention<IApiDescription, 
            ModuleDescription>> ModuleConvention { get; } = new Service<IDescriptionConvention
            <IApiDescription, ModuleDescription>> { Type = typeof(ModuleConvention) };
        public Service<IDescriptionConvention<IApiDescription, 
            ResourceDescription>> ResourceConvention { get; } = new Service<IDescriptionConvention
            <IApiDescription, ResourceDescription>> { Type = typeof(ResourceConvention) };
        public Service<IDescriptionConvention<IApiDescription, 
            EndpointDescription>> EndpointConvention { get; } = new Service<IDescriptionConvention
            <IApiDescription, EndpointDescription>> { Type = typeof(EndpointConvention) };
        public Service<IDescriptionConvention<IApiDescription, 
            List<StatusCodeDescription>>> StatusCodeConvention { get; } = new Service<IDescriptionConvention
            <IApiDescription, List<StatusCodeDescription>>> { Type = typeof(StatusCodeConvention) };
        public Service<IDescriptionConvention<IApiDescription, 
            List<HeaderDescription>>> HeaderConvention { get; } = new Service<IDescriptionConvention
            <IApiDescription, List<HeaderDescription>>> { Type = typeof(HeaderConvention) };
        public Service<IDescriptionConvention<Type, TypeDescription>> 
            TypeConvention { get; } = new Service<IDescriptionConvention<Type, 
            TypeDescription>> { Type = typeof(TypeConvention) };
        public Service<IDescriptionConvention<PropertyInfo, 
            MemberDescription>> MemberConvention { get; } = new Service<IDescriptionConvention
            <PropertyInfo, MemberDescription>> { Type = typeof(MemberConvention) };
        public Service<IDescriptionConvention<Type, EnumDescription>> 
            EnumConvention { get; } = new Service<IDescriptionConvention<Type, 
            EnumDescription>> { Type = typeof(EnumConvention) };
        public Service<IDescriptionConvention<FieldInfo, 
            OptionDescription>> EnumOptionConvention { get; } = new Service<IDescriptionConvention
            <FieldInfo, OptionDescription>> { Type = typeof(OptionConvention) };

        public List<Action<ModuleOverrideContext>> ModuleOverrides { get; } = new List<Action<ModuleOverrideContext>>();
        public List<Action<ResourceOverrideContext>> ResourceOverrides { get; } = new List<Action<ResourceOverrideContext>>();
        public List<Action<EndpointOverrideContext>> EndpointOverrides { get; } = new List<Action<EndpointOverrideContext>>();
        public List<Action<UrlParameterOverrideContext>> UrlParameterOverrides { get; } = new List<Action<UrlParameterOverrideContext>>();
        public List<Action<QuerystringOverrideContext>> QuerystringOverrides { get; } = new List<Action<QuerystringOverrideContext>>();
        public List<Action<StatusCodeOverrideContext>> StatusCodeOverrides { get; } = new List<Action<StatusCodeOverrideContext>>();
        public List<Action<HeaderOverrideContext>> RequestHeaderOverrides { get; } = new List<Action<HeaderOverrideContext>>();
        public List<Action<HeaderOverrideContext>> ResponseHeaderOverrides { get; } = new List<Action<HeaderOverrideContext>>();
        public List<Action<MessageOverrideContext>> RequestOverrides { get; } = new List<Action<MessageOverrideContext>>();
        public List<Action<MessageOverrideContext>> ResponseOverrides { get; } = new List<Action<MessageOverrideContext>>();
        public List<Action<TypeOverrideContext>> TypeOverrides { get; } = new List<Action<TypeOverrideContext>>();
        public List<Action<MemberOverrideContext>> MemberOverrides { get; } = new List<Action<MemberOverrideContext>>();
        public List<Action<OptionOverrideContext>> OptionOverrides { get; } = new List<Action<OptionOverrideContext>>();
    }
}
