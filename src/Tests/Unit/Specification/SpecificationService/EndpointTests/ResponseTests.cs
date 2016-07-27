using System.Collections.Generic;
using NUnit.Framework;
using Should;
using Tests.Common;

namespace Tests.Unit
    .Specification.SpecificationService.EndpointTests
{
    public class ResponseTests
    {
        private List<Swank.Specification.Module> _spec;

        [SetUp]
        public void Setup()
        {
            _spec = Builder.BuildSpec<ResponseDescriptions.Controller>();
        }

        [Test]
        public void should_set_get_output_type_description()
        {
            _spec.GetEndpoint<ResponseDescriptions.Controller>(
                x => x.Get(null)).Response.Comments
                    .ShouldEqual("<p>Some get <strong>response</strong> description</p>");
                
        }

        [Test]
        public void should_set_post_output_type_description()
        {
            _spec.GetEndpoint<ResponseDescriptions.Controller>(
                x => x.Post(null)).Response.Comments
                    .ShouldEqual("<p>Some post <strong>response</strong> description</p>");
                
        }

        [Test]
        public void should_set_put_output_type_description()
        {
            _spec.GetEndpoint<ResponseDescriptions.Controller>(
                x => x.Put(null)).Response.Comments
                    .ShouldEqual("<p>Some put <strong>response</strong> description</p>");
                
        }

        [Test]
        public void should_set_delete_output_type_description()
        {
            _spec.GetEndpoint<ResponseDescriptions.Controller>(
                x => x.Delete(null)).Response.Comments
                    .ShouldEqual("<p>Some delete <strong>response</strong> description</p>");
                
        }

        [Test]
        public void should_set_the_name_for_output_types()
        {
            _spec.GetEndpoint<ResponseDescriptions.Controller>(
                x => x.Get(null)).Response.Type
                    .Name.ShouldEqual("Object");

            _spec.GetEndpoint<ResponseDescriptions.Controller>(
                x => x.Post(null)).Response.Type
                    .Name.ShouldEqual("Object");

            _spec.GetEndpoint<ResponseDescriptions.Controller>(
                x => x.Put(null)).Response.Type
                    .Name.ShouldEqual("Object");

            _spec.GetEndpoint<ResponseDescriptions.Controller>(
                x => x.Delete(null)).Response.Type
                    .Name.ShouldEqual("Object");
        }

        [Test]
        public void should_set_output_type_default_collection_name_and_datatype()
        {
            var response = _spec.GetEndpoint<ResponseDescriptions
                .CollectionController>(x => x.Get(null)).Response;

            response.Comments.ShouldBeNull();
            response.Type.Name.ShouldEqual("ArrayOfResponseItem");
            response.Type.IsArray.ShouldEqual(true);
        }

        [Test]
        public void should_set_output_type_default_collection_name_of_inherited_collection_and_datatype()
        {
            var response = _spec.GetEndpoint<ResponseDescriptions
                .InheritedCollectionController>(x => x.Get(null)).Response;

            response.Comments.ShouldBeNull();
            response.Type.Name.ShouldEqual("ArrayOfResponseItem");
            response.Type.IsArray.ShouldEqual(true);
        }

        [Test]
        public void should_set_output_type_name_to_the_xml_type_name()
        {
            var response = _spec.GetEndpoint<ResponseDescriptions
                .OverridenRequestController>(x => x.Get(null)).Response;

            response.Comments.ShouldBeNull();
            response.Type.Name.ShouldEqual("NewItemName");
        }

        [Test]
        public void should_set_output_type_collection_name_to_the_xml_type_name()
        {
            var response = _spec.GetEndpoint<ResponseDescriptions
                .OverridenCollectionController>(x => x.Get(null)).Response;

            response.Comments.ShouldBeNull();
            response.Type.Name.ShouldEqual("NewCollectionName");
            response.Type.IsArray.ShouldEqual(true);
        }
    }
}