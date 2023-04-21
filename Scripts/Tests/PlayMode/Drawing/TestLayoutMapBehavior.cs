using MPewsey.Common.Random;
using MPewsey.ManiaMap.Unity.Generators;
using MPewsey.ManiaMap.Unity.Tests;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace MPewsey.ManiaMap.Unity.Drawing.Tests
{
    public class TestLayoutMapBehavior
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
        public void TestCreateLayers(string path)
        {
            var inputs = new Dictionary<string, object>()
            {
                { "LayoutId", 1 },
                { "RandomSeed", new RandomSeed(12345) },
            };

            var pipeline = Assets.InstantiatePrefab<GenerationPipeline>(path);
            var results = pipeline.Run(inputs);
            Assert.IsTrue(results.Success);
            var layout = results.GetOutput<Layout>("Layout");
            Assert.IsNotNull(layout);
            var layoutMap = Assets.InstantiatePrefab<LayoutMapBehavior>(Assets.LayoutMapPath);
            layoutMap.Initialize(layout);
            layoutMap.Draw();
            Assert.Greater(layoutMap.LayersContainer.childCount, 0);
        }

        [TestCase(Assets.BigLayoutPath, "Tests/BigLayout.png")]
        [TestCase(Assets.CrossLayoutPath, "Tests/CrossLayout.png")]
        [TestCase(Assets.GeekLayoutPath, "Tests/GeekLayout.png")]
        [TestCase(Assets.LoopLayoutPath, "Tests/LoopLayout.png")]
        [TestCase(Assets.StackedLoopLayoutPath, "Tests/StackedLoopLayout.png")]
        [TestCase(Assets.BigLayoutPath, "Tests/BigLayout.jpg")]
        [TestCase(Assets.CrossLayoutPath, "Tests/CrossLayout.jpg")]
        [TestCase(Assets.GeekLayoutPath, "Tests/GeekLayout.jpg")]
        [TestCase(Assets.LoopLayoutPath, "Tests/LoopLayout.jpg")]
        [TestCase(Assets.StackedLoopLayoutPath, "Tests/StackedLoopLayout.jpg")]
        public void TestSaveLayoutImages(string path, string imagePath)
        {
            var inputs = new Dictionary<string, object>()
            {
                { "LayoutId", 1 },
                { "RandomSeed", new RandomSeed(12345) },
            };

            var pipeline = Assets.InstantiatePrefab<GenerationPipeline>(path);
            var results = pipeline.Run(inputs);
            Assert.IsTrue(results.Success);
            var layout = results.GetOutput<Layout>("Layout");
            Assert.IsNotNull(layout);
            var layoutMap = Assets.InstantiatePrefab<LayoutMapBehavior>(Assets.LayoutMapPath);
            Directory.CreateDirectory("Tests");
            layoutMap.Initialize(layout);
            layoutMap.SaveImages(imagePath);
        }
    }
}
