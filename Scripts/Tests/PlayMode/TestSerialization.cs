using MPewsey.Common.Random;
using MPewsey.Common.Serialization;
using MPewsey.ManiaMap.Graphs;
using MPewsey.ManiaMap.Unity.Generators;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace MPewsey.ManiaMap.Unity.Tests
{
    public class TestSerialization
    {
        [SetUp]
        public void SetUp()
        {
            Assets.DestroyAllGameObjects();
            Directory.CreateDirectory("Tests");
        }

        [Test]
        public void TestSaveAndLoadLayout()
        {
            var inputs = new Dictionary<string, object>()
            {
                { "LayoutId", 1 },
                { "RandomSeed", new RandomSeed(12345) },
            };

            var pipeline = Assets.InstantiatePrefab<GenerationPipeline>(Assets.BigLayoutPath);
            var results = pipeline.Run(inputs);
            var layout = (Layout)results.Outputs["Layout"];
            Assert.IsNotNull(layout);
            var path = "Tests/Layout.json";
            JsonSerialization.SaveJson(path, layout);
            var copy = JsonSerialization.LoadJson<Layout>(path);
            Assert.AreEqual(layout.Id, copy.Id);
        }

        [Test]
        public void TestSaveAndLoadLayoutState()
        {
            var inputs = new Dictionary<string, object>()
            {
                { "LayoutId", 1 },
                { "RandomSeed", new RandomSeed(12345) },
            };

            var pipeline = Assets.InstantiatePrefab<GenerationPipeline>(Assets.BigLayoutPath);
            var results = pipeline.Run(inputs);
            var layout = (Layout)results.Outputs["Layout"];
            Assert.IsNotNull(layout);
            var state = new LayoutState(layout);
            var path = "Tests/LayoutState.json";
            JsonSerialization.SaveJson(path, state);
            var copy = JsonSerialization.LoadJson<LayoutState>(path);
            Assert.AreEqual(state.Id, copy.Id);
        }

        [Test]
        public void TestSaveAndLoadLayoutGraph()
        {
            var graph = Samples.GraphLibrary.BigGraph();
            graph.AddNodeVariation("Group1", new int[] { 1, 2, 3 });
            graph.AddNodeVariation("Group2", new int[] { 4, 5, 6 });
            var path = "Tests/LayoutGraph.json";
            JsonSerialization.SaveJson(path, graph);
            var copy = JsonSerialization.LoadJson<LayoutGraph>(path);
            Assert.AreEqual(graph.Id, copy.Id);
        }

        [Test]
        public void TestSaveAndLoadCollectableGroups()
        {
            var group = new CollectableGroups();
            group.Add("Group1", new int[] { 1, 2, 3 });
            group.Add("Group2", new int[] { 4, 5, 6 });
            var path = "Tests/CollectableGroups.json";
            JsonSerialization.SaveJson(path, group);
            var copy = JsonSerialization.LoadJson<CollectableGroups>(path);

            foreach (var pair in group.GroupsDictionary)
            {
                CollectionAssert.AreEqual(pair.Value, copy.GroupsDictionary[pair.Key]);
            }
        }
    }
}