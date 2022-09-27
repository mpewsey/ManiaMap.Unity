using MPewsey.ManiaMap.Unity.Exceptions;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace MPewsey.ManiaMap.Unity.Tests
{
    public class TestGenerationPipeline
    {
        [SetUp]
        public void SetUp()
        {
            Assets.DestroyAllGameObjects();
        }

        [TestCase(Assets.BigLayoutPath)]
        [TestCase(Assets.CrossLayoutPath)]
        [TestCase(Assets.GeekLayoutPath)]
        [TestCase(Assets.LoopLayoutPath)]
        [TestCase(Assets.StackedLoopLayoutPath)]
        public void TestGenerate(string path)
        {
            var pipeline = Assets.InstantiatePrefab<GenerationPipeline>(path);
            var results = pipeline.Generate();
            var layout = (Layout)results.Outputs["Layout"];
            Assert.IsNotNull(layout);
        }

        [UnityTest]
        public IEnumerator TestBigLayoutGeneratorAsync()
        {
            var pipeline = Assets.InstantiatePrefab<GenerationPipeline>(Assets.BigLayoutPath);
            var task = pipeline.GenerateAsync();
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
            var pipeline = Assets.InstantiatePrefab<GenerationPipeline>(Assets.BigLayoutPath);
            pipeline.SetSeed(12345);
            var task = pipeline.GenerateAsync();
            yield return new WaitUntil(() => task.IsCompleted);
            Assert.IsTrue(task.IsCompleted);
            Assert.IsTrue(task.IsCompletedSuccessfully);
            var results = task.Result;
            Assert.IsTrue(results.Success);
            var layout = (Layout)results.Outputs["Layout"];
            Assert.IsNotNull(layout);
        }

        [Test]
        public void TestIsValid()
        {
            var generator1 = Assets.InstantiatePrefab<GenerationPipeline>(Assets.BigLayoutPath);
            Assert.IsTrue(generator1.IsValid());

            var generator2 = Assets.InstantiatePrefab<GenerationPipeline>(Assets.BigLayoutPath);
            var groups = generator2.GetComponentInChildren<CollectableGroupsInput>();
            Assert.IsNotNull(groups);
            Object.DestroyImmediate(groups);
            Assert.IsFalse(generator2.IsValid());

            var generator3 = Assets.InstantiatePrefab<GenerationPipeline>(Assets.BigLayoutPath);
            generator3.InputsContainer.AddComponent<CollectableGroupsInput>();
            Assert.IsFalse(generator3.IsValid());
        }

        [Test]
        public void TestValidate()
        {
            var generator1 = Assets.InstantiatePrefab<GenerationPipeline>(Assets.BigLayoutPath);
            generator1.Validate();

            var generator2 = Assets.InstantiatePrefab<GenerationPipeline>(Assets.BigLayoutPath);
            var groups = generator2.GetComponentInChildren<CollectableGroupsInput>();
            Assert.IsNotNull(groups);
            Object.DestroyImmediate(groups);
            Assert.Throws<MissingInputException>(generator2.Validate);

            var generator3 = Assets.InstantiatePrefab<GenerationPipeline>(Assets.BigLayoutPath);
            generator3.InputsContainer.AddComponent<CollectableGroupsInput>();
            Assert.Throws<DuplicateInputException>(generator3.Validate);
        }
    }
}
