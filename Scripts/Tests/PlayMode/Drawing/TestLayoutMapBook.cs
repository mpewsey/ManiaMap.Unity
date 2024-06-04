using MPewsey.ManiaMap;
using MPewsey.ManiaMap.Samples;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace MPewsey.ManiaMapUnity.Drawing.Tests
{
    public class TestLayoutMapBook
    {
        private const string TestScene = "ManiaMap/Tests/TestLayoutMapBook";
        private GameObject GameObject { get; set; }
        private LayoutMapBook Map { get; set; }

        [SetUp]
        public void SetUp()
        {
            var resource = Resources.Load<GameObject>(TestScene);
            GameObject = Object.Instantiate(resource);
            Map = GameObject.GetComponent<LayoutMapBook>();
            Assert.IsTrue(Map != null);
        }

        [TearDown]
        public void TearDown()
        {
            Object.Destroy(GameObject);
        }

        [UnityTest]
        public IEnumerator TestDrawPages()
        {
            var task = BigLayoutSample.GenerateAsync(12345, Debug.Log);
            yield return new WaitUntil(() => task.IsCompleted);
            Assert.IsTrue(task.IsCompletedSuccessfully);
            var results = task.Result;
            Assert.IsTrue(results.Success);
            var layout = results.GetOutput<Layout>("Layout");
            Assert.IsNotNull(layout);
            Map.DrawPages(layout);
            Assert.IsTrue(Map.Container != null);
            Assert.AreEqual(Map.Container.transform.childCount, 1);
        }
    }
}
