using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bender;
using RazorEngine;
using RazorEngine.Templating;
using Swank.Configuration;
using Swank.Description.CodeExamples;
using Swank.Extensions;
using Swank.Specification;
using Swank.Web.Assets;
using Swank.Web.Handlers;
using Swank.Web.Templates;

namespace SwankUtil
{
    public static class Renderer
    {
        public static void RenderTemplate(string templatePath, 
            string specPath, string outputPath)
        {
            Console.WriteLine("Starting render template.");
            Console.WriteLine("Loading spec...");
            var spec = Deserialize.JsonFile<List<Module>>(specPath,
                x => x.Deserialization(d => d.IgnoreNameCase()));
            if (spec == null || !spec.Any()) throw new ValidationException(
                $"Could not load spec {specPath}.");
            Console.WriteLine("Loading template...");
            var template = File.ReadAllText(templatePath);
            Console.WriteLine("Getting model...");
            Console.WriteLine("Rendering template...");
            var result = Engine.Razor.RunCompile(template,
                template.Hash(), typeof(List<Module>), spec);
            Console.WriteLine("Saving results...");
            File.WriteAllText(outputPath, result);
            Console.WriteLine("Successfully rendered!");
        }

        public static void RenderEndpoint(string templatePath, 
            string specPath, string endpointId, string outputPath)
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
            var codeExamples = new List<CodeExample>
            {
                new CodeExample("Haskell", "haskell", (StringAsset)"Haskell rocks!!",
                    new RazorTemplate((StringAsset)template, configuration))
            };
            var bodyDescriptionFactory = new BodyDescriptionFactory(configuration);
            var url = new Uri("http://www.setecastronomy.com:8080/");
            Console.WriteLine("Rendering template...");
            var endpoint = spec
                .SelectMany(m => m.Resources)
                .SelectMany(r => r.Endpoints)
                .FirstOrDefault(e => e.Id == endpointId);
            if (endpoint == null) throw new ValidationException(
                $"Could not find endpoint id {endpointId}.");
            template.CompileRazor<TemplateModel>();
            var result = AppResourceHandler.MapEndpoint(url, 
                    endpoint, codeExamples, bodyDescriptionFactory)
                .CodeExamples.First().Example;
            Console.WriteLine("Saving results...");
            File.WriteAllText(outputPath, result);
            Console.WriteLine("Successfully rendered!");
        }
    }
}
