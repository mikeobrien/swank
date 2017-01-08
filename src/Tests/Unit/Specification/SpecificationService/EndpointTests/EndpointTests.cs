using System;
using System.Linq.Expressions;
using Swank.Extensions;
using NUnit.Framework;
using Should;
using Tests.Common;

namespace Tests.Unit.Specification.SpecificationService.EndpointTests
{
    public class EndpointTests
    {
        [Test]
        public void should_generate_id_based_on_the_url()
        {
            var endpoint = Builder.BuildSpecAndGetEndpoint<EndpointDescriptions
                .NoDescriptionController>(x => x.Get(null));

            var url = SpecExtensions.ToTestUrl<EndpointDescriptions
                .NoDescriptionController>(x => x.Get(null));
            endpoint.Id.ShouldEqual($"GET{url}".Hash());
            endpoint.UrlTemplate.ShouldEqual(url);
        }

        [Test]
        public void should_set_default_controller_description_for_get_endpoint_with_no_description()
        {
            should_set_default_controller_description_for_endpoint_with_no_description
                <EndpointDescriptions.NoDescriptionController>(x => x.Get(null), "Get", "GET");
        }

        [Test]
        public void should_set_default_controller_description_for_post_endpoint_with_no_description()
        {
            should_set_default_controller_description_for_endpoint_with_no_description
                <EndpointDescriptions.NoDescriptionController>(x => x.Post(null), "Post", "POST");
        }
        
        public void should_set_default_controller_description_for_endpoint_with_no_description
            <TController>(Expression<Func<TController, object>> action, string name, string verb)
        {
            var endpoint = Builder.BuildSpecAndGetEndpoint(action);
            endpoint.Name.ShouldEqual(name);
            endpoint.Comments.ShouldBeNull();
            endpoint.Method.ShouldEqual(verb);
            endpoint.MethodName.ShouldEqual(name);
            endpoint.Namespace.ShouldOnlyContain("Specification", "SpecificationService", 
                "EndpointTests", "EndpointDescriptions", "NoDescriptionController", name);
            endpoint.UrlTemplate.ShouldEqualUrl(action);
        }

        [Test]
        public void should_not_set_embedded_controller_description_when_resource_attribute_is_applied()
        {
            var resource = Builder.BuildSpec<ControllerResource.Controller>()
                .GetResource<ControllerResource.Controller>(x => x.Get(null));

            resource.Name.ShouldEqual("Some Controller");
            resource.Comments.ShouldEqual("<p><strong>This is a resource</strong></p>");

            var endpoint = resource.Endpoints[0];
            endpoint.Name.ShouldEqual("Get");
            endpoint.Comments.ShouldBeNull();
            endpoint.Method.ShouldEqual("GET");
            endpoint.UrlTemplate.ShouldEqualUrl<ControllerResource
                .Controller>(x => x.Get(null));
        }

        [Test]
        public void should_set_controller_description_for_get_endpoint()
        {
            var endpoint = Builder.BuildSpecAndGetEndpoint<EndpointDescriptions
                .ControllerDescription.Controller>(x => x.Get(null));
            endpoint.Name.ShouldEqual("Some get Controller name");
            endpoint.Comments.ShouldEqual("<p>Some get <strong>Controller</strong> description</p>");
            endpoint.Method.ShouldEqual("GET");
            endpoint.MethodName.ShouldEqual("Get");
            endpoint.Namespace.ShouldOnlyContain("Specification", "SpecificationService",
                "EndpointTests", "EndpointDescriptions", "ControllerDescription", 
                "Controller", "Get");
            endpoint.UrlTemplate.ShouldEqualUrl<EndpointDescriptions
                .ControllerDescription.Controller>(x => x.Get(null));
        }

        [Test]
        public void should_set_Controller_description_for_post_endpoint()
        {
            var endpoint = Builder.BuildSpecAndGetEndpoint<EndpointDescriptions
                .ControllerDescription.Controller>(x => x.Post(null));
            endpoint.Name.ShouldEqual("Some post Controller name");
            endpoint.Comments.ShouldEqual("<p>Some post <strong>Controller</strong> description</p>");
            endpoint.Method.ShouldEqual("POST");
            endpoint.UrlTemplate.ShouldEqualUrl<EndpointDescriptions
                .ControllerDescription.Controller>(x => x.Post(null));
        }

        [Test]
        public void should_set_Controller_description_for_put_endpoint()
        {
            var endpoint = Builder.BuildSpecAndGetEndpoint<EndpointDescriptions
                .ControllerDescription.Controller>(x => x.Put(null));
            endpoint.Name.ShouldEqual("Some put Controller name");
            endpoint.Comments.ShouldEqual("<p>Some put <strong>Controller</strong> description</p>");
            endpoint.Method.ShouldEqual("PUT");
            endpoint.UrlTemplate.ShouldEqualUrl<EndpointDescriptions
                .ControllerDescription.Controller>(x => x.Put(null));
        }

        [Test]
        public void should_set_Controller_description_for_delete_endpoint()
        {
            var endpoint = Builder.BuildSpecAndGetEndpoint<EndpointDescriptions
                .ControllerDescription.Controller>(x => x.Delete(null));
            endpoint.Name.ShouldEqual("Some delete Controller name");
            endpoint.Comments.ShouldEqual("<p>Some delete <strong>Controller</strong> description</p>");
            endpoint.Method.ShouldEqual("DELETE");
            endpoint.UrlTemplate.ShouldEqualUrl<EndpointDescriptions.ControllerDescription
                .Controller>(x => x.Delete(null));
        }

        [Test]
        public void should_set_embedded_markdown_endpoint_description()
        {
            var endpoint = Builder.BuildSpecAndGetEndpoint<EndpointDescriptions
                .ActionDescription.EmbeddedDescriptionController>(x => x.Get(null));
            endpoint.Name.ShouldEqual("Get");
            endpoint.Comments.ShouldEqual("<p><strong>An " +
                "embedded action markdown description</strong></p>");
            endpoint.Method.ShouldEqual("GET");
            endpoint.UrlTemplate.ShouldEqualUrl<EndpointDescriptions.ActionDescription
                .EmbeddedDescriptionController>(x => x.Get(null));
        }

        [Test]
        public void should_set_endpoint_description_for_get_endpoint()
        {
            var endpoint = Builder.BuildSpecAndGetEndpoint<EndpointDescriptions
                .ActionDescription.Controller>(x => x.Get(null));
            endpoint.Name.ShouldEqual("Some get action name");
            endpoint.Comments.ShouldEqual("<p>Some get <strong>action</strong> description</p>");
            endpoint.Method.ShouldEqual("GET");
            endpoint.UrlTemplate.ShouldEqualUrl<EndpointDescriptions
                .ActionDescription.Controller>(x => x.Get(null));
        }

        [Test]
        public void should_set_endpoint_description_for_post_endpoint()
        {
            var endpoint = Builder.BuildSpecAndGetEndpoint<EndpointDescriptions
                .ActionDescription.Controller>(x => x.Post(null));
            endpoint.Name.ShouldEqual("Some post action name");
            endpoint.Comments.ShouldEqual("<p>Some post <strong>action</strong> description</p>");
            endpoint.Method.ShouldEqual("POST");
            endpoint.UrlTemplate.ShouldEqualUrl<EndpointDescriptions
                .ActionDescription.Controller>(x => x.Post(null));
        }

        [Test]
        public void should_set_endpoint_description_for_put_endpoint()
        {
            var endpoint = Builder.BuildSpecAndGetEndpoint<EndpointDescriptions
                .ActionDescription.Controller>(x => x.Put(null));
            endpoint.Name.ShouldEqual("Some put action name");
            endpoint.Comments.ShouldEqual("<p>Some put <strong>action</strong> description</p>");
            endpoint.Method.ShouldEqual("PUT");
            endpoint.UrlTemplate.ShouldEqualUrl<EndpointDescriptions
                .ActionDescription.Controller>(x => x.Put(null));
        }

        [Test]
        public void should_set_endpoint_description_for_delete_endpoint()
        {
            var endpoint = Builder.BuildSpecAndGetEndpoint<EndpointDescriptions
                .ActionDescription.Controller>(x => x.Delete(null));
            endpoint.Name.ShouldEqual("Some delete action name");
            endpoint.Comments.ShouldEqual("<p>Some delete <strong>action</strong> description</p>");
            endpoint.Method.ShouldEqual("DELETE");
            endpoint.UrlTemplate.ShouldEqualUrl<EndpointDescriptions
                .ActionDescription.Controller>(x => x.Delete(null));
        }

        [Test]
        public void should_not_show_hidden_endpoints()
        {
            Builder.BuildSpecAndGetEndpoint<HiddenEndpointAttributes
                .HiddenActionController>(x => x.Get(null)).ShouldBeNull();
        }

        [Test]
        public void should_not_show_endpoints_in_hidden_Controllers()
        {
            Builder.BuildSpecAndGetEndpoint<HiddenEndpointAttributes
                .HiddenController>(x => x.Get(null)).ShouldBeNull();
        }

        [Test]
        public void should_show_endpoints_not_marked_hidden()
        {
            Builder.BuildSpecAndGetEndpoint<HiddenEndpointAttributes
                .VisibleController>(x => x.Get(null)).ShouldNotBeNull();
        }

        [Test]
        public void should_set_actions_to_unsecure_by_default()
        {
            Builder.BuildSpecAndGetEndpoint<EndpointDescriptions
                .NoDescriptionController>(x => x.Get(null))
                .Secure.ShouldBeFalse();
        }

        [Test]
        public void should_set_actions_to_secure_when_Controller_flagged()
        {
            Builder.BuildSpecAndGetEndpoint<EndpointDescriptions
                .SecureDescription.SecureController>(x => x.Get(null))
                .Secure.ShouldBeTrue();
        }

        [Test]
        public void should_set_actions_to_secure_when_endpoint_flagged()
        {
            Builder.BuildSpecAndGetEndpoint<EndpointDescriptions
                .SecureDescription.SecureActionController>(x => x.Get(null))
                .Secure.ShouldBeTrue();
        }

        [Test]
        public void should_not_mark_request_or_response_binary_by_default()
        {
            var result = Builder.BuildSpecAndGetEndpoint<EndpointDescriptions
                .NoDescriptionController>(x => x.Get(null));
            result.Request.IsBinary.ShouldBeFalse();
            result.Response.IsBinary.ShouldBeFalse();
        }

        [Test]
        public void should_not_mark_request_as_binary()
        {
            var result = Builder.BuildSpecAndGetEndpoint<EndpointDescriptions
                .BinaryDescription.BinaryRequestController>(x => x.Get(null));
            result.Request.IsBinary.ShouldBeTrue();
            result.Response.IsBinary.ShouldBeFalse();
        }

        [Test]
        public void should_not_mark_response_as_binary()
        {
            var result = Builder.BuildSpecAndGetEndpoint<EndpointDescriptions
                .BinaryDescription.BinaryResponseController>(x => x.Get(null));
            result.Request.IsBinary.ShouldBeFalse();
            result.Response.IsBinary.ShouldBeTrue();
        }
    }
}