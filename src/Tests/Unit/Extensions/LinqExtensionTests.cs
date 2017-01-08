using System.Collections.Generic;
using Swank.Extensions;
using NUnit.Framework;
using Should;
using System.Linq;
using Tests.Common;

namespace Tests.Unit.Extensions
{
    [TestFixture]
    public class LinqExtensionTests
    {
        class NodeWithChildren
        {
            public int Index { get; set; }
            public NodeWithChildren Child { get; set; }
            public List<NodeWithChildren> Children { get; set; }
        }

        [Test]
        public void Should_traverse_many()
        {
            var tip = new NodeWithChildren
            {
                Child = new NodeWithChildren
                {
                    Index = 1, Child = new NodeWithChildren { Index = 2 }
                },
                Children = new List<NodeWithChildren>
                {
                    new NodeWithChildren {
                        Index = 3,
                        Child = new NodeWithChildren { Index = 4 },
                        Children = new List<NodeWithChildren>
                        {
                            new NodeWithChildren { Index = 5 },
                            new NodeWithChildren { Index = 6 }
                        }
                    },
                    new NodeWithChildren {
                        Index = 7,
                        Child = new NodeWithChildren { Index = 8 },
                        Children = new List<NodeWithChildren>
                        {
                            new NodeWithChildren { Index = 9 },
                            new NodeWithChildren { Index = 10 }
                        }
                    }
                }
            };

            var traversal = tip.TraverseMany(x =>
            {
                var results = new List<NodeWithChildren>();
                if (x.Child != null) results.Add(x.Child);
                if (x.Children != null) results.AddRange(x.Children);
                return results;
            }).OrderBy(x => x.Index).ToList();

            traversal.Count.ShouldEqual(10);
            for (var i = 0; i < 9; i++) traversal[i].Index.ShouldEqual(i + 1);
        }

        [Test]
        public void Should_map()
        {
            "fark".Map(x => x.Length).ShouldEqual(4);
        }

        [Test]
        public void Should_map_when_not_null()
        {
            new List<string> {"fark"}.MapOrDefault(x => x.Count, 5).ShouldEqual(1);
        }

        [Test]
        public void Should_map_default_when_null()
        {
            ((List<string>)null).MapOrDefault(x => x.Count, 5).ShouldEqual(5);
        }

        [Test]
        public void should_concat()
        {
            new List<string> { "oh" }.Concat("hai")
                .ShouldEqual(new List<string> { "oh", "hai" });
        }

        [Test]
        [TestCase(null, null)]
        [TestCase("", "")]
        [TestCase("fark", "fark")]
        [TestCase("fark,fark,farker,farker,fark,fark", "fark,farker,fark")]
        public void should_return_distinct_siblings(string expected, string actual)
        {
            expected?.Split(new [] {',', '.' }).DistinctSiblings()
                .ShouldOnlyContain(actual?.Split(new[] { ',', '.' }));
        }

        [Test]
        public void should_concat_with_null_source()
        {
            ((List<string>)null).Concat("hai")
                .ShouldEqual(new List<string> { "hai" });
        }

        [Test]
        public void Should_iterate_enumerable()
        {
            var results = new List<string>();

            new List<string> {"a", "b", "c"}.ForEach(x => results.Add(x));

            results.ToArray().ShouldEqual(new [] { "a", "b", "c" });
        }

        [Test]
        public void Should_iterate_enumerable_with_index()
        {
            var results = new List<string>();

            new List<string> { "a", "b", "c" }
                .ForEach((x, i) => results.Add(x + i.ToString()));

            results.ToArray().ShouldEqual(new[] { "a0", "b1", "c2" });
        }

        public class MultipartKeyedType
        {
            public List<string> LongKey { get; set; }
            public List<string> ShortKey { get; set; }
        }

        [Test]
        public void should_shrink_multipart_key_right()
        {
            var types = new List<MultipartKeyedType>
            {
                new MultipartKeyedType { LongKey = new List<string>
                    { "Request", "Properties", "List", "Form" } },
                new MultipartKeyedType { LongKey = new List<string>
                    { "Request", "Properties", "Attributes", "List", "Form" } },
                new MultipartKeyedType { LongKey = new List<string>
                    { "Request", "Properties", "Form" } },
                new MultipartKeyedType { LongKey = new List<string>
                    { "Request", "Properties", "Form", "Form" } }
            };

            types.ShrinkMultipartKeyRight(x => x.LongKey, (t, k) => t.ShortKey = k);

            types[0].ShortKey.ToArray().ShouldEqual(new[] 
                { "Properties", "List", "Form" });
            types[0].LongKey.ToArray().ShouldEqual(new[] 
                { "Request", "Properties", "List", "Form" });

            types[1].ShortKey.ToArray().ShouldEqual(new[] 
                { "Attributes", "List", "Form" });
            types[1].LongKey.ToArray().ShouldEqual(new[] 
                { "Request", "Properties", "Attributes", "List", "Form" });

            types[2].ShortKey.ToArray().ShouldEqual(new[] 
                { "Properties", "Form" });
            types[2].LongKey.ToArray().ShouldEqual(new[] 
                { "Request", "Properties", "Form" });

            types[3].ShortKey.ToArray().ShouldEqual(new[] { "Form" });
            types[3].LongKey.ToArray().ShouldEqual(new[] 
                { "Request", "Properties", "Form", "Form" });
        }

        public class KeyedType
        {
            public string Key { get; set; }
        }

        [Test]
        public void should_return_multiplicates()
        {
            var multiplicates = new List<KeyedType>
            {
                new KeyedType { Key = "Hai" },
                new KeyedType { Key = "Oh" },
                new KeyedType { Key = "Hai" }
            }.Multiplicates(x => x.Key);

            multiplicates.Count().ShouldEqual(2);
            multiplicates.All(x => x.Key == "Hai").ShouldBeTrue();
        }

        [Test]
        public void should_return_items_hash_code()
        {
            new List<string> { "hai" }.GetItemsHashCode()
                .ShouldEqual(new List<string> { "hai" }.GetItemsHashCode());
        }

        [Test]
        public void should_indicate_if_list_ends_with_value()
        {
            ((IEnumerable<string>)null).EndsWith("hai").ShouldBeFalse();
            new List<string>().EndsWith("hai").ShouldBeFalse();
            new List<string> { "oh" }.EndsWith("hai").ShouldBeFalse();
            new List<string> { "hai" }.EndsWith("hai").ShouldBeTrue();
        }

        [Test]
        public void should_shorten_list()
        {
            ((IEnumerable<string>)null).Shorten(1).ShouldBeNull();
            new List<string>().Shorten(1).ShouldBeEmpty();
            new List<string> { "oh" }.Shorten(1).ShouldBeEmpty();
            new List<string> { "oh" }.Shorten(2).ShouldBeEmpty();
            new List<string> { "oh", "hai" }.Shorten(1)
                .ToArray().ShouldEqual(new[] { "oh" });
        }
    }
}
