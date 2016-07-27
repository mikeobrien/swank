using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Should;
using Swank.Extensions;
using Tests.Common;

namespace Tests.Unit.Description
{
    [TestFixture]
    public class XmlCommentsTests
    {
        private Swank.Description.XmlComments _comments;

        [SetUp]
        public void Setup()
        {
            _comments = new Swank.Description.XmlComments(
                new Swank.Configuration.Configuration()
                    .Configure(x => x.AddXmlComments()));
        }

        public enum Enum
        {
            /// <summary>field summary</summary>
            /// <remarks>field remarks</remarks>
            Option
        }

        /// <summary>type summary</summary>
        /// <remarks>type remarks</remarks>
        public class Type<T>
        {
            /// <summary>property summary</summary>
            /// <remarks>property remarks</remarks>
            public string Property { get; set; }

            /// <summary>method summary</summary>
            /// <remarks>method remarks</remarks>
            /// <param name="parameter1">param 1 summary</param>
            /// <param name="parameter2">param 2 summary</param>
            /// <param name="parameter3">param 3 summary</param>
            /// <returns>return summary</returns>
            public void Method<T1, T2>(
                Dictionary<int?, List<T2>> parameter1, 
                string parameter2, T1 parameter3) { }
        }

        [Test]
        public void should_get_type_comments()
        {
            var comments = _comments.GetType(typeof(Type<string>));
            comments.Assembly.ShouldEqual("Tests");
            comments.Name.ShouldEqual("T:Tests.Unit" +
                ".Description.XmlCommentsTests.Type`1");
            comments.Summary.ShouldEqual("type summary");
            comments.Remarks.ShouldEqual("type remarks");
            comments.Returns.ShouldBeNull();
            comments.Parameters.ShouldBeEmpty();
        }

        [Test]
        public void should_get_property_comments()
        {
            var comments = _comments.GetProperty(typeof(Type<string>).GetProperty("Property"));
            comments.Assembly.ShouldEqual("Tests");
            comments.Name.ShouldEqual("P:Tests.Unit" +
                ".Description.XmlCommentsTests.Type`1.Property");
            comments.Summary.ShouldEqual("property summary");
            comments.Remarks.ShouldEqual("property remarks");
            comments.Returns.ShouldBeNull();
            comments.Parameters.ShouldBeEmpty();
        }

        [Test]
        public void should_get_field_comments()
        {
            var comments = _comments.GetField(typeof(Enum).GetField("Option"));
            comments.Assembly.ShouldEqual("Tests");
            comments.Name.ShouldEqual("F:Tests.Unit.Description" +
                ".XmlCommentsTests.Enum.Option");
            comments.Summary.ShouldEqual("field summary");
            comments.Remarks.ShouldEqual("field remarks");
            comments.Returns.ShouldBeNull();
            comments.Parameters.ShouldBeEmpty();
        }

        [Test]
        public void should_get_method_comments()
        {
            var comments = _comments.GetMethod(typeof(Type<string>).GetMethod("Method"));
            comments.Assembly.ShouldEqual("Tests");
            comments.Name.ShouldEqual("M:Tests.Unit.Description" +
                ".XmlCommentsTests.Type`1.Method``2(System.Collections.Generic" +
                ".Dictionary{System.Nullable{System.Int32},System.Collections" +
                ".Generic.List{``1}},System.String,``0)");
            comments.Summary.ShouldEqual("method summary");
            comments.Remarks.ShouldEqual("method remarks");
            comments.Returns.ShouldEqual("return summary");
            comments.Parameters.Count.ShouldEqual(3);
            comments.Parameters["parameter1"].ShouldEqual("param 1 summary");
            comments.Parameters["parameter2"].ShouldEqual("param 2 summary");
            comments.Parameters["parameter3"].ShouldEqual("param 3 summary");
        }
    }
}
