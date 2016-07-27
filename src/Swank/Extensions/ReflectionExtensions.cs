using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Swank.Extensions
{
    internal static class ReflectionExtensions
    {
        public static bool IsInDebugMode(this Assembly assembly)
        {
            return assembly.GetCustomAttributes(typeof(DebuggableAttribute), false)
                .Cast<DebuggableAttribute>().Any(x => x.IsJITTrackingEnabled);
        }

        public static bool IsInNamespace(this Type type, object instance)
        {
            return type.IsInNamespace(instance.GetType());
        }

        public static bool IsInNamespace(this Type type, Type typeNamespace)
        {
            return type.IsInNamespace(typeNamespace.Namespace);
        }

        public static bool IsInNamespace(this Type type, string @namespace)
        {
            return type.Namespace != null && (type.Namespace == @namespace || 
                type.Namespace.StartsWith(@namespace + "."));
        }

        public static string ConvertResourceNameToPath(this string name)
        {
            var path = name.Split('.');
            if (path.Length <= 2) return name;
            return path.Take(path.Length - 2).Join("\\") + "\\" +
                   path.Skip(path.Length - 2).Join(".");
        }

        public static bool HasAttribute<T>(this Type type) where T : Attribute
        {
            return type.GetCustomAttributes(typeof(T), true).Any();
        }

        public static bool HasAttribute<T>(this FieldInfo field)
        {
            return field.GetCustomAttributes(typeof(T), true).Any();
        }

        public static bool HasAttribute<T>(this PropertyInfo property)
        {
            return property.GetCustomAttributes(typeof(T), true).Any();
        }

        public static bool IsSimpleType(this Type type)
        {
            Func<Type, bool> isSimpleType = x => x.IsPrimitive || x.IsEnum || 
                x == typeof(string) || x == typeof(decimal) || x == typeof(DateTime) || 
                x == typeof(TimeSpan) || x == typeof(Guid) || x == typeof(Uri);
            return isSimpleType(type) || (type.IsNullable() && 
                isSimpleType(Nullable.GetUnderlyingType(type)));
        }

        public static bool Implements<T>(this Type type)
        {
            return type.Implements(typeof(T));
        }

        public static bool Implements(this Type type, Type @interface)
        {
            return type.GetInterfaces().Any(x => @interface ==
                (x.IsGenericType && @interface.IsGenericType &&
                 @interface.IsGenericTypeDefinition ? x.GetGenericTypeDefinition() : x));
        }

        public static Type UnwrapType(this Type type)
        {
            if (type.IsDictionary()) return type.GetGenericDictionaryTypes().Value.UnwrapType();
            if (type.IsArray || type.IsList()) return type.GetListElementType().UnwrapType();
            if (type.IsNullable()) return type.GetNullableUnderlyingType().UnwrapType();
            return type;
        }

        public static bool IsNullable(this Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }

        public static Type GetNullableUnderlyingType(this Type type)
        {
            return type.IsNullable() ? Nullable.GetUnderlyingType(type) : type;
        }

        public static FieldInfo[] GetEnumOptions(this Type type)
        {
            type = type.IsNullable() ? Nullable.GetUnderlyingType(type) : type;
            return type.GetFields(BindingFlags.Public | BindingFlags.Static);
        }

        public static string GetPath(this Assembly assembly)
        {
            return new Uri(assembly.CodeBase).LocalPath;
        }

        // Lists

        private static readonly Type[] ListTypes = { typeof(IList<>), typeof(List<>) };

        public static bool IsListType(this Type type)
        {
            var genericTypeDef = type.IsGenericType ? type.GetGenericTypeDefinition() : null;
            return ListTypes.Any(x => genericTypeDef == x);
        }

        public static bool ImplementsListType(this Type type)
        {
            return type.Implements(typeof(IList<>));
        }

        public static bool IsList(this Type type)
        {
            return type.IsListType() || type.ImplementsListType();
        }

        public static Type GetListInterface(this Type type)
        {
            return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IList<>)) ? type :
                   type.GetInterfaces().First(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IList<>));
        }

        public static Type GetListElementType(this Type type)
        {
            return type.IsArray ? type.GetElementType() : type.IsList() ? type.GetListInterface().GetGenericArguments()[0] : null;
        }

        // Dictionaries

        public static bool IsDictionary(this Type type)
        {
            return type.IsNonGenericDictionary() || type.IsGenericDictionary();
        }

        public static bool IsNonGenericDictionary(this Type type)
        {
            return !type.IsGenericDictionary() && (type == typeof(IDictionary) || 
                type.Implements<IDictionary>());
        }

        public static bool IsGenericDictionary(this Type type)
        {
            return type.GetInterfaces().Any(x => x.IsGenericDictionaryInterface()) ||
                type.IsGenericDictionaryInterface();
        }

        public static bool IsGenericDictionaryInterface(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IDictionary<,>);
        }

        public static KeyValuePair<Type, Type> GetGenericDictionaryTypes(this Type type)
        {
            return (type.IsGenericDictionaryInterface() ? type : 
                type.GetInterfaces().First(x => x.IsGenericDictionaryInterface()))
                .GetGenericArguments()
                .Map(x => new KeyValuePair<Type, Type>(x[0], x[1]));
        }
        
        // Embedded Resources

        private static readonly Func<Assembly, string[]> GetEmbeddedResources =
            Func.Memoize<Assembly, string[]>(a => a.GetManifestResourceNames());

        public static string FindResourceNamed(this Assembly assembly, params string[] names)
        {
            var resourceName = GetEmbeddedResources(assembly)
                .FirstOrDefault(x => names.Any(y => y.StartsWith("*")
                    ? x.EndsWith(y.Substring(1), StringComparison.OrdinalIgnoreCase)
                    : y.Equals(x, StringComparison.OrdinalIgnoreCase)));
            if (resourceName == null) return null;
            return assembly.GetManifestResourceStream(resourceName).ReadAllText();
        }

        // Convenience methods

        public static bool IsString(this Type type) { return type == typeof(string); }
        public static bool IsBoolean(this Type type) { return type == typeof(bool) || type == typeof(bool?); }
        public static bool IsDecimal(this Type type) { return type == typeof(decimal) || type == typeof(decimal?); }
        public static bool IsDouble(this Type type) { return type == typeof(double) || type == typeof(double?); }
        public static bool IsSingle(this Type type) { return type == typeof(float) || type == typeof(float?); }
        public static bool IsByteArray(this Type type) { return type == typeof(byte[]); }
        public static bool IsByte(this Type type) { return type == typeof(byte) || type == typeof(byte?); }
        public static bool IsSByte(this Type type) { return type == typeof(sbyte) || type == typeof(sbyte?); }
        public static bool IsInt16(this Type type) { return type == typeof(short) || type == typeof(short?); }
        public static bool IsUInt16(this Type type) { return type == typeof(ushort) || type == typeof(ushort?); }
        public static bool IsInt32(this Type type) { return type == typeof(int) || type == typeof(int?); }
        public static bool IsUInt32(this Type type) { return type == typeof(uint) || type == typeof(uint?); }
        public static bool IsInt64(this Type type) { return type == typeof(long) || type == typeof(long?); }
        public static bool IsUInt64(this Type type) { return type == typeof(ulong) || type == typeof(ulong?); }
        public static bool IsDateTime(this Type type) { return type == typeof(DateTime) || type == typeof(DateTime?); }
        public static bool IsTimeSpan(this Type type) { return type == typeof(TimeSpan) || type == typeof(TimeSpan?); }
        public static bool IsGuid(this Type type) { return type == typeof(Guid) || type == typeof(Guid?); }
        public static bool IsChar(this Type type) { return type == typeof(char) || type == typeof(char?); }

        public static bool IsString(this PropertyInfo property) { return property.PropertyType.IsString(); }
        public static bool IsBoolean(this PropertyInfo property) { return property.PropertyType.IsBoolean(); }
        public static bool IsDecimal(this PropertyInfo property) { return property.PropertyType.IsDecimal(); }
        public static bool IsDouble(this PropertyInfo property) { return property.PropertyType.IsDouble(); }
        public static bool IsSingle(this PropertyInfo property) { return property.PropertyType.IsSingle(); }
        public static bool IsByteArray(this PropertyInfo property) { return property.PropertyType.IsByteArray(); }
        public static bool IsByte(this PropertyInfo property) { return property.PropertyType.IsByte(); }
        public static bool IsSByte(this PropertyInfo property) { return property.PropertyType.IsSByte(); }
        public static bool IsInt16(this PropertyInfo property) { return property.PropertyType.IsInt16(); }
        public static bool IsUInt16(this PropertyInfo property) { return property.PropertyType.IsUInt16(); }
        public static bool IsInt32(this PropertyInfo property) { return property.PropertyType.IsInt32(); }
        public static bool IsUInt32(this PropertyInfo property) { return property.PropertyType.IsUInt32(); }
        public static bool IsInt64(this PropertyInfo property) { return property.PropertyType.IsInt64(); }
        public static bool IsUInt64(this PropertyInfo property) { return property.PropertyType.IsUInt64(); }
        public static bool IsDateTime(this PropertyInfo property) { return property.PropertyType.IsDateTime(); }
        public static bool IsTimeSpan(this PropertyInfo property) { return property.PropertyType.IsTimeSpan(); }
        public static bool IsGuid(this PropertyInfo property) { return property.PropertyType.IsGuid(); }
        public static bool IsChar(this PropertyInfo property) { return property.PropertyType.IsChar(); }
    }
}
