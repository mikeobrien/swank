using NUnit.Framework;
using Should;

namespace Tests.Unit.Specification.SpecificationService.Tests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void should_only_enumerate_actions_in_the_specified_assemblies()
        {
            Builder.BuildSpec<TestHarness.Module.ModuleController>().ShouldBeEmpty();
        }

        [Test]
        public void should_filter_actions_based_on_filter_in_the_configuration()
        {
            Builder.BuildSpec<Filtering.Controller>().Count.ShouldEqual(1);
        }
    }
}