using System;
using NUnit.Framework;
using Should;
using Swank;

namespace Tests.Unit.Extensions
{
    [TestFixture]
    public class MicroContainerTests
    {
        public interface IDependency { }
        public class Dependency : IDependency { }

        public class RootWithExplicitDependency
        {
            public RootWithExplicitDependency(Dependency dependency)
            {
                Dependency = dependency;
            }

            public Dependency Dependency { get; set; }
        }

        public class RootWithImplicitDependency
        {
            public RootWithImplicitDependency(IDependency dependency)
            {
                Dependency = dependency;
            }

            public IDependency Dependency { get; set; }
        }

        [Test]
        public void Should_resolve_explicit_dependency()
        {
            var container = MicroContainer.Create();

            var result = container.GetInstance<RootWithExplicitDependency>();
            result.ShouldNotBeNull();
            result.Dependency.ShouldNotBeNull();
        }

        [Test]
        public void Should_resolve_adhoc_dependency()
        {
            var container = MicroContainer.Create();
            var dependency = new Dependency();

            var result = container.GetInstance<RootWithExplicitDependency>(dependency);
            result.ShouldNotBeNull();
            result.Dependency.ShouldEqual(dependency);
        }

        [Test]
        public void Should_resolve_implicit_dependency()
        {
            var container = MicroContainer.Create(x => 
                x.Register<IDependency, Dependency>());

            var result = container.GetInstance<RootWithImplicitDependency>();
            result.ShouldNotBeNull();
            result.Dependency.ShouldNotBeNull();
        }

        [Test]
        public void Should_throw_exception_when_implicit_dependency_not_registered()
        {
            Assert.Throws<InvalidOperationException>(() => MicroContainer.Create()
                .GetInstance<RootWithImplicitDependency>());
        }

        [Test]
        public void Should_resolve_with_factory()
        {
            var container = MicroContainer.Create(x => x.RegisterFactory(y => 
                y == typeof(IDependency) ? new Dependency() : null));

            var result = container.GetInstance<RootWithImplicitDependency>();
            result.ShouldNotBeNull();
            result.Dependency.ShouldNotBeNull();
        }

        [Test]
        public void Should_resolve_instance()
        {
            var dependency = new Dependency();
            var container = MicroContainer.Create(x => x
                .Register(dependency));

            container.GetInstance<RootWithExplicitDependency>()
                .Dependency.ShouldEqual(dependency);
        }
    }
}
