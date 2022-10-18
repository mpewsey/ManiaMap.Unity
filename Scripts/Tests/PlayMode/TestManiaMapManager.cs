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
        public void TestFindExistingSingleton()
        {
            var obj = new GameObject("ManiaManager");
            var manager = obj.AddComponent<ManiaMapManager>();
            Assert.AreEqual(manager, ManiaMapManager.Current);
        }

        [Test]
        public void TestCreateSingleton()
        {
            Assert.IsNotNull(ManiaMapManager.Current);
            Assert.IsNotNull(ManiaMapManager.Current);
        }

        [UnityTest]
        public IEnumerator TestMultipleSingletons()
        {
            var obj1 = new GameObject("ManiaManager");
            var manager1 = obj1.AddComponent<ManiaMapManager>();
            Assert.IsTrue(manager1.TryGetComponent(out DontDestroyOnLoad _));

            var obj2 = new GameObject("ManiaManager");
            var manager2 = obj2.AddComponent<ManiaMapManager>();
            Assert.IsTrue(manager2.TryGetComponent(out DontDestroyOnLoad _));

            yield return null;

            Assert.IsTrue((manager1 == null) ^ (manager2 == null));
        }
    }
}