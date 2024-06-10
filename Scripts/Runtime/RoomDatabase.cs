using MPewsey.ManiaMap;
using MPewsey.ManiaMap.Exceptions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MPewsey.ManiaMapUnity
{
    /// <summary>
    /// A room database with references to room prefabs.
    /// </summary>
    [CreateAssetMenu(menuName = "Mania Map/Room Database")]
    public class RoomDatabase : ScriptableObject
    {
        [SerializeField]
        private List<RoomComponent> _rooms = new List<RoomComponent>();
        public List<RoomComponent> Rooms { get => _rooms; set => _rooms = value; }

        private Dictionary<int, RoomComponent> RoomsByTemplateId { get; } = new Dictionary<int, RoomComponent>();
        public bool IsDirty { get; private set; } = true;

        private void Awake()
        {
            IsDirty = true;
        }

        private void OnValidate()
        {
            IsDirty = true;
        }

        public IReadOnlyDictionary<int, RoomComponent> GetRoomsByTemplateId()
        {
            PopulateIfDirty();
            return RoomsByTemplateId;
        }

        public void MarkDirty()
        {
            IsDirty = true;
        }

        private void PopulateIfDirty()
        {
            if (IsDirty)
            {
                PopulateRoomsByTemplateId();
                IsDirty = false;
            }
        }

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

        public RoomComponent GetRoomPrefab(int id)
        {
            PopulateIfDirty();
            return RoomsByTemplateId[id];
        }

        public RoomComponent GetRoomPrefab(Uid id, LayoutPack layoutPack)
        {
            var room = layoutPack.Layout.Rooms[id];
            return GetRoomPrefab(room.Template.Id);
        }

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

        public RoomComponent InstantiateRoom(Uid id, LayoutPack layoutPack, Transform parent = null, bool assignLayoutPosition = false)
        {
            var prefab = GetRoomPrefab(id, layoutPack).gameObject;
            var roomInstance = RoomComponent.InstantiateRoom(id, layoutPack, prefab, parent, true);
            return roomInstance.GetComponent<RoomComponent>();
        }
    }
}
