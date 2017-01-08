using System;

namespace Swank.Extensions
{
    public static class Xml
    {
        public const string StringType = "string";
        public const string BooleanType = "boolean";
        public const string DecimalType = "decimal";
        public const string DoubleType = "double";
        public const string FloatType = "float";
        public const string UnsignedByteType = "unsignedByte";
        public const string ByteType = "byte";
        public const string ShortType = "short";
        public const string UnsignedShortType = "unsignedShort";
        public const string IntType = "int";
        public const string UnsignedIntType = "unsignedInt";
        public const string LongType = "long";
        public const string UnsignedLongType = "unsignedLong";
        public const string DateTimeType = "dateTime";
        public const string DurationType = "duration";
        public const string UuidType = "uuid";
        public const string CharType = "char";
        public const string AnyUriType = "anyURI";
        public const string Base64BinaryType = "base64Binary";
        public const string ArrayType = "ArrayOf";
        public const string DictionaryType = "DictionaryOf";

        public static string GetXmlName(this Type type, bool enumIsString)
        {
            if (type.IsNullable()) type = Nullable.GetUnderlyingType(type);
            if (type.IsEnum) type = enumIsString ? typeof(string) : 
                type.GetEnumUnderlyingType();

            if (type == typeof(string)) return StringType;
            if (type == typeof(bool)) return BooleanType;
            if (type == typeof(decimal)) return DecimalType;
            if (type == typeof(double)) return DoubleType;
            if (type == typeof(float)) return FloatType;
            if (type == typeof(byte)) return UnsignedByteType;
            if (type == typeof(sbyte)) return ByteType;
            if (type == typeof(short)) return ShortType;
            if (type == typeof(ushort)) return UnsignedShortType;
            if (type == typeof(int)) return IntType;
            if (type == typeof(uint)) return UnsignedIntType;
            if (type == typeof(long)) return LongType;
            if (type == typeof(ulong)) return UnsignedLongType;
            if (type == typeof(DateTime)) return DateTimeType;
            if (type == typeof(TimeSpan)) return DurationType;
            if (type == typeof(Guid)) return UuidType;
            if (type == typeof(char)) return CharType;
            if (type == typeof(Uri)) return AnyUriType;
            if (type == typeof(byte[])) return Base64BinaryType;
            if (type.IsArray || type.IsList()) return ArrayType + type
                .GetListElementType().GetXmlName(enumIsString).InitialCap();
            if (type.IsDictionary()) return DictionaryType + type
                .GetGenericDictionaryTypes().Value
                .GetXmlName(enumIsString).InitialCap();
            return type.Name;
        }
    }
}
