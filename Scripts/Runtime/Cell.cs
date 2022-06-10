using MPewsey.ManiaMap.Unity.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private Room _room;
        /// <summary>
        /// The parent room template.
        /// </summary>
        public Room Room { get => _room; set => _room = value; }

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
            // Draw fill color.
            Gizmos.color = IsEmpty ? new Color(0, 0, 0, 0.25f) : new Color(0.5f, 0.5f, 0.5f, 0.25f);
            Gizmos.DrawCube(transform.position, Room.Swizzle(Room.CellSize));

            // Draw outline.
            Gizmos.color = Color.grey;
            Gizmos.DrawWireCube(transform.position, Room.Swizzle(Room.CellSize));

            // If empty, draw an X through the cell.
            if (IsEmpty)
            {
                var origin = Origin();
                var from1 = Room.Swizzle(origin);
                var to1 = Room.Swizzle(origin + Room.CellSize);
                var from2 = Room.Swizzle(origin + new Vector2(Room.CellSize.x, 0));
                var to2 = Room.Swizzle(origin + new Vector2(0, Room.CellSize.y));
                Gizmos.color = Color.grey;
                Gizmos.DrawLine(from1, to1);
                Gizmos.DrawLine(from2, to2);
            }
        }

        /// <summary>
        /// Initializes the cell properties.
        /// </summary>
        /// <param name="template">The parent room template.</param>
        /// <param name="index">The index position of the cell in the room template.</param>
        public void Init(Room template, Vector2Int index)
        {
            name = $"<Cell ({index.x}, {index.y})>";
            Room = template;
            Index = index;
            transform.localPosition = template.Swizzle(Center());
        }

        /// <summary>
        /// Returns the local position for the bottom-left corner of the cell.
        /// </summary>
        public Vector2 Origin()
        {
            var index = new Vector2(Index.y, Room.Size.x - Index.x - 1);
            return Room.CellSize * index;
        }

        /// <summary>
        /// Returns the local position for the center of the cell.
        /// </summary>
        public Vector2 Center()
        {
            return Origin() + 0.5f * Room.CellSize;
        }

        /// <summary>
        /// Returns a list of doors assigned to the cell.
        /// </summary>
        public List<Door> FindDoors()
        {
            return Room.GetComponentsInChildren<Door>().Where(x => x.Cell == this).ToList();
        }

        /// <summary>
        /// Returns a list of collectable spots assigned to the cell.
        /// </summary>
        public List<CollectableSpot> FindCollectableSpots()
        {
            return Room.GetComponentsInChildren<CollectableSpot>().Where(x => x.Cell == this).ToList();
        }

        /// <summary>
        /// Returns a new generation cell.
        /// </summary>
        /// <exception cref="EmptyCellException">Raised if the cell is empty and a door or collectable spot is assigned to it.</exception>
        /// <exception cref="DuplicateDirectionException">Raised if multiple doors with the same direction are assigned to the cell.</exception>
        /// <exception cref="UnassignedCollectableGroupException">Raised if a collectable group is not assigned to a collectable spot.</exception>
        public ManiaMap.Cell GetCell()
        {
            var doors = FindDoors();
            var spots = FindCollectableSpots();

            if (IsEmpty)
            {
                if (doors.Count > 0)
                    throw new EmptyCellException($"Doors assigned to empty cell: {Index}.");
                if (spots.Count > 0)
                    throw new EmptyCellException($"Collectable spots assigned to empty cell: {Index}.");
                return ManiaMap.Cell.Empty;
            }

            var cell = ManiaMap.Cell.New;

            // Add doors.
            foreach (var door in doors)
            {
                if (cell.GetDoor(door.Direction) != null)
                    throw new DuplicateDirectionException($"Door direction already exists: Index = {Index}, Direction = {door.Direction}.");
                cell.SetDoor(door.Direction, door.GetDoor());
            }

            // Add collectable spots.
            foreach (var spot in spots)
            {
                if (spot.Group == null)
                    throw new UnassignedCollectableGroupException($"Collectable group not assigned to collectable spot: {spot.name}.");
                cell.AddCollectableSpot(spot.Id, spot.Group.Name);
            }

            return cell;
        }

        /// <summary>
        /// Returns the closest cell in the room parented to the specified transform.
        /// </summary>
        /// <param name="transform">The transform.</param>
        /// <exception cref="ArgumentException">Raised if a Room is not a parent of the transform.</exception>
        public static Cell FindClosestCell(Transform transform)
        {
            Cell closest = null;
            var minDistance = float.PositiveInfinity;
            var room = transform.GetComponentInParent<Room>();

            if (room == null)
                throw new ArgumentException($"Parent room not found for transform: {transform.name}.");

            for (int i = 0; i < room.Size.x; i++)
            {
                for (int j = 0; j < room.Size.y; j++)
                {
                    var cell = room.GetCell(i, j);

                    if (cell.IsEmpty)
                        continue;

                    var delta = cell.transform.position - transform.position;
                    var distance = delta.sqrMagnitude;

                    if (distance < minDistance)
                    {
                        closest = cell;
                        minDistance = distance;
                    }
                }
            }

            return closest;
        }
    }
}