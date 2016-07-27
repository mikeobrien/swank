using System;
using System.Collections.Generic;
using NUnit.Framework;
using Should;
using Tests.Common;

namespace Tests.Unit.Specification.SpecificationService.EndpointTests
{
    public class RequestTests
    {
        private List<Swank.Specification.Module> _spec;

        [SetUp]
        public void Setup()
        {
            _spec = Builder.BuildSpec<InputTypeDescriptions.Controller>();
        }

        [Test]
        public void should_set_post_input_type_description()
        {
            var request = _spec.GetEndpoint<InputTypeDescriptions
                .Controller>(x => x.Post(null)).Request;

            request.Comments.ShouldEqual("<p>Some post <strong>request</strong> description</p>");
            request.Type.IsComplex.ShouldEqual(true);
        }

        [Test]
        public void should_set_put_input_type_description()
        {
            var request = _spec.GetEndpoint<InputTypeDescriptions
                .Controller>(x => x.Put(null)).Request;

            request.Comments.ShouldEqual("<p>Some put <strong>request</strong> description</p>");
            request.Type.IsComplex.ShouldEqual(true);
        }

        [Test]
        public void should_set_delete_input_type_description()
        {
            var request = _spec.GetEndpoint<InputTypeDescriptions
                .Controller>(x => x.Delete(null)).Request;

            request.Comments.ShouldEqual("<p>Some delete <strong>request</strong> description</p>");
            request.Type.IsComplex.ShouldEqual(true);
        }

        [Test]
        public void should_set_the_datatype_for_post_input_post_types()
        {
            _spec.GetEndpoint<InputTypeDescriptions.Controller>(x => x.Post(null))
                .Request.Type.Name.ShouldEqual("Object");
        }

        [Test]
        public void should_set_the_datatype_for_post_input_put_types()
        {
            _spec.GetEndpoint<InputTypeDescriptions.Controller>(x => x.Post(null))
                .Request.Type.Name.ShouldEqual("Object");
        }

        [Test]
        public void should_not_set_input_type_for_get()
        {
            _spec.GetEndpoint<InputTypeDescriptions.Controller>(x => x.Get(null))
                .Request.Type.ShouldBeNull();
        }

        [Test]
        public void should_set_input_type_default_collection_name_and_datatype()
        {
            var request = _spec.GetEndpoint<InputTypeDescriptions
                .CollectionController>(x => x.Post(null)).Request;

            request.Comments.ShouldBeNull();
            request.Type.Name.ShouldEqual("ArrayOfRequestItem");
            request.Type.IsArray.ShouldEqual(true);
        }

        [Test]
        public void should_set_input_type_default_collection_name_of_inherited_collection_and_datatype()
        {
            var request = _spec.GetEndpoint<InputTypeDescriptions
                .InheritedCollectionController>(x => x.Post(null)).Request;

            request.Comments.ShouldBeNull();
            request.Type.Name.ShouldEqual("ArrayOfRequestItem");
            request.Type.IsArray.ShouldEqual(true);
        }

        [Test]
        public void should_set_input_type_name_to_the_xml_type_name()
        {
            var request = _spec.GetEndpoint<InputTypeDescriptions
                .OverridenRequestController>(x => x.Post(null)).Request;

            request.Comments.ShouldBeNull();
            request.Type.Name.ShouldEqual("NewItemName");
        }

        [Test]
        public void should_set_input_type_collection_name_to_the_xml_type_name()
        {
            var request = _spec.GetEndpoint<InputTypeDescriptions
                .OverridenCollectionController>(x => x.Post(null)).Request;

            request.Comments.ShouldBeNull();
            request.Type.Name.ShouldEqual("NewCollectionName");
        }
    }
}