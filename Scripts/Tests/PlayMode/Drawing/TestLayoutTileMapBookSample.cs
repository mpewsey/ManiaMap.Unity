using MPewsey.ManiaMapUnity.Examples;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace MPewsey.ManiaMapUnity.Drawing.Tests
{
    public class TestLayoutTileMapBookSample
    {
        private const string TestScene = "ManiaMap/Tests/LayoutTileMapBookSample";
        private GameObject GameObject { get; set; }
        private LayoutTileMapBookSample Sample { get; set; }

        [SetUp]
        public void SetUp()
        {
            var resource = Resources.Load<GameObject>(TestScene);
            GameObject = Object.Instantiate(resource);
            Sample = GameObject.GetComponentInChildren<LayoutTileMapBookSample>();
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
            Assert.IsTrue(Sample.GenerateButton.interactable);
            Sample.GenerateButton.onClick.Invoke();
            Assert.IsFalse(Sample.GenerateButton.interactable);
            yield return new WaitUntil(() => Sample.GenerateButton.interactable);
            Assert.IsTrue(Sample.GenerateButton.interactable);
        }
    }
}
