using MPewsey.ManiaMap;
using MPewsey.ManiaMapUnity.Generators;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.TestTools;

namespace MPewsey.ManiaMapUnity.Tests
{
    public class TestCollectableSpotComponent
    {
        private const string TestScene = "TestRoomComponent2D";
        private CollectableSpotComponent CollectableSpot { get; set; }
        private LayoutPack LayoutPack { get; set; }

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            yield return Addressables.LoadSceneAsync(TestScene);
            var handle = Addressables.LoadAssetAsync<RoomTemplateDatabase>("2DRoomTemplateDatabase");
            yield return handle;
            var database = handle.Result;
            Assert.IsTrue(database != null);
            var pipeline = Object.FindAnyObjectByType<GenerationPipeline>();
            Assert.IsTrue(pipeline != null);
            var results = pipeline.Run();
            Assert.IsTrue(results.Success);
            var layout = results.GetOutput<Layout>("Layout");
            Assert.IsNotNull(layout);
            LayoutPack = new LayoutPack(layout, new LayoutState(layout));
            var rooms = database.InstantiateAllRooms(LayoutPack);
            Assert.Greater(rooms.Count, 0);
            yield return null;
            CollectableSpot = Object.FindAnyObjectByType<CollectableSpotComponent>();
            Assert.IsTrue(CollectableSpot != null);
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            yield return Addressables.LoadSceneAsync("EmptyScene");
        }

        [Test]
        public void TestCollectableId()
        {
            Assert.Greater(CollectableSpot.CollectableId(), 0);
        }

        [Test]
        public void TestCollectableExists()
        {
            Assert.IsTrue(CollectableSpot.CollectableExists());
        }

        [Test]
        public void TestIsAcquired()
        {
            Assert.IsFalse(CollectableSpot.IsAcquired());
        }

        [Test]
        public void TestCanAcquire()
        {
            Assert.IsTrue(CollectableSpot.CanAcquire());
        }

        [Test]
        public void TestAcquire()
        {
            Assert.IsTrue(CollectableSpot.Acquire());
            Assert.IsFalse(CollectableSpot.Acquire());
            Assert.IsTrue(CollectableSpot.IsAcquired());
        }
    }
}