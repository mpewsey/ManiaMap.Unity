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
        private Vector2Int _size;
        /// <summary>
        /// The size of the room template grid.
        /// </summary>
        public Vector2Int Size { get => _size; set => _size = Vector2Int.Max(value, Vector2Int.zero); }

        [SerializeField]
        private Vector2 _cellSize = Vector2.one;
        /// <summary>
        /// The size of each cell.
        /// </summary>
        public Vector2 CellSize { get => _cellSize; set => _cellSize = Vector2.Max(value, Vector2.one); }

        private void OnValidate()
        {
            Size = Size;
            CellSize = CellSize;
        }

        public Array2D<Cell> GetCells()
        {
            var result = new Array2D<Cell>(Size.x, Size.y);
            var cells = CellContainer.GetComponentsInChildren<Cell>();
            int k = 0;

            for (int i = 0; i < Size.x; i++)
            {
                for (int j = 0; j < Size.y; j++)
                {
                    result[i, j] = cells[k++];
                }
            }

            return result;
        }

        public void CreateCells()
        {
            var cells = CellContainer.GetComponentsInChildren<Cell>();
            int k = 0;

            for (int i = 0; i < Size.x; i++)
            {
                for (int j = 0; j < Size.y; j++)
                {
                    var name = $"<Cell ({i}, {j})>";
                    var index = new Vector2Int(i, j);
                    var position = index * CellSize;

                    if (k < cells.Length)
                    {
                        var cell = cells[k];
                        cell.name = name;
                        cell.Position = index;
                        cell.transform.position = position;
                    }
                    else
                    {
                        var obj = new GameObject(name);
                        obj.transform.SetParent(CellContainer);
                        var cell = obj.AddComponent<Cell>();
                        cell.Position = index;
                        cell.transform.position = position;
                    }

                    k++;
                }
            }

            for (int i = k; i < cells.Length; i++)
            {
                DestroyImmediate(cells[i].gameObject);
            }
        }
    }
}