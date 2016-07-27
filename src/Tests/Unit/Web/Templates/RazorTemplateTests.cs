using System.IO;
using System.Reflection;
using NUnit.Framework;
using Should;
using Swank.Extensions;
using Swank.Web.Templates;

namespace Tests.Unit.Web.Templates
{
    [TestFixture]
    public class RazorTemplateTests
    {
        public class Model
        {
            public string LastName { get; set; }
        }

        private Swank.Configuration.Configuration _configuration;
        private readonly Model _model = new Model { LastName = "Farker" };
        private const string Template = "Fark @Model.LastName";
        private const string Rendered = "Fark Farker";
        private string _path;

        [SetUp]
        public void Setup()
        {
            _configuration = new Swank.Configuration.Configuration();
            _path = Path.GetTempFileName();
            File.WriteAllText(_path, Template);
        }

        [TearDown]
        public void TearDown()
        {
            File.Delete(_path);
        }

        [Test]
        public void should_read_string_from_virtual_path()
        {
            RazorTemplate.FromVirtualPath<Model>(_path, _configuration)
                .RenderString(_model).ShouldEqual(Rendered);
        }

        [Test]
        public void should_read_bytes_from_virtual_path()
        {
            RazorTemplate.FromVirtualPath<Model>(_path, _configuration)
                .RenderBytes(_model).ShouldEqual(Rendered.ToBytes());
        }

        [Test]
        public void should_read_string_from_resource()
        {
            RazorTemplate.FromResource<Model>("RazorTemplateTests",
                Assembly.GetExecutingAssembly().AsList(), _configuration)
                .RenderString(_model).ShouldEqual(Rendered);
        }

        [Test]
        public void should_read_bytes_from_resource()
        {
            RazorTemplate.FromResource<Model>("RazorTemplateTests",
                Assembly.GetExecutingAssembly().AsList(), _configuration)
                .RenderBytes(_model).ShouldEqual(Rendered.ToBytes());
        }

        [Test]
        public void should_read_string()
        {
            var result = RazorTemplate.FromString<Model>(Template, _configuration)
                .RenderString(_model);

            result.ShouldEqual(Rendered);
        }

        [Test]
        public void should_read_bytes()
        {
            
            RazorTemplate.FromString<Model>(Template, _configuration)
                .RenderBytes(_model).ShouldEqual(Rendered.ToBytes());
        }
    }
}
