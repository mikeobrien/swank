using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Caching;
using System.Web.Hosting;

namespace Swank.Extensions
{
    public class IgnorePaths : VirtualPathProvider
    {
        private readonly List<string> _paths;

        private IgnorePaths(List<string> paths)
        {
            _paths = paths;
        }

        public static void Initialize(params string[] paths)
        {
            HostingEnvironment.RegisterVirtualPathProvider(
                new IgnorePaths(paths.Select(x => x.TrimStart('/')
                    .EnsureRooted().EnsureTrailingSlash()).ToList()));
        }

        public override string CombineVirtualPaths(string basePath, string relativePath)
        {
            return Previous.CombineVirtualPaths(basePath, relativePath);
        }

        public override bool DirectoryExists(string virtualDir)
        {
            if (_paths.Any(x => x.StartsWith(virtualDir, StringComparison.OrdinalIgnoreCase))) return false;
            return Previous.DirectoryExists(virtualDir);
        }

        public override bool FileExists(string virtualPath)
        {
            return Previous.FileExists(virtualPath);
        }

        public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            return Previous.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
        }

        public override string GetCacheKey(string virtualPath)
        {
            return Previous.GetCacheKey(virtualPath);
        }

        public override VirtualDirectory GetDirectory(string virtualDir)
        {
            return Previous.GetDirectory(virtualDir);
        }

        public override VirtualFile GetFile(string virtualPath)
        {
            return Previous.GetFile(virtualPath);
        }

        public override string GetFileHash(string virtualPath, IEnumerable virtualPathDependencies)
        {
            return Previous.GetFileHash(virtualPath, virtualPathDependencies);
        }
    }
}
