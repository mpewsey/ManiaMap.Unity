using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// A graph consisting of LayoutNode and LayoutEdge.
    /// </summary>
    [CreateAssetMenu(menuName = "Mania Map/Layout Graph")]
    public class LayoutGraph : ScriptableObject
    {
        /// <summary>
        /// The unique ID.
        /// </summary>
        [field: SerializeField]
        public int Id { get; set; }

        /// <summary>
        /// The graph name.
        /// </summary>
        [field: SerializeField]
        public string Name { get; set; } = "<None>";

        /// <summary>
        /// A list of nodes in the graph.
        /// </summary>
        [field: SerializeField]
        private List<LayoutNode> Nodes { get; set; } = new List<LayoutNode>();

        /// <summary>
        /// A list of edges in the graph.
        /// </summary>
        [field: SerializeField]
        private List<LayoutEdge> Edges { get; set; } = new List<LayoutEdge>();

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
        public LayoutNode CreateNode()
        {
            return AddNode(GetNextNodeId());
        }

        /// <summary>
        /// Adds a node with the ID to the graph and returns it. If the node
        /// already exists, returns the existing node.
        /// </summary>
        /// <param name="id">The unique ID.</param>
        public LayoutNode AddNode(int id)
        {
            var node = GetNode(id);

            if (node == null)
            {
                node = new LayoutNode(id);
                Nodes.Add(node);
            }

            return node;
        }

        /// <summary>
        /// Removes the node from the graph, along with any attached edges.
        /// </summary>
        /// <param name="id">The unique ID.</param>
        public void RemoveNode(int id)
        {
            Nodes.RemoveAll(x => x.Id == id);
            Edges.RemoveAll(x => x.FromNode == id || x.ToNode == id);
        }

        /// <summary>
        /// Returns the node with the ID.
        /// </summary>
        /// <param name="id">The unique ID.</param>
        public LayoutNode GetNode(int id)
        {
            return Nodes.Find(x => x.Id == id);
        }

        /// <summary>
        /// Adds an edge to the graph between the specified nodes and returns it.
        /// If an edge between the nodes already exists, returns the existing edge.
        /// </summary>
        /// <param name="node1">The first node ID.</param>
        /// <param name="node2">The second node ID.</param>
        public LayoutEdge AddEdge(int node1, int node2)
        {
            AddNode(node1);
            AddNode(node2);
            var edge = GetEdge(node1, node2);

            if (edge == null)
            {
                edge = new LayoutEdge(node1, node2);
                Edges.Add(edge);
            }

            return edge;
        }

        /// <summary>
        /// Removes the edge from the graph.
        /// </summary>
        /// <param name="node1">The first node ID.</param>
        /// <param name="node2">The second node ID.</param>
        public void RemoveEdge(int node1, int node2)
        {
            Edges.RemoveAll(x => (x.FromNode == node1 && x.ToNode == node2) || (x.FromNode == node2 && x.ToNode == node1));
        }

        /// <summary>
        /// Returns the edge between the nodes.
        /// </summary>
        /// <param name="node1">The first node ID.</param>
        /// <param name="node2">The second node ID.</param>
        public LayoutEdge GetEdge(int node1, int node2)
        {
            return Edges.Find(x => (x.FromNode == node1 && x.ToNode == node2) || (x.FromNode == node2 && x.ToNode == node1));
        }

        /// <summary>
        /// Returns a readonly list of nodes in the graph.
        /// </summary>
        public IReadOnlyList<LayoutNode> GetNodes()
        {
            return Nodes;
        }

        /// <summary>
        /// Returns a readonly list of edges in the graph.
        /// </summary>
        public IReadOnlyList<LayoutEdge> GetEdges()
        {
            return Edges;
        }

        /// <summary>
        /// Creates a new Mania Map layout graph.
        /// </summary>
        public ManiaMap.LayoutGraph GetLayoutGraph()
        {
            var graph = new ManiaMap.LayoutGraph(Id, Name);

            foreach (var node in Nodes.OrderBy(x => x.Id))
            {
                var other = graph.AddNode(node.Id);
                other.Name = node.Name;
                other.Z = node.Z;
                other.TemplateGroup = node.TemplateGroup;
                other.Color = ConvertColor(node.Color);
            }

            foreach (var edge in Edges.OrderBy(x => new EdgeIndexes(x.FromNode, x.ToNode)))
            {
                var other = graph.AddEdge(edge.FromNode, edge.ToNode);
                other.Name = edge.Name;
                other.Direction = edge.Direction;
                other.DoorCode = edge.DoorCode;
                other.Z = edge.Z;
                other.RoomChance = edge.RoomChance;
                other.TemplateGroup = edge.TemplateGroup;
                other.Color = ConvertColor(edge.Color);
            }

            return graph;
        }

        /// <summary>
        /// Converts a Unity color to a System.Drawing color.
        /// </summary>
        /// <param name="color">The unity color.</param>
        private static System.Drawing.Color ConvertColor(Color32 color)
        {
            return System.Drawing.Color.FromArgb(color.a, color.r, color.g, color.b);
        }
    }
}