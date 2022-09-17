using MPewsey.ManiaMap.Unity.Tests;
using NUnit.Framework;

namespace MPewsey.ManiaMap.Unity.Drawing.Tests
{
    public class TestLayoutTilemap
    {
        [SetUp]
        public void SetUp()
        {
            Assets.LoadEmptyScene();
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
            var layoutTilemap = Assets.InstantiatePrefab<LayoutTilemap>(Assets.LayoutTilemapPath);
            var layers = layoutTilemap.CreateLayers(layout);
            Assert.Greater(layers.Count, 0);
        }
    }
}
