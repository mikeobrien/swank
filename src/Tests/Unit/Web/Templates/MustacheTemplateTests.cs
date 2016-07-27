using NUnit.Framework;
using Should;
using Swank.Extensions;
using Swank.Web.Templates;

namespace Tests.Unit.Web.Templates
{
    [TestFixture]
    public class MustacheTemplateTests
    {
        private const string Template = "Fark {{LastName}}\r  \n<br>&nbsp;";
        private const string Rendered = "Fark Farker\r\n ";

        private Swank.Configuration.Configuration _configuration;

        [SetUp]
        public void Setup()
        {
            _configuration = new Swank.Configuration.Configuration();
        }

        [Test]
        public void should_read_string()
        {
            var result = MustacheTemplate.FromString(Template, _configuration)
                .RenderString(new { LastName = "Farker" });

            result.ShouldEqual(Rendered);
        }

        [Test]
        public void should_read_bytes()
        {
            MustacheTemplate.FromString(Template, _configuration)
                .RenderBytes(new { LastName = "Farker" }).ShouldEqual(Rendered.ToBytes());
        }
    }
}
