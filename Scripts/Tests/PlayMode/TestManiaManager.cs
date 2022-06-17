using NUnit.Framework;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.TestTools;

namespace MPewsey.ManiaMap.Unity.Tests
{
    public class TestManiaManager
    {
        [SetUp]
        public void SetUp()
        {
            Assets.LoadEmptyScene();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(ManiaManager.Current);
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

            var obj2 = new GameObject("ManiaManager");
            var manager2 = obj2.AddComponent<ManiaManager>();

            yield return null;

            Assert.IsTrue((manager1 == null) ^ (manager2 == null));
        }

        [Test]
        public void TestInit()
        {
            var pipeline = Assets.InstantiatePrefab<GenerationPipeline>(Assets.BigLayoutPath);
            var results = pipeline.Generate(12345);
            Assert.IsTrue(results.Success);
            var layout = (Layout)results.Outputs["Layout"];
            var state = new LayoutState(layout);
            ManiaManager.Current.Init(layout, state);
            Assert.AreEqual(layout, ManiaManager.Current.Layout);
            Assert.AreEqual(state, ManiaManager.Current.LayoutState);
        }
    }
}