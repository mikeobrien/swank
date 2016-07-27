using System;
using System.IO;
using System.Linq;

namespace Swank.Extensions
{
    public static class IOExtensions
    {
        public static string GetExtension(this string path)
        {
            return Path.GetExtension(path);
        }

        public static string GetDirectoryName(this string path)
        {
            return Path.GetDirectoryName(path);
        }

        public static string GetFileName(this string path)
        {
            return Path.GetFileName(path);
        }

        public static string Combine(this string path, params string[] paths)
        {
            return Path.Combine(new [] { path }.Concat(paths).ToArray());
        }

        public static string GetFileNameWithoutExtension(this string path)
        {
            return Path.GetFileNameWithoutExtension(path);
        }

        public static string GetPathWithoutExtension(this string path)
        {
            return Path.GetDirectoryName(path).Combine(
                path.GetFileNameWithoutExtension());
        }

        public static bool MatchesFileNameWithoutExtension(this string path, string filename)
        {
            return Path.GetFileNameWithoutExtension(path).EqualsIgnoreCase(filename);
        }

        public static bool MatchesExtensions(this string path, params string[] extensions)
        {
            return extensions.Any(y => path.GetExtension().EqualsIgnoreCase(y));
        }

        public static bool LeftMatches(this string path, string parent)
        {
            return path.StartsWith(parent, StringComparison.OrdinalIgnoreCase);
        }

        public static bool RightMatchesOrEquals(this string path, params string[] relativePaths)
        {
            return relativePaths.Any(p => path.EndsWith("\\" + p, StringComparison
                .OrdinalIgnoreCase) || path.EqualsIgnoreCase(p));
        }

        public static string[] GetFiles(this string path, 
            string searchPattern, SearchOption searchOption)
        {
            return Directory.GetFiles(path, searchPattern, searchOption);
        }
    }
}
