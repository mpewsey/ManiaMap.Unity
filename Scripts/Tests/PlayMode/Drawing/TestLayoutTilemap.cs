using System.Collections;
using System.Collections.Generic;
using MPewsey.ManiaMap.Unity.Tests;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

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
            var results = pipeline.Generate(12345);
            Assert.IsTrue(results.Success);
            var layout = (Layout)results.Outputs["Layout"];
            Assert.IsNotNull(layout);
            var layoutTilemap = Assets.InstantiatePrefab<LayoutTilemap>(Assets.LayoutTilemapPath);
            var layers = layoutTilemap.CreateLayers(layout);
            Assert.Greater(layers.Count, 0);
        }
    }
}
