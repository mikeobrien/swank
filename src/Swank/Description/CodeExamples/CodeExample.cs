using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Swank.Extensions;
using Swank.Web.Assets;
using Swank.Web.Templates;

namespace Swank.Description.CodeExamples
{
    public class CodeExample
    {
        private static readonly string[] CodeExampleExtensions = {
            MarkdownAsset.Extension,
            MustacheTemplate.Extension,
            RazorTemplate.Extension
        };

        private static readonly string[] TemplateExtensions = {
            MustacheTemplate.Extension,
            RazorTemplate.Extension
        };

        private readonly IAsset _comments;
        private readonly ITemplate _template;

        public CodeExample(string name, string language, IAsset comments, ITemplate template)
        {
            _comments = comments;
            _template = template;
            Name = name;
            Language = language;
        }

        public string Name { get; }
        public string Language { get; }
        public string Comments => _comments?.ReadString();

        public string Render<T>(T model)
        {
            return _template.RenderString(model);
        } 

        public static IEnumerable<CodeExample> FromResource(
            IEnumerable<Assembly> assemblies, string path, string name, 
            string language, Configuration.Configuration configuration)
        {
            return MapFiles(ResourceAsset.FindMany(assemblies, path,
                CodeExampleExtensions), configuration, name, language);
        }

        public static IEnumerable<CodeExample> FromResources(
            IEnumerable<Assembly> assemblies, string path, Configuration.Configuration configuration)
        {
            return MapFiles(ResourceAsset.FindUnder(assemblies, path, null, 
                CodeExampleExtensions), configuration);
        }

        public static IEnumerable<CodeExample> FromVirtualPath(
            string path, string name, string language, Configuration.Configuration configuration)
        {
            return MapFiles(FileAsset.InVirtualPath(path.GetDirectoryName(),
                path.GetFileNameWithoutExtension(), CodeExampleExtensions), configuration, 
                name, language);
        }

        public static IEnumerable<CodeExample> InVirtualPath(
            string path, Configuration.Configuration configuration)
        {
            return MapFiles(FileAsset.InVirtualPath(path, 
                null, CodeExampleExtensions), configuration);
        }

        private static IEnumerable<CodeExample> MapFiles(
            IEnumerable<IFileAsset> files, Configuration.Configuration configuration, string name = null, 
            string language = null)
        {
            return files
                .GroupBy(x => x.Path.GetFileNameWithoutExtension())
                .Where(x => x.Any(y => y.Path.MatchesExtensions(TemplateExtensions)))
                .Select(x => new
                {
                    Name = name ?? x.Key,
                    Language = language ?? x.Key.ToLower(),
                    Comments = new MarkdownAsset(x.FirstOrDefault(y =>
                        y.Path.MatchesExtensions(MarkdownAsset.Extension))),
                    Template = MapTemplate(x.FirstOrDefault(y => y.Path
                        .MatchesExtensions(TemplateExtensions)), configuration)
                })
                .Select(x => new CodeExample(x.Name, x.Language, x.Comments, x.Template));
        }

        private static ITemplate MapTemplate(IFileAsset asset, Configuration.Configuration configuration)
        {
            return asset.Path.MatchesExtensions(MustacheTemplate.Extension)
                ? (ITemplate)new MustacheTemplate(asset, configuration)
                : new RazorTemplate(asset, configuration).Compile<TemplateModel>();
        }
    }
}