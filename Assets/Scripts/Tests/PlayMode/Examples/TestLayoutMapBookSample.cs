using MPewsey.ManiaMapUnity.Drawing;
using NUnit.Framework;
using System.Collections;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.TestTools;

namespace MPewsey.ManiaMapUnity.Examples.Tests
{
    public class TestLayoutMapBookSample
    {
        private const string ArtifactPath = "Tests/LayoutMapBook";
        private const string TestScene = "LayoutMapBookSample";
        private LayoutMapBookSample Sample { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            if (Directory.Exists(ArtifactPath))
                Directory.Delete(ArtifactPath, true);

            Directory.CreateDirectory(ArtifactPath);
        }

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            yield return Addressables.LoadSceneAsync(TestScene);
            Sample = Object.FindAnyObjectByType<LayoutMapBookSample>();
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

        [UnityTest]
        public IEnumerator TestSetOnionskinColor()
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

            var map = Sample.GetComponent<LayoutMapBook>();
            Assert.IsTrue(map != null);

            Assert.AreEqual(0, map.SetOnionskinColors(0, Gradients.RedWhiteBlueGradient()));
        }

        [UnityTest]
        public IEnumerator TestSaveImages()
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

            var map = Sample.GetComponent<LayoutMapBook>();
            Assert.IsTrue(map != null);

            var pngPaths = map.SaveImages(Path.Combine(ArtifactPath, "png_map.png"));
            Assert.Greater(pngPaths.Count, 0);

            var jpegPaths = map.SaveImages(Path.Combine(ArtifactPath, "jpeg_map.jpeg"));
            Assert.Greater(jpegPaths.Count, 0);
        }
    }
}
