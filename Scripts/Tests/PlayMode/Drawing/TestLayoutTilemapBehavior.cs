using MPewsey.ManiaMap.Unity.Generators;
using MPewsey.ManiaMap.Unity.Tests;
using NUnit.Framework;

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
            var pipeline = Assets.InstantiatePrefab<GenerationPipeline>(path);
            pipeline.SetSeed(12345);
            var results = pipeline.Generate();
            Assert.IsTrue(results.Success);
            var layout = (Layout)results.Outputs["Layout"];
            Assert.IsNotNull(layout);
            var layoutTilemap = Assets.InstantiatePrefab<LayoutTilemapBehavior>(Assets.LayoutTilemapPath);
            layoutTilemap.CreateLayers(layout, null);
            Assert.Greater(layoutTilemap.Grid.transform.childCount, 0);
        }
    }
}