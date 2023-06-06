using NUnit.Framework;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Tests
{
    public class TestThreshold
    {
        private Threshold Threshold { get; set; }

        [SetUp]
        public void SetUp()
        {
            Assets.DestroyAllGameObjects();
            Threshold = new GameObject("Threshold").AddComponent<Threshold>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.Destroy(Threshold.gameObject);
        }

        [Test]
        public void TestParameterize()
        {
            var center = new Vector3(100, 200, 300);
            var size = new Vector3(2, 4, 6);

            Threshold.Center = center;
            Threshold.Size = size;

            Assert.IsTrue(Vector3.one == Threshold.Parameterize(center + 0.5f * size));
            Assert.IsTrue(Vector3.zero == Threshold.Parameterize(center - 0.5f * size));
            Assert.IsTrue(0.5f * Vector3.one == Threshold.Parameterize(center));
        }

        [Test]
        public void TestInterpolate()
        {
            var center = new Vector3(100, 200, 300);
            var size = new Vector3(2, 4, 6);

            Threshold.Center = center;
            Threshold.Size = size;

            Assert.IsTrue(center == Threshold.Interpolate(0.5f * Vector3.one));
            Assert.IsTrue(center + 0.5f * size == Threshold.Interpolate(Vector3.one));
            Assert.IsTrue(center - 0.5f * size == Threshold.Interpolate(Vector3.zero));
        }
    }
}