using MPewsey.Common.Mathematics;
using MPewsey.ManiaMap;
using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMapUnity
{
    /// <summary>
    /// A component representing a door.
    /// </summary>
    public class DoorComponent : CellChild
    {
        /// <summary>
        /// A dictionary of doors by their room ID.
        /// </summary>
        private static Dictionary<Uid, LinkedList<DoorComponent>> Doors { get; } = new Dictionary<Uid, LinkedList<DoorComponent>>();

        [Header("Door:")]
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

        /// <summary>
        /// The associated door connection in the layout.
        /// </summary>
        public DoorConnection Connection { get; private set; }

        /// <summary>
        /// True if the door exists in the layout.
        /// </summary>
        public bool DoorExists()
        {
            return Connection != null;
        }

        /// <summary>
        /// The room ID of the room this door connects to.
        /// </summary>
        public Uid ToRoomId()
        {
            if (!DoorExists())
                return new Uid(-1, -1, -1);

            return Connection.GetConnectingRoom(Room.RoomLayout.Id);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            RemoveFromDoorsDictionary();
        }

        protected override void Initialize()
        {
            if (!IsInitialized)
            {
                IsInitialized = true;
                AddToDoorsDictionary();
                Connection = FindDoorConnection();
                OnInitialize.Invoke();
            }
        }

        /// <summary>
        /// Finds the door with the specified room ID and door connection.
        /// Returns null if the door is not found.
        /// </summary>
        /// <param name="roomId">The room ID.</param>
        /// <param name="connection">The door connection.</param>
        public static DoorComponent FindDoor(Uid roomId, DoorConnection connection)
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
            var id = Room.RoomLayout.Id;

            if (!Doors.TryGetValue(id, out var doors))
            {
                doors = new LinkedList<DoorComponent>();
                Doors.Add(id, doors);
            }

            doors.AddLast(this);
        }

        /// <summary>
        /// Removes the door from the doors dictionary.
        /// </summary>
        private void RemoveFromDoorsDictionary()
        {
            if (Room.IsInitialized && Doors.TryGetValue(Room.RoomLayout.Id, out var doors))
                doors.Remove(this);
        }

        /// <summary>
        /// Returns the door connection in the layout associated with the door.
        /// Returns null if the door connection does not exist in the layout.
        /// </summary>
        private DoorConnection FindDoorConnection()
        {
            var roomId = Room.RoomLayout.Id;
            var position = new Vector2DInt(Row, Column);
            return Room.LayoutPack.FindDoorConnection(roomId, position, Direction);
        }

        /// <summary>
        /// Auto assigns elements to the door.
        /// </summary>
        public override void AutoAssign(RoomComponent room)
        {
            base.AutoAssign(room);

            if (AutoAssignDirection)
                Direction = room.FindClosestDoorDirection(Row, Column, transform.position);
        }

        public Door GetMMDoor()
        {
            return new Door(Type, Code);
        }
    }
}
