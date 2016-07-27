using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Should;

namespace Tests.Common
{
    public static class ShouldExtensions
    {
        public static IEnumerable<T> ShouldTotal<T>(
            this IEnumerable<T> source, int total)
        {
            source.Count().ShouldEqual(total);
            return source;
        }

        public static void ShouldEqualUrl<T>(this string url,
            Expression<Func<T, object>> action)
        {
            url.ShouldEqual(action.GetMethodInfo().ToTestUrl());
        }

        public static void ShouldEqualNameUrl<T>(this string url,
            Expression<Func<T, object>> action)
        {
            url.ShouldEqual("/" + action.GetMethodInfo().ToTestUrl());
        }

        public static void ShouldOnlyContain<T>(
            this IEnumerable<T> actual, params T[] expected)
        {
            actual.ToArray().ShouldEqual(expected);
        }
    }
}
