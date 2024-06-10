using MPewsey.ManiaMapUnity.Examples;
using NUnit.Framework;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.TestTools;

namespace MPewsey.ManiaMapUnity.Drawing.Tests
{
    public class TestLayoutTileMapSample
    {
        private const string TestScene = "ManiaMap/Tests/LayoutTileMapSample";
        private GameObject GameObject { get; set; }
        private LayoutTileMapSample Sample { get; set; }

        [SetUp]
        public void SetUp()
        {
            var resource = Resources.Load<GameObject>(TestScene);
            GameObject = Object.Instantiate(resource);
            Sample = GameObject.GetComponentInChildren<LayoutTileMapSample>();
            Assert.IsTrue(Sample != null);
        }

        [TearDown]
        public void TearDown()
        {
            Object.Destroy(GameObject);
        }

        [UnityTest]
        public IEnumerator TestPressGenerateButton()
        {
            var timeout = 20;
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Assert.IsTrue(Sample.GenerateButton.interactable);
            Sample.GenerateButton.onClick.Invoke();
            Assert.IsFalse(Sample.GenerateButton.interactable);
            yield return new WaitUntil(() => Sample.GenerateButton.interactable || stopwatch.Elapsed.TotalSeconds > timeout);
            Assert.IsTrue(Sample.GenerateButton.interactable);
            Assert.IsTrue(stopwatch.Elapsed.TotalSeconds <= timeout);
        }
    }
}
