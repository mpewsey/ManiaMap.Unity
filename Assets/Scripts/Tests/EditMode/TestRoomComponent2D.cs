using MPewsey.ManiaMap;
using MPewsey.ManiaMap.Exceptions;
using NUnit.Framework;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Tests.EditMode
{
    public class TestRoomComponent2D
    {
        private GameObject GameObject { get; set; }
        private RoomComponent Room { get; set; }

        [SetUp]
        public void SetUp()
        {
            GameObject = new GameObject("Room");
            Room = GameObject.AddComponent<RoomComponent>();
            Room.transform.position = Vector3.zero;
            Room.CellSize = new Vector3(100, 100, 100);
            Room.Size = new Vector2Int(3, 3);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(GameObject);
        }

        [Test]
        public void TestAutoAssign()
        {
            var flag = new GameObject("Room Flag").AddComponent<RoomFlag>();
            flag.transform.SetParent(Room.transform);
            flag.transform.position = new Vector3(250, -150, 0);

            var collectableSpot = new GameObject("Collectable Spot").AddComponent<CollectableSpotComponent>();
            collectableSpot.transform.SetParent(Room.transform);
            collectableSpot.transform.position = new Vector3(250, -150, 0);

            var feature = new GameObject("Feature").AddComponent<Feature>();
            feature.transform.SetParent(Room.transform);
            feature.transform.position = new Vector3(250, -150, 0);

            var door = new GameObject("Door").AddComponent<DoorComponent>();
            door.transform.SetParent(Room.transform);
            door.transform.position = new Vector3(0, -50, 0);

            Room.AutoAssign();
            Assert.AreEqual(Room, flag.Room);
            Assert.Greater(flag.Id, 0);
            Assert.AreEqual(1, flag.Row);
            Assert.AreEqual(2, flag.Column);

            Assert.AreEqual(Room, collectableSpot.Room);
            Assert.Greater(collectableSpot.Id, 0);
            Assert.AreEqual(1, collectableSpot.Row);
            Assert.AreEqual(2, collectableSpot.Column);

            Assert.AreEqual(Room, feature.Room);
            Assert.AreEqual(1, feature.Row);
            Assert.AreEqual(2, feature.Column);

            Assert.AreEqual(Room, door.Room);
            Assert.AreEqual(DoorDirection.West, door.Direction);
        }

        [Test]
        public void TestDuplicateRoomFlagsThrowsException()
        {
            var flag1 = new GameObject("Flag 1").AddComponent<RoomFlag>();
            flag1.Id = 1;
            flag1.transform.SetParent(Room.transform);

            var flag2 = new GameObject("Flag 2").AddComponent<RoomFlag>();
            flag2.Id = 1;
            flag2.transform.SetParent(Room.transform);

            Assert.Throws<DuplicateIdException>(() => Room.ValidateRoomFlags());
        }

        [Test]
        public void TestValidateRoomFlags()
        {
            var flag1 = new GameObject("Flag 1").AddComponent<RoomFlag>();
            flag1.Id = 1;
            flag1.transform.SetParent(Room.transform);

            var flag2 = new GameObject("Flag 2").AddComponent<RoomFlag>();
            flag2.Id = 2;
            flag2.transform.SetParent(Room.transform);

            Room.ValidateRoomFlags();
        }
    }
}