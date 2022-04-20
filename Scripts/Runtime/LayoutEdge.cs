using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// An edge in a LayoutGraph.
    /// </summary>
    [System.Serializable]
    public class LayoutEdge
    {
        /// <summary>
        /// The midnight blue color.
        /// </summary>
        private static Color MidnightBlue { get; } = new Color(43, 23, 115);

        /// <summary>
        /// The edge name.
        /// </summary>
        [field: SerializeField]
        public string Name { get; set; } = "<None>";

        /// <summary>
        /// The from node ID.
        /// </summary>
        [field: SerializeField]
        public int FromNode { get; private set; }

        /// <summary>
        /// The to node ID.
        /// </summary>
        [field: SerializeField]
        public int ToNode { get; private set; }

        /// <summary>
        /// The edge direction.
        /// </summary>
        [field: SerializeField]
        public EdgeDirection Direction { get; set; }

        /// <summary>
        /// The door code.
        /// </summary>
        [field: SerializeField]
        public int DoorCode { get; set; }

        /// <summary>
        /// The z (layer) coordinate.
        /// </summary>
        [field: SerializeField]
        public int Z { get; set; }

        /// <summary>
        /// The chance of inserting a room.
        /// </summary>
        [field: Range(0, 1)]
        [field: SerializeField]
        public float RoomChance { get; set; }

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
        /// Initializes a new edge.
        /// </summary>
        /// <param name="fromNode">The from node ID.</param>
        /// <param name="toNode">The to node ID.</param>
        public LayoutEdge(int fromNode, int toNode)
        {
            FromNode = fromNode;
            ToNode = toNode;
        }
    }
}
