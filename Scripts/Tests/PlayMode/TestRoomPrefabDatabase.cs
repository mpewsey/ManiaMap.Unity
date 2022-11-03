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
            var obj = new GameObject("Room Prefab Database");
            var db = obj.AddComponent<RoomPrefabDatabase>();

            var room1 = new GameObject("Room1").AddComponent<Room>();
            room1.Id = 1;

            var room2 = new GameObject("Room2").AddComponent<Room>();
            room2.Id = 2;

            db.Entries.Add(new RoomDatabaseEntry<Room>(room1.Id, room1));
            db.Entries.Add(new RoomDatabaseEntry<Room>(room2.Id, room2));

            db.CreateRoomPrefabDictionary();
            Assert.Greater(db.Entries.Count, 0);

            foreach (var entry in db.Entries)
            {
                Assert.AreEqual(entry.RoomPrefab, db.GetRoomPrefab(entry.Id));
            }
        }

        [Test]
        public void TestInstantiateRoom()
        {
            var seed = new RandomSeed(12345);
            var prefab = Assets.InstantiatePrefab<Room>(Assets.Angle3x4RoomPath);
            var template = prefab.GetTemplate();

            // Create fake layout.
            var layout = new Layout(1, "Test", seed);
            var node = new ManiaMap.LayoutNode(1);
            var roomLayout = new ManiaMap.Room(node, Vector2DInt.Zero, template, seed);
            layout.Rooms.Add(roomLayout.Id, roomLayout);
            ManiaMapManager.Current.SetLayout(layout, new LayoutState(layout));

            // Create database.
            var obj = new GameObject("Room Prefab Database");
            var db = obj.AddComponent<RoomPrefabDatabase>();
            db.Entries.Add(new RoomDatabaseEntry<Room>(prefab.Id, prefab));
            db.CreateRoomPrefabDictionary();

            var room = db.InstantiateRoom(roomLayout.Id, null, RoomPositionOption.Layout);
            Object.DestroyImmediate(prefab.gameObject);
            Assert.IsNotNull(room);
        }
    }
}