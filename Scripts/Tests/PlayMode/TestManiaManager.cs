using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace MPewsey.ManiaMap.Unity.Tests
{
    public class TestManiaManager
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
            var manager = obj.AddComponent<ManiaManager>();
            Assert.AreEqual(manager, ManiaManager.Current);
        }

        [Test]
        public void TestCreateSingleton()
        {
            Assert.IsNotNull(ManiaManager.Current);
            Assert.IsNotNull(ManiaManager.Current);
        }

        [UnityTest]
        public IEnumerator TestMultipleSingletons()
        {
            var obj1 = new GameObject("ManiaManager");
            var manager1 = obj1.AddComponent<ManiaManager>();
            Assert.IsTrue(manager1.TryGetComponent(out DontDestroyOnLoad _));

            var obj2 = new GameObject("ManiaManager");
            var manager2 = obj2.AddComponent<ManiaManager>();
            Assert.IsTrue(manager2.TryGetComponent(out DontDestroyOnLoad _));

            yield return null;

            Assert.IsTrue((manager1 == null) ^ (manager2 == null));
        }
    }
}