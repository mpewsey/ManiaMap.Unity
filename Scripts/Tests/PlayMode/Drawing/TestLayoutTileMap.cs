using MPewsey.ManiaMap;
using MPewsey.ManiaMap.Samples;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace MPewsey.ManiaMapUnity.Drawing.Tests
{
    public class TestLayoutTileMap
    {
        private const string TestScene = "ManiaMap/Tests/TestLayoutTileMap";
        private GameObject GameObject { get; set; }
        private LayoutTileMap Map { get; set; }

        [SetUp]
        public void SetUp()
        {
            var resource = Resources.Load<GameObject>(TestScene);
            GameObject = Object.Instantiate(resource);
            Map = GameObject.GetComponent<LayoutTileMap>();
            Assert.IsTrue(Map != null);
        }

        [TearDown]
        public void TearDown()
        {
            Object.Destroy(GameObject);
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
