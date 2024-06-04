using NUnit.Framework;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Tests
{
    public class TestDoorThreshold
    {
        private GameObject GameObject { get; set; }
        private DoorThreshold DoorThreshold { get; set; }

        [SetUp]
        public void SetUp()
        {
            GameObject = new GameObject("DoorThreshold");
            DoorThreshold = GameObject.AddComponent<DoorThreshold>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.Destroy(GameObject);
        }

        [Test]
        public void TestParameterize()
        {
            var center = new Vector3(100, 200, 300);
            var size = new Vector3(2, 4, 6);

            DoorThreshold.Center = center;
            DoorThreshold.Size = size;

            Assert.IsTrue(Vector3.one == DoorThreshold.Parameterize(center + 0.5f * size));
            Assert.IsTrue(Vector3.zero == DoorThreshold.Parameterize(center - 0.5f * size));
            Assert.IsTrue(0.5f * Vector3.one == DoorThreshold.Parameterize(center));
        }

        [Test]
        public void TestInterpolate()
        {
            var center = new Vector3(100, 200, 300);
            var size = new Vector3(2, 4, 6);

            DoorThreshold.Center = center;
            DoorThreshold.Size = size;

            Assert.IsTrue(center == DoorThreshold.Interpolate(0.5f * Vector3.one));
            Assert.IsTrue(center + 0.5f * size == DoorThreshold.Interpolate(Vector3.one));
            Assert.IsTrue(center - 0.5f * size == DoorThreshold.Interpolate(Vector3.zero));
        }
    }
}