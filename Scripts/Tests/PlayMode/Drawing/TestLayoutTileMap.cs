using MPewsey.Common.Random;
using MPewsey.ManiaMap;
using MPewsey.ManiaMapUnity.Generators;
using MPewsey.ManiaMapUnity.Tests;
using NUnit.Framework;
using System.Collections.Generic;

namespace MPewsey.ManiaMapUnity.Drawing.Tests
{
    public class TestLayoutTileMap
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
            var layoutTilemap = Assets.InstantiatePrefab<LayoutTileMapBook>(Assets.LayoutTilemapPath);
            layoutTilemap.DrawPages(layout);
            Assert.Greater(layoutTilemap.Grid.transform.childCount, 0);
        }
    }
}
