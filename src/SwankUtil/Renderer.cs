using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Bender;
using Swank.Configuration;
using Swank.Description.CodeExamples;
using Swank.Extensions;
using Swank.Specification;
using Swank.Web.Assets;
using Swank.Web.Handlers.App;
using Swank.Web.Handlers.Templates;
using Swank.Web.Templates;
using CodeExampleModel = Swank.Description.CodeExamples.CodeExampleModel;

namespace SwankUtil
{
    public static class Renderer
    {
        public static void RenderTemplate(string templatePath,
            string specPath, string outputPath,
            RenderingEngine? renderingEngine, bool moduleIncludedInNamespace)
        {
            Console.WriteLine("Starting render template.");
            Console.WriteLine("Loading spec...");
            var spec = Deserialize.JsonFile<List<Module>>(specPath,
                x => x.Deserialization(d => d.IgnoreNameCase()));
            if (spec == null || !spec.Any())
                throw new ValidationException(
                    $"Could not load spec {specPath}.");
            Console.WriteLine("Loading template...");
            var template = File.ReadAllText(templatePath);
            var configuration = ConfigurationDsl.CreateConfig();
            if (moduleIncludedInNamespace) configuration
                .TemplateNamespaceIncludesModule = true;
            var templateModel = new TemplateModel
            {
                Specification = spec,
                Namespaces = new NamespaceDescriptionService(configuration).Create(spec)
            };
            string result = null;
            switch (WhatTheActualEngine(renderingEngine, templatePath))
            {
                case RenderingEngine.Razor:
                    Console.WriteLine("Rendering Razor template...");
                    result = FormatRazorError(() => template
                        .RenderAndCompileRazor(templateModel)); break;
                case RenderingEngine.Mustache:
                    Console.WriteLine("Rendering mustache template...");
                    result = template.RenderMustache(templateModel); break;
            }
            Console.WriteLine("Saving results...");
            File.WriteAllText(outputPath, result);
            Console.WriteLine("Successfully rendered!");
        }

        public static void RenderEndpoint(string templatePath, 
            string specPath, string endpointId, string outputPath,
            RenderingEngine? renderingEngine)
        {
            Console.WriteLine("Starting render code example.");
            var configuration = new Configuration();
            Console.WriteLine("Loading spec...");
            var spec = Deserialize.JsonFile<List<Module>>(specPath,
                x => x.Deserialization(d => d.IgnoreNameCase()));
            if (spec == null || !spec.Any()) throw new ValidationException(
                $"Could not load spec {specPath}.");
            Console.WriteLine("Loading template...");
            var template = File.ReadAllText(templatePath);
            ITemplate templateAsset = null;
            switch (WhatTheActualEngine(renderingEngine, templatePath))
            {
                case RenderingEngine.Razor:
                    Console.WriteLine("Rendering Razor template...");
                    templateAsset = new RazorTemplate((StringAsset)
                        template, configuration); break;
                case RenderingEngine.Mustache:
                    Console.WriteLine("Rendering mustache template...");
                    templateAsset = new MustacheTemplate((StringAsset)
                        template, configuration); break;
            }
            var codeExamples = new List<CodeExample>
            {
                new CodeExample("Haskell", "haskell", (StringAsset)"Haskell rocks!!", templateAsset)
            };
            var bodyDescriptionFactory = new BodyDescriptionService(configuration);
            var url = new Uri("http://www.setecastronomy.com:8080/");
            Console.WriteLine("Rendering template...");
            var endpoint = spec
                .SelectMany(m => m.Resources)
                .SelectMany(r => r.Endpoints)
                .FirstOrDefault(e => e.Id == endpointId);
            if (endpoint == null) throw new ValidationException(
                $"Could not find endpoint id {endpointId}.");
            template.CompileRazor<CodeExampleModel>();
            var result = FormatRazorError(() => AppResourceHandler.MapEndpoint(
                    url, endpoint, codeExamples, bodyDescriptionFactory)
                .CodeExamples.First().Example);
            Console.WriteLine("Saving results...");
            File.WriteAllText(outputPath, result);
            Console.WriteLine("Successfully rendered!");
        }

        private static RenderingEngine WhatTheActualEngine(
            RenderingEngine? engine, string filename)
        {
            if (engine.HasValue) return engine.Value;
            var extension = filename.GetExtension();
            if (extension.EqualsIgnoreCase(".cshtml"))
                return RenderingEngine.Razor;
            if (extension.EqualsIgnoreCase(".mustache"))
                return RenderingEngine.Mustache;
            throw new ValidationException("Could not determine " +
                "the rendering engine for your template.");
        }

        public class RenderException : Exception
        {
            public RenderException(string message, Exception exception) : 
                base(message, exception) { }
        }

        private static string FormatRazorError(Func<string> action)
        {
            try
            {
                return action();
            }
            catch (Exception exception)
            {
                var templateError = Regex.Match(exception.Message, "the error:([\\s\\S]*?)Temporary files");
                if (templateError.Length > 0)
                {
                    throw new RenderException(templateError.Groups[1].Value, exception);
                }
                throw;
            }
        }
    }
}
