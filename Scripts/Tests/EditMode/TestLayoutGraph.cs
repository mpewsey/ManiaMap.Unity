using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Tests
{
    public class TestLayoutGraph
    {
        [Test]
        public void TestCreateNode()
        {
            var graph = ScriptableObject.CreateInstance<LayoutGraph>();
            graph.CreateNode();
            CollectionAssert.AreEquivalent(new List<int> { 1 }, graph.GetNodes().Select(x => x.Id).ToList());
            graph.CreateNode();
            CollectionAssert.AreEquivalent(new List<int> { 1, 2 }, graph.GetNodes().Select(x => x.Id).ToList());
        }

        [Test]
        public void TestAddNode()
        {
            var graph = ScriptableObject.CreateInstance<LayoutGraph>();
            var node1 = graph.AddNode(10);
            var node2 = graph.AddNode(10);
            Assert.AreEqual(node1, node2);
            CollectionAssert.AreEquivalent(new List<int> { 10 }, graph.GetNodes().Select(x => x.Id).ToList());
        }

        [Test]
        public void TestRemoveNode()
        {
            var graph = ScriptableObject.CreateInstance<LayoutGraph>();
            graph.AddEdge(1, 2);
            CollectionAssert.AreEquivalent(new List<int> { 1, 2 }, graph.GetNodes().Select(x => x.Id).ToList());
            graph.RemoveNode(1);
            CollectionAssert.AreEquivalent(new List<int> { 2 }, graph.GetNodes().Select(x => x.Id).ToList());
        }

        [Test]
        public void TestAddEdge()
        {
            var graph = ScriptableObject.CreateInstance<LayoutGraph>();
            graph.AddEdge(1, 2);
            var edge1 = graph.AddEdge(2, 3);
            var edge2 = graph.AddEdge(3, 2);
            Assert.AreEqual(edge1, edge2);
            var expected = new List<(int, int)> { (1, 2), (2, 3) };
            var result = graph.GetEdges().Select(x => (x.FromNode, x.ToNode)).ToList();
            CollectionAssert.AreEquivalent(expected, result);
        }

        [Test]
        public void TestRemoveEdge()
        {
            var graph = ScriptableObject.CreateInstance<LayoutGraph>();
            graph.AddEdge(1, 2);
            Assert.AreEqual(1, graph.GetEdges().Count);
            graph.RemoveEdge(2, 1);
            Assert.AreEqual(0, graph.GetEdges().Count);
        }
    }
}