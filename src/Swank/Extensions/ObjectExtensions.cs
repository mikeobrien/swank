using System;
using System.Collections.Generic;
using System.Linq;
using Swank.Configuration;

namespace Swank.Extensions
{
    public static class ObjectExtensions
    {
        public static T ThenDo<T>(this T source, Action<T> action)
        {
            action(source);
            return source;
        }

        public static IEnumerable<T> ThenDo<T>(this IEnumerable<T> source, Action<T> action)
        {
            source.ForEach(x => x.ThenDo(action));
            return source;
        }

        public class OtherwiseOptions<TResult>
        {
            private readonly object _value;
            private readonly Func<TResult> _returnThis;

            public OtherwiseOptions(object value, Func<TResult> returnThis)
            {
                _value = value;
                _returnThis = returnThis;
            }

            public TResult OtherwiseDefault()
            {
                return Otherwise(default(TResult));
            }

            public TResult Otherwise(params TResult[] values)
            {
                return _value != null ? _returnThis() : values.FirstOrDefault(x => x != null);
            }

            public OtherwiseOptions<TNextResult> WhenNotNull<TNextResult>(
                Func<TResult, TNextResult> returnThis)
            {
                return OtherwiseDefault().WhenNotNull(returnThis);
            }
        }

        public static OtherwiseOptions<TResult> WhenNotNull<TSource, TResult>(
            this TSource value, Func<TSource, TResult> returnThis)
        {
            return new OtherwiseOptions<TResult>(value, () => returnThis(value));
        }

        public static string GetSampleValue(this Type type, 
            Configuration.Configuration configuration)
        {
            type = type.GetNullableUnderlyingType();

            if (type == typeof(decimal) || type == typeof(double) ||
                type == typeof(float))
                    return configuration.SampleRealValue
                        .ToSampleValueString(configuration);

            if (type == typeof(bool))
                return configuration.SampleBoolValue
                    .ToSampleValueString(configuration);

            if (type == typeof(byte) || type == typeof(sbyte) ||
                type == typeof(short) || type == typeof(ushort) ||
                type == typeof(int) || type == typeof(uint) ||
                type == typeof(long) || type == typeof(ulong))
                return configuration.SampleIntegerValue
                    .ToSampleValueString(configuration);

            if (type == typeof(DateTime))
                return configuration.SampleDateTimeValue
                    .ToSampleValueString(configuration);

            if (type == typeof(TimeSpan))
                return configuration.SampleTimeSpanValue
                    .ToSampleValueString(configuration);

            if (type == typeof(Guid))
                return configuration.SampleGuidValue
                    .ToSampleValueString(configuration);
            
            if (type == typeof(Uri))
                return configuration.SampleUriValue.ToString();

            if (type.IsEnum) return Activator.CreateInstance(type)
                .ToSampleValueString(configuration);

            return configuration.SampleStringValue;
        }

        public static string ToSampleValueString(this object value, 
            Configuration.Configuration configuration)
        {
            var type = value.GetType();
            if (type == typeof(decimal)) return ((decimal)value)
                .ToString(configuration.SampleRealFormat);
            if (type == typeof(decimal?)) return ((decimal?)value)
                .Value.ToString(configuration.SampleRealFormat);
            if (type == typeof(double)) return ((double)value)
                .ToString(configuration.SampleRealFormat);
            if (type == typeof(double?)) return ((double?)value)
                .Value.ToString(configuration.SampleRealFormat);
            if (type == typeof(float)) return ((float)value)
                .ToString(configuration.SampleRealFormat);
            if (type == typeof(float?)) return ((float?)value)
                .Value.ToString(configuration.SampleRealFormat);

            if (type == typeof(bool)) return ((bool)value)
                .ToString().ToLower();
            if (type == typeof(bool?)) return ((bool?)value)
                .Value.ToString().ToLower();
            if (type == typeof(byte)) return ((byte)value)
                .ToString(configuration.SampleIntegerFormat);
            if (type == typeof(byte?)) return ((byte?)value)
                .Value.ToString(configuration.SampleIntegerFormat);
            if (type == typeof(sbyte)) return ((sbyte)value)
                .ToString(configuration.SampleIntegerFormat);
            if (type == typeof(sbyte?)) return ((sbyte?)value)
                .Value.ToString(configuration.SampleIntegerFormat);
            if (type == typeof(short)) return ((short)value)
                .ToString(configuration.SampleIntegerFormat);
            if (type == typeof(short?)) return ((short?)value)
                .Value.ToString(configuration.SampleIntegerFormat);
            if (type == typeof(ushort)) return ((ushort)value)
                .ToString(configuration.SampleIntegerFormat);
            if (type == typeof(ushort?)) return ((ushort?)value)
                .Value.ToString(configuration.SampleIntegerFormat);
            if (type == typeof(int)) return ((int)value)
                .ToString(configuration.SampleIntegerFormat);
            if (type == typeof(int?)) return ((int?)value)
                .Value.ToString(configuration.SampleIntegerFormat);
            if (type == typeof(uint)) return ((uint)value)
                .ToString(configuration.SampleIntegerFormat);
            if (type == typeof(uint?)) return ((uint?)value)
                .Value.ToString(configuration.SampleIntegerFormat);
            if (type == typeof(long)) return ((long)value)
                .ToString(configuration.SampleIntegerFormat);
            if (type == typeof(long?)) return ((long?)value)
                .Value.ToString(configuration.SampleIntegerFormat);
            if (type == typeof(ulong)) return ((ulong)value)
                .ToString(configuration.SampleIntegerFormat);
            if (type == typeof(ulong?)) return ((ulong?)value)
                .Value.ToString(configuration.SampleIntegerFormat);

            if (type == typeof(DateTime)) return ((DateTime)value)
                .ToString(configuration.SampleDateTimeFormat);
            if (type == typeof(DateTime?)) return ((DateTime?)value)
                .Value.ToString(configuration.SampleDateTimeFormat);

            if (type == typeof(TimeSpan)) return ((TimeSpan)value)
                .ToString(configuration.SampleTimeSpanFormat);
            if (type == typeof(TimeSpan?)) return ((TimeSpan?)value)
                .Value.ToString(configuration.SampleTimeSpanFormat);
            
            if (type == typeof(Guid)) return ((Guid)value)
                .ToString(configuration.SampleGuidFormat);
            if (type == typeof(Guid?)) return ((Guid?)value)
                .Value.ToString(configuration.SampleGuidFormat);

            if (type.GetNullableUnderlyingType().IsEnum)
                return configuration.EnumFormat == 
                    EnumFormat.AsString ? value.ToString() : 
                    ((int)value).ToString();

            return value.ToString();
        }
    }
}
