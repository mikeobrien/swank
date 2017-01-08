using System;
using System.Collections.Generic;
using System.Linq;
using Swank.Extensions;

namespace Swank.Web.Assets
{
    public class LazyUrl
    {
        private readonly Func<string> _url;

        public LazyUrl(Func<string> url, string filename, string mimeType)
        {
            MimeType = mimeType;
            Filename = filename;
            _url = url;
        }

        public string MimeType { get; }
        public string Filename { get; }

        public string GetUrl()
        {
            return _url();
        }

        public static explicit operator LazyUrl(string url)
        {
            return url.ToLazyUrl();
        }
    }

    public static class LazyUrlExtensions
    {
        public static LazyUrl ToLazyUrl(this string url, string mimeType = null)
        {
            var filename = url.GetFileName();
            return new LazyUrl(() => url, filename, mimeType ?? 
                filename.GetExtension().GetMimeTypeFromExtension());
        }

        public static LazyUrl FindByFilename(this IEnumerable<LazyUrl> urls, string filename)
        {
            return urls.First(x => x.Filename == filename ||
                x.Filename.GetFileNameWithoutExtension() == filename);
        }

        public static void RemoveByFilename(this List<LazyUrl> urls, params string[] filenames)
        {
            urls.RemoveAll(x => filenames.Contains(x.Filename) || 
                filenames.Contains(x.Filename.GetFileNameWithoutExtension()));
        }

        public static void PrependOrAdd(this List<LazyUrl> urls, LazyUrl url, string beforeFilename)
        {
            var prependStylesheet = urls.FirstOrDefault(x => x.Filename == beforeFilename || 
                x.Filename.GetFileNameWithoutExtension() == beforeFilename);
            if (prependStylesheet != null)
                urls.Insert(urls.IndexOf(prependStylesheet), url);
            else urls.Add(url);
        }
    }
}
