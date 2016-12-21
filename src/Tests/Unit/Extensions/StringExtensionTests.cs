using Swank.Extensions;
using NUnit.Framework;
using Should;
using Tests.Common;

namespace Tests.Unit.Extensions
{
    [TestFixture]
    public class StringExtensionTests
    {
        [Test]
        public void should_hash_string()
        {
            "oh hai".Hash().ShouldEqual("7e90827f576c594b6d604a94830d093d");
        }

        [Test]
        [TestCase("OhHai", "Oh_Hai", false, "_")]
        [TestCase("OhHai", "Oh-Hai", false, "-")]
        [TestCase("OhHai", "oh_hai", true, "_")]
        [TestCase("OhHai", "oh-hai", true, "-")]
        [TestCase("ohHai", "oh_Hai", false, "_")]
        [TestCase("ohHai", "oh-Hai", false, "-")]
        [TestCase("ohHai", "oh_hai", true, "_")]
        [TestCase("ohHai", "oh-hai", true, "-")]
        [TestCase("ABC", "abc", true, "-")]
        [TestCase("ABC", "ABC", false, "-")]
        [TestCase("IAm", "i-am", true, "-")]
        [TestCase("IAm", "I-Am", false, "-")]
        [TestCase("O", "O", false, "-")]
        [TestCase("O", "o", true, "-")]
        [TestCase("Oh", "oh", true, "-")]
        [TestCase("Oh", "Oh", false, "-")]
        [TestCase("", "", true, "-")]
        public void should_snake_case(string source, string result, bool lower, string seperator)
        {
            source.ToSeparatedCase(lower, seperator).ShouldEqual(result);
        }

        [Test]
        [TestCase(null, null)]
        [TestCase("", "")]
        [TestCase("f", "F")]
        [TestCase("FARK", "FARK")]
        [TestCase("fark", "Fark")]
        [TestCase("Fark", "Fark")]
        [TestCase("FarkFarker", "Fark Farker")]
        [TestCase("FarkJFarker", "Fark J Farker")]
        [TestCase("FarkNSAFarker", "Fark NSA Farker")]
        [TestCase("iFark", "I Fark")]
        public void Should_create_title_from_pascal_cased_string(
            string source, string expected)
        {
            source.ToTitleFromPascalCasing().ShouldEqual(expected);
        }

        [Test]
        [TestCase("f", "f")]
        [TestCase("FARK", "FARK")]
        [TestCase("fark", "fark")]
        [TestCase("Fark", "Fark")]
        [TestCase("FarkFarker", "Fark", "Farker")]
        [TestCase("FarkJFarker", "Fark", "J", "Farker")]
        [TestCase("FarkNSAFarker", "Fark", "NSA", "Farker")]
        [TestCase("iFark", "i", "Fark")]
        public void Should_split_pascal_cased_sentence(
            string source, params string[] expected)
        {
            source.SplitPascalCasedSentence().ShouldOnlyContain(expected);
        }

        [Test]
        [TestCase("", "")]
        [TestCase(null, null)]
        [TestCase("URLValue", "urlValue")]
        [TestCase("URL", "url")]
        [TestCase("ID", "id")]
        [TestCase("I", "i")]
        [TestCase("Person", "person")]
        [TestCase("iPhone", "iPhone")]
        [TestCase("IPhone", "iPhone")]
        [TestCase("I Phone", "i Phone")]
        [TestCase("I  Phone", "i  Phone")]
        [TestCase(" IPhone", " IPhone")]
        [TestCase(" IPhone ", " IPhone ")]
        [TestCase("IsCIA", "isCIA")]
        [TestCase("VmQ", "vmQ")]
        [TestCase("Xml2Json", "xml2Json")]
        [TestCase("SnAkEcAsE", "snAkEcAsE")]
        [TestCase("SnA__kEcAsE", "snA__kEcAsE")]
        [TestCase("SnA__ kEcAsE", "snA__ kEcAsE")]
        [TestCase("already_snake_case_ ", "already_snake_case_ ")]
        [TestCase("IsJSONProperty", "isJSONProperty")]
        [TestCase("SHOUTING_CASE", "shoutinG_CASE")]
        [TestCase("9999-12-31T23:59:59.9999999Z", "9999-12-31T23:59:59.9999999Z")]
        [TestCase("Hi!! This is text. Time to test.", "hi!! This is text. Time to test.")]
        public void should_camel_case(string source, string result)
        {
            source.ToCamelCase().ShouldEqual(result);
        }

        [Test]
        [TestCase(null, null)]
        [TestCase("", "")]
        [TestCase("  fark   \r\n   farker \r  yada   \n  ", "fark farker yada")]
        public void Should_join_lines(string source, string expected)
        {
            source.JoinLines().ShouldEqual(expected);
        }
    }
}
