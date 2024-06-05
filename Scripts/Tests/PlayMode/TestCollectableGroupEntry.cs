using NUnit.Framework;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Tests
{
    public class TestCollectableGroupEntry
    {
        [Test]
        public void TestEquals()
        {
            var collectable = ScriptableObject.CreateInstance<CollectableResource>();
            var entry1 = new CollectableGroupEntry(collectable, 10);
            var entry2 = new CollectableGroupEntry(collectable, 10);
            var entry3 = new CollectableGroupEntry(collectable, 1);
            Assert.IsTrue(entry1 == entry2);
            Assert.IsFalse(entry1 == entry3);
            Assert.IsFalse(entry1.Equals(null));
        }

        [Test]
        public void TestDoesNotEqual()
        {
            var collectable = ScriptableObject.CreateInstance<CollectableResource>();
            var entry1 = new CollectableGroupEntry(collectable, 10);
            var entry2 = new CollectableGroupEntry(collectable, 10);
            var entry3 = new CollectableGroupEntry(collectable, 1);
            Assert.IsFalse(entry1 != entry2);
            Assert.IsTrue(entry1 != entry3);
        }

        [Test]
        public void TestGetHashCode()
        {
            var collectable = ScriptableObject.CreateInstance<CollectableResource>();
            var entry1 = new CollectableGroupEntry(collectable, 10);
            var entry2 = new CollectableGroupEntry(collectable, 10);
            var entry3 = new CollectableGroupEntry(collectable, 1);
            Assert.AreEqual(entry1.GetHashCode(), entry2.GetHashCode());
            Assert.AreNotEqual(entry1.GetHashCode(), entry3.GetHashCode());
        }
    }
}
