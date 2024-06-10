using MPewsey.ManiaMap;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace MPewsey.ManiaMapUnity.Generators.Tests
{
    public class TestGenerationPipeline
    {
        private const string TestScene = "ManiaMap/Tests/TestGenerationPipeline";
        private GameObject GameObject { get; set; }
        private GenerationPipeline Pipeline { get; set; }

        [SetUp]
        public void SetUp()
        {
            var prefab = Resources.Load<GameObject>(TestScene);
            GameObject = Object.Instantiate(prefab);
            Pipeline = GameObject.GetComponent<GenerationPipeline>();
            Assert.IsTrue(Pipeline != null);
            Pipeline.SetRandomSeed(12345);
        }

        [TearDown]
        public void TearDown()
        {
            Object.Destroy(GameObject);
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
