using MPewsey.ManiaMap.Unity.Tests;
using NUnit.Framework;
using System.IO;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Drawing.Tests
{
    public class TestLayoutMap
    {
        [SetUp]
        public void SetUp()
        {
            AssetLoader.LoadEmptyScene();
        }

        [Test]
        public void TestCreateBigLayoutImages()
        {
            var pipeline = AssetLoader.LoadBigLayoutGenerator();
            var results = pipeline.Generate(12345);
            Assert.IsTrue(results.Success);
            var layout = (Layout)results.Outputs["Layout"];
            Assert.IsNotNull(layout);
            var layoutMap = AssetLoader.LoadLayoutMap();
            var images = layoutMap.CreateImages(layout);
            Assert.AreEqual(1, images.Count);
        }

        [Test]
        public void TestSaveBigLayoutImages()
        {
            var pipeline = AssetLoader.LoadBigLayoutGenerator();
            var results = pipeline.Generate(12345);
            Assert.IsTrue(results.Success);
            var layout = (Layout)results.Outputs["Layout"];
            Assert.IsNotNull(layout);
            var layoutMap = AssetLoader.LoadLayoutMap();
            Directory.CreateDirectory("Tests");
            layoutMap.SaveImages("Tests/BigLayoutMap.png", layout);
        }

        [Test]
        public void TestSaveBigLayoutJpegImages()
        {
            var pipeline = AssetLoader.LoadBigLayoutGenerator();
            var results = pipeline.Generate(12345);
            Assert.IsTrue(results.Success);
            var layout = (Layout)results.Outputs["Layout"];
            Assert.IsNotNull(layout);
            var layoutMap = AssetLoader.LoadLayoutMap();
            Directory.CreateDirectory("Tests");
            layoutMap.SaveImages("Tests/BigLayoutMap.jpg", layout);
        }

        [Test]
        public void TestSaveCrossLayoutImages()
        {
            var pipeline = AssetLoader.LoadCrossLayoutGenerator();
            var results = pipeline.Generate(12345);
            Assert.IsTrue(results.Success);
            var layout = (Layout)results.Outputs["Layout"];
            Assert.IsNotNull(layout);
            var layoutMap = AssetLoader.LoadLayoutMap();
            Directory.CreateDirectory("Tests");
            layoutMap.SaveImages("Tests/CrossLayoutMap.png", layout);
        }

        [Test]
        public void TestSaveGeekLayoutImages()
        {
            var pipeline = AssetLoader.LoadGeekLayoutGenerator();
            var results = pipeline.Generate(12345);
            Assert.IsTrue(results.Success);
            var layout = (Layout)results.Outputs["Layout"];
            Assert.IsNotNull(layout);
            var layoutMap = AssetLoader.LoadLayoutMap();
            Directory.CreateDirectory("Tests");
            layoutMap.SaveImages("Tests/GeekLayoutMap.png", layout);
        }

        [Test]
        public void TestSaveLoopLayoutImages()
        {
            var pipeline = AssetLoader.LoadLoopLayoutGenerator();
            var results = pipeline.Generate(12345);
            Assert.IsTrue(results.Success);
            var layout = (Layout)results.Outputs["Layout"];
            Assert.IsNotNull(layout);
            var layoutMap = AssetLoader.LoadLayoutMap();
            Directory.CreateDirectory("Tests");
            layoutMap.SaveImages("Tests/LoopLayoutMap.png", layout);
        }

        [Test]
        public void TestSaveStackedLoopLayoutImages()
        {
            var pipeline = AssetLoader.LoadStackedLoopLayoutGenerator();
            var results = pipeline.Generate(12345);
            Assert.IsTrue(results.Success);
            var layout = (Layout)results.Outputs["Layout"];
            Assert.IsNotNull(layout);
            var layoutMap = AssetLoader.LoadLayoutMap();
            Directory.CreateDirectory("Tests");
            layoutMap.SaveImages("Tests/StackedLoopLayoutMap.png", layout);
        }
    }
}
