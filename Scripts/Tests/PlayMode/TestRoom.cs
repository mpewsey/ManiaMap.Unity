using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Tests
{
    public class TestRoom
    {
        [SetUp]
        public void SetUp()
        {
            Assets.DestroyAllGameObjects();
        }

        [TestCase(Room.Plane.XY)]
        [TestCase(Room.Plane.XZ)]
        public void TestGetCellIndex(Room.Plane plane)
        {
            var obj = new GameObject("Room");
            var room = obj.AddComponent<Room>();
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
            Object.DestroyImmediate(room.gameObject);
        }

        [Test]
        public void TestGetTemplate()
        {
            var room = Assets.InstantiatePrefab<Room>(Assets.Angle3x4RoomPath);
            var template = room.GetTemplate();
            Object.DestroyImmediate(room.gameObject);
            Assert.IsNotNull(template);
        }

        [Test]
        public void TestInitialize()
        {
            var seed = new RandomSeed(12345);
            var room = Assets.InstantiatePrefab<Room>(Assets.Angle3x4RoomPath);
            var template = room.GetTemplate();

            // Create fake layout.
            var layout = new Layout(1, "Test", seed);
            var node = new Graphs.LayoutNode(1);
            var roomLayout = new ManiaMap.Room(node, Vector2DInt.Zero, template, seed);
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
            var prefab = Assets.InstantiatePrefab<Room>(Assets.Angle3x4RoomPath);
            var template = prefab.GetTemplate();

            // Create fake layout.
            var layout = new Layout(1, "Test", seed);
            var node = new Graphs.LayoutNode(1);
            var roomLayout = new ManiaMap.Room(node, Vector2DInt.Zero, template, seed);
            layout.Rooms.Add(roomLayout.Id, roomLayout);
            ManiaMapManager.Current.SetLayout(layout, new LayoutState(layout));

            var room = Room.InstantiateRoom(roomLayout.Id, prefab.gameObject, null, RoomPositionOption.LayoutPosition);
            Object.DestroyImmediate(prefab.gameObject);
            Assert.IsNotNull(room);
        }
    }
}
