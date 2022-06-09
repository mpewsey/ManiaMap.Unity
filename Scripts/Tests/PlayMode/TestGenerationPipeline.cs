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

        [Test]
        public void TestBigLayoutGenerator()
        {
            var generator = TestAssets.LoadBigLayoutGenerator(Container.transform);
            Random.InitState(12345);
            var results = generator.Generate();
            var layout = (Layout)results.Outputs["Layout"];
            Assert.IsNotNull(layout);
        }

        [Test]
        public void TestCrossLayoutGenerator()
        {
            var generator = TestAssets.LoadCrossLayoutGenerator(Container.transform);
            Random.InitState(12345);
            var results = generator.Generate();
            var layout = (Layout)results.Outputs["Layout"];
            Assert.IsNotNull(layout);
        }

        [Test]
        public void TestGeekLayoutGenerator()
        {
            var generator = TestAssets.LoadGeekLayoutGenerator(Container.transform);
            Random.InitState(12345);
            var results = generator.Generate();
            var layout = (Layout)results.Outputs["Layout"];
            Assert.IsNotNull(layout);
        }

        [Test]
        public void TestLoopLayoutGenerator()
        {
            var generator = TestAssets.LoadLoopLayoutGenerator(Container.transform);
            Random.InitState(12345);
            var results = generator.Generate();
            var layout = (Layout)results.Outputs["Layout"];
            Assert.IsNotNull(layout);
        }

        [Test]
        public void TestStackedLoopLayoutGenerator()
        {
            var generator = TestAssets.LoadStackedLoopLayoutGenerator(Container.transform);
            Random.InitState(12345);
            var results = generator.Generate();
            var layout = (Layout)results.Outputs["Layout"];
            Assert.IsNotNull(layout);
        }

        [UnityTest]
        public IEnumerator TestBigLayoutGeneratorAsync()
        {
            var generator = TestAssets.LoadBigLayoutGenerator(Container.transform);
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
            var generator = TestAssets.LoadBigLayoutGenerator(Container.transform);
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
