using MPewsey.ManiaMap.Unity.Tests;
using NUnit.Framework;
using System.IO;
using UnityEditor;
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

        private LayoutMap LoadLayoutMap()
        {
            var path = "Packages/com.mpewsey.maniamap.unity/Prefabs/LayoutMap.prefab";
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            Assert.IsNotNull(prefab);
            var obj = Object.Instantiate(prefab, Container.transform);
            var layoutMap = obj.GetComponent<LayoutMap>();
            Assert.IsNotNull(layoutMap);
            return layoutMap;
        }

        [Test]
        public void TestCreateBigLayoutImages()
        {
            var pipeline = TestAssets.LoadBigLayoutGenerator(Container.transform);
            var results = pipeline.Generate(12345);
            Assert.IsTrue(results.Success);
            var layout = (Layout)results.Outputs["Layout"];
            Assert.IsNotNull(layout);
            var layoutMap = LoadLayoutMap();
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
            var layoutMap = LoadLayoutMap();
            Directory.CreateDirectory("Tests");
            layoutMap.SaveImages("Tests/BigLayoutMap.png", layout);
        }

        [Test]
        public void TestSaveCrossLayoutImages()
        {
            var pipeline = TestAssets.LoadCrossLayoutGenerator(Container.transform);
            var results = pipeline.Generate(12345);
            Assert.IsTrue(results.Success);
            var layout = (Layout)results.Outputs["Layout"];
            Assert.IsNotNull(layout);
            var layoutMap = LoadLayoutMap();
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
            var layoutMap = LoadLayoutMap();
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
            var layoutMap = LoadLayoutMap();
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
            var layoutMap = LoadLayoutMap();
            Directory.CreateDirectory("Tests");
            layoutMap.SaveImages("Tests/StackedLoopLayoutMap.png", layout);
        }
    }
}
