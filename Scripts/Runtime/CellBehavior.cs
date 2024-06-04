using MPewsey.Common.Mathematics;
using MPewsey.ManiaMap;
using MPewsey.ManiaMapUnity.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MPewsey.ManiaMapUnity
{
    /// <summary>
    /// A component representing a RoomTemplate cell.
    /// </summary>
    public class CellBehavior : MonoBehaviour
    {
        [SerializeField]
        [HideInInspector]
        private RoomBehavior _room;
        /// <summary>
        /// The parent room template.
        /// </summary>
        public RoomBehavior Room { get => _room; set => _room = value; }

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

        private void OnValidate()
        {
            Index = Index;
        }

        private void OnDrawGizmos()
        {
            DrawFillColorGizmo();
            DrawOutlineGizmo();
            DrawXGizmo();
        }

        /// <summary>
        /// Draws the cell fill color gizmo.
        /// </summary>
        private void DrawFillColorGizmo()
        {
            Gizmos.color = IsEmpty ? Color.black : Color.grey;
            Gizmos.color *= new Color(1, 1, 1, 0.25f);
            Gizmos.DrawCube(transform.position, Room.Swizzle(Room.CellSize));
        }

        /// <summary>
        /// Draws the cell outline color gizmo.
        /// </summary>
        private void DrawOutlineGizmo()
        {
            Gizmos.color = Color.grey;
            Gizmos.DrawWireCube(transform.position, Room.Swizzle(Room.CellSize));
        }

        /// <summary>
        /// If the cell is empty, draws a gizmo for an X through the cell.
        /// </summary>
        private void DrawXGizmo()
        {
            if (IsEmpty)
            {
                var position = Room.transform.position;
                var origin = Origin();
                var from1 = Room.Swizzle(origin);
                var to1 = Room.Swizzle(origin + new Vector2(Room.CellSize.x, -Room.CellSize.y));
                var from2 = Room.Swizzle(origin + new Vector2(Room.CellSize.x, 0));
                var to2 = Room.Swizzle(origin + new Vector2(0, -Room.CellSize.y));
                Gizmos.color = Color.grey;
                Gizmos.DrawLine(from1 + position, to1 + position);
                Gizmos.DrawLine(from2 + position, to2 + position);
            }
        }

        /// <summary>
        /// Initializes the cell properties.
        /// </summary>
        /// <param name="template">The parent room template.</param>
        /// <param name="index">The index position of the cell in the room template.</param>
        public void Initialize(RoomBehavior template, Vector2Int index)
        {
            name = $"<Cell ({index.x}, {index.y})>";
            Room = template;
            Index = index;
            transform.localPosition = template.Swizzle(Center());
        }

        /// <summary>
        /// Returns the local position for the top-left corner of the cell.
        /// </summary>
        public Vector2 Origin()
        {
            var index = new Vector2(Index.y, -Index.x);
            return Room.CellSize * index;
        }

        /// <summary>
        /// Returns the local position for the center of the cell.
        /// </summary>
        public Vector2 Center()
        {
            var offset = new Vector2(0.5f, -0.5f) * Room.CellSize;
            return Origin() + offset;
        }

        /// <summary>
        /// Returns a list of doors assigned to the cell.
        /// </summary>
        private IEnumerable<DoorBehavior> FindDoors()
        {
            return Room.GetComponentsInChildren<DoorBehavior>().Where(x => x.Cell == this);
        }

        /// <summary>
        /// Returns a list of features assigned to the cell.
        /// </summary>
        private IEnumerable<Feature> FindFeatures()
        {
            return Room.GetComponentsInChildren<Feature>().Where(x => x.Cell == this);
        }

        /// <summary>
        /// Returns a new generation cell.
        /// </summary>
        public Cell CreateData()
        {
            if (IsEmpty)
                return CreateEmptyCell();
            return CreateCell();
        }

        /// <summary>
        /// Performs validation and returns an empty generation cell.
        /// </summary>
        /// <exception cref="EmptyCellException">Raised if any children are assigned to the cell.</exception>
        private Cell CreateEmptyCell()
        {
            if (FindDoors().Any())
                throw new EmptyCellException($"Doors assigned to empty cell: {this}.");
            if (FindFeatures().Any())
                throw new EmptyCellException($"Features assigned to empty cell: {this}.");
            return Cell.Empty;
        }

        /// <summary>
        /// Creates a new generation cell.
        /// </summary>
        private Cell CreateCell()
        {
            var cell = Cell.New;
            AddDoorsToCell(cell);
            AddFeaturesToCell(cell);
            return cell;
        }

        /// <summary>
        /// Adds the doors to the specified cell.
        /// </summary>
        /// <param name="cell">The cell.</param>
        /// <exception cref="DuplicateDirectionException">Raised if multiple doors with the same direction are assigned to the cell.</exception>
        private void AddDoorsToCell(Cell cell)
        {
            foreach (var door in FindDoors())
            {
                if (cell.GetDoor(door.Direction) != null)
                    throw new DuplicateDirectionException($"Door direction already exists: {door}.");
                cell.SetDoor(door.Direction, door.CreateData());
            }
        }

        /// <summary>
        /// Adds the features to the specified cell.
        /// </summary>
        /// <param name="cell">The cell.</param>
        private void AddFeaturesToCell(Cell cell)
        {
            foreach (var feature in FindFeatures())
            {
                cell.AddFeature(feature.Name);
            }
        }

        /// <summary>
        /// Returns the closest cell in the room parented to the specified transform.
        /// </summary>
        /// <param name="transform">The transform.</param>
        /// <exception cref="ArgumentException">Raised if a Room is not a parent of the transform.</exception>
        public static CellBehavior FindClosestCell(Transform transform)
        {
            var room = transform.GetComponentInParent<RoomBehavior>();

            if (room == null)
                throw new ArgumentException($"Parent room not found for transform: {transform}.");

            var cells = room.GetNonEmptyCells();
            var distances = CellSqrDistances(cells, transform.position);
            var index = Maths.MinIndex(distances);
            return index < 0 ? null : cells[index];
        }

        /// <summary>
        /// Returns the square distances between the cells and the specified position.
        /// </summary>
        /// <param name="cells">The cells.</param>
        /// <param name="position">The position.</param>
        private static float[] CellSqrDistances(List<CellBehavior> cells, Vector3 position)
        {
            var distances = new float[cells.Count];

            for (int i = 0; i < distances.Length; i++)
            {
                var delta = cells[i].transform.position - position;
                distances[i] = delta.sqrMagnitude;
            }

            return distances;
        }
    }
}