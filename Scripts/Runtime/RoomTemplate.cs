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
        /// The size of the room template grid.
        /// </summary>
        public Vector2Int Size { get => _size; set => _size = Vector2Int.Max(value, Vector2Int.one); }

        private void OnValidate()
        {
            Size = Size;
            CellSize = CellSize;
        }

        public Vector3 GetPlaneVector(Vector2 vector)
        {
            switch (CellPlane)
            {
                case Plane.XY:
                    return new Vector3(vector.x, -vector.y, 0);
                case Plane.XZ:
                    return new Vector3(vector.x, 0, -vector.y);
                default:
                    throw new System.ArgumentException($"Unhandled cell plane: {CellPlane}.");
            }
        }

        public Cell GetCell(int row, int column)
        {
            if (!CellIndexExists(row, column))
                throw new System.IndexOutOfRangeException($"Index out of range: ({row}, {column}).");
            return CellContainer.GetChild(row).GetChild(column).GetComponent<Cell>();
        }

        private bool CellIndexExists(int row, int column)
        {
            return (uint)row < Size.x && (uint)column < Size.y;
        }

        public void CreateCellContainer()
        {
            if (CellContainer == null)
            {
                var obj = new GameObject("<Cells>");
                obj.transform.SetParent(transform);
                CellContainer = obj.transform;
            }

            CellContainer.gameObject.hideFlags = HideFlags.NotEditable;
        }

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

            for (int i = 0; i < Size.x; i++)
            {
                CreateRow(i);
            }
        }

        private void CreateRow(int row)
        {
            var container = CellContainer.GetChild(row);
            container.gameObject.hideFlags = HideFlags.NotEditable;
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
                obj.transform.hideFlags = HideFlags.NotEditable;
                obj.AddComponent<Cell>();
            }

            for (int j = 0; j < Size.y; j++)
            {
                var index = new Vector2Int(row, j);
                var cell = container.GetChild(j).GetComponent<Cell>();
                cell.Init(this, index);
            }
        }

        public enum Plane
        {
            XY,
            XZ,
        }
    }
}