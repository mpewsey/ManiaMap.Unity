using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// A node in a LayoutGraph.
    /// </summary>
    [System.Serializable]
    public class LayoutNode
    {
        /// <summary>
        /// The midnight blue color.
        /// </summary>
        private static Color32 MidnightBlue { get; } = new Color32(43, 23, 115, 255);

        [SerializeField]
        private int _id;
        /// <summary>
        /// The unique ID.
        /// </summary>
        public int Id { get => _id; private set => _id = value; }

        [SerializeField]
        private string _name = "<None>";
        /// <summary>
        /// The node name.
        /// </summary>
        public string Name { get => _name; set => _name = value; }

        [SerializeField]
        private int _z;
        /// <summary>
        /// The z (layer) coordinate.
        /// </summary>
        public int Z { get => _z; set => _z = value; }

        [SerializeField]
        private string _templateGroup = "<None>";
        /// <summary>
        /// The template group name.
        /// </summary>
        public string TemplateGroup { get => _templateGroup; set => _templateGroup = value; }

        [SerializeField]
        private Color32 _color = MidnightBlue;
        /// <summary>
        /// The color.
        /// </summary>
        public Color32 Color { get => _color; set => _color = value; }

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
        }
    }
}