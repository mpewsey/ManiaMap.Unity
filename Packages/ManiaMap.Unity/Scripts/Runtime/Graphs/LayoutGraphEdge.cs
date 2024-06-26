using MPewsey.ManiaMap;
using MPewsey.ManiaMap.Graphs;
using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Graphs
{
    /// <summary>
    /// An edge in a LayoutGraph.
    /// </summary>
    public class LayoutGraphEdge : ScriptableObject
    {
        [SerializeField]
        [HideInInspector]
        private int _fromNode;
        /// <summary>
        /// The from node ID.
        /// </summary>
        public int FromNode { get => _fromNode; private set => _fromNode = value; }

        [SerializeField]
        [HideInInspector]
        private int _toNode;
        /// <summary>
        /// The to node ID.
        /// </summary>
        public int ToNode { get => _toNode; private set => _toNode = value; }

        [SerializeField]
        private string _name;
        /// <summary>
        /// The edge name.
        /// </summary>
        public string Name { get => _name; set => _name = value; }

        [SerializeField]
        private TemplateGroup _templateGroup;
        /// <summary>
        /// The template group name.
        /// </summary>
        public TemplateGroup TemplateGroup { get => _templateGroup; set => _templateGroup = value; }

        [SerializeField]
        private EdgeDirection _direction;
        /// <summary>
        /// The edge direction.
        /// </summary>
        public EdgeDirection Direction { get => _direction; set => _direction = value; }

        [SerializeField]
        private DoorCode _doorCode;
        /// <summary>
        /// The door code.
        /// </summary>
        public DoorCode DoorCode { get => _doorCode; set => _doorCode = value; }

        [SerializeField]
        private int _z;
        /// <summary>
        /// The z (layer) coordinate.
        /// </summary>
        public int Z { get => _z; set => _z = value; }

        [Range(0, 1)]
        [SerializeField]
        private float _roomChance;
        /// <summary>
        /// The chance of inserting a room.
        /// </summary>
        public float RoomChance { get => _roomChance; set => _roomChance = value; }

        [SerializeField]
        private bool _requireRoom;
        /// <summary>
        /// If true and the room chance is satisfied, a room is required for the edge
        /// to form a valid layout. Otherwise, the insertion of the room may be skipped
        /// if a valid room cannot be found.
        /// </summary>
        public bool RequireRoom { get => _requireRoom; set => _requireRoom = value; }

        [SerializeField]
        private Color32 _color = new Color32(0, 255, 0, 255);
        /// <summary>
        /// The color.
        /// </summary>
        public Color32 Color { get => _color; set => _color = value; }

        [SerializeField]
        private List<string> _tags = new List<string>();
        /// <summary>
        /// A list of tags.
        /// </summary>
        public List<string> Tags { get => _tags; set => _tags = value; }

        /// <summary>
        /// Creates a new edge.
        /// </summary>
        /// <param name="fromNode">The from node ID.</param>
        /// <param name="toNode">The to node ID.</param>
        public static LayoutGraphEdge Create(int fromNode, int toNode)
        {
            var edge = CreateInstance<LayoutGraphEdge>();
            edge.FromNode = fromNode;
            edge.ToNode = toNode;
            edge.Name = $"Edge ({fromNode}, {toNode})";
            edge.name = edge.Name;
            return edge;
        }

        public void AddMMLayoutEdge(LayoutGraph graph)
        {
            var edge = graph.AddEdge(FromNode, ToNode);
            edge.Name = Name;
            edge.Direction = Direction;
            edge.DoorCode = DoorCode;
            edge.Z = Z;
            edge.RoomChance = RoomChance;
            edge.RequireRoom = RequireRoom;
            edge.TemplateGroup = TemplateGroup != null ? TemplateGroup.Name : null;
            edge.Color = ColorUtility.ConvertColor32ToColor4(Color);
            edge.Tags = new List<string>(Tags);
        }
    }
}
