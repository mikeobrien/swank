using System;
using System.Linq;
using NUnit.Framework;
using Should;
using Swank.Configuration;
using Swank.Specification;
using Swank.Web.Handlers.Templates;
using Tests.Common;
using Tests.Unit.Specification;

namespace Tests.Unit.Web.Handlers.Templates
{
    [TestFixture]
    public class NamespaceDescriptionServiceTests
    {
        public class Request { }

        public class Response { }

        public class Controller
        {
            public Response Post(Request request)
            {
                throw new NotImplementedException();
            }
        }

        [Test]
        public void should_create_namespace_model_for_endpoints()
        {
            var namespaceModels = new NamespaceDescriptionService(
                ConfigurationDsl.CreateConfig())
                .Create(Builder.BuildSpec<Controller>());
            
            namespaceModels.Count.ShouldEqual(1);

            var child = namespaceModels.First();
            should_equal_namespace(child, 1, 0, "Web");

            child = child.Children.First();
            should_equal_namespace(child, 1, 0, "Web", "Handlers");

            child = child.Children.First();
            should_equal_namespace(child, 1, 0, "Web", "Handlers", "Templates");

            child = child.Children.First();
            should_equal_namespace(child, 1, 0, "Web", "Handlers", "Templates", "Controller");

            child = child.Children.First();
            should_equal_namespace(child, 0, 1, "Web", "Handlers", "Templates", "Controller", "Post");

            var endpoint = child.Endpoints.First();

            endpoint.Namespace.ShouldOnlyContain("Web", "Handlers", "Templates", "Controller", "Post");

            should_equal_type(endpoint.Request.Type, "Post", "Post", "Request");
            should_equal_type(endpoint.Response.Type, "Post", "Post", "Response");
        }

        [Test]
        public void should_create_namespace_model_for_modules_and_endpoints()
        {
            var namespaceModels = new NamespaceDescriptionService(
                ConfigurationDsl.CreateConfig(x => x.TemplateNamespaceIncludesModule()))
                .Create(Builder.BuildSpec<Controller>());

            namespaceModels.Count.ShouldEqual(1);

            var child = namespaceModels.First();
            should_equal_namespace(child, 1, 0, "Resources");

            child = child.Children.First();
            should_equal_namespace(child, 1, 0, "Resources", "Web");

            child = child.Children.First();
            should_equal_namespace(child, 1, 0, "Resources", "Web", "Handlers");

            child = child.Children.First();
            should_equal_namespace(child, 1, 0, "Resources", "Web", "Handlers", "Templates");

            child = child.Children.First();
            should_equal_namespace(child, 1, 0, "Resources", "Web", "Handlers", "Templates", "Controller");

            child = child.Children.First();
            should_equal_namespace(child, 0, 1, "Resources", "Web", "Handlers", "Templates", "Controller", "Post");

            var endpoint = child.Endpoints.First();

            endpoint.Namespace.ShouldOnlyContain("Web", "Handlers", "Templates", "Controller", "Post");

            should_equal_type(endpoint.Request.Type, "Post", "Post", "Request");
            should_equal_type(endpoint.Response.Type, "Post", "Post", "Response");
        }

        private void should_equal_type(DataType type,
            string @namespace, string fullNamespace, string logicalName)
        {
            type.FullNamespace.ShouldOnlyContain(fullNamespace);
            type.Namespace.ShouldEqual(@namespace);
            type.LogicalName.ShouldEqual(logicalName);
        }

        private void should_equal_namespace(NamespaceModel model, 
            int children, int endpoints, params string[] fullName)
        {
            model.Name.ShouldEqual(fullName.Last());
            model.FullName.ShouldOnlyContain(fullName);
            model.Endpoints.Count.ShouldEqual(endpoints);
            model.Children.Count.ShouldEqual(children);
        }
    }
}
