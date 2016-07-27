using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Swank.Extensions;
using NUnit.Framework;
using Should;
using System.Linq;
using System.Xml.Serialization;
using Tests.Common;
using Tests.Unit.Extensions.RootNamespace;
using Tests.Unit.Extensions.RootNamespace.ChildNamespace;

namespace Tests.Unit.Extensions
{
    public class NotInNamespace { }

    namespace RootNamespace
    {
        public class InNamespace
        {
            public class InNamespaceNested { }
        }

        namespace ChildNamespace
        {
            public class UnderNamespace
            {
                public class UnderNamespaceNested { }
            }
        }
    }

    [TestFixture]
    public class ReflectionExtensionTests
    {
        public class NestedInNamespace { }

        [Test]
        [TestCase(typeof(NotInNamespace), false)]
        [TestCase(typeof(InNamespace), true)]
        [TestCase(typeof(InNamespace.InNamespaceNested), true)]
        [TestCase(typeof(UnderNamespace), true)]
        [TestCase(typeof(UnderNamespace.UnderNamespaceNested), true)]
        public void Should_indicate_if_in_namespace(Type type, bool inNamespace)
        {
            type.IsInNamespace(typeof(InNamespace).Namespace).ShouldEqual(inNamespace);
        }

        public class NoAttributeClass { }

        [Description("fark")]
        public class AttributeClass { }

        [Test]
        [TestCase(typeof(AttributeClass), true)]
        [TestCase(typeof(NoAttributeClass), false)]
        public void Should_indicate_if_type_has_attribute(Type type, bool attribute)
        {
            type.HasAttribute<DescriptionAttribute>().ShouldEqual(attribute);
        }

        public class MethodAttributeClass
        {
            public void MethodWithoutAttribute() { }
            [Description("fark")]
            public void MethodWithAttribute() { }
        }

        [Test]
        [TestCase(nameof(MethodAttributeClass.MethodWithAttribute), true)]
        [TestCase(nameof(MethodAttributeClass.MethodWithoutAttribute), false)]
        public void Should_indicate_if_method_has_attribute(string method, bool attribute)
        {
            typeof(MethodAttributeClass).GetMethod(method)
                .HasAttribute<DescriptionAttribute>().ShouldEqual(attribute);
        }

        public class PropertyAttributeClass
        {
            public string PropertyWithoutAttribute { get; }
            [XmlAnyAttribute]
            public string PropertyWithAttribute { get; }
        }

        [Test]
        [TestCase(nameof(PropertyAttributeClass.PropertyWithoutAttribute), false)]
        [TestCase(nameof(PropertyAttributeClass.PropertyWithAttribute), true)]
        public void Should_indicate_if_property_has_attribute(string property, bool attribute)
        {
            typeof(PropertyAttributeClass).GetProperty(property, 
                    BindingFlags.Public | BindingFlags.Instance)
                .HasAttribute<XmlAnyAttributeAttribute>().ShouldEqual(attribute);
        }

        public class FieldAttributeClass
        {
            public string FieldWithoutAttribute;
            [XmlAnyAttribute]
            public string FieldWithAttribute;
        }

        [Test]
        [TestCase(nameof(FieldAttributeClass.FieldWithoutAttribute), false)]
        [TestCase(nameof(FieldAttributeClass.FieldWithAttribute), true)]
        public void Should_indicate_if_field_has_attribute(string property, bool attribute)
        {
            typeof(FieldAttributeClass).GetField(property,
                    BindingFlags.Public | BindingFlags.Instance)
                .HasAttribute<XmlAnyAttributeAttribute>().ShouldEqual(attribute);
        }

        [Test]
        [TestCase(typeof(int), true)]
        [TestCase(typeof(string), true)]
        [TestCase(typeof(UriFormat), true)]
        [TestCase(typeof(decimal), true)]
        [TestCase(typeof(DateTime), true)]
        [TestCase(typeof(TimeSpan), true)]
        [TestCase(typeof(Guid), true)]
        [TestCase(typeof(Uri), true)]
        [TestCase(typeof(int?), true)]
        [TestCase(typeof(ApplicationIdentity), false)]
        public void Should_determine_if_type_is_simple_type(Type type, bool simple)
        {
            type.IsSimpleType().ShouldEqual(simple);
        }

        [Test]
        [TestCase(typeof(string), typeof(IComparable), true)]
        [TestCase(typeof(List<string>), typeof(IList<>), true)]
        [TestCase(typeof(string), typeof(IDisposable), false)]
        public void Should_indicate_if_type_implements_an_interface(
            Type type, Type @interface, bool implements)
        {
            type.Implements(@interface).ShouldEqual(implements);
        }

        [Test]
        [TestCase(typeof(int))]
        [TestCase(typeof(int?))]
        [TestCase(typeof(Dictionary<string, int>))]
        [TestCase(typeof(IDictionary<string, int>))]
        [TestCase(typeof(Dictionary<string, Dictionary<string, int>>))]
        [TestCase(typeof(int[]))]
        [TestCase(typeof(List<int>))]
        [TestCase(typeof(IList<int>))]
        [TestCase(typeof(List<List<int>>))]
        public void should_unwrap_types(Type wrappedType)
        {
            wrappedType.UnwrapType().ShouldEqual(typeof(int));
        }

        [Test]
        [TestCase(typeof(int?), true)]
        [TestCase(typeof(string), false)]
        public void Should_indicate_if_type_is_nullable(
            Type type, bool nullable)
        {
            type.IsNullable().ShouldEqual(nullable);
        }

        [Test]
        [TestCase(typeof(int?), typeof(int))]
        [TestCase(typeof(string), typeof(string))]
        public void Should_get_underlying_nullable_type(
            Type type, Type underlying)
        {
            type.GetNullableUnderlyingType().ShouldEqual(underlying);
        }

        private class ListSubClass : List<string> { }
        private class ListSubClass<T> : List<string> { }

        [Test]
        [TestCase(typeof(List<string>), true)]
        [TestCase(typeof(IList<string>), true)]
        [TestCase(typeof(ListSubClass), false)]
        [TestCase(typeof(ListSubClass<string>), false)]
        public void should_determine_if_a_type_is_a_list_type(Type type, bool isListType)
        {
            type.IsListType().ShouldEqual(isListType);
        }

        [Test]
        [TestCase(typeof(List<string>), true)]
        [TestCase(typeof(IList<string>), false)]
        [TestCase(typeof(ListSubClass), true)]
        [TestCase(typeof(ListSubClass<string>), true)]
        public void should_determine_if_a_type_implements_a_list_type(
            Type type, bool implementsListType)
        {
            type.ImplementsListType().ShouldEqual(implementsListType);
        }

        [Test]
        [TestCase(typeof(List<string>), true)]
        [TestCase(typeof(IList<string>), true)]
        [TestCase(typeof(ListSubClass), true)]
        [TestCase(typeof(ListSubClass<string>), true)]
        [TestCase(typeof(string), false)]
        public void should_determine_if_a_type_is_a_list(Type type, bool isList)
        {
            type.IsList().ShouldEqual(isList);
        }

        [Test]
        [TestCase(typeof(List<string>), typeof(IList<string>))]
        [TestCase(typeof(IList<string>), typeof(IList<string>))]
        [TestCase(typeof(ListSubClass), typeof(IList<string>))]
        [TestCase(typeof(ListSubClass<string>), typeof(IList<string>))]
        public void should_return_types_list_interface(Type type, Type @interface)
        {
            type.GetListInterface().ShouldEqual(@interface);
        }

        [Test]
        [TestCase(typeof(List<string>), typeof(string))]
        [TestCase(typeof(IList<string>), typeof(string))]
        [TestCase(typeof(ListSubClass), typeof(string))]
        [TestCase(typeof(ListSubClass<string>), typeof(string))]
        [TestCase(typeof(string[]), typeof(string))]
        [TestCase(typeof(string), null)]
        public void should_return_list_element_type(Type type, Type elementType)
        {
            type.GetListElementType().ShouldEqual(elementType);
        }

        [Test]
        [TestCase(typeof(Hashtable), true)]
        [TestCase(typeof(Dictionary<string, string>), true)]
        [TestCase(typeof(string), false)]
        public void should_determine_if_a_type_is_a_dictionary_type(Type type, bool isDictionaryType)
        {
            type.IsDictionary().ShouldEqual(isDictionaryType);
        }

        [Test]
        [TestCase(typeof(Hashtable), true)]
        [TestCase(typeof(Dictionary<string, string>), false)]
        [TestCase(typeof(string), false)]
        public void should_determine_if_a_type_is_a_non_generic_dictionary_type(
            Type type, bool isNonGenericDictionaryType)
        {
            type.IsNonGenericDictionary().ShouldEqual(isNonGenericDictionaryType);
        }

        [Test]
        [TestCase(typeof(Hashtable) ,false)]
        [TestCase(typeof(Dictionary<string, string>), true)]
        [TestCase(typeof(string), false)]
        public void should_determine_if_a_type_is_a_generic_dictionary_type(
            Type type, bool isGenericDictionaryType)
        {
            type.IsGenericDictionary().ShouldEqual(isGenericDictionaryType);
        }

        [Test]
        [TestCase(typeof(IDictionary), false)]
        [TestCase(typeof(IDictionary<string, string>), true)]
        [TestCase(typeof(IList<string>), false)]
        public void should_determine_if_a_type_is_a_generic_dictionary_interface(
            Type type, bool isGenericDictionaryType)
        {
            type.IsGenericDictionaryInterface().ShouldEqual(isGenericDictionaryType);
        }

        [Test]
        [TestCase(typeof(IDictionary<int, string>))]
        [TestCase(typeof(Dictionary<int, string>))]
        public void Should_get_generic_dictionary_types(Type type)
        {
            var result = type.GetGenericDictionaryTypes();
            result.Key.ShouldEqual(typeof(int));
            result.Value.ShouldEqual(typeof(string));
        }

        enum SomeEnum { Oh, Hai }

        [Test]
        public void should_get_enum_values()
        {
            var values = typeof(SomeEnum).GetEnumOptions();
            values.Count().ShouldEqual(2);
            values[0].Name.ShouldEqual("Oh");
            values[1].Name.ShouldEqual("Hai");
        }

        [Test]
        public void should_get_nullable_enum_values()
        {
            var values = typeof(SomeEnum?).GetEnumOptions();
            values.Count().ShouldEqual(2);
            values[0].Name.ShouldEqual("Oh");
            values[1].Name.ShouldEqual("Hai");
        }

        [Test]
        public void should_return_markdown_resource()
        {
            Assembly.GetExecutingAssembly().FindResourceNamed(
                "Tests.Unit.Extensions.EmbeddedStringResource.txt")
                    .ShouldEqual("string resource");
        }

        [Test]
        public void should_return_null_when_no_resource_found()
        {
            Assembly.GetExecutingAssembly().FindResourceNamed(
                "fark").ShouldBeNull();
        }

        [Test]
        [TestCase("", "")]
        [TestCase("Fark", "Fark")]
        [TestCase("Fark.Farker", "Fark.Farker")]
        [TestCase("Farkety.Fark.Farker", "Farkety\\Fark.Farker")]
        public void should_convert_resource_name_to_path(
            string name, string expected)
        {
            name.ConvertResourceNameToPath().ShouldEqual(expected);
        }
    }
}
