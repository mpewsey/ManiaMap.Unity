using NUnit.Framework;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.TestTools;

namespace MPewsey.ManiaMapUnity.Examples.Tests
{
    public class TestRoomLayout2DSample
    {
        private const string TestScene = "RoomLayout2DSample";
        private RoomLayout2DSample Sample { get; set; }

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            yield return Addressables.LoadSceneAsync(TestScene);
            Sample = Object.FindAnyObjectByType<RoomLayout2DSample>();
            Assert.IsTrue(Sample != null);
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            yield return Addressables.LoadSceneAsync("EmptyScene");
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
