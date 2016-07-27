using System;
using System.Collections.Generic;
using NUnit.Framework;
using Should;
using Swank.Extensions;

namespace Tests.Unit.Extensions
{
    [TestFixture]
    public class FuncTests
    {
        [Test]
        public void Should_return_memoized_result()
        {
            var memoizedFunction = Func.Memoize<int, string>(x => x.ToString());

            memoizedFunction(5).ShouldEqual("5");
            memoizedFunction(5).ShouldEqual("5");

            memoizedFunction(6).ShouldEqual("6");
            memoizedFunction(6).ShouldEqual("6");
        }

        public class ApplyClass
        {
            public string Value { get; set; }
        }

        [Test]
        public void Should_apply_with_no_parameters()
        {
            var result = new ApplyClass();
            new List<Action<ApplyClass>>
            {
                x => x.Value += "a",
                x => x.Value += "b",
                x => x.Value += "c"
            }.Apply(result).Value.ShouldEqual("abc");
        }

        [Test]
        public void Should_apply_with_one_parameter()
        {
            var result = new ApplyClass();
            new List<Action<string, ApplyClass>>
            {
                (p, x) => x.Value += $"{p}a",
                (p, x) => x.Value += $"{p}b",
                (p, x) => x.Value += $"{p}c"
            }.Apply("1", result).Value.ShouldEqual("1a1b1c");
        }

        [Test]
        public void Should_apply_with_two_parameters()
        {
            var result = new ApplyClass();
            new List<Action<string, string, ApplyClass>>
            {
                (p1, p2, x) => x.Value += $"{p1}{p2}a",
                (p1, p2, x) => x.Value += $"{p1}{p2}b",
                (p1, p2, x) => x.Value += $"{p1}{p2}c"
            }.Apply("1", "2", result).Value.ShouldEqual("12a12b12c");
        }
    }
}
