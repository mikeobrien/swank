using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Swank.Extensions;
using Swank.Web.Assets;

namespace Swank.Description
{
    public static class Assert
    {
        public static void AllEmbeddedCommentsMatchTypes(
            Func<string, bool> resourceFilter = null)
        {
            AllEmbeddedCommentsMatchTypes(Assembly
                .GetCallingAssembly(), resourceFilter);
        }

        public static void AllEmbeddedCommentsMatchTypes<TAssembly>(
            Func<string, bool> resourceFilter = null)
        {
            AllEmbeddedCommentsMatchTypes(typeof(TAssembly), resourceFilter);
        }

        public static void AllEmbeddedCommentsMatchTypes(Type type, 
            Func<string, bool> resourceFilter = null)
        {
            AllEmbeddedCommentsMatchTypes(type.Assembly, resourceFilter);
        }

        private static readonly string[] CommentsExtensions = { "",
            EndpointConvention.RequestCommentsExtension,
            EndpointConvention.ResponseCommentsExtension };

        public static void AllEmbeddedCommentsMatchTypes(Assembly assembly,
            Func<string, bool> resourceFilter = null)
        {
            var types = assembly.GetTypes().Where(x => x.IsPublic).ToList();
            var validNames = types.Select(x => x.FullName)
                .Concat(types.Select(x => x.Namespace).Distinct())
                .Concat(types.SelectMany(x => x.GetMethods()
                    .Where(y => y.IsPublic && y.DeclaringType == x)
                    .Select(y => x.FullName + "." + y.Name))
                    .GroupJoin(CommentsExtensions, x => true, x => true, 
                        (f, e) => e.Select(x => f + x))
                    .SelectMany(x => x))
                .Where(x => x != null)
                .Select(x => x.AddMarkdownExtension())
                .ToList();
            var orphans = assembly.GetManifestResourceNames()
                .Where(x => x.EndsWith(MarkdownAsset.Extension))
                .Where(resourceFilter ?? (x => true))
                .Where(x => !validNames.Any(y => y.EqualsIgnoreCase(x)))
                .ToList();
            if (orphans.Any())
                throw new Exception("The following embedded comments do not refer " +
                    $"to a type:\r\n{string.Join(",\r\n", orphans.ToArray())}");
        }
    }
}