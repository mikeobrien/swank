using Swank.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Swank.Web.Assets
{
    public class ResourceAsset : IFileAsset
    {
        private static readonly Func<Assembly, Tuple<Assembly, string>[]> 
            GetEmbeddedResources = Func.Memoize<Assembly, Tuple<Assembly, string>[]>(a => 
                a.GetManifestResourceNames().Select(x => Tuple.Create(a, x)).ToArray());

        private readonly string _name;
        private readonly Assembly _assembly;

        public ResourceAsset() { }

        public ResourceAsset(Assembly assembly, string name, string path, string relativePath)
        {
            Path = path;
            RelativePath = relativePath;
            _name = name;
            _assembly = assembly;
        }

        public string Path { get; }
        public string RelativePath { get; }

        public byte[] ReadBytes()
        {
            return _assembly.WhenNotNull(x => x
                .GetManifestResourceStream(_name)
                    .ReadAllBytes()).Otherwise(null);
        }

        public string ReadString()
        {
            return _assembly.WhenNotNull(x => x
                .GetManifestResourceStream(_name)
                    .ReadAllText()).Otherwise(null);
        }

        public static List<ResourceAsset> FindMany(IEnumerable<Assembly> assemblies,
            string path, params string[] extensions)
        {
            path = path.ConvertResourceNameToPath()
                .NormalizePathSlashes().TrimStart('\\');
            var paths = (!extensions.Any() ? path.AsList() :
                extensions.Select(x => path.GetExtension() != x 
                    ? path + x : path)).ToArray();
            return assemblies
                .SelectMany(x => GetEmbeddedResources(x))
                .Select(x => new
                {
                    Assembly = x.Item1,
                    Name = x.Item2,
                    Path = x.Item2.ConvertResourceNameToPath()
                })
                .Select(x => new ResourceAsset(x.Assembly, x.Name, x.Path, x.Path.GetFileName()))
                .Where(x => x.Path.RightMatchesOrEquals(paths)).ToList();
        }

        public static ResourceAsset FindSingle(IEnumerable<Assembly> assemblies, 
            string path, params string[] extensions)
        {
            return FindMany(assemblies, path, extensions).FirstOrDefault();
        }

        public static List<IFileAsset> FindUnder(
            IEnumerable<Assembly> assemblies, string path,
            string filename, params string[] extensions)
        {
            path = path.NormalizePathSlashes().Replace(".", "\\").TrimEnd('\\') + "\\";
            return assemblies.SelectMany(x => GetEmbeddedResources(x))
                .Select(x => new 
                {
                    Assembly = x.Item1,
                    Name = x.Item2,
                    Path = x.Item2.ConvertResourceNameToPath()
                })
                .Select(x => new ResourceAsset(x.Assembly, x.Name, x.Path, x.Path.MakeRelative(path)))
                .Where(x => x.Path.LeftMatches(path) &&
                    (filename == null || x.Path.MatchesFileNameWithoutExtension(filename)) &&
                    x.Path.MatchesExtensions(extensions))
                .Cast<IFileAsset>().ToList();
        }
    }
}