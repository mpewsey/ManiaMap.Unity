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
    public class TestRoomDatabase
    {
        private const string TestScene = "TestRoomComponent2D";
        private RoomDatabase RoomDatabase { get; set; }
        private LayoutPack LayoutPack { get; set; }

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            yield return Addressables.LoadSceneAsync(TestScene);
            var handle = Addressables.LoadAssetAsync<RoomDatabase>("2DRoomDatabase");
            yield return handle;
            RoomDatabase = handle.Result;
            Assert.IsTrue(RoomDatabase != null);
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
            var rooms = RoomDatabase.InstantiateAllRooms(LayoutPack);
            Assert.Greater(rooms.Count, 0);

            foreach (var room in rooms)
            {
                Assert.IsTrue(room.IsInitialized);
            }
        }

        [Test]
        public void TestInstantiateRooms()
        {
            var rooms = RoomDatabase.InstantiateRooms(LayoutPack);
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
            var room = RoomDatabase.InstantiateRoom(roomId, LayoutPack);
            Assert.IsTrue(room != null);
            Assert.IsTrue(room.IsInitialized);
        }
    }
}