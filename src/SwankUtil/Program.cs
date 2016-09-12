using System;
using System.Reflection;
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
                        Renderer.RenderEndpoint(
                            arguments.Object.TemplatePath,
                            arguments.Object.SpecPath,
                            arguments.Object.EndpointId,
                            arguments.Object.OutputPath,
                            arguments.Object.RenderingEngine); break;
                    case Command.Template:
                        Renderer.RenderTemplate(
                            arguments.Object.TemplatePath,
                            arguments.Object.SpecPath,
                            arguments.Object.OutputPath,
                            arguments.Object.RenderingEngine); break;
                    default: Console.WriteLine("No command passed."); break;
                }
            }
            catch (ValidationException exception)
            {
                Console.WriteLine($"Error: {exception.Message}");
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Error: {exception}");
            }
        }
    }
}
