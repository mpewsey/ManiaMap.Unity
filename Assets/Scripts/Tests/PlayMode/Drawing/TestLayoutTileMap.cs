using MPewsey.ManiaMap;
using MPewsey.ManiaMap.Samples;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.TestTools;

namespace MPewsey.ManiaMapUnity.Drawing.Tests
{
    public class TestLayoutTileMap
    {
        private const string TestScene = "TestLayoutTileMap";
        private LayoutTileMap Map { get; set; }

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            yield return Addressables.LoadSceneAsync(TestScene);
            Map = Object.FindAnyObjectByType<LayoutTileMap>();
            Assert.IsTrue(Map != null);
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            yield return Addressables.LoadSceneAsync("EmptyScene");
        }

        [UnityTest]
        public IEnumerator TestDrawMap()
        {
            var task = BigLayoutSample.GenerateAsync(12345, Debug.Log);
            yield return new WaitUntil(() => task.IsCompleted);
            Assert.IsTrue(task.IsCompletedSuccessfully);
            var results = task.Result;
            Assert.IsTrue(results.Success);
            var layout = results.GetOutput<Layout>("Layout");
            Assert.IsNotNull(layout);
            Map.DrawMap(layout);
            Assert.IsTrue(Map.Grid != null);
            Assert.AreEqual(Map.Grid.transform.childCount, 1);
        }
    }
}
