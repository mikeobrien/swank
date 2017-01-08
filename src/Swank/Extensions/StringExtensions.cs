using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Swank.Extensions
{
    public static class StringExtensions
    {
        public static string JoinLines(this string value)
        {
            return value.IsNullOrEmpty() ? value : 
                value.Split(new [] { "\r\n", "\r", "\n" })
                .Select(x => x.Trim())
                .Where(x => x.IsNotNullOrEmpty()).Join(" ");
        }

        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static bool IsNotNullOrEmpty(this string value)
        {
            return !string.IsNullOrEmpty(value);
        }

        public static bool EqualsIgnoreCase(this string value, string compare)
        {
            return value.Equals(compare, StringComparison.OrdinalIgnoreCase);
        }

        public static string[] Split(this string value, params string[] splitBy)
        {
            return value.Split(splitBy, StringSplitOptions.None);
        }

        public static string Hash(this string value)
        {
            using (var hash = MD5.Create())
                return hash.ComputeHash(Encoding.Unicode
                    .GetBytes(value)).ToHex().ToLower();
        }

        private static string ToHex(this IEnumerable<byte> bytes)
        {
            return bytes.Select(b => $"{b:X2}").Aggregate((a, i) => a + i);
        }

        public static string Repeat(this string value, int count)
        {
            if (string.IsNullOrEmpty(value)) return value;
            var buffer = "";
            for (var i = 0; i < count; i++) buffer += value;
            return buffer;
        }

        public static byte[] ToBytes(this string source)
        {
            return source == null ? null : Encoding.UTF8.GetBytes(source);
        }

        public static string ToSnakeCase(this object value)
        {
            return value.ToSeparatedCase(true, "-");
        }

        public static string ToSeparatedCase(this object @object, bool lower, string seperator)
        {
            var value = @object.ToString();
            if (string.IsNullOrEmpty(value)) return value;
            var result = value[0].ToString() + Regex.Replace(value.Substring(1),
                "((?<=[a-z])[A-Z]|[A-Z](?=[a-z]))", seperator + "$1");
            return lower ? result.ToLower() : result;
        }

        private static readonly Regex PascalCaseToSentenceRegex = new Regex(@"
            (?<=[A-Z])(?=[A-Z][a-z]) | 
            (?<=[^A-Z])(?=[A-Z]) | 
            (?<=[A-Za-z])(?=[^A-Za-z])",
            RegexOptions.IgnorePatternWhitespace);

        public static string[] SplitPascalCasedSentence(this string value)
        {
            return value.IsNullOrEmpty() ? new string[] { } :
                PascalCaseToSentenceRegex.Replace(value, " ").Split(' ');
        }

        public static string ToTitleFromPascalCasing(this object value)
        {
            var stringValue = value?.ToString();
            if (stringValue.IsNullOrEmpty()) return stringValue;
            return stringValue.SplitPascalCasedSentence()
                .Select(x => x.InitialCap()).Join(" ");
        }

        public static string InitialCap(this string value)
        {
            if (value.IsNullOrEmpty()) return value;
            return value[0].ToString().ToUpper() + 
                (value.Length > 1 ? value.Substring(1) : "");
        }

        public static string ToCamelCase(this string value)
        {
            if (string.IsNullOrEmpty(value) ||
                !char.IsUpper(value[0])) return value;

            var chars = value.ToCharArray();

            for (var i = 0; i < chars.Length; i++)
            {
                if (i == 1 && !char.IsUpper(chars[i])) break;

                var hasNext = i + 1 < chars.Length;
                if (i > 0 && hasNext && !char.IsUpper(chars[i + 1])) break;

                chars[i] = char.ToLowerInvariant(chars[i]);
            }

            return new string(chars);
        }
    }
}
