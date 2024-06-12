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
    public class TestRoomFlag
    {
        private const string TestScene = "TestRoomComponent2D";
        private RoomComponent Room { get; set; }
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
            var roomId = layout.Rooms.Keys.First();
            LayoutPack = new LayoutPack(layout, new LayoutState(layout));
            Room = database.InstantiateRoom(roomId, LayoutPack);
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            yield return Addressables.LoadSceneAsync("EmptyScene");
        }

        [Test]
        public void TestSettingFlags()
        {
            var flag = Object.FindAnyObjectByType<RoomFlag>();
            var roomState = Room.RoomState;
            Assert.IsTrue(flag != null);

            Assert.IsFalse(flag.FlagIsSet());
            Assert.IsFalse(roomState.Flags.Contains(flag.Id));

            Assert.IsTrue(flag.SetFlag());
            Assert.IsTrue(roomState.Flags.Contains(flag.Id));
            Assert.IsTrue(flag.FlagIsSet());

            Assert.IsTrue(flag.RemoveFlag());
            Assert.IsFalse(roomState.Flags.Contains(flag.Id));
            Assert.IsFalse(flag.FlagIsSet());
            Assert.IsFalse(flag.RemoveFlag());

            Assert.IsTrue(flag.ToggleFlag());
            Assert.IsTrue(roomState.Flags.Contains(flag.Id));
            Assert.IsTrue(flag.FlagIsSet());

            Assert.IsFalse(flag.ToggleFlag());
            Assert.IsFalse(roomState.Flags.Contains(flag.Id));
            Assert.IsFalse(flag.FlagIsSet());
        }
    }
}