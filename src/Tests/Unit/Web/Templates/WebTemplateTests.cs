using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Should;
using Swank.Extensions;
using Swank.Web.Templates;
using Tests.Common;

namespace Tests.Unit.Web.Templates
{
    [TestFixture]
    public class WebTemplateTests
    {
        private readonly Swank.Configuration.Configuration _configuration = 
            new Swank.Configuration.Configuration
        {
            AppUrl = "api"
        };
        public class Model { public string Value { get; set; } }
        private readonly Model _model = new Model { Value = "Farker" };
        private const string RazorTemplate = "Fark @Model.Value";
        private const string MustacheTemplate = "Fark {{Value}}";
        private const string RenderedTemplate = "Fark Farker";
        private const string MustacheFile = "MustacheTemplate.mustache";
        private const string RazorFile = "RazorTemplate.cshtml";

        [Test]
        public void should_get_razor_web_template_from_a_string()
        {
            var template = WebTemplate.RazorFromString<Model>(
                "url", "mime", RazorTemplate, _configuration);

            template.GetUrl(_configuration).ShouldEqual("api/url");
            template.MimeType.ShouldEqual("mime");
            template.Render(_model).ShouldEqual(RenderedTemplate.ToBytes());
        }

        [Test]
        public void should_get_mustache_web_template_from_a_string()
        {
            var template = WebTemplate.MustacheFromString("url", "mime", MustacheTemplate, _configuration);

            template.GetUrl(_configuration).ShouldEqual("api/url");
            template.MimeType.ShouldEqual("mime");
            template.Render(_model).ShouldEqual(RenderedTemplate.ToBytes());
        }

        [Test]
        public void should_get_web_template_from_a_virtual_path(
            [Values(MustacheFile, RazorFile)] string templateFile)
        {
            SetupFiles((virtualPath, nestedPath) =>
            {
                var templates = WebTemplate.FromVirtualPath<Model>(Path.Combine(virtualPath, 
                    nestedPath, templateFile), "url", "fark/farker", _configuration);

                templates.Count().ShouldEqual(1);

                var template = templates.First();

                template.GetUrl(_configuration).ShouldEqual("api/url");
                template.MimeType.ShouldEqual("fark/farker");
                template.Render(_model).StripBom().ShouldEqual(RenderedTemplate.ToBytes());
            });
        }

        [Test]
        public void should_get_web_templates_under_a_virtual_path()
        {
            SetupFiles((virtualPath, nestedPath) =>
            {
                var templates = WebTemplate.InVirtualPath<Model>(virtualPath, "url", "fark/farker", _configuration);

                templates.Count().ShouldEqual(2);
                
                var template = templates.FirstOrDefault(x => x.GetUrl(_configuration)
                    .Contains(MustacheFile.GetFileNameWithoutExtension()));
                template.GetUrl(_configuration).ShouldEqual(
                    $"api/url/{nestedPath}/{MustacheFile.GetFileNameWithoutExtension()}");
                template.MimeType.ShouldEqual("fark/farker");
                template.Render(_model).StripBom().ShouldEqual(RenderedTemplate.ToBytes());

                template = templates.FirstOrDefault(x => x
                    .GetUrl(_configuration).EndsWith(RazorFile.GetFileNameWithoutExtension()));
                template.GetUrl(_configuration).ShouldEqual(
                    $"api/url/{nestedPath}/{RazorFile.GetFileNameWithoutExtension()}");
                template.MimeType.ShouldEqual("fark/farker");
                template.Render(_model).StripBom().ShouldEqual(RenderedTemplate.ToBytes());
            });
        }

        private static void SetupFiles(Action<string, string> test)
        {
            var virtualPath = Guid.NewGuid().ToString("n");
            var nestedPath = Guid.NewGuid().ToString("n");
            var path = Path.Combine(Path.GetTempPath(), virtualPath, nestedPath);
            Directory.CreateDirectory(path);

            try
            {
                File.WriteAllText(Path.Combine(path, MustacheFile), MustacheTemplate);
                File.WriteAllText(Path.Combine(path, RazorFile), RazorTemplate);

                test(virtualPath, nestedPath);
            }
            finally
            {
                Directory.Delete(path, true);
            }
        }

        private readonly IEnumerable<Assembly> _thisAssembly = Assembly
            .GetExecutingAssembly().AsList();

        [Test]
        public void should_get_web_template_from_a_resource(
            [Values(MustacheFile, RazorFile)] string templateFile)
        {
            var templates = WebTemplate.FromResource<Model>(_thisAssembly, 
                templateFile, "url", "fark/farker", _configuration);

            templates.Count().ShouldEqual(1);

            var template = templates.First();

            template.GetUrl(_configuration).ShouldEqual("api/url");
            template.MimeType.ShouldEqual("fark/farker");
            template.Render(_model).StripBom().ShouldEqual(RenderedTemplate.ToBytes());
        }

        [Test]
        public void should_get_web_templates_from_a_resource()
        {
            var templates = WebTemplate.FromResources<Model>(_thisAssembly,
                "Tests.Unit.Web.Templates.WebTemplateTests",
                "url", "fark/farker", _configuration);

            templates.Count().ShouldEqual(2);

            var template = templates.FirstOrDefault(x => x.GetUrl(_configuration)
                .Contains(MustacheFile.GetFileNameWithoutExtension()));
            template.GetUrl(_configuration).ShouldEqual("api/url/Resources/" + 
                MustacheFile.GetFileNameWithoutExtension());
            template.MimeType.ShouldEqual("fark/farker");
            template.Render(_model).StripBom().ShouldEqual(RenderedTemplate.ToBytes());

            template = templates.FirstOrDefault(x => x.GetUrl(_configuration)
                .EndsWith(RazorFile.GetFileNameWithoutExtension()));
            template.GetUrl(_configuration).ShouldEqual("api/url/Resources/" + 
                RazorFile.GetFileNameWithoutExtension());
            template.MimeType.ShouldEqual("fark/farker");
            template.Render(_model).StripBom().ShouldEqual(RenderedTemplate.ToBytes());
        }
    }
}
