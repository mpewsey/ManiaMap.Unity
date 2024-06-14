using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.TestTools;

namespace MPewsey.ManiaMapUnity.Tests
{
    public class TestDoorThreshold
    {
        private Vector3[] Points { get; } = new Vector3[]
        {
            new Vector3(200, 300, 1000),
            new Vector3(150, 300, 900),
            new Vector3(200, 275, 900),
            new Vector3(250, 300, 1100),
            new Vector3(200, 325, 900),
            new Vector3(175, 300, 950),
            new Vector3(200, 287.5f, 1050),
        };

        private Vector3[] Parameters { get; } = new Vector3[]
        {
            new Vector3(0.5f, 0.5f, 0.5f),
            new Vector3(0, 0.5f, 0),
            new Vector3(0.5f, 0, 0),
            new Vector3(1, 0.5f, 1),
            new Vector3(0.5f, 1, 0),
            new Vector3(0.25f, 0.5f, 0.25f),
            new Vector3(0.5f, 0.25f, 0.75f),
        };

        private DoorThreshold DoorThreshold { get; set; }

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            yield return Addressables.LoadSceneAsync("EmptyScene");
            var obj = new GameObject("Door Threshold");
            DoorThreshold = obj.AddComponent<DoorThreshold>();
            DoorThreshold.Size = new Vector3(100, 50, 200);
            DoorThreshold.transform.position = new Vector3(200, 300, 1000);
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            yield return Addressables.LoadSceneAsync("EmptyScene");
        }

        [Test]
        public void TestOutOfBoundsParameterizePosition()
        {
            var parameter1 = new Vector3(-100, 300, -5000);
            var point1 = DoorThreshold.ParameterizePosition(parameter1);
            Assert.AreEqual(new Vector3(0, 0.5f, 0), point1);

            var parameter2 = new Vector3(200, -100, 5000);
            var point2 = DoorThreshold.ParameterizePosition(parameter2);
            Assert.AreEqual(new Vector3(0.5f, 0, 1), point2);

            var parameter3 = new Vector3(1000, 300, -5000);
            var point3 = DoorThreshold.ParameterizePosition(parameter3);
            Assert.AreEqual(new Vector3(1, 0.5f, 0), point3);

            var parameter4 = new Vector3(200, 1000, 5000);
            var point4 = DoorThreshold.ParameterizePosition(parameter4);
            Assert.AreEqual(new Vector3(0.5f, 1, 1), point4);
        }

        [Test]
        public void TestParameterizePosition()
        {
            var points = Points;
            var expected = Parameters;

            for (int i = 0; i < points.Length; i++)
            {
                var point = points[i];
                var parameters = DoorThreshold.ParameterizePosition(point);
                var checkPoint = DoorThreshold.InterpolatePosition(parameters);
                Assert.AreEqual(expected[i], parameters, $"Parameter error at index {i}");
                Assert.AreEqual(point, checkPoint, $"Point error at index {i}");
            }
        }

        [Test]
        public void TestOutOfBoundsInterpolatePosition()
        {
            var point1 = new Vector3(-100, 0.5f, 0.5f);
            var parameter1 = DoorThreshold.InterpolatePosition(point1);
            Assert.AreEqual(new Vector3(150, 300, 1000), parameter1);

            var point2 = new Vector3(0.5f, -100, 0.5f);
            var parameter2 = DoorThreshold.InterpolatePosition(point2);
            Assert.AreEqual(new Vector3(200, 275, 1000), parameter2);

            var point3 = new Vector3(100, 0.5f, -5000);
            var parameter3 = DoorThreshold.InterpolatePosition(point3);
            Assert.AreEqual(new Vector3(250, 300, 900), parameter3);

            var point4 = new Vector3(0.5f, 100, 5000);
            var parameter4 = DoorThreshold.InterpolatePosition(point4);
            Assert.AreEqual(new Vector3(200, 325, 1100), parameter4);
        }

        [Test]
        public void TestInterpolatePosition()
        {
            var parameters = Parameters;
            var expected = Points;

            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                var point = DoorThreshold.InterpolatePosition(parameter);
                var checkParameter = DoorThreshold.ParameterizePosition(point);
                Assert.AreEqual(expected[i], point, $"Point error at index {i}");
                Assert.AreEqual(parameter, checkParameter, $"Parameter error at index {i}");
            }
        }
    }
}