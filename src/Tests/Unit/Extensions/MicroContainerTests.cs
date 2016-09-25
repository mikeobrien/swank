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

        public class Root1WithExplicitDependency
        {
            public Root1WithExplicitDependency(Dependency dependency)
            {
                Dependency = dependency;
            }

            public Dependency Dependency { get; set; }
        }
        
        public class Root2WithExplicitDependency
        {
            public Root2WithExplicitDependency(Dependency dependency)
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

            var result = container.GetInstance<Root1WithExplicitDependency>();
            result.ShouldNotBeNull();
            result.Dependency.ShouldNotBeNull();
        }

        [Test]
        public void Should_resolve_adhoc_dependency()
        {
            var container = MicroContainer.Create();
            var dependency = new Dependency();

            var result = container.GetInstance<Root1WithExplicitDependency>(dependency);
            result.ShouldNotBeNull();
            result.Dependency.ShouldEqual(dependency);
        }

        [Test]
        public void Should_not_cache_instance_or_adhoc_dependencies_when_specified()
        {
            var container = MicroContainer.Create();
            var dependency1 = new Dependency();

            var result1 = container.CreateInstance<Root1WithExplicitDependency>(dependency1);
            result1.ShouldNotBeNull();
            result1.Dependency.ShouldEqual(dependency1);

            var dependency2 = new Dependency();

            var result2 = container.CreateInstance<Root1WithExplicitDependency>(dependency2);
            result2.ShouldNotBeNull();
            result2.Dependency.ShouldEqual(dependency2);
            result1.ShouldNotEqual(result2);
        }

        [Test]
        public void Should_cache_instance_when_specified()
        {
            var container = MicroContainer.Create();
            var dependency = new Dependency();

            var result1 = container.GetInstance<Root1WithExplicitDependency>(dependency);
            result1.ShouldNotBeNull();
            result1.Dependency.ShouldEqual(dependency);

            var result2 = container.GetInstance<Root1WithExplicitDependency>(new Dependency());
            result2.ShouldNotBeNull();
            result1.ShouldEqual(result2);
        }

        [Test]
        public void Should_not_cache_adhoc_dependencies()
        {
            var container = MicroContainer.Create();
            var dependency1 = new Dependency();

            var result1 = container.GetInstance<Root1WithExplicitDependency>(dependency1);
            result1.ShouldNotBeNull();
            result1.Dependency.ShouldEqual(dependency1);

            var dependency2 = new Dependency();

            var result2 = container.GetInstance<Root2WithExplicitDependency>(dependency2);
            result2.ShouldNotBeNull();
            result2.Dependency.ShouldNotEqual(dependency1);
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

            container.GetInstance<Root1WithExplicitDependency>()
                .Dependency.ShouldEqual(dependency);
        }
    }
}
