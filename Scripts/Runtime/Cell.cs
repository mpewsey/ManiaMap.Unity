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
        /// Returns the local position for the bottom-left corner of the cell.
        /// </summary>
        public Vector2 Origin()
        {
            return Room.CellSize * new Vector2(Index.y, Room.Size.x - Index.x - 1);
        }

        /// <summary>
        /// Returns the local position for the center of the cell.
        /// </summary>
        public Vector2 Center()
        {
            return Origin() + 0.5f * Room.CellSize;
        }

        /// <summary>
        /// Returns the bounds of the cell.
        /// </summary>
        public Bounds GetBounds()
        {
            var size = Room.Swizzle(Room.CellSize);

            if (size.x == 0)
                size.x = float.PositiveInfinity;
            if (size.y == 0)
                size.y = float.PositiveInfinity;
            if (size.z == 0)
                size.z = float.PositiveInfinity;

            return new Bounds(transform.position, size);
        }

        /// <summary>
        /// Returns a list of doors assigned to the cell.
        /// </summary>
        public List<Door> FindDoors()
        {
            return FindObjectsOfType<Door>().Where(x => x.Cell == this).ToList();
        }

        /// <summary>
        /// Returns a list of collectable spots assigned to the cell.
        /// </summary>
        public List<CollectableSpot> FindCollectableSpots()
        {
            return FindObjectsOfType<CollectableSpot>().Where(x => x.Cell == this).ToList();
        }

        /// <summary>
        /// Returns a new generation cell.
        /// </summary>
        public ManiaMap.Cell GetCell()
        {
            if (IsEmpty)
            {
                ValidateEmptyCell();
                return ManiaMap.Cell.Empty;
            }

            var cell = ManiaMap.Cell.New;
            SetCellDoors(cell);
            AddCollectableGroups(cell);
            return cell;
        }

        /// <summary>
        /// Checks that no doors or collectable spots are assigned to the cell.
        /// Raises exceptions if there are.
        /// </summary>
        /// <exception cref="Exception">Raised if a door or collectable spot is assigned to the cell.</exception>
        private void ValidateEmptyCell()
        {
            if (FindDoors().Count > 0)
                throw new Exception($"Doors assigned to empty cell: {Index}.");
            if (FindCollectableSpots().Count > 0)
                throw new Exception($"Collectable spots assigned to empty cell: {Index}.");
        }

        /// <summary>
        /// Adds the collectable groups to the generation cell.
        /// </summary>
        /// <param name="cell">The generation cell.</param>
        private void AddCollectableGroups(ManiaMap.Cell cell)
        {
            foreach (var spot in FindCollectableSpots())
            {
                cell.AddCollectableGroup(spot.Id, spot.Group.Name);
            }
        }

        /// <summary>
        /// Sets the doors to the generation cell.
        /// </summary>
        /// <param name="cell">The generation cell.</param>
        /// <exception cref="Exception">Raised if a door is already assigned to the door direction.</exception>
        /// <exception cref="ArgumentException">Raised if the door direction is unhandled.</exception>
        private void SetCellDoors(ManiaMap.Cell cell)
        {
            foreach (var door in FindDoors())
            {
                switch (door.Direction)
                {
                    case DoorDirection.North when cell.NorthDoor != null:
                    case DoorDirection.South when cell.SouthDoor != null:
                    case DoorDirection.East when cell.EastDoor != null:
                    case DoorDirection.West when cell.WestDoor != null:
                    case DoorDirection.Top when cell.TopDoor != null:
                    case DoorDirection.Bottom when cell.BottomDoor != null:
                        throw new Exception($"Door direction already exists: ({Index}, {door.Direction}).");
                    case DoorDirection.North:
                        cell.NorthDoor = door.GetDoor();
                        break;
                    case DoorDirection.South:
                        cell.SouthDoor = door.GetDoor();
                        break;
                    case DoorDirection.East:
                        cell.EastDoor = door.GetDoor();
                        break;
                    case DoorDirection.West:
                        cell.WestDoor = door.GetDoor();
                        break;
                    case DoorDirection.Top:
                        cell.TopDoor = door.GetDoor();
                        break;
                    case DoorDirection.Bottom:
                        cell.BottomDoor = door.GetDoor();
                        break;
                    default:
                        throw new ArgumentException($"Unhandled door direction: {door.Direction}.");
                }
            }
        }

        public static Cell FindClosestCell(Transform transform)
        {
            Cell closest = null;
            var minDistance = float.PositiveInfinity;
            var room = transform.GetComponentInParent<Room>();

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