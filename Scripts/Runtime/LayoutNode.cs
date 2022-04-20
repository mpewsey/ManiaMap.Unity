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
        private static Color MidnightBlue { get; } = new Color(43, 23, 115);

        /// <summary>
        /// The unique ID.
        /// </summary>
        [field: SerializeField]
        public int Id { get; private set; }

        /// <summary>
        /// The node name.
        /// </summary>
        [field: SerializeField]
        public string Name { get; set; } = "<None>";

        /// <summary>
        /// The z (layer) coordinate.
        /// </summary>
        [field: SerializeField]
        public int Z { get; set; }

        /// <summary>
        /// The template group name.
        /// </summary>
        [field: SerializeField]
        public string TemplateGroup { get; set; } = "<None>";

        /// <summary>
        /// The color.
        /// </summary>
        [field: SerializeField]
        public Color Color { get; set; } = MidnightBlue;

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