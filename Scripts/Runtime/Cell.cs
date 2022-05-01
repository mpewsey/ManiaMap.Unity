using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    public class Cell : MonoBehaviour
    {
        [SerializeField]
        private RoomTemplate _template;
        public RoomTemplate Template { get => _template; set => _template = value; }

        [SerializeField]
        private Vector2Int _position;
        public Vector2Int Position { get => _position; set => _position = value; }

        [SerializeField]
        private Vector2 _cellSize = Vector2.one;
        public Vector2 CellSize { get => _cellSize; set => _cellSize = Vector2.Max(value, Vector2.one); }

        [SerializeField]
        private bool _isEmpty;
        public bool IsEmpty { get => _isEmpty; set => _isEmpty = value; }

        public void Init(RoomTemplate template, Vector2Int position)
        {
            name = $"<Cell ({position.x}, {position.y})>";
            Template = template;
            Position = position;
            CellSize = template.CellSize;
            transform.localPosition = new Vector2(CellSize.x * position.y, -CellSize.y * position.x);
        }
    }
}