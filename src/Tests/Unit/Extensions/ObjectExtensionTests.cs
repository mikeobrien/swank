using System;
using System.Collections.Generic;
using Swank.Extensions;
using NUnit.Framework;
using Should;
using Swank.Configuration;
using Tests.Common;

namespace Tests.Unit.Extensions
{
    [TestFixture]
    public class ObjectExtensionTests
    {
        private static readonly object[][] FormatSampleValueTestCases = TestCaseSource.Create(4, x => x
            .Add((decimal)5, "5.00").Add((decimal)5, "5.000", nameof(Swank.Configuration.Configuration.SampleRealFormat), "0.000")
            .Add((decimal?)5, "5.00").Add((decimal?)5, "5.000", nameof(Swank.Configuration.Configuration.SampleRealFormat), "0.000")
            .Add((double)5, "5.00").Add((double)5, "5.000", nameof(Swank.Configuration.Configuration.SampleRealFormat), "0.000")
            .Add((double?)5, "5.00").Add((double?)5, "5.000", nameof(Swank.Configuration.Configuration.SampleRealFormat), "0.000")
            .Add((float)5, "5.00").Add((float)5, "5.000", nameof(Swank.Configuration.Configuration.SampleRealFormat), "0.000")
            .Add((float?)5, "5.00").Add((float?)5, "5.000", nameof(Swank.Configuration.Configuration.SampleRealFormat), "0.000")
            .Add(true, "true").Add((bool?)true, "true")
            .Add((byte)5, "5").Add((byte)5, "5.000", nameof(Swank.Configuration.Configuration.SampleIntegerFormat), "0.000")
            .Add((byte?)5, "5").Add((byte?)5, "5.000", nameof(Swank.Configuration.Configuration.SampleIntegerFormat), "0.000")
            .Add((sbyte)5, "5").Add((sbyte)5, "5.000", nameof(Swank.Configuration.Configuration.SampleIntegerFormat), "0.000")
            .Add((sbyte?)5, "5").Add((sbyte?)5, "5.000", nameof(Swank.Configuration.Configuration.SampleIntegerFormat), "0.000")
            .Add((short)5, "5").Add((short)5, "5.000", nameof(Swank.Configuration.Configuration.SampleIntegerFormat), "0.000")
            .Add((short?)5, "5").Add((short?)5, "5.000", nameof(Swank.Configuration.Configuration.SampleIntegerFormat), "0.000")
            .Add((ushort)5, "5").Add((ushort)5, "5.000", nameof(Swank.Configuration.Configuration.SampleIntegerFormat), "0.000")
            .Add((ushort?)5, "5").Add((ushort?)5, "5.000", nameof(Swank.Configuration.Configuration.SampleIntegerFormat), "0.000")
            .Add((int)5, "5").Add((int)5, "5.000", nameof(Swank.Configuration.Configuration.SampleIntegerFormat), "0.000")
            .Add((int?)5, "5").Add((int?)5, "5.000", nameof(Swank.Configuration.Configuration.SampleIntegerFormat), "0.000")
            .Add((uint)5, "5").Add((uint)5, "5.000", nameof(Swank.Configuration.Configuration.SampleIntegerFormat), "0.000")
            .Add((uint?)5, "5").Add((uint?)5, "5.000", nameof(Swank.Configuration.Configuration.SampleIntegerFormat), "0.000")
            .Add((long)5, "5").Add((byte)5, "5.000", nameof(Swank.Configuration.Configuration.SampleIntegerFormat), "0.000")
            .Add((long?)5, "5").Add((byte?)5, "5.000", nameof(Swank.Configuration.Configuration.SampleIntegerFormat), "0.000")
            .Add((ulong)5, "5").Add((ulong)5, "5.000", nameof(Swank.Configuration.Configuration.SampleIntegerFormat), "0.000")
            .Add((ulong?)5, "5").Add((ulong?)5, "5.000", nameof(Swank.Configuration.Configuration.SampleIntegerFormat), "0.000")
            .Add(new DateTime(1985, 10, 22), new DateTime(1985, 10, 22).ToString("g"))
            .Add(new DateTime(1985, 10, 22), "10/22/1985", nameof(Swank.Configuration.Configuration.SampleDateTimeFormat), "MM/dd/yyy")
            .Add((DateTime?)new DateTime(1985, 10, 22), new DateTime(1985, 10, 22).ToString("g"))
            .Add((DateTime?)new DateTime(1985, 10, 22), "10/22/1985", nameof(Swank.Configuration.Configuration.SampleDateTimeFormat), "MM/dd/yyy")
            .Add(TimeSpan.FromMinutes(5), "0:05:00")
            .Add(TimeSpan.FromMinutes(5), "00:05", nameof(Swank.Configuration.Configuration.SampleTimeSpanFormat), @"hh\:mm")
            .Add((TimeSpan?)TimeSpan.FromMinutes(5), "0:05:00")
            .Add((TimeSpan?)TimeSpan.FromMinutes(5), "00:05", nameof(Swank.Configuration.Configuration.SampleTimeSpanFormat), @"hh\:mm")
            .Add(Guid.Empty, "00000000-0000-0000-0000-000000000000")
            .Add(Guid.Empty, "00000000000000000000000000000000", nameof(Swank.Configuration.Configuration.SampleGuidFormat), "n")
            .Add((Guid?)Guid.Empty, "00000000-0000-0000-0000-000000000000")
            .Add((Guid?)Guid.Empty, "00000000000000000000000000000000", nameof(Swank.Configuration.Configuration.SampleGuidFormat), "n")
            .Add(UriFormat.SafeUnescaped, "SafeUnescaped")
            .Add(UriFormat.SafeUnescaped, "SafeUnescaped", nameof(Swank.Configuration.Configuration.EnumFormat), EnumFormat.AsString)
            .Add((UriFormat?)UriFormat.SafeUnescaped, "SafeUnescaped")
            .Add((UriFormat?)UriFormat.SafeUnescaped, "SafeUnescaped", nameof(Swank.Configuration.Configuration.EnumFormat), EnumFormat.AsString));

        [Test]
        [TestCaseSource(nameof(FormatSampleValueTestCases))]
        public void Should_format_sample_value(object value, 
            string expected, string formatProperty, object format)
        {
            var configuration = new Swank.Configuration.Configuration();
            if (formatProperty.IsNotNullOrEmpty())
                configuration.SetProperty(formatProperty, format);
            var sample = value.ToSampleValueString(configuration);
            sample.ShouldEqual(expected);
        }

        private static readonly object[][] SampleValueTestCases = TestCaseSource.Create(x => x
            .Add<string>("fark").Add<decimal>("5.00").Add<decimal?>("5.00").Add<double>("5.00")
            .Add<double?>("5.00").Add<float>("5.00").Add<float?>("5.00").Add<bool>("true")
            .Add<bool?>("true").Add<byte>("5").Add<byte?>("5").Add<sbyte>("5")
            .Add<sbyte?>("5").Add<short>("5").Add<short?>("5").Add<ushort>("5")
            .Add<ushort?>("5").Add<int>("5").Add<int?>("5").Add<uint>("5")
            .Add<uint?>("5").Add<long>("5").Add<long?>("5").Add<ulong>("5")
            .Add<ulong?>("5").Add<DateTime>(new DateTime(1985, 10, 22).ToString("g"))
            .Add<DateTime?>(new DateTime(1985, 10, 22).ToString("g")).Add<TimeSpan>("0:05:00")
            .Add<TimeSpan?>("0:05:00").Add<Guid>("00000000-0000-0000-0000-000000000000")
            .Add<Guid?>("00000000-0000-0000-0000-000000000000").Add<UriFormat>("UriEscaped")
            .Add<UriFormat?>("UriEscaped"));

        [Test]       
        [TestCaseSource(nameof(SampleValueTestCases))]
        public void Should_return_sample_value(Type type, string expected)
        {
            var configuration = new Swank.Configuration.Configuration
            {
                SampleIntegerValue = 5,
                SampleBoolValue = true,
                SampleDateTimeValue = new DateTime(1985, 10, 22),
                SampleRealValue = 5,
                SampleTimeSpanValue = TimeSpan.FromMinutes(5),
                SampleStringValue = "fark"
            };
            type.GetSampleValue(configuration).ShouldEqual(expected);
        }

        public enum ImplicitEnum
        {
            Fark, Farker
        }

        public enum ExplicitEnum
        {
            Fark = 1, Farker = 2
        }

        [Test]
        public void Should_return_sample_enum_value(
            [Values(typeof(ImplicitEnum), typeof(ExplicitEnum))] Type type)
        {
            var configuration = new Swank.Configuration.Configuration
            {
                EnumFormat = EnumFormat.AsString
            };
            type.GetSampleValue(configuration).ShouldEqual("Fark");
        }
    }
}
