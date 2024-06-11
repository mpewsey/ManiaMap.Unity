using MPewsey.ManiaMap;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.TestTools;

namespace MPewsey.ManiaMapUnity.Generators.Tests
{
    public class TestGenerationPipeline
    {
        private const string TestScene = "TestGenerationPipeline";
        private GenerationPipeline Pipeline { get; set; }

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            yield return Addressables.LoadSceneAsync(TestScene);
            Pipeline = Object.FindAnyObjectByType<GenerationPipeline>();
            Assert.IsTrue(Pipeline != null);
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            yield return Addressables.LoadSceneAsync("EmptyScene");
        }

        [Test]
        public void TestRun()
        {
            var results = Pipeline.Run(logger: Debug.Log);
            Assert.IsTrue(results.Success);
            var layout = results.GetOutput<Layout>("Layout");
            Assert.IsNotNull(layout);
        }

        [UnityTest]
        public IEnumerator TestRunAsync()
        {
            var task = Pipeline.RunAsync(logger: Debug.Log);
            yield return new WaitUntil(() => task.IsCompleted);
            Assert.IsTrue(task.IsCompletedSuccessfully);
            var results = task.Result;
            Assert.IsTrue(results.Success);
            var layout = results.GetOutput<Layout>("Layout");
            Assert.IsNotNull(layout);
        }

        [UnityTest]
        public IEnumerator TestRunAttemptsAsync()
        {
            var randomSeed = Pipeline.GetComponentInChildren<RandomSeedInput>();
            Assert.IsTrue(randomSeed != null);
            Object.DestroyImmediate(randomSeed);
            var task = Pipeline.RunAttemptsAsync(12345);
            yield return new WaitUntil(() => task.IsCompleted);
            Assert.IsTrue(task.IsCompletedSuccessfully);
            var results = task.Result;
            Assert.IsTrue(results.Success);
            var layout = results.GetOutput<Layout>("Layout");
            Assert.IsNotNull(layout);
        }
    }
}
