using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// A node in a LayoutGraph.
    /// </summary>
    [System.Serializable]
    public class LayoutNode
    {
        [SerializeField]
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
        private Vector2 _position;
        /// <summary>
        /// The draw position of the node in the graph.
        /// </summary>
        public Vector2 Position { get => _position; set => _position = Vector2.Max(value, Vector2.zero); }

        /// <summary>
        /// The room ID.
        /// </summary>
        public Uid RoomId => new Uid(Id);

        /// <summary>
        /// Initializes a new node.
        /// </summary>
        /// <param name="id">The unique ID.</param>
        public LayoutNode(int id)
        {
            Id = id;
            Name = $"<Node {id}>";
        }
    }
}