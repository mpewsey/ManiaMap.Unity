using MPewsey.Common.Serialization;
using MPewsey.ManiaMap;
using MPewsey.ManiaMap.Graphs;
using MPewsey.ManiaMap.Samples;
using NUnit.Framework;
using System.IO;

namespace MPewsey.ManiaMapUnity.Tests
{
    public class TestSerialization
    {
        [SetUp]
        public void SetUp()
        {
            Directory.CreateDirectory("Tests");
        }

        [Test]
        public void TestSaveAndLoadLayout()
        {
            var path = "Tests/Layout.json";
            var results = BigLayoutSample.Generate(12345);
            Assert.IsTrue(results.Success);
            var layout = results.GetOutput<Layout>("Layout");
            Assert.IsNotNull(layout);
            JsonSerialization.SaveJson(path, layout);
            var copy = JsonSerialization.LoadJson<Layout>(path);
            Assert.AreEqual(layout.Id, copy.Id);
        }

        [Test]
        public void TestSaveAndLoadLayoutState()
        {
            var path = "Tests/LayoutState.json";
            var results = BigLayoutSample.Generate(12345);
            Assert.IsTrue(results.Success);
            var layout = results.GetOutput<Layout>("Layout");
            Assert.IsNotNull(layout);
            var layoutState = new LayoutState(layout);
            JsonSerialization.SaveJson(path, layoutState);
            var copy = JsonSerialization.LoadJson<Layout>(path);
            Assert.AreEqual(layoutState.Id, copy.Id);
        }

        [Test]
        public void TestSaveAndLoadLayoutGraph()
        {
            var path = "Tests/LayoutGraph.json";
            var graph = GraphLibrary.BigGraph();
            graph.AddNodeVariations("Group1", new int[] { 1, 2, 3 });
            graph.AddNodeVariations("Group2", new int[] { 4, 5, 6 });
            JsonSerialization.SaveJson(path, graph);
            var copy = JsonSerialization.LoadJson<LayoutGraph>(path);
            Assert.AreEqual(graph.Id, copy.Id);
        }

        [Test]
        public void TestSaveAndLoadCollectableGroups()
        {
            var path = "Tests/CollectableGroups.json";
            var group = new CollectableGroups();
            group.Add("Group1", new int[] { 1, 2, 3 });
            group.Add("Group2", new int[] { 4, 5, 6 });
            JsonSerialization.SaveJson(path, group);
            var copy = JsonSerialization.LoadJson<CollectableGroups>(path);

            foreach (var pair in group.GroupsDictionary)
            {
                CollectionAssert.AreEqual(pair.Value, copy.GroupsDictionary[pair.Key]);
            }
        }
    }
}