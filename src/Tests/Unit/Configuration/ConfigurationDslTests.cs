using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Http.Description;
using System.Web.Http.Routing;
using NUnit.Framework;
using Should;
using Swank.Configuration;
using Swank.Description;
using Swank.Description.CodeExamples;
using Swank.Extensions;
using Swank.Specification;
using Swank.Web.Assets;
using Swank.Web.Templates;
using TestHarness.Module;
using Tests.Common;

namespace Tests.Unit.Configuration
{
    [TestFixture]
    public class ConfigurationDslTests
    {
        private Swank.Configuration.Configuration _configuration;
        private ConfigurationDsl _dsl;

        [SetUp]
        public void Setup()
        {
            _configuration = new Swank.Configuration.Configuration { DebugMode = true };
            _configuration.CodeExamples.Clear();
            _dsl = new ConfigurationDsl(_configuration);
        }

        [Test]
        public void should_set_applies_to_this_assembly_by_default()
        {
            _configuration.AppliesToAssemblies.Count.ShouldEqual(1);
            _configuration.AppliesToAssemblies.First()
                .ShouldEqual(Assembly.GetExecutingAssembly());
        }

        [Test]
        public void should_set_generic_applies_to_type_assembly()
        {
            _configuration.AppliesToAssemblies.Clear();
            _dsl.AppliesTo<ModuleController>();

            _configuration.AppliesToAssemblies.Count.ShouldEqual(1);
            _configuration.AppliesToAssemblies.First()
                .ShouldEqual(typeof(ModuleController).Assembly);
        }

        [Test]
        public void should_set_applies_to_type_assembly()
        {
            _configuration.AppliesToAssemblies.Clear();
            _dsl.AppliesTo(typeof(ModuleController));

            _configuration.AppliesToAssemblies.Count.ShouldEqual(1);
            _configuration.AppliesToAssemblies.First()
                .ShouldEqual(typeof(ModuleController).Assembly);
        }

        [Test]
        public void should_set_applies_to_assembly()
        {
            _configuration.AppliesToAssemblies.Clear();
            _dsl.AppliesTo(typeof(ModuleController).Assembly);

            _configuration.AppliesToAssemblies.Count.ShouldEqual(1);
            _configuration.AppliesToAssemblies.First()
                .ShouldEqual(typeof(ModuleController).Assembly);
        }

        [Test]
        public void should_set_app_template_from_resource()
        {
            _dsl.WithAppTemplateResource("ConfigurationDslTests/App/razor.cshtml");

            _configuration.AppTemplate.RenderString(
                    new Swank.Web.Handlers.App.AppModel { Name = "Farker"})
                .ShouldEqual("Fark Farker");
        }

        [Test]
        public void should_set_app_template_from_virtual_path()
        {
            SetupTempFile((root, path, relativePath, nestedFolder) =>
            {
                File.WriteAllText(path, "Fark @Model.Name");
                _dsl.WithAppTemplateFromVirtualPath(relativePath);

                _configuration.AppTemplate.RenderString(
                    new Swank.Web.Handlers.App.AppModel { Name = "Farker" })
                    .ShouldEqual("Fark Farker");
            });
        }

        [Test]
        public void should_set_app_template_from_string()
        {
            _dsl.WithAppTemplate("Fark @Model.Name");

            _configuration.AppTemplate.RenderString(
                    new Swank.Web.Handlers.App.AppModel { Name = "Farker" })
                .ShouldEqual("Fark Farker");
        }

        [Test]
        public void should_set_app_url()
        {
            _dsl.WithAppAt("path/path");

            _configuration.AppUrl.ShouldEqual("path/path");
        }

        [Test]
        public void should_set_specification_url()
        {
            _dsl.WithSpecificationAtUrl("path/path");

            _configuration.SpecificationUrl.ShouldEqual("path/path");
        }

        [Test]
        public void should_set_favicon_url()
        {
            _dsl.WithFavIconAt("path/path");

            _configuration.FavIconUrl.ShouldEqual("path/path");
        }

        [Test]
        public void should_set_title()
        {
            _dsl.WithPageTitle("title");

            _configuration.Title.ShouldEqual("title");
        }

        [Test]
        public void should_set_name()
        {
            _dsl.WithHeader("name");

            _configuration.Name.ShouldEqual("name");
        }

        [Test]
        public void should_set_logo_url()
        {
            _dsl.WithLogoAt("logo");

            _configuration.LogoUrl.ShouldEqual("logo");
        }

        [Test]
        public void should_set_overview_from_resource()
        {
            _dsl.WithOverviewResource("ConfigurationDslTests/markdown.md");

            _configuration.Overview.ReadString()
                .ShouldEqual("<p><em>fark</em></p>");
        }

        [Test]
        public void should_set_overview_from_virtual_path()
        {
            SetupTempFile((root, path, relativePath, nestedFolder) =>
            {
                File.WriteAllText(path, "*fark*");
                _dsl.WithOverviewFromVirtualPath(relativePath);

                _configuration.Overview.ReadString()
                    .ShouldEqual("<p><em>fark</em></p>");
            });
        }

        [Test]
        public void should_set_overview_from_string()
        {
            _dsl.WithOverview("*fark*");

            _configuration.Overview.ReadString()
                .ShouldEqual("<p><em>fark</em></p>");
        }

        [Test]
        public void should_set_copyright()
        {
            _dsl.WithCopyright("copyright {year}");

            _configuration.Copyright.ShouldEqual("copyright " + DateTime.Now.Year);
        }

        [Test]
        public void should_load_template_from_resource()
        {
            _dsl.WithTemplateResource("ConfigurationDslTests/Templates/Nested/razor.cshtml", 
                "path2", "text/html");
            _configuration.AppUrl = "app/path1";

            _configuration.Templates.Count.ShouldEqual(1);

            var template = _configuration.Templates.First();

            var result = template.Render(new Swank.Specification.Module
                { Name = "Farker" }.AsList());

            result.ShouldEqual(result, "Fark Farker");
            template.GetUrl(_configuration).ShouldEqual("app/path1/path2");
            template.MimeType.ShouldEqual("text/html");
        }

        [Test]
        public void should_load_templates_from_resources()
        {
            _dsl.WithTemplateResources("Tests.Unit.Configuration.ConfigurationDslTests.Templates",
                "path2", "text/html");
            _configuration.AppUrl = "app/path1";

            _configuration.Templates.Count.ShouldEqual(2);

            var templates = _configuration.Templates;

            var template = templates.First();

            template.Render(new Swank.Specification.Module { Name = "Farker" }.AsList())
                .ShouldEqual("Fark Farker");
            template.GetUrl(_configuration).ShouldEqual("app/path1/path2/Nested/razor");
            template.MimeType.ShouldEqual("text/html");

            template = templates.Skip(1).First();

            template.Render(new Swank.Specification.Module { Name = "Farker" }.AsList())
                .ShouldEqual("Fark Farker");
            template.GetUrl(_configuration).ShouldEqual("app/path1/path2/Nested/template");
            template.MimeType.ShouldEqual("text/html");
        }

        [Test]
        public void should_load_template_from_virtual_path()
        {
            SetupTempFile((root, path, relativePath, nestedFolder) =>
            {
                File.WriteAllText(path, "Fark @Model[0].Name");
                _dsl.WithTemplateFromVirtualPath(relativePath, "path", "text/html");

                var template = _configuration.Templates.First();

                template.Render(new Swank.Specification.Module { Name = "Farker" }.AsList())
                    .ShouldEqual("Fark Farker");
                template.GetUrl(_configuration).ShouldEqual("api/path");
                template.MimeType.ShouldEqual("text/html");
            });
        }

        [Test]
        [TestCase(RazorTemplate.Extension, "Fark @Model[0].Name")]
        [TestCase(MustacheTemplate.Extension, "Fark {{#.}}{{Name}}{{/.}}")]
        public void should_load_templates_from_virtual_path(
            string extension, string templateContent)
        {
            SetupTempFile(extension, (root, path, relativePath, nestedFolder) =>
            {
                File.WriteAllText(path, templateContent);
                _dsl.WithTemplatesInVirtualPath(nestedFolder, "text/html", "url");

                _configuration.Templates.Count.ShouldBeGreaterThanOrEqualTo(1);
                var template = _configuration.Templates
                    .FirstOrDefault(x => x.GetUrl(_configuration)
                    .Contains(path.GetFileNameWithoutExtension()));

                template.ShouldNotBeNull();
                template.Render(new Swank.Specification.Module { Name = "Farker" }.AsList())
                    .ShouldEqual("Fark Farker");
                template.GetUrl(_configuration).ShouldEqual("api/url/nested"
                    .CombineUrls(relativePath.GetFileNameWithoutExtension()));
                template.MimeType.ShouldEqual("text/html");
            });
        }

        [Test]
        public void should_load_razor_template_from_string()
        {
            _dsl.WithRazorTemplate("url", "Fark @Model[0].Name", "text/html");

            var template = _configuration.Templates.First();

            template.Render(new Swank.Specification.Module { Name = "Farker" }.AsList())
                .ShouldEqual("Fark Farker");
            template.GetUrl(_configuration).ShouldEqual("api/url");
            template.MimeType.ShouldEqual("text/html");
        }

        [Test]
        public void should_load_mustache_template_from_string()
        {
            _dsl.WithMustacheTemplate("url", "text/html", "Fark {{Name}}");

            var template = _configuration.Templates.First();
            var result = template.Render(new Swank.Specification
                .Module { Name = "Farker" });

            result.ShouldEqual(result, "Fark Farker");
            template.GetUrl(_configuration).ShouldEqual("api/url");
            template.MimeType.ShouldEqual("text/html");
        }

        [Test]
        public void should_add_stylesheets()
        {
            _dsl.WithStylesheets("url1/stylesheet1.css", "url2/stylesheet2.css");

            _configuration.Stylesheets.Count.ShouldBeGreaterThanOrEqualTo(2);

            var stylesheet = _configuration.Stylesheets
                .First(x => x.Filename == "stylesheet1.css");
            stylesheet.MimeType.ShouldEqual("text/css");
            stylesheet.GetUrl().ShouldEqual("url1/stylesheet1.css");

            stylesheet = _configuration.Stylesheets
                .First(x => x.Filename == "stylesheet2.css");
            stylesheet.MimeType.ShouldEqual("text/css");
            stylesheet.GetUrl().ShouldEqual("url2/stylesheet2.css");
        }

        [Test]
        public void should_add_scripts()
        {
            _dsl.WithScripts("url1/script1.js", "url2/script2.js");

            _configuration.Stylesheets.Count.ShouldBeGreaterThanOrEqualTo(2);

            var script = _configuration.Scripts
                .First(x => x.Filename == "script1.js");
            script.MimeType.ShouldEqual("application/javascript");
            script.GetUrl().ShouldEqual("url1/script1.js");

            script = _configuration.Scripts
                .First(x => x.Filename == "script2.js");
            script.MimeType.ShouldEqual("application/javascript");
            script.GetUrl().ShouldEqual("url2/script2.js");
        }

        [Test]
        public void should_set_hide_json()
        {
            _dsl.HideJsonData();

            _configuration.DisplayJsonData.ShouldBeFalse();
        }

        [Test]
        public void should_set_hide_xml()
        {
            _dsl.HideXmlData();

            _configuration.DisplayXmlData.ShouldBeFalse();
        }

        [Test]
        [TestCase("url/path1", true)]
        [TestCase("url/path2", false)]
        public void should_filter_api_descriptions(
            string route, bool matches)
        {
            _dsl.Where(x => x.Route.RouteTemplate == route);

            _configuration.Filter(new ApiDescription
            {
                Route = new HttpRoute("url/path1")
            }).ShouldEqual(matches);
        }

        [Test]
        public void should_set_default_dictionary_key_name()
        {
            _dsl.WithDefaultDictionaryKeyName("key");

            _configuration.DefaultDictionaryKeyName.ShouldEqual("key");
        }

        [Test]
        public void should_set_numeric_enum()
        {
            _dsl.WithNumericEnum();

            _configuration.EnumFormat.ShouldEqual(EnumFormat.AsNumber);
        }

        [Test]
        public void should_set_code_example_theme()
        {
            var stylesheets = _configuration.Stylesheets;
            stylesheets.Any(x => x.Filename == Swank.Configuration.Configuration
                .DefaultCodeExampleTheme).ShouldBeTrue();

            _dsl.WithCodeExampleTheme(CodeExampleTheme.Arta);

            stylesheets.Any(x => x.Filename == Swank.Configuration.Configuration
                .DefaultCodeExampleTheme).ShouldBeFalse();

            var appStylesheetIndex = stylesheets.IndexOf(stylesheets
                .FindByFilename(Swank.Configuration.Configuration.AppStylesheet));
            var themeStylesheet = stylesheets.FindByFilename("arta.css");
            var themeStylesheetIndex = stylesheets.IndexOf(themeStylesheet);

            appStylesheetIndex.ShouldBeGreaterThan(0);
            themeStylesheetIndex.ShouldBeGreaterThan(0);
            appStylesheetIndex.ShouldBeGreaterThan(themeStylesheetIndex);

            themeStylesheet.Filename.ShouldEqual("arta.css");
            themeStylesheet.MimeType.ShouldEqual("text/css");
            themeStylesheet.GetUrl().ShouldEqual("/api/css/highlight/arta.css/");
        }

        [Test]
        [TestCase("csharp", "C#", "C#")]
        [TestCase("csharp", null, "C#")]
        [TestCase("curl", "Curl", "Curl")]
        [TestCase("curl", null, "Curl")]
        public void should_load_code_example_from_resource(
            string filename, string name, string comments)
        {
            _dsl.WithCodeExampleResource("ConfigurationDslTests/CodeExamples/" + filename,
                name, CodeExampleLanguage.Ada);

            _configuration.CodeExamples.Count.ShouldEqual(1);

            var example = _configuration.CodeExamples.First();

            example.Render(new CodeExampleModel { Name = "Farker" })
                .ShouldEqual("Fark Farker");
            example.Language.ShouldEqual("ada");
            example.Comments.ShouldEqual($"<p>{comments}</p>");
            example.Name.ShouldEqual(name ?? filename);
        }

        [Test]
        public void should_load_code_example_from_resources()
        {
            _dsl.WithCodeExampleResources("Tests/Unit/Configuration/ConfigurationDslTests/CodeExamples");

            _configuration.CodeExamples.Count.ShouldEqual(2);

            var example = _configuration.CodeExamples.First();

            example.Render(new CodeExampleModel { Name = "Farker" })
                .ShouldEqual("Fark Farker");
            example.Language.ShouldEqual("curl");
            example.Comments.ShouldEqual("<p>Curl</p>");
            example.Name.ShouldEqual("curl");

            example = _configuration.CodeExamples.Skip(1).First();

            example.Render(new CodeExampleModel { Name = "Farker" })
                .ShouldEqual("Fark Farker");
            example.Language.ShouldEqual("csharp");
            example.Comments.ShouldEqual("<p>C#</p>");
            example.Name.ShouldEqual("csharp");
        }

        [Test]
        [TestCase("csharp", ".cshtml", "C#", "C#", "Fark @Model.Name")]
        [TestCase("csharp", ".cshtml", null, "C#", "Fark @Model.Name")]
        [TestCase("curl", ".mustache", "Curl", "Curl", "Fark {{Name}}")]
        [TestCase("curl", ".mustache", null, "Curl", "Fark {{Name}}")]
        public void should_load_code_example_from_virtual_path(string filename, 
            string extension, string name, string comments, string template)
        {
            var tempPath = Path.GetTempPath();
            var examplePath = tempPath.Combine(Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(examplePath);

            File.WriteAllText(examplePath.Combine(filename + extension), template);
            File.WriteAllText(examplePath.Combine(filename + ".md"), comments);

            _dsl.WithCodeExampleFromVirtualPath(examplePath.Combine(filename), 
                name, CodeExampleLanguage.Ada);

            var example = _configuration.CodeExamples.First();

            example.Render(new CodeExampleModel { Name = "Farker" })
                .ShouldEqual("Fark Farker");
            example.Language.ShouldEqual("ada");
            example.Comments.ShouldEqual($"<p>{comments}</p>");
            example.Name.ShouldEqual(name ?? filename);
        }

        [Test]
        public void should_load_code_examples_in_virtual_path()
        {
            var tempPath = Path.GetTempPath();
            var relativePath = Guid.NewGuid().ToString("N");
            var examplePath = tempPath.Combine(relativePath);
            Directory.CreateDirectory(examplePath);

            File.WriteAllText(examplePath.Combine("csharp.cshtml"), "Fark @Model.Name");
            File.WriteAllText(examplePath.Combine("csharp.md"), "C#");
            File.WriteAllText(examplePath.Combine("curl.mustache"), "Fark {{Name}}");
            File.WriteAllText(examplePath.Combine("curl.md"), "Curl");

            _dsl.WithCodeExamplesInVirtualPath(relativePath);

            _configuration.CodeExamples.Count.ShouldEqual(2);

            var example = _configuration.CodeExamples.First(x => x.Name == "csharp");

            example.Render(new CodeExampleModel { Name = "Farker" })
                .ShouldEqual("Fark Farker");
            example.Language.ShouldEqual("csharp");
            example.Comments.ShouldEqual("<p>C#</p>");

            example = _configuration.CodeExamples.First(x => x.Name == "curl");

            example.Render(new CodeExampleModel { Name = "Farker" })
                .ShouldEqual("Fark Farker");
            example.Language.ShouldEqual("curl");
            example.Comments.ShouldEqual("<p>Curl</p>");
        }

        [Test]
        public void should_load_razor_code_example_from_string()
        {
            _dsl.WithRazorCodeExample("name", CodeExampleLanguage.Ada, 
                "comments", "Fark @Model.Name");

            var example = _configuration.CodeExamples.First();

            example.Render(new CodeExampleModel { Name = "Farker" })
                .ShouldEqual("Fark Farker");
            example.Language.ShouldEqual("ada");
            example.Name.ShouldEqual("name");
            example.Comments.ShouldEqual("<p>comments</p>");
        }

        [Test]
        public void should_load_mustache_code_example_from_string()
        {
            _dsl.WithMustacheCodeExample("name", CodeExampleLanguage.Ada, 
                "comments", "Fark {{Name}}");

            var example = _configuration.CodeExamples.First();

            example.Render(new CodeExampleModel { Name = "Farker" })
                .ShouldEqual("Fark Farker");
            example.Language.ShouldEqual("ada");
            example.Name.ShouldEqual("name");
            example.Comments.ShouldEqual("<p>comments</p>");
        }

        [Test]
        public void should_set_sample_datetime_format()
        {
            _dsl.WithSampleDateTimeFormat("format");

            _configuration.SampleDateTimeFormat.ShouldEqual("format");
        }

        [Test]
        public void should_set_sample_integer_format()
        {
            _dsl.WithSampleIntegerFormat("format");

            _configuration.SampleIntegerFormat.ShouldEqual("format");
        }

        [Test]
        public void should_set_sample_real_format()
        {
            _dsl.WithSampleRealFormat("format");

            _configuration.SampleRealFormat.ShouldEqual("format");
        }

        [Test]
        public void should_set_sample_timespan_format()
        {
            _dsl.WithSampleTimeSpanFormat("format");

            _configuration.SampleTimeSpanFormat.ShouldEqual("format");
        }

        [Test]
        public void should_set_sample_guid_format()
        {
            _dsl.WithSampleGuidFormat("format");

            _configuration.SampleGuidFormat.ShouldEqual("format");
        }

        [Test]
        public void should_set_sample_string_value()
        {
            _dsl.WithSampleStringValue("format");

            _configuration.SampleStringValue.ShouldEqual("format");
        }

        [Test]
        public void should_set_sample_bool_value()
        {
            _dsl.WithSampleBoolValue(true);

            _configuration.SampleBoolValue.ShouldEqual(true);
        }

        [Test]
        public void should_set_sample_datetime_value()
        {
            _dsl.WithSampleDateTimeValue(DateTime.MaxValue);

            _configuration.SampleDateTimeValue.ShouldEqual(DateTime.MaxValue);
        }

        [Test]
        public void should_set_sample_integer_value()
        {
            _dsl.WithSampleIntegerValue(5);

            _configuration.SampleIntegerValue.ShouldEqual(5);
        }

        [Test]
        public void should_set_sample_real_value()
        {
            _dsl.WithSampleRealValue(5.5m);

            _configuration.SampleRealValue.ShouldEqual(5.5m);
        }

        [Test]
        public void should_set_sample_timespan_value()
        {
            _dsl.WithSampleTimeSpanValue(TimeSpan.MaxValue);

            _configuration.SampleTimeSpanValue.ShouldEqual(TimeSpan.MaxValue);
        }

        [Test]
        public void should_set_sample_guid_value()
        {
            var guid = Guid.NewGuid();
            _dsl.WithSampleGuidValue(guid);

            _configuration.SampleGuidValue.ShouldEqual(guid);
        }

        [Test]
        public void should_set_module_convention()
        {
            _dsl.WithModuleConvention<ModuleConvention>();

            var convention = _configuration.ModuleConvention;
            convention.ShouldNotBeNull();
            convention.Type.ShouldEqual(typeof(ModuleConvention));
            convention.Config.ShouldBeNull();
        }

        public class ServiceConfig { public string Value { get; set; } }

        [Test]
        public void should_set_module_convention_with_configuration()
        {
            _dsl.WithModuleConvention<ModuleConvention, ServiceConfig>(x => x.Value = "fark");

            var convention = _configuration.ModuleConvention;
            convention.ShouldNotBeNull();
            convention.Type.ShouldEqual(typeof(ModuleConvention));
            convention.Config.ShouldBeType<ServiceConfig>();
            ((ServiceConfig)convention.Config).Value.ShouldEqual("fark");
        }

        [Test]
        public void should_set_resource_convention()
        {
            _dsl.WithResourceConvention<ResourceConvention>();

            var convention = _configuration.ResourceConvention;
            convention.ShouldNotBeNull();
            convention.Type.ShouldEqual(typeof(ResourceConvention));
            convention.Config.ShouldBeNull();
        }

        [Test]
        public void should_set_resource_convention_with_configuration()
        {
            _dsl.WithResourceConvention<ResourceConvention, ServiceConfig>(x => x.Value = "fark");

            var convention = _configuration.ResourceConvention;
            convention.ShouldNotBeNull();
            convention.Type.ShouldEqual(typeof(ResourceConvention));
            convention.Config.ShouldBeType<ServiceConfig>();
            ((ServiceConfig)convention.Config).Value.ShouldEqual("fark");
        }

        [Test]
        public void should_set_endpoint_convention()
        {
            _dsl.WithEndpointConvention<EndpointConvention>();

            var convention = _configuration.EndpointConvention;
            convention.ShouldNotBeNull();
            convention.Type.ShouldEqual(typeof(EndpointConvention));
            convention.Config.ShouldBeNull();
        }

        [Test]
        public void should_set_endpoint_convention_with_configuration()
        {
            _dsl.WithEndpointConvention<EndpointConvention, ServiceConfig>(x => x.Value = "fark");

            var convention = _configuration.EndpointConvention;
            convention.ShouldNotBeNull();
            convention.Type.ShouldEqual(typeof(EndpointConvention));
            convention.Config.ShouldBeType<ServiceConfig>();
            ((ServiceConfig)convention.Config).Value.ShouldEqual("fark");
        }

        [Test]
        public void should_set_member_convention()
        {
            _dsl.WithMemberConvention<MemberConvention>();

            var convention = _configuration.MemberConvention;
            convention.ShouldNotBeNull();
            convention.Type.ShouldEqual(typeof(MemberConvention));
            convention.Config.ShouldBeNull();
        }

        [Test]
        public void should_set_member_convention_with_configuration()
        {
            _dsl.WithMemberConvention<MemberConvention, ServiceConfig>(x => x.Value = "fark");

            var convention = _configuration.MemberConvention;
            convention.ShouldNotBeNull();
            convention.Type.ShouldEqual(typeof(MemberConvention));
            convention.Config.ShouldBeType<ServiceConfig>();
            ((ServiceConfig)convention.Config).Value.ShouldEqual("fark");
        }

        [Test]
        public void should_set_enum_convention()
        {
            _dsl.WithEnumConvention<EnumConvention>();

            var convention = _configuration.EnumConvention;
            convention.ShouldNotBeNull();
            convention.Type.ShouldEqual(typeof(EnumConvention));
            convention.Config.ShouldBeNull();
        }

        [Test]
        public void should_set_enum_convention_with_configuration()
        {
            _dsl.WithEnumConvention<EnumConvention, ServiceConfig>(x => x.Value = "fark");

            var convention = _configuration.EnumConvention;
            convention.ShouldNotBeNull();
            convention.Type.ShouldEqual(typeof(EnumConvention));
            convention.Config.ShouldBeType<ServiceConfig>();
            ((ServiceConfig)convention.Config).Value.ShouldEqual("fark");
        }

        [Test]
        public void should_set_enum_option_convention()
        {
            _dsl.WithEnumOptionConvention<OptionConvention>();

            var convention = _configuration.EnumOptionConvention;
            convention.ShouldNotBeNull();
            convention.Type.ShouldEqual(typeof(OptionConvention));
            convention.Config.ShouldBeNull();
        }

        [Test]
        public void should_set_enum_option_convention_with_configuration()
        {
            _dsl.WithEnumOptionConvention<OptionConvention, ServiceConfig>(x => x.Value = "fark");

            var convention = _configuration.EnumOptionConvention;
            convention.ShouldNotBeNull();
            convention.Type.ShouldEqual(typeof(OptionConvention));
            convention.Config.ShouldBeType<ServiceConfig>();
            ((ServiceConfig)convention.Config).Value.ShouldEqual("fark");
        }

        [Test]
        public void should_set_status_code_convention()
        {
            _dsl.WithStatusCodeConvention<StatusCodeConvention>();

            var convention = _configuration.StatusCodeConvention;
            convention.ShouldNotBeNull();
            convention.Type.ShouldEqual(typeof(StatusCodeConvention));
            convention.Config.ShouldBeNull();
        }

        [Test]
        public void should_set_status_code_convention_with_configuration()
        {
            _dsl.WithStatusCodeConvention<StatusCodeConvention, ServiceConfig>(x => x.Value = "fark");

            var convention = _configuration.StatusCodeConvention;
            convention.ShouldNotBeNull();
            convention.Type.ShouldEqual(typeof(StatusCodeConvention));
            convention.Config.ShouldBeType<ServiceConfig>();
            ((ServiceConfig)convention.Config).Value.ShouldEqual("fark");
        }

        [Test]
        public void should_set_header_convention()
        {
            _dsl.WithHeaderConvention<HeaderConvention>();

            var convention = _configuration.HeaderConvention;
            convention.ShouldNotBeNull();
            convention.Type.ShouldEqual(typeof(HeaderConvention));
            convention.Config.ShouldBeNull();
        }

        [Test]
        public void should_set_header_convention_with_configuration()
        {
            _dsl.WithHeaderConvention<HeaderConvention, ServiceConfig>(x => x.Value = "fark");

            var convention = _configuration.HeaderConvention;
            convention.ShouldNotBeNull();
            convention.Type.ShouldEqual(typeof(HeaderConvention));
            convention.Config.ShouldBeType<ServiceConfig>();
            ((ServiceConfig)convention.Config).Value.ShouldEqual("fark");
        }

        [Test]
        public void should_set_type_convention()
        {
            _dsl.WithTypeConvention<TypeConvention>();

            var convention = _configuration.TypeConvention;
            convention.ShouldNotBeNull();
            convention.Type.ShouldEqual(typeof(TypeConvention));
            convention.Config.ShouldBeNull();
        }

        [Test]
        public void should_set_type_convention_with_configuration()
        {
            _dsl.WithTypeConvention<TypeConvention, ServiceConfig>(x => x.Value = "fark");

            var convention = _configuration.TypeConvention;
            convention.ShouldNotBeNull();
            convention.Type.ShouldEqual(typeof(TypeConvention));
            convention.Config.ShouldBeType<ServiceConfig>();
            ((ServiceConfig)convention.Config).Value.ShouldEqual("fark");
        }

        [Test]
        public void should_set_default_module_factory()
        {
            _dsl.WithDefaultModuleName("fark");

            _configuration.DefaultModuleName.ShouldEqual("fark");
        }

        [Test]
        public void should_set_orphaned_module_action()
        {
            _dsl.WhenModuleOrphaned(OrphanedEndpoints.Fail);

            _configuration.OrphanedModuleEndpoint.ShouldEqual(OrphanedEndpoints.Fail);
        }

        [Test]
        public void should_set_default_resource_factory()
        {
            Func<ApiDescription, ResourceDescription> factory = x => null;

            _dsl.WithDefaultResource(factory);

            _configuration.DefaultResourceFactory.ShouldEqual(factory);
        }

        [Test]
        public void should_set_orphaned_resource_action()
        {
            _dsl.WhenResourceOrphaned(OrphanedEndpoints.Fail);

            _configuration.OrphanedResourceEndpoint.ShouldEqual(OrphanedEndpoints.Fail);
        }

        [Test]
        public void should_add_module_override()
        {
            Action<ModuleOverrideContext> @override = x => {};

            _dsl.OverrideModules(@override);

            _configuration.ModuleOverrides.Count.ShouldEqual(1);
            _configuration.ModuleOverrides.ShouldContain(@override);
        }

        [Test]
        [TestCase("fark", "oh hai")]
        [TestCase("farker", null)]
        public void should_add_module_override_with_predicate(
            string name, string comments)
        {
            _dsl.OverrideModulesWhen(
                x => x.Module.Comments = "oh hai", 
                x => x.Module.Name == name);

            _configuration.ModuleOverrides.Count.ShouldEqual(1);

            var module = new Swank.Specification.Module { Name = "fark" };

            _configuration.ModuleOverrides.First()(new ModuleOverrideContext
            {
                Module = module
            });

            module.Name.ShouldEqual("fark");
            module.Comments.ShouldEqual(comments);
        }

        [Test]
        public void should_add_resource_override()
        {
            Action<ResourceOverrideContext> @override = x => { };

            _dsl.OverrideResources(@override);

            _configuration.ResourceOverrides.Count.ShouldEqual(1);
            _configuration.ResourceOverrides.ShouldContain(@override);
        }

        [Test]
        [TestCase("fark", "oh hai")]
        [TestCase("farker", null)]
        public void should_add_resource_override_with_predicate(
            string name, string comments)
        {
            _dsl.OverrideResourcesWhen(
                x => x.Resource.Comments = "oh hai", 
                x => x.Resource.Name == name);

            _configuration.ResourceOverrides.Count.ShouldEqual(1);

            var resource = new Resource { Name = "fark" };

            _configuration.ResourceOverrides.First()(new ResourceOverrideContext
            {
                Resource = resource
            });

            resource.Name.ShouldEqual("fark");
            resource.Comments.ShouldEqual(comments);
        }

        [Test]
        public void should_add_endpoint_override()
        {
            Action<EndpointOverrideContext> @override = x => { };

            _dsl.OverrideEndpoints(@override);

            _configuration.EndpointOverrides.Count.ShouldEqual(1);
            _configuration.EndpointOverrides.ShouldContain(@override);
        }

        [Test]
        [TestCase("fark", "oh hai")]
        [TestCase("farker", null)]
        public void should_add_endpoint_override_with_predicate(
            string name, string comments)
        {
            _dsl.OverrideEndpointsWhen(e => e.Endpoint.Comments = "oh hai", 
                e => e.Endpoint.Name == name);

            _configuration.EndpointOverrides.Count.ShouldEqual(1);

            var endpoint = new Endpoint { Name = "fark" };

            _configuration.EndpointOverrides.First()(new EndpointOverrideContext
            {
                Endpoint =  endpoint
            });

            endpoint.Name.ShouldEqual("fark");
            endpoint.Comments.ShouldEqual(comments);
        }

        [Test]
        public void should_add_url_parameters_override()
        {
            Action<UrlParameterOverrideContext> @override = x => { };

            _dsl.OverrideUrlParameters(@override);

            _configuration.UrlParameterOverrides.Count.ShouldEqual(1);
            _configuration.UrlParameterOverrides.ShouldContain(@override);
        }

        [Test]
        [TestCase("fark", "oh hai")]
        [TestCase("farker", null)]
        public void should_add_url_parameters_override_with_predicate(
            string name, string comments)
        {
            _dsl.OverrideUrlParametersWhen(e => e.UrlParameter.Comments = "oh hai",
                e => e.UrlParameter.Name == name);

            _configuration.UrlParameterOverrides.Count.ShouldEqual(1);

            var urlParameter = new UrlParameter { Name = "fark" };

            _configuration.UrlParameterOverrides.First()(new UrlParameterOverrideContext
            {
                UrlParameter =  urlParameter
            });

            urlParameter.Name.ShouldEqual("fark");
            urlParameter.Comments.ShouldEqual(comments);
        }

        [Test]
        public void should_add_querystring_override()
        {
            Action<QuerystringOverrideContext> @override = x => { };

            _dsl.OverrideQuerystring(@override);

            _configuration.QuerystringOverrides.Count.ShouldEqual(1);
            _configuration.QuerystringOverrides.ShouldContain(@override);
        }

        [Test]
        [TestCase("fark", "oh hai")]
        [TestCase("farker", null)]
        public void should_add_querystring_override_with_predicate(
            string name, string comments)
        {
            _dsl.OverrideQuerystringWhen(
                e => e.Querystring.Comments = "oh hai",
                e => e.Querystring.Name == name);

            _configuration.QuerystringOverrides.Count.ShouldEqual(1);

            var querystring = new QuerystringParameter { Name = "fark" };

            _configuration.QuerystringOverrides.First()(new QuerystringOverrideContext
            {
                Querystring = querystring
            });

            querystring.Name.ShouldEqual("fark");
            querystring.Comments.ShouldEqual(comments);
        }

        [Test]
        public void should_add_parameter_override()
        {
            _dsl.OverrideParameters(e => e.Parameter.Comments = "oh hai");

            _configuration.UrlParameterOverrides.Count.ShouldEqual(1);

            var urlParameter = new UrlParameter { Name = "fark" };

            _configuration.UrlParameterOverrides.First()(new UrlParameterOverrideContext
            {
                UrlParameter = urlParameter
            });

            urlParameter.Name.ShouldEqual("fark");
            urlParameter.Comments.ShouldEqual("oh hai");

            _configuration.QuerystringOverrides.Count.ShouldEqual(1);

            var querystring = new QuerystringParameter { Name = "fark" };

            _configuration.QuerystringOverrides.First()(new QuerystringOverrideContext
            {
                Querystring = querystring
            });

            querystring.Name.ShouldEqual("fark");
            querystring.Comments.ShouldEqual("oh hai");
        }

        [Test]
        [TestCase("fark", "oh hai")]
        [TestCase("farker", null)]
        public void should_add_parameter_override_with_predicate(
            string name, string comments)
        {
            _dsl.OverrideParametersWhen(
                e => e.Parameter.Comments = "oh hai",
                e => e.Parameter.Name == name);

            _configuration.UrlParameterOverrides.Count.ShouldEqual(1);

            var urlParameter = new UrlParameter { Name = "fark" };

            _configuration.UrlParameterOverrides.First()(new UrlParameterOverrideContext
            {
                UrlParameter = urlParameter
            });

            urlParameter.Name.ShouldEqual("fark");
            urlParameter.Comments.ShouldEqual(comments);

            _configuration.QuerystringOverrides.Count.ShouldEqual(1);

            var querystring = new QuerystringParameter { Name = "fark" };

            _configuration.QuerystringOverrides.First()(new QuerystringOverrideContext
            {
                Querystring = querystring
            });

            querystring.Name.ShouldEqual("fark");
            querystring.Comments.ShouldEqual(comments);
        }

        [Test]
        public void should_add_status_code_override()
        {
            Action<StatusCodeOverrideContext> @override = x => { };

            _dsl.OverrideStatusCodes(@override);

            _configuration.StatusCodeOverrides.Count.ShouldEqual(1);
            _configuration.StatusCodeOverrides.ShouldContain(@override);
        }

        [Test]
        [TestCase("fark", "oh hai")]
        [TestCase("farker", null)]
        public void should_add_status_code_override_with_predicate(
            string name, string comments)
        {
            _dsl.OverrideStatusCodesWhen(
                e => e.StatusCode.Comments = "oh hai",
                e => e.StatusCode.Name == name);

            _configuration.StatusCodeOverrides.Count.ShouldEqual(1);

            var statusCode = new StatusCode { Name = "fark" };

            _configuration.StatusCodeOverrides.First()(new StatusCodeOverrideContext
            {
                StatusCode = statusCode
            });

            statusCode.Name.ShouldEqual("fark");
            statusCode.Comments.ShouldEqual(comments);
        }

        [Test]
        public void should_add_header_override()
        {
            Action<HeaderOverrideContext> @override = x => { };

            _dsl.OverrideHeaders(@override);

            _configuration.RequestHeaderOverrides.Count.ShouldEqual(1);
            _configuration.RequestHeaderOverrides.ShouldContain(@override);

            _configuration.ResponseHeaderOverrides.Count.ShouldEqual(1);
            _configuration.ResponseHeaderOverrides.ShouldContain(@override);
        }

        [Test]
        [TestCase("fark", "oh hai")]
        [TestCase("farker", null)]
        public void should_add_header_override_with_predicate(
            string name, string comments)
        {
            _dsl.OverrideHeadersWhen(
                e => e.Header.Comments = "oh hai",
                e => e.Header.Name == name);

            _configuration.RequestHeaderOverrides.Count.ShouldEqual(1);

            var header = new Header { Name = "fark" };

            _configuration.RequestHeaderOverrides.First()(new HeaderOverrideContext
            {
                Header = header
            });

            header.Name.ShouldEqual("fark");
            header.Comments.ShouldEqual(comments);

            _configuration.ResponseHeaderOverrides.Count.ShouldEqual(1);

            header = new Header { Name = "fark" };

            _configuration.ResponseHeaderOverrides.First()(new HeaderOverrideContext
            {
                Header = header
            });

            header.Name.ShouldEqual("fark");
            header.Comments.ShouldEqual(comments);
        }

        [Test]
        public void should_add_request_header_override()
        {
            Action<HeaderOverrideContext> @override = x => { };

            _dsl.OverrideRequestHeaders(@override);

            _configuration.RequestHeaderOverrides.Count.ShouldEqual(1);
            _configuration.RequestHeaderOverrides.ShouldContain(@override);

            _configuration.ResponseHeaderOverrides.ShouldBeEmpty();
        }

        [Test]
        [TestCase("fark", "oh hai")]
        [TestCase("farker", null)]
        public void should_add_request_header_override_with_predicate(
            string name, string comments)
        {
            _dsl.OverrideRequestHeadersWhen(
                e => e.Header.Comments = "oh hai",
                e => e.Header.Name == name);

            _configuration.RequestHeaderOverrides.Count.ShouldEqual(1);

            var header = new Header { Name = "fark" };

            _configuration.RequestHeaderOverrides.First()(new HeaderOverrideContext
            {
                Header = header
            });

            header.Name.ShouldEqual("fark");
            header.Comments.ShouldEqual(comments);

            _configuration.ResponseHeaderOverrides.ShouldBeEmpty();
        }

        [Test]
        public void should_add_response_header_override()
        {
            Action<HeaderOverrideContext> @override = x => { };

            _dsl.OverrideResponseHeaders(@override);

            _configuration.RequestHeaderOverrides.ShouldBeEmpty();

            _configuration.ResponseHeaderOverrides.Count.ShouldEqual(1);
            _configuration.ResponseHeaderOverrides.ShouldContain(@override);
        }

        [Test]
        [TestCase("fark", "oh hai")]
        [TestCase("farker", null)]
        public void should_add_response_header_override_with_predicate(
            string name, string comments)
        {
            _dsl.OverrideResponseHeadersWhen(
                e => e.Header.Comments = "oh hai",
                e => e.Header.Name == name);

            _configuration.RequestHeaderOverrides.ShouldBeEmpty();

            _configuration.ResponseHeaderOverrides.Count.ShouldEqual(1);

            var header = new Header { Name = "fark" };

            _configuration.ResponseHeaderOverrides.First()(new HeaderOverrideContext
            {
                Header = header
            });

            header.Name.ShouldEqual("fark");
            header.Comments.ShouldEqual(comments);
        }

        [Test]
        public void should_add_request_override()
        {
            Action<MessageOverrideContext> @override = x => { };

            _dsl.OverrideRequest(@override);

            _configuration.ResponseOverrides.ShouldBeEmpty();

            _configuration.RequestOverrides.Count.ShouldEqual(1);
            _configuration.RequestOverrides.ShouldContain(@override);
        }

        [Test]
        [TestCase("fark", "fark farker")]
        [TestCase("farker", "fark")]
        public void should_add_request_override_with_predicate(
            string commentMatches, string comments)
        {
            _dsl.OverrideRequestWhen(
                e => e.Message.Comments += " farker",
                e => e.Message.Comments == commentMatches);

            _configuration.ResponseOverrides.ShouldBeEmpty();

            _configuration.RequestOverrides.Count.ShouldEqual(1);

            var data = new Message { Comments = "fark" };

            _configuration.RequestOverrides.First()(new MessageOverrideContext
            {
                Message = data
            });
            
            data.Comments.ShouldEqual(comments);
        }

        [Test]
        public void should_add_response_override()
        {
            Action<MessageOverrideContext> @override = x => { };

            _dsl.OverrideResponse(@override);

            _configuration.RequestOverrides.ShouldBeEmpty();

            _configuration.ResponseOverrides.Count.ShouldEqual(1);
            _configuration.ResponseOverrides.ShouldContain(@override);
        }

        [Test]
        [TestCase("fark", "fark farker")]
        [TestCase("farker", "fark")]
        public void should_add_response_override_with_predicate(
            string commentMatches, string comments)
        {
            _dsl.OverrideResponseWhen(
                e => e.Message.Comments += " farker",
                e => e.Message.Comments == commentMatches);

            _configuration.RequestOverrides.ShouldBeEmpty();

            _configuration.ResponseOverrides.Count.ShouldEqual(1);

            var data = new Message { Comments = "fark" };

            _configuration.ResponseOverrides.First()(new MessageOverrideContext
            {
                Message = data
            });

            data.Comments.ShouldEqual(comments);
        }

        [Test]
        public void should_add_data_override()
        {
            Action< MessageOverrideContext> @override = x => { };

            _dsl.OverrideMessage(@override);

            _configuration.RequestOverrides.Count.ShouldEqual(1);
            _configuration.RequestOverrides.ShouldContain(@override);

            _configuration.ResponseOverrides.Count.ShouldEqual(1);
            _configuration.ResponseOverrides.ShouldContain(@override);
        }

        [Test]
        [TestCase("fark", "fark farker")]
        [TestCase("farker", "fark")]
        public void should_add_data_override_with_predicate(
            string commentMatches, string comments)
        {
            _dsl.OverrideMessageWhen(
                e => e.Message.Comments += " farker",
                e => e.Message.Comments == commentMatches);

            _configuration.RequestOverrides.Count.ShouldEqual(1);

            var data = new Message { Comments = "fark" };

            _configuration.RequestOverrides.First()(new MessageOverrideContext
            {
                Message = data
            });

            data.Comments.ShouldEqual(comments);

            _configuration.ResponseOverrides.Count.ShouldEqual(1);

            data = new Message { Comments = "fark" };

            _configuration.ResponseOverrides.First()(new MessageOverrideContext
            {
                Message = data
            });

            data.Comments.ShouldEqual(comments);
        }

        [Test]
        public void should_add_type_override()
        {
            Action<TypeOverrideContext> @override = x => { };

            _dsl.OverrideTypes(@override);

            _configuration.TypeOverrides.Count.ShouldEqual(1);
            _configuration.TypeOverrides.ShouldContain(@override);
        }

        [Test]
        [TestCase("fark", "oh hai")]
        [TestCase("farker", null)]
        public void should_add_type_override_with_predicate(
            string name, string comments)
        {
            _dsl.OverrideTypesWhen(
                e => e.DataType.Comments = "oh hai",
                e => e.DataType.Name == name);

            _configuration.TypeOverrides.Count.ShouldEqual(1);

            var dataType = new DataType { Name = "fark" };

            _configuration.TypeOverrides.First()(new TypeOverrideContext
            {
                DataType = dataType
            });

            dataType.Name.ShouldEqual("fark");
            dataType.Comments.ShouldEqual(comments);
        }

        [Test]
        public void should_add_member_override()
        {
            Action<MemberOverrideContext> @override = x => { };

            _dsl.OverrideMembers(@override);

            _configuration.MemberOverrides.Count.ShouldEqual(1);
            _configuration.MemberOverrides.ShouldContain(@override);
        }

        [Test]
        [TestCase("fark", "oh hai")]
        [TestCase("farker", null)]
        public void should_add_member_override_with_predicate(
            string name, string comments)
        {
            _dsl.OverrideMembersWhen(
                e => e.Member.Comments = "oh hai",
                e => e.Member.Name == name);

            _configuration.MemberOverrides.Count.ShouldEqual(1);

            var member = new Member { Name = "fark" };

            _configuration.MemberOverrides.First()(new MemberOverrideContext
            {
                Member = member
            });

            member.Name.ShouldEqual("fark");
            member.Comments.ShouldEqual(comments);
        }

        [Test]
        public void should_add_option_override()
        {
            Action<OptionOverrideContext> @override = x => { };

            _dsl.OverrideOptions(@override);

            _configuration.OptionOverrides.Count.ShouldEqual(1);
            _configuration.OptionOverrides.ShouldContain(@override);
        }

        [Test]
        [TestCase("fark", "oh hai")]
        [TestCase("farker", null)]
        public void should_add_option_override_with_predicate(
            string name, string comments)
        {
            _dsl.OverrideOptionsWhen(e => e.Option.Comments = "oh hai",
                e => e.Option.Name == name);

            _configuration.OptionOverrides.Count.ShouldEqual(1);
            
            var option = new Option { Name = "fark" };

            _configuration.OptionOverrides.First()(new OptionOverrideContext
            {
                Option = option
            });

            option.Name.ShouldEqual("fark");
            option.Comments.ShouldEqual(comments);
        }

        public void SetupTempFile(Action<string, string, string, string> test)
        {
            SetupTempFile(null, test);
        }

        public void SetupTempFile(string extension, Action<string, string, string, string> test)
        {
            var tempFolder = Guid.NewGuid().ToString("N");
            var tempPath = Path.Combine(Path.GetTempPath(), tempFolder);
            Directory.CreateDirectory(tempPath);
            var nestedPath = tempPath.Combine("nested");
            var path = nestedPath.Combine(Guid.NewGuid()
                .ToString("N")) + (extension ?? "");
            if (!Directory.Exists(nestedPath)) Directory.CreateDirectory(nestedPath);
            try
            {
                test(tempPath, path, tempFolder.Combine("nested", path.GetFileName()), tempFolder);
            }
            catch
            {
                if (File.Exists(path)) File.Delete(path);
                throw;
            }
        }
    }
}
