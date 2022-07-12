using NUnit.Framework;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Tests
{
    public class TestRoom
    {
        [SetUp]
        public void SetUp()
        {
            Assets.LoadEmptyScene();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(ManiaManager.Current);
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
        }

        [Test]
        public void TestGetTemplate()
        {
            var room = Assets.InstantiatePrefab<Room>(Assets.Angle3x4RoomPath);
            var template = room.GetTemplate();
            Assert.IsNotNull(template);
        }

        [Test]
        public void TestInit()
        {
            var seed = new RandomSeed(12345);
            var room = Assets.InstantiatePrefab<Room>(Assets.Angle3x4RoomPath);
            var template = room.GetTemplate();

            // Create fake layout.
            var layout = new Layout(1, "Test", seed);
            var node = new ManiaMap.LayoutNode(1);
            var roomData = new ManiaMap.Room(node, Vector2DInt.Zero, template, seed);
            layout.Rooms.Add(roomData.Id, roomData);
            ManiaManager.Current.LayoutData = new LayoutData(layout, new LayoutState(layout));

            room.Init(roomData.Id, RoomPositionOption.Layout);
        }

        [Test]
        public void TestIntantiateRoom()
        {
            var seed = new RandomSeed(12345);
            var prefab = Assets.InstantiatePrefab<Room>(Assets.Angle3x4RoomPath);
            var template = prefab.GetTemplate();

            // Create fake layout.
            var layout = new Layout(1, "Test", seed);
            var node = new ManiaMap.LayoutNode(1);
            var roomData = new ManiaMap.Room(node, Vector2DInt.Zero, template, seed);
            layout.Rooms.Add(roomData.Id, roomData);
            ManiaManager.Current.LayoutData = new LayoutData(layout, new LayoutState(layout));

            var room = Room.InstantiateRoom(roomData.Id, prefab.gameObject, null, RoomPositionOption.Layout);
            Assert.IsNotNull(room);
        }
    }
}
