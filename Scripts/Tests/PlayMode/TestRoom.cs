using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace MPewsey.ManiaMap.Unity.Tests
{
    public class TestRoom
    {
        private GameObject Container { get; set; }
        
        [SetUp]
        public void SetUp()
        {
            Container = new GameObject("TestRoom");
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(Container);
        }

        [Test]
        public void TestCreateCells()
        {
            var obj = new GameObject("Room");
            obj.transform.SetParent(Container.transform);
            var room = obj.AddComponent<Room>();
            room.CellSize = new Vector2(10, 10);
            room.Size = new Vector2Int(4, 5);
            room.CreateCells();
            Assert.AreEqual(room.Size.x, room.CellContainer.childCount);
            Assert.AreEqual(room.Size.y, room.CellContainer.GetChild(0).childCount);
            room.Size = new Vector2Int(1, 2);
            room.CreateCells();
            Assert.AreEqual(room.Size.x, room.CellContainer.childCount);
            Assert.AreEqual(room.Size.y, room.CellContainer.GetChild(0).childCount);
        }
    }
}
