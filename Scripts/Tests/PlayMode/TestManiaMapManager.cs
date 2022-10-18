using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace MPewsey.ManiaMap.Unity.Tests
{
    public class TestManiaMapManager
    {
        [SetUp]
        public void SetUp()
        {
            Assets.DestroyAllGameObjects();
        }

        [Test]
        public void TestCreateSingleton()
        {
            Assert.IsNotNull(ManiaMapManager.Current);
            Assert.IsNotNull(ManiaMapManager.Current);
        }
    }
}