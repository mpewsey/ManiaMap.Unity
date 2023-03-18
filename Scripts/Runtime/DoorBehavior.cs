using MPewsey.Common.Mathematics;
using UnityEngine;
using UnityEngine.Events;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// A component representing a door.
    /// </summary>
    public class DoorBehavior : CellChild
    {
        /// <summary>
        /// An event that passes a Door argument.
        /// </summary>
        [System.Serializable]
        public class UnityDoorEvent : UnityEvent<DoorBehavior> { }

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
        private UnityDoorEvent _onInitialize = new UnityDoorEvent();
        /// <summary>
        /// The event invoked after the door is initialized. This occurs on start.
        /// </summary>
        public UnityDoorEvent OnInitialize { get => _onInitialize; set => _onInitialize = value; }

        /// <summary>
        /// The associated door connection in the layout.
        /// </summary>
        public DoorConnection Connection { get; private set; }

        /// <summary>
        /// True if the door exists in the layout.
        /// </summary>
        public bool Exists() => Connection != null;

        /// <summary>
        /// The room ID of the room this door connects to.
        /// </summary>
        public Uid ToRoomId()
        {
            if (!Exists())
                return new Uid(-1, -1, -1);
            if (Connection.FromRoom == RoomId())
                return Connection.ToRoom;
            return Connection.FromRoom;
        }

        /// <summary>
        /// The door position in the room that this door connects to.
        /// </summary>
        public DoorPosition ToDoorPosition()
        {
            if (!Exists())
                return null;
            if (Connection.FromRoom == RoomId())
                return Connection.ToDoor;
            return Connection.FromDoor;
        }

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
            OnInitialize.Invoke(this);
        }

        /// <summary>
        /// Returns the door connection in the layout associated with the door.
        /// Returns null if the door connection does not exist in the layout.
        /// </summary>
        private DoorConnection FindDoorConnection()
        {
            var roomId = RoomId();
            var position = new Vector2DInt(Cell.Index.x, Cell.Index.y);

            foreach (var connection in DoorConnections())
            {
                if (connection.ContainsDoor(roomId, position, Direction))
                    return connection;
            }

            return null;
        }

        /// <summary>
        /// Returns a new generation door.
        /// </summary>
        public Door CreateData()
        {
            return new Door(Type, Code);
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
            if (Cell != null)
                Direction = FindClosestDirection();
        }

        /// <summary>
        /// Finds the closest door direction based on the door's relative position to the assigned cell.
        /// </summary>
        private DoorDirection FindClosestDirection()
        {
            var room = Room();
            var delta = transform.position - Cell.transform.position;

            System.Span<DoorDirection> directions = stackalloc DoorDirection[]
            {
                DoorDirection.North,
                DoorDirection.South,
                DoorDirection.East,
                DoorDirection.West,
                DoorDirection.Top,
                DoorDirection.Bottom,
            };

            var distances = new float[]
            {
                Vector3.Dot(delta, room.Swizzle(Vector2.up)), // North
                Vector3.Dot(delta, room.Swizzle(Vector2.down)), // South
                Vector3.Dot(delta, room.Swizzle(Vector2.right)), // East
                Vector3.Dot(delta, room.Swizzle(Vector2.left)), // West
                Vector3.Dot(delta, room.Swizzle(Vector3.forward)), // Top
                Vector3.Dot(delta, room.Swizzle(Vector3.back)), // Bottom
            };

            var max = Mathf.Max(distances);

            for (int i = 0; i < distances.Length; i++)
            {
                if (distances[i] == max)
                    return directions[i];
            }

            return directions[0];
        }
    }
}
