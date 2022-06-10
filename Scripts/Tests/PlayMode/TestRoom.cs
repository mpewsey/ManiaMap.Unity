using NUnit.Framework;
using UnityEngine;

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
            Object.DestroyImmediate(ManiaManager.Current);
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

        [Test]
        public void TestGetTemplate()
        {
            var room = TestAssets.LoadAngle3x4Room(Container.transform);
            var template = room.GetTemplate();
            Assert.IsNotNull(template);
        }

        [Test]
        public void TestInit()
        {
            var seed = new RandomSeed(12345);
            var room = TestAssets.LoadAngle3x4Room(Container.transform);
            var template = room.GetTemplate();

            // Create fake layout.
            var layout = new Layout(1, "Test", seed);
            var node = new ManiaMap.LayoutNode(1);
            var roomData = new ManiaMap.Room(node, Vector2DInt.Zero, template, seed);
            layout.Rooms.Add(roomData.Id, roomData);

            ManiaManager.Current.Init(layout, new LayoutState(layout));
            room.Init(roomData.Id);
        }
    }
}
