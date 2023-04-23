using MPewsey.Common.Mathematics;
using MPewsey.ManiaMap.Unity.Exceptions;
using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// A component representing a door.
    /// </summary>
    public class DoorBehavior : CellChild
    {
        /// <summary>
        /// A dictionary of doors by their room ID.
        /// </summary>
        private static Dictionary<Uid, LinkedList<DoorBehavior>> Doors { get; } = new Dictionary<Uid, LinkedList<DoorBehavior>>();

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
        private DoorCode _code;
        /// <summary>
        /// The door code.
        /// </summary>
        public DoorCode Code { get => _code; set => _code = value; }

        [SerializeField]
        private DoorBehaviorEvent _onInitialize;
        /// <summary>
        /// The event invoked after the door is initialized.
        /// </summary>
        public DoorBehaviorEvent OnInitialize { get => _onInitialize; set => _onInitialize = value; }

        private DoorConnection _connection;
        /// <summary>
        /// The associated door connection in the layout.
        /// </summary>
        public DoorConnection Connection
        {
            get
            {
                AssertIsInitialized();
                return _connection;
            }
            private set => _connection = value;
        }

        /// <summary>
        /// True if the door has been initialized.
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// True if the door exists in the layout.
        /// </summary>
        public bool Exists() => Connection != null;

        /// <summary>
        /// The room ID of the room this door connects to.
        /// </summary>
        public Uid ToRoomId()
        {
            if (Exists())
            {
                var roomId = RoomId();

                if (Connection.FromRoom == roomId)
                    return Connection.ToRoom;
                if (Connection.ToRoom == roomId)
                    return Connection.FromRoom;
            }

            return new Uid(-1, -1, -1);
        }

        private void Awake()
        {
            Room().OnInitialize.AddListener(Initialize);
        }

        private void OnDestroy()
        {
            Room().OnInitialize.RemoveListener(Initialize);
            RemoveFromDoorsDictionary();
        }

        /// <summary>
        /// Initializes the door.
        /// </summary>
        private void Initialize()
        {
            if (!IsInitialized)
            {
                IsInitialized = true;
                AddToDoorsDictionary();
                Connection = FindDoorConnection();
                OnInitialize.Invoke(this);
            }
        }

        /// <summary>
        /// Checks that the door is initialized and throws an exception if it isn't.
        /// </summary>
        private void AssertIsInitialized()
        {
            if (!IsInitialized)
                throw new DoorNotInitializedException($"Attempting to access initialized member on uninitialized door: {this}.");
        }

        /// <summary>
        /// Finds the door with the specified room ID and door connection.
        /// Returns null if the door is not found.
        /// </summary>
        /// <param name="roomId">The room ID.</param>
        /// <param name="connection">The door connection.</param>
        public static DoorBehavior FindDoor(Uid roomId, DoorConnection connection)
        {
            if (connection != null && Doors.TryGetValue(roomId, out var doors))
            {
                foreach (var door in doors)
                {
                    if (door.Connection == connection)
                        return door;
                }
            }

            return null;
        }

        /// <summary>
        /// Adds the door to the doors dictionary.
        /// </summary>
        private void AddToDoorsDictionary()
        {
            var id = RoomId();

            if (!Doors.TryGetValue(id, out var doors))
            {
                doors = new LinkedList<DoorBehavior>();
                Doors.Add(id, doors);
            }

            doors.AddLast(this);
        }

        /// <summary>
        /// Removes the door from the doors dictionary.
        /// </summary>
        private void RemoveFromDoorsDictionary()
        {
            if (IsInitialized && Doors.TryGetValue(RoomId(), out var doors))
                doors.Remove(this);
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

            return directions[Maths.MaxIndex(distances)];
        }
    }
}
