using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Should;
using Swank.Configuration;
using Tests.Common;
using Tests.Unit.Specification.SpecificationService.EndpointTests.UrlParameters;

namespace Tests.Unit.Specification.SpecificationService.EndpointTests
{
    public class UrlParameterTests
    {
        private List<Swank.Specification.Module> _spec;

        [SetUp]
        public void Setup()
        {
            _spec = Builder.BuildSpec<NoCommentsController>();
        }

        [Test]
        public void should_set_url_parameter_to_default_when_no_comments_specified()
        {
            var parameter = _spec.GetEndpoint<NoCommentsController>
                (x => x.Get(null, Guid.Empty))
                .GetUrlParameter("urlParameter");
            
            parameter.Type.Name.ShouldEqual("uuid");
            parameter.Comments.ShouldBeNull();
            parameter.Type.Enumeration.ShouldBeNull();
        }

        [Test]
        public void should_set_url_parameter_with_comments()
        {
            var parameter = _spec.GetEndpoint<CommentsController>(
                x => x.Get(null, Guid.Empty)).GetUrlParameter("urlParameter");
            
            parameter.Type.Name.ShouldEqual("uuid");
            parameter.Comments.ShouldEqual("url <strong>parameter</strong> comments");
            parameter.Type.Enumeration.ShouldBeNull();
        }

        [Test]
        public void should_get_url_paramaters_option_description()
        {
            var option = _spec.GetEndpoint<OptionController>(
                x => x.Get(null, Options.Option1)).GetUrlParameter("urlParameter")
                    .Type.Enumeration.Options[0];

            option.Name.ShouldEqual("Option 1");
            option.Value.ShouldEqual("Option1");
            option.Comments.ShouldEqual("Option <strong>1</strong> description.");
        }

        [Test]
        public void should_set_url_paramaters_option_description_to_default_when_not_specified()
        {
            var option = _spec.GetEndpoint<OptionController>(
                x => x.Get(null, Options.Option1)).GetUrlParameter("urlParameter")
                    .Type.Enumeration.Options[1];

            option.Name.ShouldEqual("Option3");
            option.Value.ShouldEqual("Option3");
            option.Comments.ShouldBeNull();
        }

        [Test]
        public void should_hide_url_paramaters_options_marked_with_the_hide_attribute()
        {
            _spec.GetEndpoint<OptionController>(x => x.Get(null, Options.Option1))
                .GetUrlParameter("urlParameter").Type.Enumeration.Options
                .Any(x => x.Value == "Option2").ShouldBeFalse();
        }

        [Test]
        [TestCase("urlParameter", AuthenticationLocation.UrlParameter, true)]
        [TestCase("urlParameter", AuthenticationLocation.Header, false)]
        [TestCase("urlParameter", AuthenticationLocation.Querystring, false)]
        [TestCase("fark", AuthenticationLocation.UrlParameter, false)]
        public void should_indicate_if_a_parameter_is_for_auth(string name,
            AuthenticationLocation location, bool isAuth)
        {
            _spec = Builder.BuildSpec<NoCommentsController>(x => x.WithCustomAuthenticationScheme(
                new AuthenticationScheme {
                    Components = new List<AuthenticationComponent> {
                        new AuthenticationComponent
                        {
                            Name = name,
                            Location = location
                        }
                    }
                }));
            var parameter = _spec.GetEndpoint<NoCommentsController>
                (x => x.Get(null, Guid.Empty))
                .GetUrlParameter("urlParameter");

            parameter.IsAuth.ShouldEqual(isAuth);
        }
    }
}