using System;
using System.Collections.Generic;
using NUnit.Framework;
using Should;
using Swank.Extensions;

namespace Tests.Unit.Extensions
{
    [TestFixture]
    public class XmlTests
    {
        [Test]
        [TestCase(typeof(string), "string")]
        [TestCase(typeof(bool), "boolean"), TestCase(typeof(bool?), "boolean")]
        [TestCase(typeof(decimal), "decimal"), TestCase(typeof(decimal?), "decimal")]
        [TestCase(typeof(double), "double"), TestCase(typeof(double?), "double")]
        [TestCase(typeof(float), "float"), TestCase(typeof(float?), "float")]
        [TestCase(typeof(byte), "unsignedByte"), TestCase(typeof(byte?), "unsignedByte")]
        [TestCase(typeof(sbyte), "byte"), TestCase(typeof(sbyte?), "byte")]
        [TestCase(typeof(short), "short"), TestCase(typeof(short?), "short")]
        [TestCase(typeof(ushort), "unsignedShort"), TestCase(typeof(ushort?), "unsignedShort")]
        [TestCase(typeof(int), "int"), TestCase(typeof(int?), "int")]
        [TestCase(typeof(uint), "unsignedInt"), TestCase(typeof(uint?), "unsignedInt")]
        [TestCase(typeof(long), "long"), TestCase(typeof(long?), "long")]
        [TestCase(typeof(ulong), "unsignedLong"), TestCase(typeof(ulong?), "unsignedLong")]
        [TestCase(typeof(DateTime), "dateTime"), TestCase(typeof(DateTime?), "dateTime")]
        [TestCase(typeof(TimeSpan), "duration"), TestCase(typeof(TimeSpan?), "duration")]
        [TestCase(typeof(Guid), "uuid"), TestCase(typeof(Guid?), "uuid")]
        [TestCase(typeof(char), "char"), TestCase(typeof(char?), "char")]
        [TestCase(typeof(Uri), "anyURI")]
        [TestCase(typeof(byte[]), "base64Binary")]
        [TestCase(typeof(int[]), "ArrayOfInt")]
        [TestCase(typeof(List<int>), "ArrayOfInt")]
        [TestCase(typeof(List<List<int>>), "ArrayOfArrayOfInt")]
        [TestCase(typeof(Dictionary<string, int>), "DictionaryOfInt")]
        [TestCase(typeof(Dictionary<string, Dictionary<string, int>>), "DictionaryOfDictionaryOfInt")]
        [TestCase(typeof(ArgumentException), "ArgumentException")]
        public void should_return_xml_name(Type type, string name)
        {
            type.GetXmlName(false).ShouldEqual(name);
        }

        [TestCase(typeof(ConsoleColor), true, "string"), TestCase(typeof(ConsoleColor?), true, "string")]
        [TestCase(typeof(ConsoleColor), false, "int"), TestCase(typeof(ConsoleColor?), false, "int")]
        public void should_return_enum_xml_name(Type type, bool enumAsString, string name)
        {
            type.GetXmlName(enumAsString).ShouldEqual(name);
        }
    }
}
