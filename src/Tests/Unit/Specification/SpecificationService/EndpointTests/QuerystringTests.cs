using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Should;
using Swank.Configuration;
using Swank.Specification;
using Tests.Common;
using Tests.Unit.Specification.SpecificationService.EndpointTests.Querystrings;
using OptionController = Tests.Unit.Specification.SpecificationService.EndpointTests.Querystrings.OptionController;
using Options = Tests.Unit.Specification.SpecificationService.EndpointTests.Querystrings.Options;

namespace Tests.Unit
    .Specification.SpecificationService.EndpointTests
{
    public class QuerystringTests
    {
        private Endpoint _endpoint;
        private List<Module> _spec;

        [SetUp]
        public void Setup()
        {
            _spec = Builder.BuildSpec<Controller>();
            _endpoint = _spec.GetEndpoint<Controller>(x => x.Get(null, null, 
                null, null, null, null, null, null, 0, 0, null));
        }

        [Test]
        public void should_return_querystring_with_default_description()
        {
            var querystring = _endpoint.GetQuerystring("querystring");
            querystring.Type.Name.ShouldEqual("string");
            querystring.Comments.ShouldBeNull();
            querystring.DefaultValue.ShouldBeNull();
            querystring.MultipleAllowed.ShouldBeFalse();
            querystring.Type.Enumeration.ShouldBeNull();
            querystring.Required.ShouldBeTrue();
            querystring.SampleValue.ShouldEqual("");
        }

        [Test]
        public void should_set_description_when_there_is_a_description_attribute()
        {
            _endpoint.GetQuerystring("description")
                .Comments.ShouldEqual("description <strong>comments</strong>");
        }

        [Test]
        public void should_set_comments_when_there_is_a_comments_attribute()
        {
            _endpoint.GetQuerystring("commentsQuerystring")
                .Comments.ShouldEqual("querystring <strong>comments</strong>");
        }

        [Test]
        public void should_hide_parameters_marked_with_the_hide_sttribute()
        {
            _endpoint.QuerystringParameters.Any(x => x.Name ==
                "hiddenQuerystring").ShouldBeFalse();
        }

        [Test]
        public void should_hide_parameters_marked_with_the_xml_ignore_sttribute()
        {
            _endpoint.QuerystringParameters.Any(x => x.Name ==
                "xmlIgnoreQuerystring").ShouldBeFalse();
        }

        [Test]
        public void should_indicate_that_multiple_are_allowed_when_the_multiple_attribute_is_applied()
        {
            _endpoint.GetQuerystring("multipleQuerystring")
                .MultipleAllowed.ShouldBeTrue();
        }

        [Test]
        public void should_set_the_element_type_when_multiple_are_allowed()
        {
            _endpoint.GetQuerystring("multipleQuerystring")
                .Type.Name.ShouldEqual("ArrayOfInt");
        }

        [Test]
        public void should_indicate_when_the_parameter_is_optional()
        {
            _endpoint.GetQuerystring("optionalQuerystring")
                .Required.ShouldBeFalse();
        }

        [Test]
        public void should_indicate_querystring_default_value()
        {
            _endpoint.GetQuerystring("defaultQuerystring")
                .DefaultValue.ShouldEqual("5");
        }

        [Test]
        public void should_indicate_querystring_sample_value()
        {
            _endpoint.GetQuerystring("sampleValueQuerystring")
                .SampleValue.ShouldEqual("5");
        }

        [Test]
        public void should_order_querystring_options_by_name_or_value()
        {
            var options = _spec.GetEndpoint<OptionController>(
                    x => x.Get(null, null, Options.Option1))
                .GetQuerystring("querystring").Type.Enumeration;

            options.Options[0].Value.ShouldEqual("Option1");
            options.Options[1].Value.ShouldEqual("Option3");
        }

        [Test]
        public void should_get_querystring_option_description()
        {
            var option = _spec.GetEndpoint<OptionController>(
                    x => x.Get(null, null, Options.Option1))
                .GetQuerystring("querystring").Type.Enumeration.Options[0];

            option.Name.ShouldEqual("Option 1");
            option.Value.ShouldEqual("Option1");
            option.Comments.ShouldEqual("Option <strong>1</strong> description.");
        }

        [Test]
        public void should_set_querystring_option_description_to_default_when_not_specified()
        {
            var option = _spec.GetEndpoint<OptionController>(x => 
                    x.Get(null, null, Options.Option1))
                .GetQuerystring("querystring").Type.Enumeration.Options[1];

            option.Name.ShouldEqual("Option3");
            option.Value.ShouldEqual("Option3");
            option.Comments.ShouldBeNull();
        }

        [Test]
        public void should_hide_querystring_options_marked_with_the_hide_attribute()
        {
            _spec.GetEndpoint<OptionController>(x => x.Get(null, null, Options.Option1))
                .GetQuerystring("querystring").Type.Enumeration.Options
                .Any(x => x.Value == "Option2").ShouldBeFalse();
        }

        [Test]
        [TestCase("querystring", AuthenticationLocation.Querystring, true)]
        [TestCase("querystring", AuthenticationLocation.Header, false)]
        [TestCase("querystring", AuthenticationLocation.UrlParameter, false)]
        [TestCase("fark", AuthenticationLocation.Querystring, false)]
        public void should_indicate_if_a_parameter_is_for_auth(string name,
            AuthenticationLocation location, bool isAuth)
        {
            _spec = Builder.BuildSpec<Controller>(x => x.WithCustomAuthenticationScheme(
                new AuthenticationScheme
                {
                    Components = new List<AuthenticationComponent> {
                        new AuthenticationComponent
                        {
                            Name = name,
                            Location = location
                        }
                    }
                }));
            var parameter = _spec.GetEndpoint<Controller>(x => x.Get(
                null, null, null, null, null, null, null, null, 0, 0, null))
                .GetQuerystring("querystring");

            parameter.IsAuth.ShouldEqual(isAuth);
        }
    }
}