using System;
using System.Collections.Generic;
using System.Linq;

namespace Swank.Extensions
{
    public static class LinqExtensions
    {
        public static TValue TryGetValue<TKey, TValue>(
            this IDictionary<TKey, TValue> source, TKey key)
        {
            TValue value;
            source.TryGetValue(key, out value);
            return value;
        }

        public static IEnumerable<T> TraverseMany<T>(this T source,
            Func<T, IEnumerable<T>> results) where T : class
        {
            var nodes = results(source).ToList();
            return nodes.Concat(nodes.SelectMany(x => x.TraverseMany(results))).ToList();
        }

        public static TValue Map<T, TValue>(this T source, Func<T, TValue> map)
        {
            return map(source);
        }

        public static TResult MapOrDefault<T, TResult>(this T source, 
            Func<T, TResult> map, TResult @default = default(TResult))
            where T : class
        {
            return source != null ? map(source) : @default;
        }

        public static IEnumerable<T> Concat<T>(this IEnumerable<T> source, params T[] item)
        {
            return source != null ? new List<T>(Enumerable.Concat(source, item)).ToList() : item.ToList();
        }

        public static IEnumerable<T> DistinctSiblings<T>(this IEnumerable<T> source)
        {
            if (source != null)
            {
                var lastItem = default(T);
                foreach (var item in source)
                {
                    if (!item.Equals(lastItem)) yield return item;
                    lastItem = item;
                }
            }
        }

        public static List<T> AsList<T>(this T item)
        {
            return new List<T> { item };
        }

        public static IEnumerable<T> SelectMany<T>(this IEnumerable<IEnumerable<T>> source)
        {
            return source.SelectMany(x => x);
        }

        public static string Join(this IEnumerable<string> items, string delimeter)
        {
            return string.Join(delimeter, items);
        }

        public static bool EndsWith<T>(this IEnumerable<T> source, T value)
        {
            return source != null && source.Any() && source.Last().Equals(value);
        }

        public static IEnumerable<T> Shorten<T>(this IEnumerable<T> source, int by)
        {
            if (source == null || !source.Any()) return source;
            return @by >= source.Count() ? new List<T>() : source.Take(source.Count() - @by);
        }

        public static List<T> ForEach<T>(this List<T> source, Action<T, int> action)
        {
            var index = 0;
            source.ForEach(x => action(x, index++));
            return source;
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source) action(item);
            return source;
        }

        public static void ShrinkMultipartKeyRight<T, TKeyPart>(
            this IEnumerable<T> source,
            Func<T, IEnumerable<TKeyPart>> key,
            Action<T, List<TKeyPart>> setKey)
        {
            var keys = source.Select(x => new
            {
                Source = x,
                ShortKey = new Stack<TKeyPart>(),
                FullKey = new Stack<TKeyPart>(key(x).ToList())
            }).ToList();
            while (true)
            {
                var multiplicates = keys.Where(x => x.FullKey.Any())
                    .Multiplicates(x => x.ShortKey.GetItemsHashCode()).ToList();
                if (multiplicates.Count == 0) break;
                multiplicates.ForEach(x =>
                {
                    var segment = x.FullKey.Pop();
                    if (!x.ShortKey.Any() || !x.ShortKey.Peek().Equals(segment))
                        x.ShortKey.Push(segment);
                });
            }
            keys.ForEach(x => setKey(x.Source, x.ShortKey.ToList()));
        }

        public static IEnumerable<T> Multiplicates<T, TKey>(
            this IEnumerable<T> source, Func<T, TKey> key)
        {
            return source.Select(x => new { Context = x, Key = key(x) })
                .GroupBy(x => x.Key)
                .Where(x => x.Count() > 1)
                .SelectMany(x => x.Select(y => y.Context));
        }

        public static int GetItemsHashCode<T>(this IEnumerable<T> source)
        {
            return source == null || !source.Any() ? 0 : source.Select(
                x => x.GetHashCode()).Aggregate((a, i) => a | i);
        }
    }
}
