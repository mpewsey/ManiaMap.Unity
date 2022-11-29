using NUnit.Framework;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Drawing.Tests
{
    public class TestMapTileKey
    {
        [Test]
        public void TestEqualsOperator()
        {
            var key1 = new MapTileKey(1, Color.blue);
            var key2 = new MapTileKey(1, Color.blue);
            var key3 = new MapTileKey(2, Color.red);
            Assert.IsTrue(key1 == key2);
            Assert.IsFalse(key1 == key3);
            Assert.IsFalse(key1.Equals(null));
        }

        [Test]
        public void TestNotEqualOperator()
        {
            var key1 = new MapTileKey(1, Color.blue);
            var key2 = new MapTileKey(1, Color.blue);
            var key3 = new MapTileKey(2, Color.red);
            Assert.IsFalse(key1 != key2);
            Assert.IsTrue(key1 != key3);
        }

        [Test]
        public void TestGetHashCode()
        {
            var key1 = new MapTileKey(1, Color.blue);
            var key2 = new MapTileKey(1, Color.blue);
            var key3 = new MapTileKey(2, Color.red);
            Assert.AreEqual(key1.GetHashCode(), key2.GetHashCode());
            Assert.AreNotEqual(key1.GetHashCode(), key3.GetHashCode());
        }
    }
}
