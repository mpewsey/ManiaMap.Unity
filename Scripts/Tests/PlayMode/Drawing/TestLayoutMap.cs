using MPewsey.ManiaMap.Unity.Tests;
using NUnit.Framework;
using System.IO;

namespace MPewsey.ManiaMap.Unity.Drawing.Tests
{
    public class TestLayoutMap
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
            var results = pipeline.Generate(12345);
            Assert.IsTrue(results.Success);
            var layout = (Layout)results.Outputs["Layout"];
            Assert.IsNotNull(layout);
            var layoutMap = Assets.InstantiatePrefab<LayoutMap>(Assets.LayoutMapPath);
            var images = layoutMap.CreateLayers(layout);
            Assert.Greater(images.Count, 0);
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
            var pipeline = Assets.InstantiatePrefab<GenerationPipeline>(path);
            var results = pipeline.Generate(12345);
            Assert.IsTrue(results.Success);
            var layout = (Layout)results.Outputs["Layout"];
            Assert.IsNotNull(layout);
            var layoutMap = Assets.InstantiatePrefab<LayoutMap>(Assets.LayoutMapPath);
            Directory.CreateDirectory("Tests");
            layoutMap.SaveLayerImages(imagePath, layout);
        }
    }
}
