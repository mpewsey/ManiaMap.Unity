using NUnit.Framework;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Drawing.Tests
{
    public class TestMapTileHash
    {
        [Test]
        public void TestEqualsOperator()
        {
            var hash1 = new MapTileHash(MapTileTypes.NorthDoor, Color.blue);
            var hash2 = new MapTileHash(MapTileTypes.NorthDoor, Color.blue);
            var hash3 = new MapTileHash(MapTileTypes.SouthDoor, Color.red);
            Assert.IsTrue(hash1 == hash2);
            Assert.IsFalse(hash1 == hash3);
            Assert.IsFalse(hash1.Equals(null));
        }

        [Test]
        public void TestNotEqualOperator()
        {
            var hash1 = new MapTileHash(MapTileTypes.NorthDoor, Color.blue);
            var hash2 = new MapTileHash(MapTileTypes.NorthDoor, Color.blue);
            var hash3 = new MapTileHash(MapTileTypes.SouthDoor, Color.red);
            Assert.IsFalse(hash1 != hash2);
            Assert.IsTrue(hash1 != hash3);
        }

        [Test]
        public void TestGetHashCode()
        {
            var hash1 = new MapTileHash(MapTileTypes.NorthDoor, Color.blue);
            var hash2 = new MapTileHash(MapTileTypes.NorthDoor, Color.blue);
            var hash3 = new MapTileHash(MapTileTypes.SouthDoor, Color.red);
            Assert.AreEqual(hash1.GetHashCode(), hash2.GetHashCode());
            Assert.AreNotEqual(hash1.GetHashCode(), hash3.GetHashCode());
        }
    }
}
