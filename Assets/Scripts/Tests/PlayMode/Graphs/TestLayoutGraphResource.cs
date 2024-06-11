using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Graphs.Tests
{
    public class TestLayoutGraphResource
    {
        [Test]
        public void TestCreateNode()
        {
            var graph = ScriptableObject.CreateInstance<LayoutGraphResource>();
            graph.CreateNode();
            CollectionAssert.AreEquivalent(new List<int> { 1 }, graph.GetNodes().Select(x => x.Id).ToList());
            graph.CreateNode();
            CollectionAssert.AreEquivalent(new List<int> { 1, 2 }, graph.GetNodes().Select(x => x.Id).ToList());
        }

        [Test]
        public void TestAddNode()
        {
            var graph = ScriptableObject.CreateInstance<LayoutGraphResource>();
            var node1 = graph.AddNode(10);
            var node2 = graph.AddNode(10);
            Assert.AreEqual(node1, node2);
            CollectionAssert.AreEquivalent(new List<int> { 10 }, graph.GetNodes().Select(x => x.Id).ToList());
        }

        [Test]
        public void TestRemoveNode()
        {
            var graph = ScriptableObject.CreateInstance<LayoutGraphResource>();
            graph.AddEdge(1, 2);
            CollectionAssert.AreEquivalent(new List<int> { 1, 2 }, graph.GetNodes().Select(x => x.Id).ToList());
            graph.RemoveNode(1);
            CollectionAssert.AreEquivalent(new List<int> { 2 }, graph.GetNodes().Select(x => x.Id).ToList());
        }

        [Test]
        public void TestAddEdge()
        {
            var graph = ScriptableObject.CreateInstance<LayoutGraphResource>();
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
            var graph = ScriptableObject.CreateInstance<LayoutGraphResource>();
            graph.AddEdge(1, 2);
            Assert.AreEqual(1, graph.GetEdges().Count);
            graph.RemoveEdge(2, 1);
            Assert.AreEqual(0, graph.GetEdges().Count);
        }

        [Test]
        public void TestGetRect()
        {
            var graph = ScriptableObject.CreateInstance<LayoutGraphResource>();
            var node1 = graph.AddNode(1);
            var node2 = graph.AddNode(2);
            node1.Position = new Vector2(10, 20);
            node2.Position = new Vector2(20, 50);
            var rect = graph.GetRect();
            var expected = new Rect(10, 20, 10, 30);
            Assert.AreEqual(expected, rect);
        }

        [Test]
        public void TestPaginate()
        {
            var graph = ScriptableObject.CreateInstance<LayoutGraphResource>();
            graph.AddNode(1);
            graph.AddNode(2);
            graph.AddNode(3);
            graph.AddNode(4);
            graph.Paginate(new Vector2(20, 10));
        }

        [Test]
        public void TestGetEdgeIndex()
        {
            var graph = ScriptableObject.CreateInstance<LayoutGraphResource>();
            graph.AddEdge(1, 2);
            graph.AddEdge(2, 3);
            graph.AddEdge(7, 8);
            Assert.AreEqual(1, graph.GetEdgeIndex(2, 3));
            Assert.AreEqual(-1, graph.GetEdgeIndex(4, 5));
        }

        [Test]
        public void TestGetNodeIndex()
        {
            var graph = ScriptableObject.CreateInstance<LayoutGraphResource>();
            graph.AddNode(1);
            graph.AddNode(2);
            graph.AddNode(3);
            Assert.AreEqual(2, graph.GetNodeIndex(3));
            Assert.AreEqual(-1, graph.GetNodeIndex(10));
        }

        [Test]
        public void TestNodeCount()
        {
            var graph = ScriptableObject.CreateInstance<LayoutGraphResource>();
            graph.AddNode(1);
            graph.AddNode(2);
            graph.AddNode(3);
            Assert.AreEqual(3, graph.NodeCount);
        }

        [Test]
        public void TestEdgeCount()
        {
            var graph = ScriptableObject.CreateInstance<LayoutGraphResource>();
            graph.AddEdge(1, 2);
            graph.AddEdge(2, 3);
            graph.AddEdge(3, 4);
            Assert.AreEqual(3, graph.EdgeCount);
        }
    }
}