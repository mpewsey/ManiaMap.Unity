using MPewsey.ManiaMap.Graphs;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Graphs
{
    /// <summary>
    /// A graph consisting of LayoutNode and LayoutEdge.
    /// </summary>
    [CreateAssetMenu(menuName = "Mania Map/Layout Graph")]
    public class LayoutGraphResource : ScriptableObject
    {
        [SerializeField]
        private int _id = -1;
        /// <summary>
        /// The unique ID.
        /// </summary>
        public int Id { get => _id; set => _id = value; }

        [SerializeField]
        private string _name = "<None>";
        /// <summary>
        /// The graph name.
        /// </summary>
        public string Name { get => _name; set => _name = value; }

        [SerializeField]
        [HideInInspector]
        private List<LayoutGraphNode> _nodes = new List<LayoutGraphNode>();
        /// <summary>
        /// A list of nodes in the graph.
        /// </summary>
        private List<LayoutGraphNode> Nodes { get => _nodes; set => _nodes = value; }

        [SerializeField]
        [HideInInspector]
        private List<LayoutGraphEdge> _edges = new List<LayoutGraphEdge>();
        /// <summary>
        /// A list of edges in the graph.
        /// </summary>
        private List<LayoutGraphEdge> Edges { get => _edges; set => _edges = value; }

        /// <summary>
        /// Returns the number of nodes in the graph.
        /// </summary>
        public int NodeCount => Nodes.Count;

        /// <summary>
        /// Returns the number of edges in the graph.
        /// </summary>
        public int EdgeCount => Edges.Count;

        private void OnValidate()
        {
            Id = Rand.AutoAssignId(Id);
        }

        /// <summary>
        /// Returns the next available unique node ID.
        /// </summary>
        /// <returns></returns>
        private int GetNextNodeId()
        {
            var set = new HashSet<int>(Nodes.Select(x => x.Id));
            var id = 1;

            while (set.Contains(id))
            {
                id++;
            }

            return id;
        }

        /// <summary>
        /// Creates a new node and returns it.
        /// </summary>
        public LayoutGraphNode CreateNode()
        {
            return AddNode(GetNextNodeId());
        }

        /// <summary>
        /// Adds a node with the ID to the graph and returns it. If the node
        /// already exists, returns the existing node.
        /// </summary>
        /// <param name="id">The unique ID.</param>
        public LayoutGraphNode AddNode(int id)
        {
            var node = GetNode(id);

            if (node == null)
            {
                node = LayoutGraphNode.Create(id);
                Nodes.Add(node);
            }

            return node;
        }

        /// <summary>
        /// Removes the node from the graph, along with any attached edges.
        /// Returns true if a node is removed.
        /// </summary>
        /// <param name="id">The unique ID.</param>
        public bool RemoveNode(int id)
        {
            var nodeCount = Nodes.Count;
            var edgeCount = Edges.Count;
            Nodes.RemoveAll(x => x.Id == id);
            Edges.RemoveAll(x => x.FromNode == id || x.ToNode == id);
            return nodeCount != Nodes.Count || edgeCount != Edges.Count;
        }

        /// <summary>
        /// Returns the node with the ID.
        /// </summary>
        /// <param name="id">The unique ID.</param>
        public LayoutGraphNode GetNode(int id)
        {
            return Nodes.Find(x => x.Id == id);
        }

        /// <summary>
        /// Returns the node index for the ID.
        /// </summary>
        /// <param name="id">The node ID.</param>
        public int GetNodeIndex(int id)
        {
            return Nodes.FindIndex(x => x.Id == id);
        }

        /// <summary>
        /// Adds an edge to the graph between the specified nodes and returns it.
        /// If an edge between the nodes already exists, returns the existing edge.
        /// </summary>
        /// <param name="node1">The first node ID.</param>
        /// <param name="node2">The second node ID.</param>
        public LayoutGraphEdge AddEdge(int node1, int node2)
        {
            AddNode(node1);
            AddNode(node2);
            var edge = GetEdge(node1, node2);

            if (edge == null)
            {
                edge = LayoutGraphEdge.Create(node1, node2);
                Edges.Add(edge);
            }

            return edge;
        }

        /// <summary>
        /// Removes the edge from the graph. Returns true if an edge was removed.
        /// </summary>
        /// <param name="node1">The first node ID.</param>
        /// <param name="node2">The second node ID.</param>
        public bool RemoveEdge(int node1, int node2)
        {
            var count = Edges.Count;
            Edges.RemoveAll(x => (x.FromNode == node1 && x.ToNode == node2) || (x.FromNode == node2 && x.ToNode == node1));
            return Edges.Count != count;
        }

        /// <summary>
        /// Returns the edge between the nodes.
        /// </summary>
        /// <param name="node1">The first node ID.</param>
        /// <param name="node2">The second node ID.</param>
        public LayoutGraphEdge GetEdge(int node1, int node2)
        {
            return Edges.Find(x => (x.FromNode == node1 && x.ToNode == node2) || (x.FromNode == node2 && x.ToNode == node1));
        }

        /// <summary>
        /// Returns the index of the edge.
        /// </summary>
        /// <param name="node1">The first node ID.</param>
        /// <param name="node2">The second node ID.</param>
        public int GetEdgeIndex(int node1, int node2)
        {
            return Edges.FindIndex(x => (x.FromNode == node1 && x.ToNode == node2) || (x.FromNode == node2 && x.ToNode == node1));
        }

        /// <summary>
        /// Returns a readonly list of nodes in the graph.
        /// </summary>
        public IReadOnlyList<LayoutGraphNode> GetNodes()
        {
            return Nodes;
        }

        /// <summary>
        /// Returns a readonly list of edges in the graph.
        /// </summary>
        public IReadOnlyList<LayoutGraphEdge> GetEdges()
        {
            return Edges;
        }

        /// <summary>
        /// Creates a new Mania Map layout graph.
        /// </summary>
        public LayoutGraph GetMMLayoutGraph()
        {
            var graph = new LayoutGraph(Id, Name);
            AddMMLayoutNodes(graph);
            AddMMLayoutEdges(graph);
            return graph;
        }

        /// <summary>
        /// Adds the nodes to the generation graph.
        /// </summary>
        /// <param name="graph">The generation graph.</param>
        private void AddMMLayoutNodes(LayoutGraph graph)
        {
            foreach (var node in Nodes)
            {
                node.AddMMLayoutNode(graph);
            }
        }

        /// <summary>
        /// Adds the edges to the generation graph.
        /// </summary>
        /// <param name="graph">The generation graph.</param>
        private void AddMMLayoutEdges(LayoutGraph graph)
        {
            foreach (var edge in Edges)
            {
                edge.AddMMLayoutEdge(graph);
            }
        }

        /// <summary>
        /// Returns a rectangle containing all node positions.
        /// </summary>
        public Rect GetRect()
        {
            var min = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
            var max = new Vector2(float.NegativeInfinity, float.NegativeInfinity);

            foreach (var node in Nodes)
            {
                min = Vector2.Min(min, node.Position);
                max = Vector2.Max(max, node.Position);
            }

            return new Rect(min, max - min);
        }

        /// <summary>
        /// Adjusts the positions of the nodes to provide the specified spacing.
        /// Returns true if adjustments are made.
        /// </summary>
        /// <param name="spacing">The minimum x and y spacing between the node positions.</param>
        public bool Paginate(Vector2 spacing)
        {
            var result = false;
            var changed = true;

            for (int k = 0; changed && k < 1000; k++)
            {
                changed = false;

                for (int i = 0; i < Nodes.Count; i++)
                {
                    for (int j = i + 1; j < Nodes.Count; j++)
                    {
                        var iNode = Nodes[i];
                        var jNode = Nodes[j];
                        var delta = jNode.Position - iNode.Position;

                        if (Mathf.Abs(delta.x) > spacing.x || Mathf.Abs(delta.y) > spacing.y)
                            continue;

                        var sign = new Vector2(Mathf.Sign(delta.x), Mathf.Sign(delta.y));
                        delta = spacing * sign - delta;

                        if (Mathf.Abs(delta.x) * spacing.y > Mathf.Abs(delta.y) * spacing.x)
                            delta.x = 0;
                        else
                            delta.y = 0;

                        if ((k & 1) == 0)
                            jNode.Position += delta;
                        else
                            iNode.Position -= delta;

                        changed = true;
                    }
                }

                result |= changed;
            }

            return result;
        }
    }
}