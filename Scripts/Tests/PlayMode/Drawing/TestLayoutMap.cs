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
        public void TestCreateBigLayoutCreateLayers()
        {
            var pipeline = AssetLoader.InstantiateBigLayoutGenerator();
            var results = pipeline.Generate(12345);
            Assert.IsTrue(results.Success);
            var layout = (Layout)results.Outputs["Layout"];
            Assert.IsNotNull(layout);
            var layoutMap = AssetLoader.InstantiateLayoutMap();
            var images = layoutMap.CreateLayers(layout);
            Assert.AreEqual(1, images.Count);
        }

        [Test]
        public void TestSaveBigLayoutImages()
        {
            var pipeline = AssetLoader.InstantiateBigLayoutGenerator();
            var results = pipeline.Generate(12345);
            Assert.IsTrue(results.Success);
            var layout = (Layout)results.Outputs["Layout"];
            Assert.IsNotNull(layout);
            var layoutMap = AssetLoader.InstantiateLayoutMap();
            Directory.CreateDirectory("Tests");
            layoutMap.SaveLayerImages("Tests/BigLayoutMap.png", layout);
        }

        [Test]
        public void TestSaveBigLayoutJpegImages()
        {
            var pipeline = AssetLoader.InstantiateBigLayoutGenerator();
            var results = pipeline.Generate(12345);
            Assert.IsTrue(results.Success);
            var layout = (Layout)results.Outputs["Layout"];
            Assert.IsNotNull(layout);
            var layoutMap = AssetLoader.InstantiateLayoutMap();
            Directory.CreateDirectory("Tests");
            layoutMap.SaveLayerImages("Tests/BigLayoutMap.jpg", layout);
        }

        [Test]
        public void TestSaveCrossLayoutImages()
        {
            var pipeline = AssetLoader.InstantiateCrossLayoutGenerator();
            var results = pipeline.Generate(12345);
            Assert.IsTrue(results.Success);
            var layout = (Layout)results.Outputs["Layout"];
            Assert.IsNotNull(layout);
            var layoutMap = AssetLoader.InstantiateLayoutMap();
            Directory.CreateDirectory("Tests");
            layoutMap.SaveLayerImages("Tests/CrossLayoutMap.png", layout);
        }

        [Test]
        public void TestSaveGeekLayoutImages()
        {
            var pipeline = AssetLoader.InstantiateGeekLayoutGenerator();
            var results = pipeline.Generate(12345);
            Assert.IsTrue(results.Success);
            var layout = (Layout)results.Outputs["Layout"];
            Assert.IsNotNull(layout);
            var layoutMap = AssetLoader.InstantiateLayoutMap();
            Directory.CreateDirectory("Tests");
            layoutMap.SaveLayerImages("Tests/GeekLayoutMap.png", layout);
        }

        [Test]
        public void TestSaveLoopLayoutImages()
        {
            var pipeline = AssetLoader.InstantiateLoopLayoutGenerator();
            var results = pipeline.Generate(12345);
            Assert.IsTrue(results.Success);
            var layout = (Layout)results.Outputs["Layout"];
            Assert.IsNotNull(layout);
            var layoutMap = AssetLoader.InstantiateLayoutMap();
            Directory.CreateDirectory("Tests");
            layoutMap.SaveLayerImages("Tests/LoopLayoutMap.png", layout);
        }

        [Test]
        public void TestSaveStackedLoopLayoutImages()
        {
            var pipeline = AssetLoader.InstantiateStackedLoopLayoutGenerator();
            var results = pipeline.Generate(12345);
            Assert.IsTrue(results.Success);
            var layout = (Layout)results.Outputs["Layout"];
            Assert.IsNotNull(layout);
            var layoutMap = AssetLoader.InstantiateLayoutMap();
            Directory.CreateDirectory("Tests");
            layoutMap.SaveLayerImages("Tests/StackedLoopLayoutMap.png", layout);
        }
    }
}
