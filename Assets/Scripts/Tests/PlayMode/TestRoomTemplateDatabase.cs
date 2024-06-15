using MPewsey.ManiaMap;
using MPewsey.ManiaMapUnity.Generators;
using NUnit.Framework;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.TestTools;

namespace MPewsey.ManiaMapUnity.Tests
{
    public class TestRoomTemplateDatabase
    {
        private const string TestScene = "TestRoomComponent2D";
        private LayoutPack LayoutPack { get; set; }
        private RoomTemplateDatabase RoomTemplateDatabase { get; set; }

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            yield return Addressables.LoadSceneAsync(TestScene);
            var handle = Addressables.LoadAssetAsync<RoomTemplateDatabase>("2DRoomTemplateDatabase");
            yield return handle;
            RoomTemplateDatabase = handle.Result;
            Assert.IsTrue(RoomTemplateDatabase != null);
            var pipeline = Object.FindAnyObjectByType<GenerationPipeline>();
            Assert.IsTrue(pipeline != null);
            var results = pipeline.Run();
            Assert.IsTrue(results.Success);
            var layout = results.GetOutput<Layout>("Layout");
            Assert.IsNotNull(layout);
            LayoutPack = new LayoutPack(layout, new LayoutState(layout));
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            yield return Addressables.LoadSceneAsync("EmptyScene");
        }

        [Test]
        public void TestInstantiateAllRooms()
        {
            var rooms = RoomTemplateDatabase.InstantiateAllRooms(LayoutPack);
            Assert.Greater(rooms.Count, 0);

            foreach (var room in rooms)
            {
                Assert.IsTrue(room.IsInitialized);
            }
        }

        [UnityTest]
        public IEnumerator TestInstantiateAllRoomsAsync()
        {
            var task = RoomTemplateDatabase.InstantiateAllRoomsAsync(LayoutPack);
            yield return new WaitUntil(() => task.IsCompleted);
            Assert.IsTrue(task.IsCompletedSuccessfully);
            var rooms = task.Result;
            Assert.Greater(rooms.Count, 0);

            foreach (var room in rooms)
            {
                Assert.IsTrue(room.IsInitialized);
            }
        }

        [Test]
        public void TestInstantiateRooms()
        {
            var rooms = RoomTemplateDatabase.InstantiateRooms(LayoutPack);
            Assert.Greater(rooms.Count, 0);

            foreach (var room in rooms)
            {
                Assert.IsTrue(room.IsInitialized);
            }
        }

        [UnityTest]
        public IEnumerator TestInstantiateRoomsAsync()
        {
            var task = RoomTemplateDatabase.InstantiateRoomsAsync(LayoutPack);
            yield return new WaitUntil(() => task.IsCompleted);
            Assert.IsTrue(task.IsCompletedSuccessfully);
            var rooms = task.Result;
            Assert.Greater(rooms.Count, 0);

            foreach (var room in rooms)
            {
                Assert.IsTrue(room.IsInitialized);
            }
        }

        [Test]
        public void TestInstantiateRoom()
        {
            var roomId = LayoutPack.Layout.Rooms.Keys.First();
            var room = RoomTemplateDatabase.InstantiateRoom(roomId, LayoutPack);
            Assert.IsTrue(room != null);
            Assert.IsTrue(room.IsInitialized);
        }

        [UnityTest]
        public IEnumerator TestInstantiateRoomAsync()
        {
            var roomId = LayoutPack.Layout.Rooms.Keys.First();
            var handle = RoomTemplateDatabase.InstantiateRoomAsync(roomId, LayoutPack);
            yield return handle;
            var room = handle.Result.GetComponent<RoomComponent>();
            Assert.IsTrue(room != null);
            Assert.IsTrue(room.IsInitialized);
        }
    }
}