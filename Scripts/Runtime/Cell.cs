using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    public class Cell : MonoBehaviour
    {
        [SerializeField]
        private RoomTemplate _template;
        /// <summary>
        /// The parent room template.
        /// </summary>
        public RoomTemplate Template { get => _template; set => _template = value; }

        [SerializeField]
        private Vector2Int _position;
        /// <summary>
        /// The index position of the cell in the room template.
        /// </summary>
        public Vector2Int Position { get => _position; set => _position = Vector2Int.Max(value, Vector2Int.zero); }

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
        /// <param name="position">The index position of the cell in the room template.</param>
        public void Init(RoomTemplate template, Vector2Int position)
        {
            name = $"<Cell ({position.x}, {position.y})>";
            Template = template;
            Position = position;
            transform.localPosition = Center();
        }

        private void OnValidate()
        {
            Position = Position;
        }

        private void OnDrawGizmos()
        {
            var cellSize = Template.GetPlaneVector(Template.CellSize);

            // Draw fill color.
            Gizmos.color = IsEmpty ? new Color(0, 0, 0, 0.25f) : new Color(0.5f, 0.5f, 0.5f, 0.25f);
            Gizmos.DrawCube(transform.position, cellSize);
            
            // Draw outline.
            Gizmos.color = Color.grey;
            Gizmos.DrawWireCube(transform.position, cellSize);

            // If empty, draw an X through the cell.
            if (IsEmpty)
            {
                var right = Template.GetPlaneVector(Vector2.right);
                var up = Template.GetPlaneVector(Vector2.up);
                var from1 = Origin();
                var to1 = from1 + cellSize;
                var from2 = from1 + new Vector3(cellSize.x * Mathf.Abs(right.x), cellSize.y * Mathf.Abs(right.y), cellSize.z * Mathf.Abs(right.z));
                var to2 = from1 + new Vector3(cellSize.x * Mathf.Abs(up.x), cellSize.y * Mathf.Abs(up.y), cellSize.z * Mathf.Abs(up.z));
                Gizmos.color = Color.grey;
                Gizmos.DrawLine(from1, to1);
                Gizmos.DrawLine(from2, to2);
            }
        }

        /// <summary>
        /// Returns the local position for the origin of the cell.
        /// </summary>
        public Vector3 Origin()
        {
            var cellSize = Template.GetPlaneVector(Template.CellSize);
            var index = Template.GetPlaneVector(new Vector2(Position.y, Position.x));
            return new Vector3(cellSize.x * Mathf.Abs(index.x), cellSize.y * Mathf.Abs(index.y), cellSize.z * Mathf.Abs(index.z));
        }

        /// <summary>
        /// Returns the local position for the center of the cell.
        /// </summary>
        public Vector3 Center()
        {
            return Origin() + 0.5f * Template.GetPlaneVector(Template.CellSize);
        }
    }
}