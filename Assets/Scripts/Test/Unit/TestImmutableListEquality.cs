using System;
using System.Collections.Immutable;
using NUnit.Framework;
using System.Linq;

namespace Test.Unit
{
    // テスト対象のレコード
    public record Foo(ImmutableArray<int> Numbers)
    {
        public ImmutableArray<int> Numbers { get; } = Numbers;
        public virtual bool Equals(Foo? other) =>
            other is not null
            && Numbers.SequenceEqual(other.Numbers);

        public override int GetHashCode() =>
            Numbers.Aggregate(0, (h, v) => HashCode.Combine(h, v));
    }

    [TestFixture]
    public class TestImmutableArrayEquality
    {
        [Test]
        public void EmptyImmutableArray_ShouldBeEqual()
        {
            // ImmutableArray.Empty はシングルトンなので、同じインスタンスが返ってくる
            var a = new Foo(ImmutableArray<int>.Empty);
            var b = new Foo(ImmutableArray<int>.Empty);
            
            Assert.AreEqual(a, b, "Empty lists should be equal");
            Assert.IsTrue(a == b, "operator== should consider them equal");
        }

        [Test]
        public void SameContentsDifferentInstances_ShouldBeEqual()
        {
            // Create で生成しても、内部的には要素比較で等価と判断される
            var list1 = ImmutableArray.Create(1, 2, 3);
            var list2 = ImmutableArray.Create(1, 2, 3);
            
            Assert.AreNotSame(list1, list2, "Instances should be different");
            
            var a = new Foo(list1);
            var b = new Foo(list2);
            
            Assert.AreEqual(a, b, "Lists with same elements should be equal");
            Assert.IsTrue(a.Equals(b), "Equals() should consider them equal");
        }

        [Test]
        public void DifferentContents_ShouldNotBeEqual()
        {
            var a = new Foo(ImmutableArray.Create(1, 2, 3));
            var b = new Foo(ImmutableArray.Create(1, 2, 4));
            
            Assert.AreNotEqual(a, b, "Different element lists should not be equal");
            Assert.IsFalse(a == b, "operator== should consider them not equal");
        }

        [Test]
        public void Immutability_Check()
        {
            var baseList = ImmutableArray.Create(1, 2);
            var a = new Foo(baseList);
            var modified = baseList.Add(3);
            var b = new Foo(modified);
            
            // 元のレコード a の Numbers は変更されていない
            Assert.AreEqual(2, a.Numbers.Length, "Original record should remain unchanged");
            Assert.AreEqual(3, b.Numbers.Length, "New record sees the added element");
            Assert.AreNotEqual(a, b, "Records with different lengths should not be equal");
        }
    }
}
