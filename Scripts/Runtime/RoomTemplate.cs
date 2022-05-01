using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// A component for creating a room template.
    /// </summary>
    public class RoomTemplate : MonoBehaviour
    {
        [SerializeField]
        private int _id;
        /// <summary>
        /// The unique ID.
        /// </summary>
        public int Id { get => _id; set => _id = value; }

        [SerializeField]
        private string _name = "<None>";
        /// <summary>
        /// The template name.
        /// </summary>
        public string Name { get => _name; set => _name = value; }

        [Header("Cells")]

        [SerializeField]
        private Transform _cellContainer;
        public Transform CellContainer { get => _cellContainer; set => _cellContainer = value; }

        [SerializeField]
        private Vector2Int _size = Vector2Int.one;
        /// <summary>
        /// The size of the room template grid.
        /// </summary>
        public Vector2Int Size { get => _size; set => _size = Vector2Int.Max(value, Vector2Int.one); }

        [SerializeField]
        private Vector2 _cellSize = Vector2.one;
        /// <summary>
        /// The size of each cell.
        /// </summary>
        public Vector2 CellSize { get => _cellSize; set => _cellSize = Vector2.Max(value, Vector2.one); }

        [SerializeField]
        private List<Cell> _cells = new List<Cell>();
        public List<Cell> Cells { get => _cells; set => _cells = value; }

        private void OnValidate()
        {
            Size = Size;
            CellSize = CellSize;
        }

        public Cell GetCell(int row, int column)
        {
            if (!CellIndexExists(row, column))
                throw new System.IndexOutOfRangeException($"Index out of range: ({row}, {column}).");
            return Cells[FlatCellIndex(row, column)];
        }

        public bool CellIndexExists(int row, int column)
        {
            return (uint)row < Size.x && (uint)column < Size.y;
        }

        private int FlatCellIndex(int row, int column)
        {
            return row * Size.y + column;
        }

        public void CreateCellContainer()
        {
            if (CellContainer == null)
            {
                var obj = new GameObject("<Cells>");
                obj.transform.SetParent(transform);
                CellContainer = obj.transform;
            }
        }

        public void CreateCells()
        {
            CreateCellContainer();
            Cells.RemoveAll(x => x == null);

            // Destroy extra cells.
            while (Cells.Count > Size.x * Size.y)
            {
                var cell = Cells[Cells.Count - 1];
                Cells.RemoveAt(Cells.Count - 1);
                DestroyImmediate(cell.gameObject);
            }

            // Create new cells.
            while (Cells.Count < Size.x * Size.y)
            {
                var obj = new GameObject();
                obj.transform.SetParent(CellContainer);
                var cell = obj.AddComponent<Cell>();
                Cells.Add(cell);
            }

            // Initialize cells
            int k = 0;

            for (int i = 0; i < Size.x; i++)
            {
                for (int j = 0; j < Size.y; j++)
                {
                    var index = new Vector2Int(i, j);
                    Cells[k++].Init(this, index);
                }
            }
        }
    }
}