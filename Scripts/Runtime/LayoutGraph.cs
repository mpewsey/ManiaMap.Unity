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
        [SerializeField]
        private int _id;
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
        private List<LayoutNode> _nodes = new List<LayoutNode>();
        /// <summary>
        /// A list of nodes in the graph.
        /// </summary>
        private List<LayoutNode> Nodes { get => _nodes; set => _nodes = value; }

        [SerializeField]
        [HideInInspector]
        private List<LayoutEdge> _edges = new List<LayoutEdge>();
        /// <summary>
        /// A list of edges in the graph.
        /// </summary>
        private List<LayoutEdge> Edges { get => _edges; set => _edges = value; }

        /// <summary>
        /// Returns the number of nodes in the graph.
        /// </summary>
        public int NodeCount => Nodes.Count;

        /// <summary>
        /// Returns the number of edges in the graph.
        /// </summary>
        public int EdgeCount => Edges.Count;

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
                node = LayoutNode.Create(id);
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
        public LayoutEdge AddEdge(int node1, int node2)
        {
            AddNode(node1);
            AddNode(node2);
            var edge = GetEdge(node1, node2);

            if (edge == null)
            {
                edge = LayoutEdge.Create(node1, node2);
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
                other.TemplateGroup = node.TemplateGroup.Name;
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
                other.TemplateGroup = edge.TemplateGroup.Name;
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
        /// Returns true if ajustments are made.
        /// </summary>
        /// <param name="spacing">The minimum x and y spacing between the node positions.</param>
        public bool Paginate(Vector2 spacing)
        {
            var result = false;

            for (int k = 0; k < 1000; k++)
            {
                var changed = false;

                for (int i = 0; i < Nodes.Count; i++)
                {
                    for (int j = i + 1; j < Nodes.Count; j++)
                    {
                        var iNode = Nodes[i];
                        var jNode = Nodes[j];
                        var delta = jNode.Position - iNode.Position;

                        if (Mathf.Abs(delta.x) > spacing.x)
                            continue;
                        if (Mathf.Abs(delta.y) > spacing.y)
                            continue;

                        var move = 0.5f * (spacing - delta);
                        iNode.Position -= move;
                        jNode.Position += move;
                        changed = true;
                        result = true;
                    }
                }

                if (!changed)
                    break;
            }

            return result;
        }
    }
}