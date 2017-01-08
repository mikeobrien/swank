using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Swank.Configuration;
using Swank.Extensions;
using Swank.Web.Assets;

namespace Tests.Common
{
    public static class Extensions
    {
        public static Configuration Configure(this Configuration configuration,
            Action<ConfigurationDsl> configure)
        {
            configure(new ConfigurationDsl(configuration));
            return configuration;
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<bool, T> action)
        {
            var first = true;
            foreach (var item in source)
            {
                action(first, item);
                first = false;
            }
        }

        public class MapDsl
        {
            private readonly int _index;

            public MapDsl(int index)
            {
                _index = index;
            }

            public T To<T>(params T[] result)
            {
                return _index > -1 && _index < result.Length ?
                    result[_index] : default(T);
            }
        }

        public static MapDsl Map<T>(this T value, params T[] source)
        {
            return new MapDsl(Array.IndexOf(source, value));
        }

        public static byte[] StripBom(this byte[] bytes)
        {
            return bytes.Length >= 3 && bytes[0] == 239 && bytes[1] == 187 && 
                bytes[2] == 191 ? bytes.Skip(3).ToArray() : bytes;
        }

        public static string GetString(this byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }

        public static IAsset FindResourceAsset(this string path)
        {
            return ResourceAsset.FindSingle(Assembly
                .GetExecutingAssembly().AsList(), path);
        }
    }
}
