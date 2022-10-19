using NUnit.Framework;
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
            var pipeline = Assets.InstantiatePrefab<GenerationPipeline>(Assets.BigLayoutPath);
            var results = pipeline.Generate();
            var layout = (Layout)results.Outputs["Layout"];
            Assert.IsNotNull(layout);
            var path = "Tests/Layout.xml";
            Serialization.SaveXml(path, layout);
            var copy = Serialization.LoadXml<Layout>(path);
            Assert.AreEqual(layout.Id, copy.Id);
        }

        [Test]
        public void TestSaveAndLoadLayoutState()
        {
            var pipeline = Assets.InstantiatePrefab<GenerationPipeline>(Assets.BigLayoutPath);
            var results = pipeline.Generate();
            var layout = (Layout)results.Outputs["Layout"];
            Assert.IsNotNull(layout);
            var state = new LayoutState(layout);
            var path = "Tests/LayoutState.xml";
            Serialization.SaveXml(path, state);
            var copy = Serialization.LoadXml<LayoutState>(path);
            Assert.AreEqual(state.Id, copy.Id);
        }

        [Test]
        public void TestSaveAndLoadLayoutGraph()
        {
            var graph = Samples.GraphLibrary.BigGraph();
            graph.AddNodeVariation("Group1", new int[] { 1, 2, 3 });
            graph.AddNodeVariation("Group2", new int[] { 4, 5, 6 });
            var path = "Tests/LayoutGraph.xml";
            Serialization.SaveXml(path, graph);
            var copy = Serialization.LoadXml<ManiaMap.LayoutGraph>(path);
            Assert.AreEqual(graph.Id, copy.Id);
        }

        [Test]
        public void TestSaveAndLoadCollectableGroups()
        {
            var group = new CollectableGroups();
            group.Add("Group1", new int[] { 1, 2, 3 });
            group.Add("Group2", new int[] { 4, 5, 6 });
            var path = "Tests/CollectableGroups.xml";
            Serialization.SaveXml(path, group);
            var copy = Serialization.LoadXml<CollectableGroups>(path);
            
            foreach (var pair in group.GetGroups())
            {
                CollectionAssert.AreEqual(pair.Value, copy.Get(pair.Key));
            }
        }
    }
}