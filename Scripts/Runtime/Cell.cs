using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// A component representing a RoomTemplate cell.
    /// </summary>
    public class Cell : MonoBehaviour
    {
        [SerializeField]
        [HideInInspector]
        private RoomTemplate _template;
        /// <summary>
        /// The parent room template.
        /// </summary>
        public RoomTemplate Template { get => _template; set => _template = value; }

        [SerializeField]
        [HideInInspector]
        private Vector2Int _index;
        /// <summary>
        /// The index position of the cell in the room template.
        /// </summary>
        public Vector2Int Index { get => _index; set => _index = Vector2Int.Max(value, Vector2Int.zero); }

        [SerializeField]
        private bool _isEmpty;
        /// <summary>
        /// True if the cell is empty.
        /// </summary>
        public bool IsEmpty { get => _isEmpty; set => _isEmpty = value; }

        /// <summary>
        /// Initializes the cell properties.
        /// </summary>
        /// <param name="template">The parent room template.</param>
        /// <param name="index">The index position of the cell in the room template.</param>
        public void Init(RoomTemplate template, Vector2Int index)
        {
            name = $"<Cell ({index.x}, {index.y})>";
            Template = template;
            Index = index;
            transform.localPosition = template.Swizzle(Center());
        }

        private void OnValidate()
        {
            Index = Index;
        }

        private void OnDrawGizmos()
        {
            // Draw fill color.
            Gizmos.color = IsEmpty ? new Color(0, 0, 0, 0.25f) : new Color(0.5f, 0.5f, 0.5f, 0.25f);
            Gizmos.DrawCube(transform.position, Template.Swizzle(Template.CellSize));

            // Draw outline.
            Gizmos.color = Color.grey;
            Gizmos.DrawWireCube(transform.position, Template.Swizzle(Template.CellSize));

            // If empty, draw an X through the cell.
            if (IsEmpty)
            {
                var origin = Origin();
                var from1 = Template.Swizzle(origin);
                var to1 = Template.Swizzle(origin + Template.CellSize);
                var from2 = Template.Swizzle(origin + new Vector2(Template.CellSize.x, 0));
                var to2 = Template.Swizzle(origin + new Vector2(0, Template.CellSize.y));
                Gizmos.color = Color.grey;
                Gizmos.DrawLine(from1, to1);
                Gizmos.DrawLine(from2, to2);
            }
        }

        /// <summary>
        /// Returns the local position for the bottom-left corner of the cell.
        /// </summary>
        public Vector2 Origin()
        {
            return Template.CellSize * new Vector2(Index.y, Template.Size.x - Index.x - 1);
        }

        /// <summary>
        /// Returns the local position for the center of the cell.
        /// </summary>
        public Vector2 Center()
        {
            return Origin() + 0.5f * Template.CellSize;
        }

        /// <summary>
        /// Returns the bounds of the cell.
        /// </summary>
        public Bounds GetBounds()
        {
            var size = Template.Swizzle(Template.CellSize);

            if (size.x == 0)
                size.x = float.PositiveInfinity;
            if (size.y == 0)
                size.y = float.PositiveInfinity;
            if (size.z == 0)
                size.z = float.PositiveInfinity;

            return new Bounds(transform.position, size);
        }
    }
}