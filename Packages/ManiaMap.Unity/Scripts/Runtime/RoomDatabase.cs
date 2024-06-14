using MPewsey.ManiaMap;
using MPewsey.ManiaMap.Exceptions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MPewsey.ManiaMapUnity
{
    /// <summary>
    /// A database of room prefabs.
    /// </summary>
    [CreateAssetMenu(menuName = "Mania Map/Room Database")]
    public class RoomDatabase : ScriptableObject
    {
        [SerializeField]
        private List<RoomComponent> _rooms = new List<RoomComponent>();
        /// <summary>
        /// A list of room prefabs.
        /// </summary>
        public List<RoomComponent> Rooms { get => _rooms; set => _rooms = value; }

        /// <summary>
        /// A dictionary of room prefabs by room template ID.
        /// </summary>
        private Dictionary<int, RoomComponent> RoomsByTemplateId { get; } = new Dictionary<int, RoomComponent>();

        /// <summary>
        /// If true, the database is dirty and requires population.
        /// </summary>
        public bool IsDirty { get; private set; } = true;

        private void Awake()
        {
            IsDirty = true;
        }

        private void OnValidate()
        {
            IsDirty = true;
        }

        /// <summary>
        /// Returns the dictionary of rooms by room template ID.
        /// </summary>
        public IReadOnlyDictionary<int, RoomComponent> GetRoomsByTemplateId()
        {
            PopulateIfDirty();
            return RoomsByTemplateId;
        }

        /// <summary>
        /// Sets the object as dirty.
        /// </summary>
        public void MarkDirty()
        {
            IsDirty = true;
        }

        /// <summary>
        /// If the object is dirty, populates the room dictionary.
        /// </summary>
        private void PopulateIfDirty()
        {
            if (IsDirty)
            {
                PopulateRoomsByTemplateId();
                IsDirty = false;
            }
        }

        /// <summary>
        /// Populates the room dictionary.
        /// </summary>
        /// <exception cref="DuplicateIdException">Raised if two unique rooms have the same template ID.</exception>
        private void PopulateRoomsByTemplateId()
        {
            RoomsByTemplateId.Clear();

            foreach (var room in Rooms)
            {
                var id = room.RoomTemplate.Id;

                if (!RoomsByTemplateId.TryGetValue(id, out var storedRoom))
                    RoomsByTemplateId.Add(id, room);
                else if (room != storedRoom)
                    throw new DuplicateIdException($"Duplicate room template ID: (ID = {id}, Room1 = {storedRoom}, Room2 = {room}).");
            }
        }

        /// <summary>
        /// Returns the room prefab with the specified template ID.
        /// </summary>
        /// <param name="id">The template ID.</param>
        public RoomComponent GetRoomPrefab(int id)
        {
            PopulateIfDirty();
            return RoomsByTemplateId[id];
        }

        /// <summary>
        /// Returns the room prefab with the specified room ID.
        /// </summary>
        /// <param name="id">The room ID.</param>
        /// <param name="layoutPack">The layout pack.</param>
        public RoomComponent GetRoomPrefab(Uid id, LayoutPack layoutPack)
        {
            var room = layoutPack.Layout.Rooms[id];
            return GetRoomPrefab(room.Template.Id);
        }

        /// <summary>
        /// Instantiates all rooms in the layout and returns a list of them.
        /// </summary>
        /// <param name="layoutPack">The layout pack.</param>
        /// <param name="parent">The parent transform.</param>
        public List<RoomComponent> InstantiateAllRooms(LayoutPack layoutPack, Transform parent = null)
        {
            var result = new List<RoomComponent>(layoutPack.Layout.Rooms.Count);

            foreach (var room in layoutPack.Layout.Rooms.Values)
            {
                var prefab = GetRoomPrefab(room.Template.Id).gameObject;
                var roomInstance = RoomComponent.InstantiateRoom(room.Id, layoutPack, prefab, parent, true);
                result.Add(roomInstance.GetComponent<RoomComponent>());
            }

            return result;
        }

        /// <summary>
        /// Instantiates the rooms in a layer of a layout and returns a list of them.
        /// </summary>
        /// <param name="layoutPack">The layout pack.</param>
        /// <param name="z">The layer (z) coordinate of the rooms to instantiate. If null, the first layer coordinate found will be used.</param>
        /// <param name="parent">The parent transform.</param>
        public List<RoomComponent> InstantiateRooms(LayoutPack layoutPack, int? z = null, Transform parent = null)
        {
            z ??= layoutPack.Layout.Rooms.Values.Select(x => x.Position.Z).First();
            var result = new List<RoomComponent>();

            foreach (var room in layoutPack.Layout.Rooms.Values)
            {
                if (room.Position.Z == z)
                {
                    var prefab = GetRoomPrefab(room.Template.Id).gameObject;
                    var roomInstance = RoomComponent.InstantiateRoom(room.Id, layoutPack, prefab, parent, true);
                    result.Add(roomInstance.GetComponent<RoomComponent>());
                }
            }

            return result;
        }

        /// <summary>
        /// Instantiates the room with the specified ID.
        /// </summary>
        /// <param name="id">The room ID.</param>
        /// <param name="layoutPack">The layout pack.</param>
        /// <param name="parent">The parent transform.</param>
        /// <param name="assignLayoutPosition">If true, moves the instantiated room's local position to its position in the layout.</param>
        public RoomComponent InstantiateRoom(Uid id, LayoutPack layoutPack, Transform parent = null, bool assignLayoutPosition = false)
        {
            var prefab = GetRoomPrefab(id, layoutPack).gameObject;
            var roomInstance = RoomComponent.InstantiateRoom(id, layoutPack, prefab, parent, assignLayoutPosition);
            return roomInstance.GetComponent<RoomComponent>();
        }
    }
}
