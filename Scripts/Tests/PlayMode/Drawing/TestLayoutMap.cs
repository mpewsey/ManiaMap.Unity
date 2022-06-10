using MPewsey.ManiaMap.Unity.Tests;
using NUnit.Framework;
using System.IO;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Drawing.Tests
{
    public class TestLayoutMap
    {
        private GameObject Container { get; set; }

        [SetUp]
        public void SetUp()
        {
            Container = new GameObject("TestLayoutMap");
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(Container);
        }

        [Test]
        public void TestCreateBigLayoutImages()
        {
            var pipeline = TestAssets.LoadBigLayoutGenerator(Container.transform);
            var results = pipeline.Generate(12345);
            Assert.IsTrue(results.Success);
            var layout = (Layout)results.Outputs["Layout"];
            Assert.IsNotNull(layout);
            var layoutMap = TestAssets.LoadLayoutMap(Container.transform);
            var images = layoutMap.CreateImages(layout);
            Assert.AreEqual(1, images.Count);
        }

        [Test]
        public void TestSaveBigLayoutImages()
        {
            var pipeline = TestAssets.LoadBigLayoutGenerator(Container.transform);
            var results = pipeline.Generate(12345);
            Assert.IsTrue(results.Success);
            var layout = (Layout)results.Outputs["Layout"];
            Assert.IsNotNull(layout);
            var layoutMap = TestAssets.LoadLayoutMap(Container.transform);
            Directory.CreateDirectory("Tests");
            layoutMap.SaveImages("Tests/BigLayoutMap.png", layout);
        }

        [Test]
        public void TestSaveBigLayoutJpegImages()
        {
            var pipeline = TestAssets.LoadBigLayoutGenerator(Container.transform);
            var results = pipeline.Generate(12345);
            Assert.IsTrue(results.Success);
            var layout = (Layout)results.Outputs["Layout"];
            Assert.IsNotNull(layout);
            var layoutMap = TestAssets.LoadLayoutMap(Container.transform);
            Directory.CreateDirectory("Tests");
            layoutMap.SaveImages("Tests/BigLayoutMap.jpg", layout);
        }

        [Test]
        public void TestSaveCrossLayoutImages()
        {
            var pipeline = TestAssets.LoadCrossLayoutGenerator(Container.transform);
            var results = pipeline.Generate(12345);
            Assert.IsTrue(results.Success);
            var layout = (Layout)results.Outputs["Layout"];
            Assert.IsNotNull(layout);
            var layoutMap = TestAssets.LoadLayoutMap(Container.transform);
            Directory.CreateDirectory("Tests");
            layoutMap.SaveImages("Tests/CrossLayoutMap.png", layout);
        }

        [Test]
        public void TestSaveGeekLayoutImages()
        {
            var pipeline = TestAssets.LoadGeekLayoutGenerator(Container.transform);
            var results = pipeline.Generate(12345);
            Assert.IsTrue(results.Success);
            var layout = (Layout)results.Outputs["Layout"];
            Assert.IsNotNull(layout);
            var layoutMap = TestAssets.LoadLayoutMap(Container.transform);
            Directory.CreateDirectory("Tests");
            layoutMap.SaveImages("Tests/GeekLayoutMap.png", layout);
        }

        [Test]
        public void TestSaveLoopLayoutImages()
        {
            var pipeline = TestAssets.LoadLoopLayoutGenerator(Container.transform);
            var results = pipeline.Generate(12345);
            Assert.IsTrue(results.Success);
            var layout = (Layout)results.Outputs["Layout"];
            Assert.IsNotNull(layout);
            var layoutMap = TestAssets.LoadLayoutMap(Container.transform);
            Directory.CreateDirectory("Tests");
            layoutMap.SaveImages("Tests/LoopLayoutMap.png", layout);
        }

        [Test]
        public void TestSaveStackedLoopLayoutImages()
        {
            var pipeline = TestAssets.LoadStackedLoopLayoutGenerator(Container.transform);
            var results = pipeline.Generate(12345);
            Assert.IsTrue(results.Success);
            var layout = (Layout)results.Outputs["Layout"];
            Assert.IsNotNull(layout);
            var layoutMap = TestAssets.LoadLayoutMap(Container.transform);
            Directory.CreateDirectory("Tests");
            layoutMap.SaveImages("Tests/StackedLoopLayoutMap.png", layout);
        }
    }
}
