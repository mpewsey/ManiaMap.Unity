using MPewsey.ManiaMap.Graphs;
using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Graphs
{
    /// <summary>
    /// A node in a LayoutGraph.
    /// </summary>
    public class LayoutGraphNode : ScriptableObject
    {
        [SerializeField]
        [HideInInspector]
        private int _id;
        /// <summary>
        /// The unique ID.
        /// </summary>
        public int Id { get => _id; private set => _id = value; }

        [SerializeField]
        private string _name;
        /// <summary>
        /// The node name.
        /// </summary>
        public string Name { get => _name; set => _name = value; }

        [SerializeField]
        private TemplateGroup _templateGroup;
        /// <summary>
        /// The template group name.
        /// </summary>
        public TemplateGroup TemplateGroup { get => _templateGroup; set => _templateGroup = value; }

        [SerializeField]
        private int _z;
        /// <summary>
        /// The z (layer) coordinate.
        /// </summary>
        public int Z { get => _z; set => _z = value; }

        [SerializeField]
        private Color32 _color = new Color32(0, 255, 0, 255);
        /// <summary>
        /// The color.
        /// </summary>
        public Color32 Color { get => _color; set => _color = value; }

        [SerializeField]
        private string _variationGroup;
        /// <summary>
        /// The node variation group.
        /// </summary>
        public string VariationGroup { get => _variationGroup; set => _variationGroup = value; }

        [SerializeField]
        [HideInInspector]
        private Vector2 _position;
        /// <summary>
        /// The draw position of the node in the graph.
        /// </summary>
        public Vector2 Position { get => _position; set => _position = Vector2.Max(value, Vector2.zero); }

        [SerializeField]
        private List<string> _tags = new List<string>();
        /// <summary>
        /// A list of tags.
        /// </summary>
        public List<string> Tags { get => _tags; set => _tags = value; }

        /// <summary>
        /// Creates a new node.
        /// </summary>
        /// <param name="id">The unique ID.</param>
        public static LayoutGraphNode Create(int id)
        {
            var node = CreateInstance<LayoutGraphNode>();
            node.Id = id;
            node.Name = $"Node {id}";
            node.name = node.Name;
            return node;
        }

        public void AddMMLayoutNode(LayoutGraph graph)
        {
            var node = graph.AddNode(Id);
            node.Name = Name;
            node.Z = Z;
            node.TemplateGroup = TemplateGroup.Name;
            node.Color = ColorUtility.ConvertColor32ToColor4(Color);
            node.Tags = new List<string>(Tags);

            if (!string.IsNullOrWhiteSpace(VariationGroup))
                graph.AddNodeVariation(VariationGroup, Id);
        }
    }
}