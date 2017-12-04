using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using Swank.Description;
using Swank.Description.CodeExamples;
using Swank.Extensions;
using Swank.Specification;
using Swank.Web.Assets;
using Swank.Web.Handlers.App;
using Swank.Web.Templates;

namespace Swank.Configuration
{
    public class ConfigurationDsl
    {
        private readonly Configuration _configuration;

        public ConfigurationDsl(Configuration configuration)
        {
            _configuration = configuration;
        }

        public static Configuration CreateConfig(Action<ConfigurationDsl> configure = null)
        {
            var configuration = new Configuration(Assembly.GetCallingAssembly());
            var configurationDsl = new ConfigurationDsl(configuration);
            configure?.Invoke(configurationDsl);
            return configuration;
        }

        /// <summary>
        /// Specifies the api explorer type.
        /// </summary>
        public ConfigurationDsl WithApiExplorer<T>() where T : IApiExplorer
        {
            _configuration.ApiExplorer = typeof(T);
            return this;
        }

        /// <summary>
        /// Specifies that Swank is in debug mode. This mode 
        /// will display template error messages.
        /// </summary>
        public ConfigurationDsl IsInDebugMode()
        {
            _configuration.DebugMode = true;
            return this;
        }

        /// <summary>
        /// Specifies that Swank is in debug mode when the calling assembly is 
        /// in debug mode. This mode will display template error messages.
        /// </summary>
        public ConfigurationDsl IsInDebugModeWhenAppIsInDebugMode()
        {
            _configuration.DebugMode = Assembly.GetCallingAssembly().IsInDebugMode();
            return this;
        }

        /// <summary>
        /// Indicates that module sections should be collapsed when first displayed.
        /// </summary>
        public ConfigurationDsl CollapseModules()
        {
            _configuration.CollapseModules = true;
            return this;
        }

        /// <summary>
        /// Overrides the configured virtual path provider to ignore
        /// physical folders that match Swank urls. Additional 
        /// urls can be passed if desired.
        /// </summary>
        public ConfigurationDsl IgnoreFolders(params string[] urls)
        {
            _configuration.IgnoreFolders = true;
            _configuration.IgnoreFolderUrls = urls;
            return this;
        }

        /// <summary>
        /// Includes the assembly of the specified type.
        /// This call is additive, so you can specify multiple assemblies.
        /// </summary>
        public ConfigurationDsl AppliesTo<T>()
        {
            AppliesTo(typeof(T));
            return this;
        }

        /// <summary>
        /// Includes the assembly of the specified type.
        /// This call is additive, so you can specify multiple assemblies..
        /// </summary>
        public ConfigurationDsl AppliesTo(Type type)
        {
            _configuration.AppliesToAssemblies.Add(type.Assembly);
            return this;
        }

        /// <summary>
        /// Includes the specified assemblies.
        /// </summary>
        public ConfigurationDsl AppliesTo(params Assembly[] assemblies)
        {
            _configuration.AppliesToAssemblies.AddRange(assemblies);
            return this;
        }

        /// <summary>
        /// Sets a custom app template from an embedded resource. 
        /// Must be Razor template. If the extension is not 
        /// specified it must have a .cshtml extension.
        /// </summary>
        public ConfigurationDsl WithAppTemplateResource(string name)
        {
            _configuration.AppTemplate = RazorTemplate.FromResource<AppModel>(
                name, _configuration.AppliesToAssemblies, _configuration);
            return this;
        }

        /// <summary>
        /// Sets a custom app template from a virtual path. Must be Razor template.
        /// </summary>
        public ConfigurationDsl WithAppTemplateFromVirtualPath(string virtualPath)
        {
            _configuration.AppTemplate = RazorTemplate
                .FromVirtualPath<AppModel>(virtualPath, _configuration);
            return this;
        }

        /// <summary>
        /// Sets a custom app template. Must be Razor template.
        /// </summary>
        public ConfigurationDsl WithAppTemplate(string template)
        {
            _configuration.AppTemplate = RazorTemplate
                .FromString<AppModel>(template, _configuration);
            return this;
        }

        /// <summary>
        /// Url of the UI.
        /// </summary>
        public ConfigurationDsl WithAppAt(string url)
        {
            _configuration.AppUrl = url;
            return this;
        }

        /// <summary>
        /// The url used in the documentation.
        /// </summary>
        public ConfigurationDsl WithApiAt(string url)
        {
            _configuration.ApiUrl = url;
            return this;
        }

        /// <summary>
        /// Url of the specification.
        /// </summary>
        public ConfigurationDsl WithSpecificationAtUrl(string url)
        {
            _configuration.SpecificationUrl = url;
            return this;
        }

        /// <summary>
        /// Cache key for all requests.
        /// </summary>
        public ConfigurationDsl WithCacheKey(Func<HttpRequestMessage, string> getKey)
        {
            _configuration.CacheKey = getKey;
            return this;
        }

        /// <summary>
        /// Allows you to modify the app template model before rendering.
        /// </summary>
        public ConfigurationDsl WithAppPrerender(Action<HttpRequestMessage, AppModel> prerender)
        {
            _configuration.AppPreRender = prerender;
            return this;
        }

        /// <summary>
        /// Allows you to modify the resource model before rendering.
        /// </summary>
        public ConfigurationDsl WithResourcePrerender(Action<HttpRequestMessage, ResourceModel> prerender)
        {
            _configuration.ResourcePreRender = prerender;
            return this;
        }

        /// <summary>
        /// Allows you to modify the spec model before rendering.
        /// </summary>
        public ConfigurationDsl WithSpecPrerender(Action<HttpRequestMessage, 
            Specification.Module> prerender)
        {
            _configuration.SpecPreRender = prerender;
            return this;
        }

        /// <summary>
        /// Url of the favicon.
        /// </summary>
        public ConfigurationDsl WithFavIconAt(string url)
        {
            _configuration.FavIconUrl = url;
            return this;
        }

        /// <summary>
        /// The title of the page. Defaults to the name if not set.
        /// </summary>
        public ConfigurationDsl WithPageTitle(string title)
        {
            _configuration.Title = title;
            return this;
        }

        /// <summary>
        /// The name of the displayed in the page header.
        /// </summary>
        public ConfigurationDsl WithHeader(string name)
        {
            _configuration.Name = name;
            return this;
        }

        /// <summary>
        /// Url of the logo displayed in the page header.
        /// </summary>
        public ConfigurationDsl WithLogoAt(string url)
        {
            _configuration.LogoUrl = url;
            return this;
        }

        /// <summary>
        /// Adds xml comments from the bin folder of the calling assembly.
        /// Expects the file name to follow the standard convention of Assembly.xml.
        /// </summary>
        public ConfigurationDsl AddXmlComments()
        {
            _configuration.XmlComments.Add(FileAsset.FromPath(
                Assembly.GetCallingAssembly().GetPath()
                    .GetPathWithoutExtension() + ".xml"));
            return this;
        }

        /// <summary>
        /// Adds xml comments from a virtual path.
        /// </summary>
        public ConfigurationDsl AddXmlComments(string path)
        {
            _configuration.XmlComments.Add(FileAsset.FromVirtualPath(path));
            return this;
        }

        /// <summary>
        /// Sets the overview embedded resource. If the extension 
        /// is not specified it must have a .md extension.
        /// </summary>
        public ConfigurationDsl WithOverviewResource(string name)
        {
            _configuration.Overview = MarkdownAsset.FromResource(name, 
                _configuration.AppliesToAssemblies);
            return this;
        }

        /// <summary>
        /// Sets the virtual path of the overview.
        /// </summary>
        public ConfigurationDsl WithOverviewFromVirtualPath(string name)
        {
            _configuration.Overview = MarkdownAsset.FromVirtualPath(name);
            return this;
        }

        /// <summary>
        /// Sets the overview.
        /// </summary>
        public ConfigurationDsl WithOverview(string comments)
        {
            _configuration.Overview = MarkdownAsset.FromString(comments);
            return this;
        }

        /// <summary>
        /// Adds an overview fragment link. Defaults the 
        /// fragment to the name snake cased.
        /// </summary>
        public ConfigurationDsl WithOverviewLink(string name)
        {
            return WithOverviewLink(name, name.ToFragmentId());
        }

        /// <summary>
        /// Adds an overview fragment link.
        /// </summary>
        public ConfigurationDsl WithOverviewLink(string name, string fragmentId)
        {
            _configuration.OverviewLinks.Add(new Configuration.OverviewLink
            {
                Name = name, FragmentId = fragmentId
            });
            return this;
        }

        /// <summary>
        /// Adds the basic authentication scheme.
        /// </summary>
        public ConfigurationDsl WithBasicAuthenticationScheme()
        {
            _configuration.AuthenticationSchemes.Add(new AuthenticationScheme
            {
                Name = "Basic",
                Components = new List<AuthenticationComponent>
                {
                    new AuthenticationComponent
                    {
                        Name = "Authorization",
                        ClientSideGenerator = @"function(username, password) { " + 
                                "return 'Basic ' + base64.encode(username + ':' + password); " + 
                            "}",
                        Location = AuthenticationLocation.Header,
                        Parameters = new List<AuthenticationParameter>
                        {
                            new AuthenticationParameter("Username"),
                            new AuthenticationParameter("Password", true)
                        }
                    }
                }
            });
            return this;
        }

        /// <summary>
        /// Adds the token authentication scheme.
        /// </summary>
        public ConfigurationDsl WithTokenAuthenticationScheme()
        {
            return WithAuthenticationScheme("Token");
        }

        /// <summary>
        /// Adds the bearer authentication scheme.
        /// </summary>
        public ConfigurationDsl WithBearerAuthenticationScheme()
        {
            return WithAuthenticationScheme("Bearer");
        }

        private ConfigurationDsl WithAuthenticationScheme(string scheme)
        {
            _configuration.AuthenticationSchemes.Add(new AuthenticationScheme
            {
                Name = scheme,
                Components = new List<AuthenticationComponent>
                {
                    new AuthenticationComponent
                    {
                        Name = "Authorization",
                        ClientSideGenerator = $"function({scheme.ToLower()}) " +
                                              "{{ " +
                                              $"    return '{scheme} ' + {scheme.ToLower()}; " +
                                              "}}",
                        Location = AuthenticationLocation.Header,
                        Parameters = new List<AuthenticationParameter>
                        {
                            new AuthenticationParameter(scheme)
                        }
                    }
                }
            });
            return this;
        }

        /// <summary>
        /// Adds a custom authentication scheme.
        /// </summary>
        public ConfigurationDsl WithCustomAuthenticationScheme(AuthenticationScheme scheme)
        {
            _configuration.AuthenticationSchemes.Add(scheme);
            return this;
        }

        /// <summary>
        /// Hides the test drive section.
        /// </summary>
        public ConfigurationDsl HideTestDriveSection()
        {
            _configuration.HideTestDriveSection = true;
            return this;
        }

        /// <summary>
        /// Hides the examples section.
        /// </summary>
        public ConfigurationDsl HideCodeExamplesSection()
        {
            _configuration.HideCodeExamplesSection = true;
            return this;
        }

        /// <summary>
        /// Hides the url parameters section.
        /// </summary>
        public ConfigurationDsl HideUrlParametersSection()
        {
            _configuration.HideUrlParametersSection = true;
            return this;
        }

        /// <summary>
        /// Hides the querystring section.
        /// </summary>
        public ConfigurationDsl HideQueryStringSection()
        {
            _configuration.HideQueryStringSection = true;
            return this;
        }

        /// <summary>
        /// Hides the request headers section.
        /// </summary>
        public ConfigurationDsl HideRequestHeadersSection()
        {
            _configuration.HideRequestHeadersSection = true;
            return this;
        }

        /// <summary>
        /// Hides the request body section.
        /// </summary>
        public ConfigurationDsl HideRequestBodySection()
        {
            _configuration.HideRequestBodySection = true;
            return this;
        }

        /// <summary>
        /// Hides the request section.
        /// </summary>
        public ConfigurationDsl HideRequestSection()
        {
            _configuration.HideRequestSection = true;
            return this;
        }

        /// <summary>
        /// Hides the response headers section.
        /// </summary>
        public ConfigurationDsl HideResponseHeadersSection()
        {
            _configuration.HideResponseHeadersSection = true;
            return this;
        }

        /// <summary>
        /// Hides the response body section.
        /// </summary>
        public ConfigurationDsl HideResponseBodySection()
        {
            _configuration.HideResponseBodySection = true;
            return this;
        }

        /// <summary>
        /// Hides the response section.
        /// </summary>
        public ConfigurationDsl HideResponseSection()
        {
            _configuration.HideResponseSection = true;
            return this;
        }

        /// <summary>
        /// Hides the status code section.
        /// </summary>
        public ConfigurationDsl HideStatusCodeSection()
        {
            _configuration.HideStatusCodeSection = true;
            return this;
        }

        /// <summary>
        /// Adds a copyright. The {{year}} token is replaced by the current year.
        /// </summary>
        public ConfigurationDsl WithCopyright(string copyright)
        {
            _configuration.Copyright = copyright.Replace(
                "{{year}}", DateTime.Now.Year.ToString());
            return this;
        }

        /// <summary>
        /// Specifies that the namespace in the template model includes the module name.
        /// </summary>
        public ConfigurationDsl TemplateNamespaceIncludesModule()
        {
            _configuration.TemplateNamespaceIncludesModule = true;
            return this;
        }

        /// <summary>
        /// Adds a template embedded resource. If the extension is not 
        /// specified it must have a .mustache or .cshtml extension.
        /// </summary>
        public ConfigurationDsl WithTemplateResource(
            string path, string url, string mimeType)
        {
            _configuration.Templates.AddRange(WebTemplate.FromResource<List<Specification.Module>>(
                _configuration.AppliesToAssemblies, path, url, mimeType, _configuration));
            return this;
        }

        /// <summary>
        /// Adds template embedded resources in the specified namespace. 
        /// They must have a .mustache or .cshtml extension.
        /// </summary>
        public ConfigurationDsl WithTemplateResources(
            string path, string url, string mimeType)
        {
            _configuration.Templates.AddRange(WebTemplate.FromResources<List<Specification.Module>>(
                _configuration.AppliesToAssemblies, path, url, mimeType, _configuration));
            return this;
        }

        /// <summary>
        /// Adds a template from a virtual path. 
        /// </summary>
        public ConfigurationDsl WithTemplateFromVirtualPath(
            string virtualPath, string url, string mimeType)
        {
            _configuration.Templates.AddRange(WebTemplate
                .FromVirtualPath<List<Specification.Module>>(
                    virtualPath, url, mimeType, _configuration));
            return this;
        }

        /// <summary>
        /// Adds templates from a virtual path.  
        /// They must have a .mustache or .cshtml extension.
        /// </summary>
        public ConfigurationDsl WithTemplatesInVirtualPath( 
            string virtualPath, string mimeType, string url)
        {
            _configuration.Templates.AddRange(WebTemplate
                    .InVirtualPath<List<Specification.Module>>(
                        virtualPath, url, mimeType, _configuration));
            return this;
        }

        /// <summary>
        /// Adds a mustache template.  
        /// </summary>
        public ConfigurationDsl WithMustacheTemplate(
            string url, string mimeType, string template)
        {
            _configuration.Templates.Add(WebTemplate
                .MustacheFromString(url, mimeType, template, _configuration));
            return this;
        }

        /// <summary>
        /// Adds a Razor template.  
        /// </summary>
        public ConfigurationDsl WithRazorTemplate(
            string url, string template, string mimeType)
        {
            _configuration.Templates.Add(WebTemplate
                .RazorFromString<List<Specification.Module>>(url, mimeType, template, _configuration));
            return this;
        }

        /// <summary>
        /// Specify stylesheets to be included in the documentation page.
        /// This can be used to override the appearance of the page.
        /// You can use application relative paths a la ~/styles/style.css.
        /// </summary>
        public ConfigurationDsl WithStylesheets(params string[] urls)
        {
            _configuration.Stylesheets.AddRange(urls.Select(x => (LazyUrl)x));
            return this;
        }

        /// <summary>
        /// Specify scripts to be included in the documentation page.
        /// This can be used to override the behavior of the page.
        /// You can use application relative paths a la ~/scripts/script.js.
        /// </summary>
        public ConfigurationDsl WithScripts(params string[] urls)
        {
            _configuration.Scripts.AddRange(urls.Select(x => (LazyUrl)x));
            return this;
        }

        /// <summary>
        /// Do not show json data samples.
        /// </summary>
        public ConfigurationDsl HideJsonData()
        {
            _configuration.DisplayJsonData = false;
            return this;
        }

        /// <summary>
        /// Do not show xml data samples.
        /// </summary>
        public ConfigurationDsl HideXmlData()
        {
            _configuration.DisplayXmlData = false;
            return this;
        }

        /// <summary>
        /// Specifies a convention for generating the namespace of actions.
        /// </summary>
        public ConfigurationDsl WithActionNamespaceConvention(
            Func<IApiDescription, List<string>> @namespace)
        {
            _configuration.ActionNamespace = @namespace;
            return this;
        }

        /// <summary>
        /// Specifies a convention for generating the name of actions.
        /// </summary>
        public ConfigurationDsl WithActionNameConvention(
            Func<IApiDescription, string> name)
        {
            _configuration.ActionName = name;
            return this;
        }

        /// <summary>
        /// Specifies an action filter.
        /// </summary>
        public ConfigurationDsl Where(Func<IApiDescription, bool> filter)
        {
            _configuration.Filter = filter;
            return this;
        }

        /// <summary>
        /// The data example dictionary key name.
        /// </summary>
        public ConfigurationDsl WithDefaultDictionaryKeyName(string keyName)
        {
            _configuration.DefaultDictionaryKeyName = keyName;
            return this;
        }

        /// <summary>
        /// Indicates that enum values should be numeric.
        /// </summary>
        public ConfigurationDsl WithNumericEnum()
        {
            _configuration.EnumFormat = EnumFormat.AsNumber;
            return this;
        }

        /// <summary>
        /// Specifies the default optional scope for all model properties.
        /// </summary>
        public ConfigurationDsl WithDefaultOptionalScope(OptionalScope scope)
        {
            _configuration.DefaultOptionalScope = scope;
            return this;
        }

        /// <summary>
        /// Specifes the highlight.js theme to use for code samples.
        /// </summary>
        public ConfigurationDsl WithCodeExampleTheme(CodeExampleTheme theme)
        {
            var stylesheets = _configuration.Stylesheets;
            stylesheets.RemoveByFilename(CodeExampleThemes.Files);
            var url = _configuration.Assets.FindByFilename(theme
                .ToSnakeCase()).ToLazyUrl(_configuration);
            stylesheets.PrependOrAdd(url, Configuration.AppStylesheet);
            return this;
        }

        /// <summary>
        /// Removes the stock code examples. 
        /// </summary>
        public ConfigurationDsl ClearCodeExamples()
        {
            _configuration.CodeExamples.Clear();
            return this;
        }

        /// <summary>
        /// Adds a code example embedded resource. 
        /// </summary>
        public ConfigurationDsl WithCodeExampleResource(string path, 
            string name, CodeExampleLanguage language)
        {
            _configuration.CodeExamples.AddRange(CodeExample.FromResource(
                _configuration.AppliesToAssemblies, path, name, 
                language.ToString().ToLower(), _configuration));
            return this;
        }

        /// <summary>
        /// Adds code example embedded resources and optional comments under the specified namespace.
        /// These must be named as [highlight.js language].[md|cshtml|mustache].
        /// The name defaults to the filename.
        /// </summary>
        public ConfigurationDsl WithCodeExampleResources(string path)
        {
            _configuration.CodeExamples.AddRange(CodeExample.FromResources(
                _configuration.AppliesToAssemblies, path, _configuration));
            return this;
        }

        /// <summary>
        /// Adds a code example from a relative or virtual path.
        /// </summary>
        public ConfigurationDsl WithCodeExampleFromVirtualPath(string virtualPath, 
            string name, CodeExampleLanguage language)
        {
            _configuration.CodeExamples.AddRange(CodeExample.FromVirtualPath(virtualPath, 
                name, language.ToString().ToLower(), _configuration));
            return this;
        }

        /// <summary>
        /// Adds code examples and optional comments under a virtual path.
        /// These must be named as [highlight.js language].[md|cshtml|mustache].
        /// The name defaults to the filename.
        /// </summary>
        public ConfigurationDsl WithCodeExamplesInVirtualPath(string virtualPath)
        {
            _configuration.CodeExamples.AddRange(CodeExample.InVirtualPath(virtualPath, _configuration));
            return this;
        }

        /// <summary>
        /// Adds a mustache code example to the documentation.  
        /// </summary>
        public ConfigurationDsl WithMustacheCodeExample(string name, 
            CodeExampleLanguage language, string comments, string template)
        {
            _configuration.CodeExamples.Add(new CodeExample(name,
                language.ToString().ToLower(), MarkdownAsset.FromString(comments), 
                MustacheTemplate.FromString(template, _configuration)));
            return this;
        }

        /// <summary>
        /// Adds a Razor code example to the documentation.  
        /// </summary>
        public ConfigurationDsl WithRazorCodeExample(string name,
            CodeExampleLanguage language, string comments, string template)
        {
            _configuration.CodeExamples.Add(new CodeExample(name,
                language.ToString().ToLower(), MarkdownAsset.FromString(comments),
                RazorTemplate.FromString<Description.CodeExamples.CodeExampleModel>(template, _configuration)));
            return this;
        }

        /// <summary>
        /// The format of default and sample DateTime values displayed in the documentation.
        /// </summary>
        public ConfigurationDsl WithSampleDateTimeFormat(string format)
        {
            _configuration.SampleDateTimeFormat = format;
            return this;
        }

        /// <summary>
        /// The format of default and sample integer values displayed in the documentation.
        /// </summary>
        public ConfigurationDsl WithSampleIntegerFormat(string format)
        {
            _configuration.SampleIntegerFormat = format;
            return this;
        }

        /// <summary>
        /// The format of default and sample real values displayed in the documentation.
        /// </summary>
        public ConfigurationDsl WithSampleRealFormat(string format)
        {
            _configuration.SampleRealFormat = format;
            return this;
        }

        /// <summary>
        /// The format of default and sample TimeSpan values displayed in the documentation.
        /// </summary>
        public ConfigurationDsl WithSampleTimeSpanFormat(string format)
        {
            _configuration.SampleTimeSpanFormat = format;
            return this;
        }

        /// <summary>
        /// The format of default and sample Guid values displayed in the documentation.
        /// </summary>
        public ConfigurationDsl WithSampleGuidFormat(string format)
        {
            _configuration.SampleGuidFormat = format;
            return this;
        }

        /// <summary>
        /// Sample string value displayed in the documentation.
        /// </summary>
        public ConfigurationDsl WithSampleStringValue(string value)
        {
            _configuration.SampleStringValue = value;
            return this;
        }

        /// <summary>
        /// Sample boolean value displayed in the documentation.
        /// </summary>
        public ConfigurationDsl WithSampleBoolValue(bool value)
        {
            _configuration.SampleBoolValue = value;
            return this;
        }

        /// <summary>
        /// Sample DateTime value displayed in the documentation.
        /// </summary>
        public ConfigurationDsl WithSampleDateTimeValue(DateTime value)
        {
            _configuration.SampleDateTimeValue = value;
            return this;
        }

        /// <summary>
        /// Sample integer value displayed in the documentation.
        /// </summary>
        public ConfigurationDsl WithSampleIntegerValue(int value)
        {
            _configuration.SampleIntegerValue = value;
            return this;
        }

        /// <summary>
        /// Sample real value displayed in the documentation.
        /// </summary>
        public ConfigurationDsl WithSampleRealValue(decimal value)
        {
            _configuration.SampleRealValue = value;
            return this;
        }

        /// <summary>
        /// Sample TimeSpan value displayed in the documentation.
        /// </summary>
        public ConfigurationDsl WithSampleTimeSpanValue(TimeSpan value)
        {
            _configuration.SampleTimeSpanValue = value;
            return this;
        }

        /// <summary>
        /// Sample Guid value displayed in the documentation.
        /// </summary>
        public ConfigurationDsl WithSampleGuidValue(Guid value)
        {
            _configuration.SampleGuidValue = value;
            return this;
        }

        /// <summary>
        /// Sample URI value displayed in the documentation.
        /// </summary>
        public ConfigurationDsl WithSampleUriValue(Uri value)
        {
            _configuration.SampleUriValue = value;
            return this;
        }

        /// <summary>
        /// Sets the module convention.
        /// </summary>
        public ConfigurationDsl WithModuleConvention<T>() where T :
            IDescriptionConvention<IApiDescription, ModuleDescription>
        {
            return WithModuleConvention<T, object>(null);
        }

        /// <summary>
        /// Sets the module convention.
        /// </summary>
        public ConfigurationDsl WithModuleConvention<T, TConfig>(Action<TConfig> configure)
            where T : IDescriptionConvention<IApiDescription, ModuleDescription>
            where TConfig : class, new()
        {
            _configuration.ModuleConvention.Type = typeof(T);
            _configuration.ModuleConvention.Config = CreateConfig(configure);
            return this;
        }

        /// <summary>
        /// Sets the resource identifier convention.
        /// </summary>
        public ConfigurationDsl WithResourceIdentifier(
            Func<IApiDescription, string> identifier)
        {
            _configuration.DefaultResourceIdentifier = identifier;
            return this;
        }

        /// <summary>
        /// Sets the resource convention.
        /// </summary>
        public ConfigurationDsl WithResourceConvention<T>() where T :
            IDescriptionConvention<IApiDescription, ResourceDescription>
        {
            return WithResourceConvention<T, object>(null);
        }

        /// <summary>
        /// Sets the resource convention.
        /// </summary>
        public ConfigurationDsl WithResourceConvention<T, TConfig>(Action<TConfig> configure)
            where T : IDescriptionConvention<IApiDescription, ResourceDescription>
            where TConfig : class, new()
        {
            _configuration.ResourceConvention.Type = typeof(T);
            _configuration.ResourceConvention.Config = CreateConfig(configure);
            return this;
        }

        /// <summary>
        /// Sets the endpoint convention.
        /// </summary>
        public ConfigurationDsl WithEndpointConvention<T>() where T :
            IDescriptionConvention<IApiDescription, EndpointDescription>
        {
            return WithEndpointConvention<T, object>(null);
        }

        /// <summary>
        /// Sets the endpoint convention.
        /// </summary>
        public ConfigurationDsl WithEndpointConvention<T, TConfig>(Action<TConfig> configure)
            where T : IDescriptionConvention<IApiDescription, EndpointDescription>
            where TConfig : class, new()
        {
            _configuration.EndpointConvention.Type = typeof(T);
            _configuration.EndpointConvention.Config = CreateConfig(configure);
            return this;
        }

        /// <summary>
        /// Sets the member convention.
        /// </summary>
        public ConfigurationDsl WithMemberConvention<T>() where T :
            IDescriptionConvention<PropertyInfo, MemberDescription>
        {
            return WithMemberConvention<T, object>(null);
        }

        /// <summary>
        /// Sets the member convention.
        /// </summary>
        public ConfigurationDsl WithMemberConvention<T, TConfig>(Action<TConfig> configure)
            where T : IDescriptionConvention<PropertyInfo, MemberDescription>
            where TConfig : class, new()
        {
            _configuration.MemberConvention.Type = typeof(T);
            _configuration.MemberConvention.Config = CreateConfig(configure);
            return this;
        }

        /// <summary>
        /// Sets the enum convention.
        /// </summary>
        public ConfigurationDsl WithEnumConvention<T>() where T :
            IDescriptionConvention<Type, EnumDescription>
        {
            return WithEnumConvention<T, object>(null);
        }

        /// <summary>
        /// Sets the enum convention.
        /// </summary>
        public ConfigurationDsl WithEnumConvention<T, TConfig>(Action<TConfig> configure)
            where T : IDescriptionConvention<Type, EnumDescription>
            where TConfig : class, new()
        {
            _configuration.EnumConvention.Type = typeof(T);
            _configuration.EnumConvention.Config = CreateConfig(configure);
            return this;
        }

        /// <summary>
        /// Sets the enum option convention.
        /// </summary>
        public ConfigurationDsl WithEnumOptionConvention<T>() where T :
            IDescriptionConvention<FieldInfo, OptionDescription>
        {
            return WithEnumOptionConvention<T, object>(null);
        }

        /// <summary>
        /// Sets the enum option convention.
        /// </summary>
        public ConfigurationDsl WithEnumOptionConvention<T, TConfig>(Action<TConfig> configure)
            where T : IDescriptionConvention<FieldInfo, OptionDescription>
            where TConfig : class, new()
        {
            _configuration.EnumOptionConvention.Type = typeof(T);
            _configuration.EnumOptionConvention.Config = CreateConfig(configure);
            return this;
        }

        /// <summary>
        /// Sets the status code convention.
        /// </summary>
        public ConfigurationDsl WithStatusCodeConvention<T>() where T :
            IDescriptionConvention<IApiDescription, List<StatusCodeDescription>>
        {
            return WithStatusCodeConvention<T, object>(null);
        }

        /// <summary>
        /// Sets the status code convention.
        /// </summary>
        public ConfigurationDsl WithStatusCodeConvention<T, TConfig>(Action<TConfig> configure)
            where T : IDescriptionConvention<IApiDescription, List<StatusCodeDescription>>
            where TConfig : class, new()
        {
            _configuration.StatusCodeConvention.Type = typeof(T);
            _configuration.StatusCodeConvention.Config = CreateConfig(configure);
            return this;
        }

        /// <summary>
        /// Sets the header convention.
        /// </summary>
        public ConfigurationDsl WithHeaderConvention<T>() where T :
            IDescriptionConvention<IApiDescription, List<HeaderDescription>>
        {
            return WithHeaderConvention<T, object>(null);
        }

        /// <summary>
        /// Sets the header convention.
        /// </summary>
        public ConfigurationDsl WithHeaderConvention<T, TConfig>(Action<TConfig> configure)
            where T : IDescriptionConvention<IApiDescription, List<HeaderDescription>>
            where TConfig : class, new()
        {
            _configuration.HeaderConvention.Type = typeof(T);
            _configuration.HeaderConvention.Config = CreateConfig(configure);
            return this;
        }

        /// <summary>
        /// Sets the type convention.
        /// </summary>
        public ConfigurationDsl WithTypeConvention<T>() where T : IDescriptionConvention<Type, TypeDescription>
        {
            return WithTypeConvention<T, object>(null);
        }

        /// <summary>
        /// Sets the type convention.
        /// </summary>
        public ConfigurationDsl WithTypeConvention<T, TConfig>(Action<TConfig> configure)
            where T : IDescriptionConvention<Type, TypeDescription>
            where TConfig : class, new()
        {
            _configuration.TypeConvention.Type = typeof(T);
            _configuration.TypeConvention.Config = CreateConfig(configure);
            return this;
        }

        private TConfig CreateConfig<TConfig>(Action<TConfig> configure) where TConfig : class, new()
        {
            if (configure == null) return null;
            var config = new TConfig();
            configure(config);
            return config;
        }

        /// <summary>
        /// Specifies the default module name.
        /// </summary>
        public ConfigurationDsl WithDefaultModuleName(string name)
        {
            _configuration.DefaultModuleName = name;
            return this;
        }

        /// <summary>
        /// Determines what happens when a module is not defined for a resource.
        /// </summary>
        public ConfigurationDsl WhenModuleOrphaned(OrphanedEndpoints behavior)
        {
            _configuration.OrphanedModuleEndpoint = behavior;
            return this;
        }

        /// <summary>
        /// Defines a default resource that endpoints are added to when none are defined for it.
        /// </summary>
        public ConfigurationDsl WithDefaultResource(
            Func<IApiDescription, string, ResourceDescription> factory)
        {
            _configuration.DefaultResourceFactory = factory;
            return this;
        }

        /// <summary>
        /// Determines what happens when a resource is not defined for an endpoint.
        /// </summary>
        public ConfigurationDsl WhenResourceOrphaned(OrphanedEndpoints behavior)
        {
            _configuration.OrphanedResourceEndpoint = behavior;
            return this;
        }

        /// <summary>
        /// This indicates that you would like to do it for the lulz.
        /// </summary>
        public ConfigurationDsl DoItForTheLulz()
        {
            return this;
        }

        /// <summary>
        /// Overrides modules.
        /// </summary>
        public ConfigurationDsl OverrideModules(Action<ModuleOverrideContext> @override)
        {
            _configuration.ModuleOverrides.Add(@override);
            return this;
        }

        /// <summary>
        /// Overrides modules when a condition is met.
        /// </summary>
        public ConfigurationDsl OverrideModulesWhen(Action<ModuleOverrideContext> @override,
            Func<ModuleOverrideContext, bool> when)
        {
            _configuration.ModuleOverrides.Add(OverrideWhen(@override, when));
            return this;
        }

        /// <summary>
        /// Overrides resources.
        /// </summary>
        public ConfigurationDsl OverrideResources(Action<ResourceOverrideContext> @override)
        {
            _configuration.ResourceOverrides.Add(@override);
            return this;
        }

        /// <summary>
        /// Overrides resources when a condition is met.
        /// </summary>
        public ConfigurationDsl OverrideResourcesWhen(Action<ResourceOverrideContext> @override,
            Func<ResourceOverrideContext, bool> when)
        {
            _configuration.ResourceOverrides.Add(OverrideWhen(@override, when));
            return this;
        }

        /// <summary>
        /// Overrides endpoints.
        /// </summary>
        public ConfigurationDsl OverrideEndpoints(Action<EndpointOverrideContext> @override)
        {
            _configuration.EndpointOverrides.Add(@override);
            return this;
        }

        /// <summary>
        /// Overrides endpoints when a condition is met.
        /// </summary>
        public ConfigurationDsl OverrideEndpointsWhen(Action<EndpointOverrideContext> @override,
            Func<EndpointOverrideContext, bool> when)
        {
            _configuration.EndpointOverrides.Add(OverrideWhen(@override, when));
            return this;
        }

        /// <summary>
        /// Overrides url parameters.
        /// </summary>
        public ConfigurationDsl OverrideUrlParameters(Action<UrlParameterOverrideContext> @override)
        {
            _configuration.UrlParameterOverrides.Add(@override);
            return this;
        }

        /// <summary>
        /// Overrides url parameters when a condition is met.
        /// </summary>
        public ConfigurationDsl OverrideUrlParametersWhen(Action<UrlParameterOverrideContext> @override,
            Func<UrlParameterOverrideContext, bool> when)
        {
            _configuration.UrlParameterOverrides.Add(OverrideWhen(@override, when));
            return this;
        }

        /// <summary>
        /// Overrides querystrings.
        /// </summary>
        public ConfigurationDsl OverrideQuerystring(Action<QuerystringOverrideContext> @override)
        {
            _configuration.QuerystringOverrides.Add(@override);
            return this;
        }

        /// <summary>
        /// Overrides querystrings when a condition is met.
        /// </summary>
        public ConfigurationDsl OverrideQuerystringWhen(Action<QuerystringOverrideContext> @override,
            Func<QuerystringOverrideContext, bool> when)
        {
            _configuration.QuerystringOverrides.Add(OverrideWhen(@override, when));
            return this;
        }

        /// <summary>
        /// Overrides properties. Properties include members, url parameters 
        /// and querystring parameters.
        /// </summary>
        public ConfigurationDsl OverrideParameters(
            Action<IParameterOverrideContext> @override)
        {
            return OverrideQuerystring(@override)
                .OverrideUrlParameters(@override);
        }

        /// <summary>
        /// Overrides parameters when a condition is met. Parameters include 
        /// url parameters and querystring parameters.
        /// </summary>
        public ConfigurationDsl OverrideParametersWhen(
            Action<IParameterOverrideContext> @override,
            Func<IParameterOverrideContext, bool> when)
        {
            return OverrideQuerystringWhen(@override, when)
                .OverrideUrlParametersWhen(@override, when);
        }

        /// <summary>
        /// Overrides status codes.
        /// </summary>
        public ConfigurationDsl OverrideStatusCodes(
            Action<StatusCodeOverrideContext> @override)
        {
            _configuration.StatusCodeOverrides.Add(@override);
            return this;
        }

        /// <summary>
        /// Overrides status codes when a condition is met.
        /// </summary>
        public ConfigurationDsl OverrideStatusCodesWhen(
            Action<StatusCodeOverrideContext> @override,
            Func<StatusCodeOverrideContext, bool> when)
        {
            _configuration.StatusCodeOverrides.Add(OverrideWhen(@override, when));
            return this;
        }

        /// <summary>
        /// Overrides headers.
        /// </summary>
        public ConfigurationDsl OverrideHeaders(Action<HeaderOverrideContext> @override)
        {
            _configuration.RequestHeaderOverrides.Add(@override);
            _configuration.ResponseHeaderOverrides.Add(@override);
            return this;
        }

        /// <summary>
        /// Overrides headers when a condition is met.
        /// </summary>
        public ConfigurationDsl OverrideHeadersWhen(Action<HeaderOverrideContext> @override,
            Func<HeaderOverrideContext, bool> when)
        {
            _configuration.RequestHeaderOverrides.Add(OverrideWhen(@override, when));
            _configuration.ResponseHeaderOverrides.Add(OverrideWhen(@override, when));
            return this;
        }

        /// <summary>
        /// Overrides request headers.
        /// </summary>
        public ConfigurationDsl OverrideRequestHeaders(Action<HeaderOverrideContext> @override)
        {
            _configuration.RequestHeaderOverrides.Add(@override);
            return this;
        }

        /// <summary>
        /// Overrides request headers when a condition is met.
        /// </summary>
        public ConfigurationDsl OverrideRequestHeadersWhen(
            Action<HeaderOverrideContext> @override,
            Func<HeaderOverrideContext, bool> when)
        {
            _configuration.RequestHeaderOverrides.Add(OverrideWhen(@override, when));
            return this;
        }

        /// <summary>
        /// Overrides response headers.
        /// </summary>
        public ConfigurationDsl OverrideResponseHeaders(
            Action<HeaderOverrideContext> @override)
        {
            _configuration.ResponseHeaderOverrides.Add(@override);
            return this;
        }

        /// <summary>
        /// Overrides response headers when a condition is met.
        /// </summary>
        public ConfigurationDsl OverrideResponseHeadersWhen(
            Action<HeaderOverrideContext> @override,
            Func<HeaderOverrideContext, bool> when)
        {
            _configuration.ResponseHeaderOverrides.Add(OverrideWhen(@override, when));
            return this;
        }

        /// <summary>
        /// Overrides requests.
        /// </summary>
        public ConfigurationDsl OverrideRequest(Action<MessageOverrideContext> @override)
        {
            _configuration.RequestOverrides.Add(@override);
            return this;
        }

        /// <summary>
        /// Overrides requests when a condition is met.
        /// </summary>
        public ConfigurationDsl OverrideRequestWhen(Action<MessageOverrideContext> @override,
            Func<MessageOverrideContext, bool> when)
        {
            _configuration.RequestOverrides.Add(OverrideWhen(@override, when));
            return this;
        }

        /// <summary>
        /// Overrides responses.
        /// </summary>
        public ConfigurationDsl OverrideResponse(Action<MessageOverrideContext> @override)
        {
            _configuration.ResponseOverrides.Add(@override);
            return this;
        }

        /// <summary>
        /// Overrides responses when a condition is met.
        /// </summary>
        public ConfigurationDsl OverrideResponseWhen(Action<MessageOverrideContext> @override,
            Func<MessageOverrideContext, bool> when)
        {
            _configuration.ResponseOverrides.Add(OverrideWhen(@override, when));
            return this;
        }

        /// <summary>
        /// Overrides both requests and responses.
        /// </summary>
        public ConfigurationDsl OverrideMessage(Action<MessageOverrideContext> @override)
        {
            return OverrideRequest(@override).OverrideResponse(@override);
        }

        /// <summary>
        /// Overrides both requests and responses when a condition is met.
        /// </summary>
        public ConfigurationDsl OverrideMessageWhen(Action<MessageOverrideContext> @override,
            Func<MessageOverrideContext, bool> when)
        {
            return OverrideRequestWhen(@override, when).OverrideResponseWhen(@override, when);
        }

        /// <summary>
        /// Overrides types.
        /// </summary>
        public ConfigurationDsl OverrideTypes(Action<TypeOverrideContext> @override)
        {
            _configuration.TypeOverrides.Add(@override);
            return this;
        }

        /// <summary>
        /// Overrides types when a condition is met.
        /// </summary>
        public ConfigurationDsl OverrideTypesWhen(Action<TypeOverrideContext> @override,
            Func<TypeOverrideContext, bool> when)
        {
            _configuration.TypeOverrides.Add(OverrideWhen(@override, when));
            return this;
        }

        /// <summary>
        /// Camel case member names.
        /// </summary>
        public ConfigurationDsl CamelCaseMemberNames()
        {
            _configuration.MemberOverrides.Add(x => 
                x.Member.Name = x.Member.Name.ToCamelCase());
            return this;
        }

        /// <summary>
        /// Overrides members.
        /// </summary>
        public ConfigurationDsl OverrideMembers(Action<MemberOverrideContext> @override)
        {
            _configuration.MemberOverrides.Add(@override);
            return this;
        }

        /// <summary>
        /// Overrides members when a condition is met.
        /// </summary>
        public ConfigurationDsl OverrideMembersWhen(Action<MemberOverrideContext> @override,
            Func<MemberOverrideContext, bool> when)
        {
            _configuration.MemberOverrides.Add(OverrideWhen(@override, when));
            return this;
        }

        /// <summary>
        /// Overrides options.
        /// </summary>
        public ConfigurationDsl OverrideOptions(Action<OptionOverrideContext> @override)
        {
            _configuration.OptionOverrides.Add(@override);
            return this;
        }

        /// <summary>
        /// Overrides options when a condition is met.
        /// </summary>
        public ConfigurationDsl OverrideOptionsWhen(Action<OptionOverrideContext> @override,
            Func<OptionOverrideContext, bool> when)
        {
            _configuration.OptionOverrides.Add(OverrideWhen(@override, when));
            return this;
        }

        private static Action<T> OverrideWhen<T>(Action<T> @override, Func<T, bool> when)
        {
            return x => { if (when(x)) @override(x); };
        }
    }
}