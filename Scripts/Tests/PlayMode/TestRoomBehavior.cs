using MPewsey.Common.Mathematics;
using MPewsey.Common.Random;
using MPewsey.ManiaMap.Graphs;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Tests
{
    public class TestRoomBehavior
    {
        [SetUp]
        public void SetUp()
        {
            Assets.DestroyAllGameObjects();
        }

        [TestCase(CellPlane.XY)]
        [TestCase(CellPlane.XZ)]
        public void TestGetCellIndex(CellPlane plane)
        {
            var obj = new GameObject("Room");
            var room = obj.AddComponent<RoomBehavior>();
            room.CellPlane = plane;
            room.CellSize = new Vector2(3, 5);
            room.Size = new Vector2Int(8, 10);
            room.CreateCells();

            for (int i = 0; i < room.Size.x; i++)
            {
                for (int j = 0; j < room.Size.y; j++)
                {
                    var cell = room.GetCell(i, j);
                    var expected = new Vector2Int(i, j);
                    var result = room.GetCellIndex(cell.transform.position);
                    Assert.AreEqual(expected, result);
                }
            }

            Object.DestroyImmediate(obj);
        }

        [Test]
        public void TestCreateCells()
        {
            var obj = new GameObject("Room");
            var room = obj.AddComponent<RoomBehavior>();
            room.CellSize = new Vector2(10, 10);
            room.Size = new Vector2Int(4, 5);
            room.CreateCells();
            Assert.AreEqual(room.Size.x, room.CellContainer.childCount);
            Assert.AreEqual(room.Size.y, room.CellContainer.GetChild(0).childCount);
            room.Size = new Vector2Int(1, 2);
            room.CreateCells();
            Assert.AreEqual(room.Size.x, room.CellContainer.childCount);
            Assert.AreEqual(room.Size.y, room.CellContainer.GetChild(0).childCount);
            Object.DestroyImmediate(room.gameObject);
        }

        [Test]
        public void TestGetTemplate()
        {
            var room = Assets.InstantiatePrefab<RoomBehavior>(Assets.Angle3x4RoomPath);
            var template = room.CreateData();
            Object.DestroyImmediate(room.gameObject);
            Assert.IsNotNull(template);
        }

        [Test]
        public void TestInitialize()
        {
            var seed = new RandomSeed(12345);
            var room = Assets.InstantiatePrefab<RoomBehavior>(Assets.Angle3x4RoomPath);
            var template = room.CreateData();

            // Create fake layout.
            var layout = new Layout(1, "Test", seed.Seed);
            var node = new LayoutNode(1);
            var roomLayout = new Room(node, Vector2DInt.Zero, template, seed);
            layout.Rooms.Add(roomLayout.Id, roomLayout);
            var layoutState = new LayoutState(layout);
            var doorConnections = new List<DoorConnection>();

            room.Initialize(roomLayout.Id, layout, layoutState, doorConnections, RoomPositionOption.LayoutPosition);
            Assert.IsTrue(room.IsInitialized);
        }

        [Test]
        public void TestIntantiateRoom()
        {
            var seed = new RandomSeed(12345);
            var prefab = Assets.InstantiatePrefab<RoomBehavior>(Assets.Angle3x4RoomPath);
            var template = prefab.CreateData();

            // Create fake layout.
            var layout = new Layout(1, "Test", seed.Seed);
            var node = new LayoutNode(1);
            var roomLayout = new Room(node, Vector2DInt.Zero, template, seed);
            layout.Rooms.Add(roomLayout.Id, roomLayout);
            ManiaMapManager.Current.Initialize(layout, new LayoutState(layout));

            var room = RoomBehavior.InstantiateRoom(roomLayout.Id, prefab.gameObject, null, RoomPositionOption.LayoutPosition);
            Object.DestroyImmediate(prefab.gameObject);
            Assert.IsNotNull(room);
        }
    }
}
