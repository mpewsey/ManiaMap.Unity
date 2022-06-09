using NUnit.Framework;
using System.Collections;
using UnityEditor;
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

        private GenerationPipeline LoadBigLayoutGenerator()
        {
            var path = "Packages/com.mpewsey.maniamap.unity/Prefabs/BigLayout/BigLayoutGenerator.prefab";
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            Assert.IsNotNull(prefab);
            var obj = Object.Instantiate(prefab, Container.transform);
            var generator = obj.GetComponent<GenerationPipeline>();
            Assert.IsNotNull(generator);
            return generator;
        }

        [Test]
        public void TestBigLayoutGenerator()
        {
            var generator = LoadBigLayoutGenerator();
            Random.InitState(12345);
            var results = generator.Generate();
            var layout = (Layout)results.Outputs["Layout"];
            Assert.IsNotNull(layout);
        }

        [UnityTest]
        public IEnumerator TestBigLayoutGeneratorAsync()
        {
            var generator = LoadBigLayoutGenerator();
            Random.InitState(12345);
            var task = generator.GenerateAsync();
            yield return new WaitUntil(() => task.IsCompleted);
            Assert.IsTrue(task.IsCompleted);
            Assert.IsTrue(task.IsCompletedSuccessfully);
            var results = task.Result;
            Assert.IsTrue(results.Success);
            var layout = (Layout)results.Outputs["Layout"];
            Assert.IsNotNull(layout);
        }

        [UnityTest]
        public IEnumerator TestSeededBigLayoutGeneratorAsync()
        {
            var generator = LoadBigLayoutGenerator();
            var task = generator.GenerateAsync(12345);
            yield return new WaitUntil(() => task.IsCompleted);
            Assert.IsTrue(task.IsCompleted);
            Assert.IsTrue(task.IsCompletedSuccessfully);
            var results = task.Result;
            Assert.IsTrue(results.Success);
            var layout = (Layout)results.Outputs["Layout"];
            Assert.IsNotNull(layout);
        }
    }
}
