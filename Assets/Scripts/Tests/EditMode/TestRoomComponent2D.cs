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
            var flag = new GameObject("Flag 1").AddComponent<RoomFlag>();
            flag.transform.SetParent(Room.transform);
            flag.transform.position = new Vector3(250, -150, 0);

            Room.AutoAssign();
            Assert.AreEqual(Room, flag.Room);
            Assert.Greater(flag.Id, 0);
            Assert.AreEqual(1, flag.Row);
            Assert.AreEqual(2, flag.Column);
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