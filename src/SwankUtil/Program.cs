﻿using System;
using System.Collections.Generic;
using System.Linq;
using Bender.Collections;
using Fclp;

namespace SwankUtil
{
    public enum Command
    {
        CodeExample,
        Template
    }

    public enum RenderingEngine
    {
        Mustache,
        Razor
    }

    public class Arguments
    {
        public Command Command { get; set; }
        public string TemplatePath { get; set; }
        public string SpecPath { get; set; }
        public string EndpointId { get; set; }
        public string OutputPath { get; set; }
        public List<string> Values { get; set; }
        public bool TemplateNamespaceIncludesModule { get; set; }
        public RenderingEngine? RenderingEngine { get; set; }
    }

    public class ValidationException : Exception 
    {
        public ValidationException(string message) : base(message) { }
    }

    public class Program
    {
        static void Main(string[] args)
        {
            // Force loading of the following assemblies
            typeof(System.Web.HttpApplication).ToString();

            Console.WriteLine();

            var arguments = new FluentCommandLineParser<Arguments>();

            arguments.SetupHelp().UseForEmptyArgs()
                .Callback(text => Console.WriteLine(text));

            arguments.Setup(a => a.Command)
                .As('c', "command")
                .WithDescription("The action you would like to perform.")
                .Required();

            arguments.Setup(a => a.TemplatePath)
                .As('t', "template")
                .WithDescription("Template path.")
                .Required();

            arguments.Setup(a => a.SpecPath)
                .As('s', "spec")
                .WithDescription("Specification path.")
                .Required();

            arguments.Setup(a => a.OutputPath)
                .As('o', "output")
                .WithDescription("Path of the rendered file.")
                .Required();

            arguments.Setup(a => a.EndpointId)
                .As('e', "endpoint")
                .WithDescription("Id of the endpoint. Only necessary for code examples.");

            arguments.Setup(a => a.Command)
                .As('r', "engine")
                .WithDescription("The rendering engine. If not supplied, " +
                    "will use the file extension to determine this.");

            arguments.Setup(a => a.TemplateNamespaceIncludesModule)
                .As('m', "module")
                .WithDescription("Specifies that the namespace in the " +
                    "template model includes the module name.");

            arguments.Setup(a => a.Values)
                .As('v', "value")
                .WithDescription("Ad hoc values passed into the template.");

            var result = arguments.Parse(args);

            if (result.HasErrors)
            {
                Console.WriteLine(result.ErrorText);
                return;
            }
            if (result.EmptyArgs) return;

            try
            {
                switch (arguments.Object.Command)
                {
                    case Command.CodeExample:
                        Console.WriteLine($"Template Path: {arguments.Object.TemplatePath}");
                        Console.WriteLine($"Spec Path: {arguments.Object.SpecPath}");
                        Console.WriteLine($"Endpoint Id: {arguments.Object.EndpointId}");
                        Console.WriteLine($"Output Path: {arguments.Object.OutputPath}");
                        Console.WriteLine($"Rendering Engine: {arguments.Object.RenderingEngine}");
                        Renderer.RenderEndpoint(
                            arguments.Object.TemplatePath,
                            arguments.Object.SpecPath,
                            arguments.Object.EndpointId,
                            arguments.Object.OutputPath,
                            arguments.Object.RenderingEngine); break;
                    case Command.Template:
                        var values = arguments.Object.Values?
                            .Select(x => x.Split(':'))
                            .ToDictionary(x => x[0], x => x[1],
                                StringComparer.OrdinalIgnoreCase);
                        Console.WriteLine($"TemplatePath: {arguments.Object.TemplatePath}");
                        Console.WriteLine($"SpecPath: {arguments.Object.SpecPath}");
                        Console.WriteLine($"OutputPath: {arguments.Object.OutputPath}");
                        Console.WriteLine($"RenderingEngine: {arguments.Object.RenderingEngine}");
                        Console.WriteLine($"TemplateNamespaceIncludesModule: {arguments.Object.TemplateNamespaceIncludesModule}");
                        values.ForEach(x => Console.WriteLine($"{x.Key}: {x.Value}"));
                        Renderer.RenderTemplate(
                            arguments.Object.TemplatePath,
                            arguments.Object.SpecPath,
                            arguments.Object.OutputPath,
                            arguments.Object.RenderingEngine,
                            arguments.Object.TemplateNamespaceIncludesModule,
                            values); break;
                    default: Console.WriteLine("No command passed."); break;
                }
            }
            catch (Renderer.RenderException exception)
            {
                Console.WriteLine($"Error:\r\n\r\n{exception}\r\n\r\n");
                Console.WriteLine($"Render Error:\r\n\r\n{exception.Message}");
            }
            catch (ValidationException exception)
            {
                Console.WriteLine($"Error:\r\n\r\n{exception.Message}");
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Error:\r\n\r\n{exception}");
            }
        }
    }
}
