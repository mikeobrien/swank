using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Should;
using Swank.Extensions;

namespace Tests.Unit.Extensions
{
    [TestFixture]
    public class TemplateExtensionTests
    {
        [Test]
        public void should_unwrap_paragraph_tags()
        {
            "<p><p>Oh<p> </p>hai</p></p>".UnwrapParagraphTags()
                .ShouldEqual("<p>Oh<p> </p>hai</p>");
        }

        [Test]
        [TestCase("", "")]
        [TestCase(null, null)]
        [TestCase("Oh [Hai](http://www.google.com)",
            "<p>Oh <a href=\"http://www.google.com\">Hai</a></p>")]
        [TestCase("Oh [{{someValue}}](mailto:{{someValue}})",
            "<p>Oh <a href=\"mailto:{{someValue}}\">{{someValue}}</a></p>")]
        public void should_transform_markdown_block(string text, string result)
        {
            text.TransformMarkdownBlock().ShouldEqual(result);
        }

        [Test]
        [TestCase("", "")]
        [TestCase(null, null)]
        [TestCase("Oh [Hai](http://www.google.com)",
             "Oh <a href=\"http://www.google.com\">Hai</a>")]
        [TestCase("Oh [{{someValue}}](mailto:{{someValue}})",
            "Oh <a href=\"mailto:{{someValue}}\">{{someValue}}</a>")]
        public void should_transform_markdown_inline(string text, string result)
        {
            text.TransformMarkdownInline().ShouldEqual(result);
        }

        [Test]
        [TestCase("", "")]
        [TestCase(null, null)]
        [TestCase("```javascript\r\nfark() {\r\n}\r\n```",
            "<pre class=\"javascript\"><code>fark() {\r\n}</code></pre>")]

        [TestCase("```\r\nfark() {\r\n}\r\n```",
            "<pre class=\"\"><code>fark() {\r\n}</code></pre>")]

        [TestCase("```javascript\nfark() {\n}\n```",
            "<pre class=\"javascript\"><code>fark() {\n}</code></pre>")]

        [TestCase("\r\n```javascript\nfark() {\n}\n```\r\n",
            "\r\n<pre class=\"javascript\"><code>fark() {\n}</code></pre>\r\n")]

        [TestCase("<div>\r\n```javascript\nfark() {\n}\n```\r\n</div>",
            "<div>\r\n<pre class=\"javascript\"><code>fark() {\n}</code></pre>\r\n</div>")]
        public void should_render_fenced_code_blocks(string text, string expected)
        {
            text.RenderMarkdownFencedCodeBlocks().ShouldEqual(expected);
        }

        [Test]
        [TestCase("", "")]
        [TestCase(null, null)]
        [TestCase("Fark", "fark")]
        [TestCase("Fark Farker", "fark-farker")]
        public void should_convert_text_to_fragment_id(string text, string expected)
        {
            text.ToFragmentId().ShouldEqual(expected);
        }

        [Test]
        public void should_replace_nbsp_with_space(
            [Values("&nbsp;", "&NBSP;")] string text)
        {
            text.ConvertNbspHtmlEntityToSpaces().ShouldEqual(" ");
        }

        [Test]
        public void should_replace_br_with_line_break(
            [Values("<br>", "<br/>", "<br />")] string text)
        {
            ("oh" + text.ConvertBrHtmlTagsToLineBreaks() + "hai").ShouldEqual("oh\r\nhai");
        }

        [Test]
        public void should_remove_whitespace()
        {
            "\r\r fark \r\n\r\n farker \n\n".RemoveWhitespace().ShouldEqual("farkfarker");
        }

        [Test]
        public void should_normalize_line_breaks()
        {
            "\rfark\r\nfarker\n".NormalizeLineBreaks().ShouldEqual("\r\nfark\r\nfarker\r\n");
        }

        [Test]
        public void should_convert_emojis_to_html()
        {
            "fark :trollface: farker :rage:".ConvertEmojisToHtml()
                .ShouldEqual("fark <img class=\"emoji trollface\" /> " +
                             "farker <img class=\"emoji rage\" />");
        }
    }
}
