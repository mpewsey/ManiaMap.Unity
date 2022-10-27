using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// A component representing a door.
    /// </summary>
    [RequireComponent(typeof(CommonEvents))]
    public class Door : CellChild
    {
        /// <summary>
        /// An event that passes a Door argument.
        /// </summary>
        [System.Serializable]
        public class DoorEvent : UnityEvent<Door> { }

        [SerializeField]
        private bool _autoAssignDirection = true;
        /// <summary>
        /// If true, the door direction will be automatically assigned to the cell when update and save
        /// operations are performed.
        /// </summary>
        public bool AutoAssignDirection { get => _autoAssignDirection; set => _autoAssignDirection = value; }

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
        public bool Exists => Connection != null;

        /// <summary>
        /// The associated door connection in the layout.
        /// </summary>
        public DoorConnection Connection { get; private set; }

        private void Start()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes the door based on the layout and layout state
        /// assigned to the current manager.
        /// </summary>
        private void Initialize()
        {
            Connection = FindDoorConnection();

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
            if (RoomData == null)
                return null;

            var roomId = RoomData.Id;
            var manager = ManiaMapManager.Current;
            var position = new Vector2DInt(Cell.Index.x, Cell.Index.y);

            foreach (var neighbor in manager.GetAdjacentRooms(roomId))
            {
                var connection = manager.Layout.GetDoorConnection(roomId, neighbor);

                if (connection.ContainsDoor(roomId, position, Direction))
                    return connection;
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
        public override void AutoAssign()
        {
            base.AutoAssign();

            if (AutoAssignDirection)
                AssignClosestDirection();
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

            var distances = new List<float>
            {
                Vector3.Dot(delta, Cell.Room.Swizzle(Vector2.up)), // North
                Vector3.Dot(delta, Cell.Room.Swizzle(Vector2.down)), // South
                Vector3.Dot(delta, Cell.Room.Swizzle(Vector2.right)), // East
                Vector3.Dot(delta, Cell.Room.Swizzle(Vector2.left)), // West
                Vector3.Dot(delta, Cell.Room.Swizzle(Vector3.forward)), // Top
                Vector3.Dot(delta, Cell.Room.Swizzle(Vector3.back)), // Bottom
            };

            var max = distances.Max();
            var index = distances.FindIndex(x => x == max);
            Direction = index < 0 ? DoorDirection.North : (DoorDirection)index;
        }
    }
}
