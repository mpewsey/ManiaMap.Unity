using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace MPewsey.ManiaMap.Unity.Tests
{
    public class TestGenerationPipeline
    {
        private GameObject Container { get; set; }

        [SetUp]
        public void SetUp()
        {
            Container = new GameObject("TestGenerationPipeline");
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(Container);
        }

        [Test]
        public void TestBigLayoutGenerator()
        {
            var prefab = Resources.Load<GameObject>("ManiaMap/BigLayoutGenerator");
            Assert.IsNotNull(prefab);
            var obj = Object.Instantiate(prefab, Container.transform);
            var generator = obj.GetComponent<GenerationPipeline>();
            Assert.IsNotNull(generator);
            Random.InitState(12345);
            var results = generator.Generate();
            var layout = (Layout)results.Outputs["Layout"];
            Assert.IsNotNull(layout);
        }

        [UnityTest]
        public IEnumerator TestBigLayoutGeneratorAsync()
        {
            var prefab = Resources.Load<GameObject>("ManiaMap/BigLayoutGenerator");
            Assert.IsNotNull(prefab);
            var obj = Object.Instantiate(prefab, Container.transform);
            var generator = obj.GetComponent<GenerationPipeline>();
            Assert.IsNotNull(generator);
            Random.InitState(12345);
            var task = generator.GenerateAsync();
            yield return new WaitUntil(() => task.IsCompleted);
            Assert.IsTrue(task.IsCompleted);
            Assert.IsTrue(task.IsCompletedSuccessfully);
            var results = task.Result;
            var layout = (Layout)results.Outputs["Layout"];
            Assert.IsNotNull(layout);
        }

        [UnityTest]
        public IEnumerator TestSeededBigLayoutGeneratorAsync()
        {
            var prefab = Resources.Load<GameObject>("ManiaMap/BigLayoutGenerator");
            Assert.IsNotNull(prefab);
            var obj = Object.Instantiate(prefab, Container.transform);
            var generator = obj.GetComponent<GenerationPipeline>();
            Assert.IsNotNull(generator);
            var task = generator.GenerateAsync(12345);
            yield return new WaitUntil(() => task.IsCompleted);
            Assert.IsTrue(task.IsCompleted);
            Assert.IsTrue(task.IsCompletedSuccessfully);
            var results = task.Result;
            var layout = (Layout)results.Outputs["Layout"];
            Assert.IsNotNull(layout);
        }
    }
}
