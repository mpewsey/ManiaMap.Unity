using System;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// A component for creating a room.
    /// </summary>
    public class Room : MonoBehaviour
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
        /// <summary>
        /// The cell container.
        /// </summary>
        public Transform CellContainer { get => _cellContainer; set => _cellContainer = value; }

        [SerializeField]
        private Plane _cellPlane;
        /// <summary>
        /// The plane in which the cells reside.
        /// </summary>
        public Plane CellPlane { get => _cellPlane; set => _cellPlane = value; }

        [SerializeField]
        private Vector2 _cellSize = Vector2.one;
        /// <summary>
        /// The size of each cell.
        /// </summary>
        public Vector2 CellSize { get => _cellSize; set => _cellSize = Vector2.Max(value, Vector2.one); }

        [SerializeField]
        private Vector2Int _size = Vector2Int.one;
        /// <summary>
        /// The size of the room grid.
        /// </summary>
        public Vector2Int Size { get => _size; set => _size = Vector2Int.Max(value, Vector2Int.one); }

        private void OnValidate()
        {
            Size = Size;
            CellSize = CellSize;
        }

        /// <summary>
        /// Returns a swizzled vector for a given 2D vector.
        /// </summary>
        /// <param name="vector">The local 2D vector.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Raised if the cell plane is unhandled.</exception>
        public Vector3 Swizzle(Vector2 vector)
        {
            switch (CellPlane)
            {
                case Plane.XY:
                    return new Vector3(vector.x, vector.y, 0);
                case Plane.XZ:
                    return new Vector3(vector.x, 0, vector.y);
                default:
                    throw new ArgumentException($"Unhandled cell plane: {CellPlane}.");
            }
        }

        /// <summary>
        /// Returns the cell at the specified index.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <param name="column">The column index.</param>
        /// <exception cref="IndexOutOfRangeException">Raised if the index is out of range.</exception>
        public Cell GetCell(int row, int column)
        {
            if (!CellIndexExists(row, column))
                throw new IndexOutOfRangeException($"Index out of range: ({row}, {column}).");
            return CellContainer.GetChild(row).GetChild(column).GetComponent<Cell>();
        }

        /// <summary>
        /// Returns true if the cell index exists.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <param name="column">The column index.</param>
        private bool CellIndexExists(int row, int column)
        {
            return (uint)row < Size.x && (uint)column < Size.y;
        }

        /// <summary>
        /// Creates the cell container if it does not exist and assigns it to the object.
        /// </summary>
        public void CreateCellContainer()
        {
            if (CellContainer == null)
            {
                var obj = new GameObject("<Cells>");
                obj.transform.SetParent(transform);
                CellContainer = obj.transform;
            }
        }

        /// <summary>
        /// Creates and initializes the room cells. This method will attempt
        /// to reuse existing objects if it can. Extra cells are also destroyed.
        /// </summary>
        public void CreateCells()
        {
            CreateCellContainer();

            // Destroy extra rows.
            while (CellContainer.childCount > Size.x)
            {
                var row = CellContainer.GetChild(CellContainer.childCount - 1);
                DestroyImmediate(row.gameObject);
            }

            // Create new rows.
            while (CellContainer.childCount < Size.x)
            {
                var obj = new GameObject("<New Row>");
                obj.transform.SetParent(CellContainer);
            }

            for (int i = 0; i < CellContainer.childCount; i++)
            {
                CreateRow(i);
            }
        }

        /// <summary>
        /// Creates the row and cells for the specified index. This method will attempt
        /// to reuse existing objects if it can. Extra cells are also destroyed.
        /// </summary>
        /// <param name="row">The row index.</param>
        private void CreateRow(int row)
        {
            var container = CellContainer.GetChild(row);
            container.name = $"<Row {row}>";

            // Destroy extra cells.
            while (container.childCount > Size.y)
            {
                var cell = container.GetChild(container.childCount - 1);
                DestroyImmediate(cell.gameObject);
            }

            // Create new cells.
            while (container.childCount < Size.y)
            {
                var obj = new GameObject("<New Cell>");
                obj.transform.SetParent(container);
                obj.AddComponent<Cell>();
            }

            for (int j = 0; j < container.childCount; j++)
            {
                var index = new Vector2Int(row, j);
                var cell = container.GetChild(j).GetComponent<Cell>();
                cell.Init(this, index);
            }
        }

        /// <summary>
        /// Returns a new generation room template.
        /// </summary>
        public ManiaMap.RoomTemplate GetTemplate()
        {
            CreateCells();
            var cells = new Array2D<ManiaMap.Cell>(Size.x, Size.y);

            for (int i = 0; i < cells.Rows; i++)
            {
                for (int j = 0; j < cells.Columns; j++)
                {
                    cells[i, j] = GetCell(i, j).GetCell();
                }
            }

            return new ManiaMap.RoomTemplate(Id, Name, cells);
        }

        /// <summary>
        /// The cell plane.
        /// </summary>
        public enum Plane
        {
            /// The XY plane.
            XY,
            /// The XZ plane.
            XZ,
        }
    }
}