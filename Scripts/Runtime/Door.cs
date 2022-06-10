using UnityEngine;
using UnityEngine.Events;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// A component representing a door.
    /// </summary>
    public class Door : MonoBehaviour
    {
        /// <summary>
        /// An event that passes a Door argument.
        /// </summary>
        [System.Serializable]
        public class DoorEvent : UnityEvent<Door> { }

        [SerializeField]
        private bool _autoAssignCell = true;
        /// <summary>
        /// If true, the cell will be automatically assigned to the cell when update and save operations
        /// are performed.
        /// </summary>
        public bool AutoAssignCell { get => _autoAssignCell; set => _autoAssignCell = value; }

        [SerializeField]
        private bool _autoAssignDirection = true;
        /// <summary>
        /// If true, the door direction will be automatically assigned to the cell when update and save
        /// operations are performed.
        /// </summary>
        public bool AutoAssignDirection { get => _autoAssignDirection; set => _autoAssignDirection = value; }

        [SerializeField]
        private Cell _cell;
        /// <summary>
        /// The parent cell.
        /// </summary>
        public Cell Cell { get => _cell; set => _cell = value; }

        [SerializeField]
        private DoorDirection _direction;
        /// <summary>
        /// The door direction.
        /// </summary>
        public DoorDirection Direction { get => _direction; set => _direction = value; }

        [SerializeField]
        private DoorType _type;
        /// <summary>
        /// The door type.
        /// </summary>
        public DoorType Type { get => _type; set => _type = value; }

        [SerializeField]
        private int _code;
        /// <summary>
        /// The door code.
        /// </summary>
        public int Code { get => _code; set => _code = value; }

        [SerializeField]
        private DoorEvent _onDoorExists = new DoorEvent();
        /// <summary>
        /// The event invoked when a door exists at the location.
        /// </summary>
        public DoorEvent OnDoorExists { get => _onDoorExists; set => _onDoorExists = value; }

        [SerializeField]
        private DoorEvent _onNoDoorExists = new DoorEvent();
        /// <summary>
        /// The event invoked when no door exists at the location.
        /// </summary>
        public DoorEvent OnNoDoorExists { get => _onNoDoorExists; set => _onNoDoorExists = value; }

        /// <summary>
        /// True if the door exists in the layout.
        /// </summary>
        public bool Exists { get; private set; }

        /// <summary>
        /// The associated door connection in the layout.
        /// </summary>
        public DoorConnection Connection { get; private set; }

        /// <summary>
        /// The room ID.
        /// </summary>
        public Uid RoomId { get => Cell.Room.RoomId; }

        /// <summary>
        /// Initializes the door based on the layout and layout state.
        /// </summary>
        /// <param name="room">The parent room.</param>
        public void OnRoomInit()
        {
            Connection = FindDoorConnection();
            Exists = Connection != null;

            if (Exists)
                OnDoorExists.Invoke(this);
            else
                OnNoDoorExists.Invoke(this);
        }

        /// <summary>
        /// Returns the door connection in the layout associated with the door.
        /// Returns null if the door connection does not exist in the layout.
        /// </summary>
        private DoorConnection FindDoorConnection()
        {
            var manager = ManiaManager.Current;
            var position = new Vector2DInt(Cell.Index.x, Cell.Index.y);

            foreach (var neighbor in manager.GetAdjacentRooms(RoomId))
            {
                var connection = manager.Layout.GetDoorConnection(RoomId, neighbor);

                if (connection.ContainsDoor(RoomId, position, Direction))
                {
                    return connection;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns a new generation door.
        /// </summary>
        public ManiaMap.Door GetDoor()
        {
            return new ManiaMap.Door(Type, Code);
        }

        /// <summary>
        /// Auto assigns elements to the door.
        /// </summary>
        public void AutoAssign()
        {
            if (AutoAssignCell)
                AssignClosestCell();

            if (AutoAssignDirection)
                AssignClosestDirection();
        }

        /// <summary>
        /// Assigns the closest cell to the door.
        /// </summary>
        public void AssignClosestCell()
        {
            Cell = Cell.FindClosestCell(transform);
        }

        /// <summary>
        /// Assigns the closest direction to the door based on the door's relative
        /// position to the assigned cell.
        /// </summary>
        public void AssignClosestDirection()
        {
            if (Cell == null)
                return;

            var delta = transform.position - Cell.transform.position;
            var north = Vector3.Dot(delta, Cell.Room.Swizzle(Vector2.up));
            var south = Vector3.Dot(delta, Cell.Room.Swizzle(Vector2.down));
            var east = Vector3.Dot(delta, Cell.Room.Swizzle(Vector2.right));
            var west = Vector3.Dot(delta, Cell.Room.Swizzle(Vector2.left));
            var top = Vector3.Dot(delta, Cell.Room.Swizzle(Vector3.forward));
            var bottom = Vector3.Dot(delta, Cell.Room.Swizzle(Vector3.back));
            var max = Mathf.Max(north, south, east, west, top, bottom);

            if (max == north)
                Direction = DoorDirection.North;
            else if (max == south)
                Direction = DoorDirection.South;
            else if (max == east)
                Direction = DoorDirection.East;
            else if (max == west)
                Direction = DoorDirection.West;
            else if (max == top)
                Direction = DoorDirection.Top;
            else
                Direction = DoorDirection.Bottom;
        }
    }
}
