using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

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

        private GenerationPipeline LoadBigLayoutGenerator()
        {
            var path = "Packages/com.mpewsey.maniamap.unity/Prefabs/BigLayoutGenerator.prefab";
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            Assert.IsNotNull(prefab);
            var obj = Object.Instantiate(prefab, Container.transform);
            var generator = obj.GetComponent<GenerationPipeline>();
            Assert.IsNotNull(generator);
            return generator;
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
        public void TestSaveImages()
        {
            var random = new RandomSeed(12345);
            var graph = Samples.GraphLibrary.BigGraph();
            var templateGroups = Samples.BigLayoutSample.BigLayoutTemplateGroups();
            var collectableGroups = new CollectableGroups();
            collectableGroups.Add("Default", new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });

            var inputs = new Dictionary<string, object>
            {
                { "LayoutId", 1 },
                { "LayoutGraph", graph },
                { "TemplateGroups", templateGroups },
                { "CollectableGroups", collectableGroups },
                { "RandomSeed", random },
            };

            var pipeline = ManiaMap.GenerationPipeline.CreateDefaultPipeline();
            var results = pipeline.Generate(inputs);
            Assert.IsTrue(results.Success);
            var layout = (Layout)results.Outputs["Layout"];
            Assert.IsNotNull(layout);
            var layoutMap = LoadLayoutMap();
            layoutMap.Init(layout);
            Directory.CreateDirectory("Tests");
            layoutMap.SaveImages("Tests/BigLayout.png");
        }
    }
}
