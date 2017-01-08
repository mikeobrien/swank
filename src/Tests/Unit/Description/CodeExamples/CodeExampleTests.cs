using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Should;
using Swank.Description.CodeExamples;

namespace Tests.Unit.Description.CodeExamples
{
    [TestFixture]
    public class CodeExampleTests
    {
        private static readonly Assembly[] ThisAssembly = { Assembly.GetExecutingAssembly() };
        private const string MarkdownCompiled = "<p><em>comments</em></p>";
        private const string RazorTemplate = "Fark @Model.Name";
        private const string MustacheTemplate = "Fark {{Name}}";
        private static readonly CodeExampleModel model = new CodeExampleModel { Name = "Farker" };
        private const string RenderedTemplate = "Fark Farker";

        private Swank.Configuration.Configuration _configuration;

        [SetUp]
        public void Setup()
        {
            _configuration = new Swank.Configuration.Configuration
            {
                DebugMode = true
            };
        }

        [Test]
        public void should_pull_code_example_from_resources_by_filename(
            [Values(null, "fark")] string name)
        {
            var examples = CodeExample.FromResource(ThisAssembly,
                "CodeExampleWithCommentsAndTemplate", name, "language", _configuration).ToList();

            examples.Count.ShouldEqual(1);

            var example = examples.First();
            example.Name.ShouldEqual(name ?? "CodeExampleWithCommentsAndTemplate");
            example.Language.ShouldEqual("language");
            example.Comments.ShouldEqual(MarkdownCompiled);
            example.Render(model).ShouldEqual(RenderedTemplate);
        }

        [Test]
        public void should_pull_code_examples_from_resources()
        {
            var examples = CodeExample.FromResources(ThisAssembly, 
                typeof(CodeExampleTests).Namespace, _configuration).ToList();

            examples.Count.ShouldEqual(2);

            var example = examples.First();
            example.Name.ShouldEqual("CodeExampleWithCommentsAndTemplate");
            example.Language.ShouldEqual("codeexamplewithcommentsandtemplate");
            example.Comments.ShouldEqual(MarkdownCompiled);
            example.Render(model).ShouldEqual(RenderedTemplate);

            example = examples.Skip(1).First();
            example.Name.ShouldEqual("CodeExampleWithNoComments");
            example.Language.ShouldEqual("codeexamplewithnocomments");
            example.Comments.ShouldEqual(null);
            example.Render(model).ShouldEqual(RenderedTemplate);
        }

        [Test]
        public void should_pull_code_example_from_folder_by_filename(
            [Values(null, "fark")] string name)
        {
            SetupFiles(path =>
            {
                var examples = CodeExample.FromVirtualPath(
                    $"{path}\\CodeExampleWithCommentsAndTemplate", 
                    name, "language", _configuration).ToList();

                examples.Count.ShouldEqual(1);

                var example = examples.First();
                example.Name.ShouldEqual(name ?? "CodeExampleWithCommentsAndTemplate");
                example.Language.ShouldEqual("language");
                example.Comments.ShouldEqual(MarkdownCompiled);
                example.Render(model).ShouldEqual(RenderedTemplate);               
            });
        }

        [Test]
        public void should_pull_code_examples_from_folder()
        {
            SetupFiles(path =>
            {
                var examples = CodeExample.InVirtualPath(path, _configuration).ToList();

                examples.Count.ShouldEqual(2);

                var example = examples.First(x => x.Name == "CodeExampleWithCommentsAndTemplate");
                example.ShouldNotBeNull();
                example.Language.ShouldEqual("codeexamplewithcommentsandtemplate");
                example.Comments.ShouldEqual(MarkdownCompiled);
                example.Render(model).ShouldEqual(RenderedTemplate);

                example = examples.First(x => x.Name == "CodeExampleWithNoComments");
                example.ShouldNotBeNull();
                example.Language.ShouldEqual("codeexamplewithnocomments");
                example.Comments.ShouldEqual(null);
                example.Render(model).ShouldEqual(RenderedTemplate);
            });
        }

        private static void SetupFiles(Action<string> test)
        {
            var virtualPath = Guid.NewGuid().ToString("n");
            var path = Path.Combine(Path.GetTempPath(), virtualPath) ;
            Directory.CreateDirectory(path);

            try
            {
                File.WriteAllText(Path.Combine(path, "CodeExampleWithCommentsAndTemplate.md"), "*comments*");
                File.WriteAllText(Path.Combine(path, "CodeExampleWithCommentsAndTemplate.cshtml"), RazorTemplate);
                File.WriteAllText(Path.Combine(path, "CodeExampleWithNoTemplate.md"), "*comments*");
                File.WriteAllText(Path.Combine(path, "CodeExampleWithNoComments.mustache"), MustacheTemplate);

                test(virtualPath);
            }
            finally
            {
                Directory.Delete(path, true);
            }
        }
    }
}
