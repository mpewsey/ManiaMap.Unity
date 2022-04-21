using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// A component for creating a room template.
    /// </summary>
    public class RoomTemplate : MonoBehaviour
    {
        /// <summary>
        /// The unique ID.
        /// </summary>
        [field: SerializeField]
        public int Id { get; set; }

        /// <summary>
        /// The template name.
        /// </summary>
        [field: SerializeField]
        public string Name { get; set; } = "<None>";

        /// <summary>
        /// The size of the room template grid.
        /// </summary>
        [field: SerializeField]
        public Vector2Int Size { get; set; }

        /// <summary>
        /// The size of each cell.
        /// </summary>
        [field: SerializeField]
        public Vector2 CellSize { get; set; } = Vector2.one;

        private void OnValidate()
        {
            Size = Vector2Int.Max(Size, Vector2Int.zero);
            CellSize = Vector2.Max(CellSize, new Vector2(0.01f, 0.01f));
        }
    }
}