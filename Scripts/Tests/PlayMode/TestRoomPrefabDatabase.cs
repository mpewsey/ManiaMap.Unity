using MPewsey.Common.Mathematics;
using MPewsey.Common.Random;
using MPewsey.ManiaMap.Graphs;
using NUnit.Framework;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Tests
{
    public class TestRoomPrefabDatabase
    {
        [SetUp]
        public void SetUp()
        {
            Assets.DestroyAllGameObjects();
        }

        [Test]
        public void TestCreateRoomDataDictionary()
        {
            var db = ScriptableObject.CreateInstance<RoomPrefabDatabase>();

            var room1 = new GameObject("Room1").AddComponent<RoomBehavior>();
            room1.Id = 1;

            var room2 = new GameObject("Room2").AddComponent<RoomBehavior>();
            room2.Id = 2;

            db.AddEntry(room1.Id, room1);
            db.AddEntry(room2.Id, room2);
            Assert.AreEqual(2, db.Entries.Count);

            foreach (var entry in db.Entries)
            {
                Assert.AreEqual(entry.Prefab, db.GetPrefab(entry.Id));
            }
        }

        [Test]
        public void TestInstantiateRoom()
        {
            var seed = new RandomSeed(12345);
            var prefab = Assets.InstantiatePrefab<RoomBehavior>(Assets.Angle3x4RoomPath);
            var template = prefab.CreateData();

            // Create fake layout.
            var layout = new Layout(1, "Test", seed.Seed);
            var node = new LayoutNode(1);
            var roomLayout = new Room(node, Vector2DInt.Zero, template, seed.Next());
            layout.Rooms.Add(roomLayout.Id, roomLayout);
            ManiaMapManager.Current.Initialize(layout, new LayoutState(layout));

            // Create database.
            var db = ScriptableObject.CreateInstance<RoomPrefabDatabase>();
            db.AddEntry(prefab.Id, prefab);

            var room = db.InstantiateRoom(roomLayout.Id, null, RoomPositionOption.LayoutPosition);
            Object.DestroyImmediate(prefab.gameObject);
            Assert.IsNotNull(room);
        }
    }
}