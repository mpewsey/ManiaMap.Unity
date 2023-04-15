using MPewsey.Common.Random;
using MPewsey.ManiaMap.Unity.Generators;
using MPewsey.ManiaMap.Unity.Tests;
using NUnit.Framework;
using System.Collections.Generic;

namespace MPewsey.ManiaMap.Unity.Drawing.Tests
{
    public class TestLayoutTilemapBehavior
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
            var results = pipeline.Generate(inputs);
            Assert.IsTrue(results.Success);
            var layout = (Layout)results.Outputs["Layout"];
            Assert.IsNotNull(layout);
            var layoutTilemap = Assets.InstantiatePrefab<LayoutTilemapBehavior>(Assets.LayoutTilemapPath);
            layoutTilemap.Initialize(layout);
            layoutTilemap.Draw();
            Assert.Greater(layoutTilemap.Grid.transform.childCount, 0);
        }
    }
}
