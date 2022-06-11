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
            AssetLoader.LoadEmptyScene();
        }

        [Test]
        public void TestBigLayoutGenerator()
        {
            var generator = AssetLoader.LoadBigLayoutGenerator();
            Random.InitState(12345);
            var results = generator.Generate();
            var layout = (Layout)results.Outputs["Layout"];
            Assert.IsNotNull(layout);
        }

        [Test]
        public void TestCrossLayoutGenerator()
        {
            var generator = AssetLoader.LoadCrossLayoutGenerator();
            Random.InitState(12345);
            var results = generator.Generate();
            var layout = (Layout)results.Outputs["Layout"];
            Assert.IsNotNull(layout);
        }

        [Test]
        public void TestGeekLayoutGenerator()
        {
            var generator = AssetLoader.LoadGeekLayoutGenerator();
            Random.InitState(12345);
            var results = generator.Generate();
            var layout = (Layout)results.Outputs["Layout"];
            Assert.IsNotNull(layout);
        }

        [Test]
        public void TestLoopLayoutGenerator()
        {
            var generator = AssetLoader.LoadLoopLayoutGenerator();
            Random.InitState(12345);
            var results = generator.Generate();
            var layout = (Layout)results.Outputs["Layout"];
            Assert.IsNotNull(layout);
        }

        [Test]
        public void TestStackedLoopLayoutGenerator()
        {
            var generator = AssetLoader.LoadStackedLoopLayoutGenerator();
            Random.InitState(12345);
            var results = generator.Generate();
            var layout = (Layout)results.Outputs["Layout"];
            Assert.IsNotNull(layout);
        }

        [UnityTest]
        public IEnumerator TestBigLayoutGeneratorAsync()
        {
            var generator = AssetLoader.LoadBigLayoutGenerator();
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
            var generator = AssetLoader.LoadBigLayoutGenerator();
            var task = generator.GenerateAsync(12345);
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
            var generator1 = AssetLoader.LoadBigLayoutGenerator();
            Assert.IsTrue(generator1.IsValid());

            var generator2 = AssetLoader.LoadBigLayoutGenerator();
            var groups = generator2.GetComponentInChildren<CollectableGroupsInput>();
            Assert.IsNotNull(groups);
            Object.DestroyImmediate(groups);
            Assert.IsFalse(generator2.IsValid());

            var generator3 = AssetLoader.LoadBigLayoutGenerator();
            generator3.InputsContainer.AddComponent<CollectableGroupsInput>();
            Assert.IsFalse(generator3.IsValid());
        }

        [Test]
        public void TestValidate()
        {
            var generator1 = AssetLoader.LoadBigLayoutGenerator();
            generator1.Validate();

            var generator2 = AssetLoader.LoadBigLayoutGenerator();
            var groups = generator2.GetComponentInChildren<CollectableGroupsInput>();
            Assert.IsNotNull(groups);
            Object.DestroyImmediate(groups);
            Assert.Throws<MissingInputException>(generator2.Validate);

            var generator3 = AssetLoader.LoadBigLayoutGenerator();
            generator3.InputsContainer.AddComponent<CollectableGroupsInput>();
            Assert.Throws<DuplicateInputException>(generator3.Validate);
        }
    }
}
