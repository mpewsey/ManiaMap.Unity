using MPewsey.ManiaMap;
using MPewsey.ManiaMap.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace MPewsey.ManiaMapUnity
{
    /// <summary>
    /// A room database with references to room addressables.
    /// </summary>
    [CreateAssetMenu(menuName = "Mania Map/Room Template Database")]
    public class RoomTemplateDatabase : ScriptableObject
    {
        [SerializeField]
        private List<TemplateGroup> _templateGroups = new List<TemplateGroup>();
        public List<TemplateGroup> TemplateGroups { get => _templateGroups; set => _templateGroups = value; }

        private Dictionary<int, RoomTemplateResource> RoomTemplates { get; } = new Dictionary<int, RoomTemplateResource>();
        public bool IsDirty { get; private set; } = true;

        private void Awake()
        {
            IsDirty = true;
        }

        private void OnValidate()
        {
            IsDirty = true;
        }

        public IReadOnlyDictionary<int, RoomTemplateResource> GetRoomTemplates()
        {
            PopulateIfDirty();
            return RoomTemplates;
        }

        public void MarkDirty()
        {
            IsDirty = true;
        }

        private void PopulateIfDirty()
        {
            if (IsDirty)
            {
                PopulateRoomTemplates();
                IsDirty = false;
            }
        }

        private void PopulateRoomTemplates()
        {
            RoomTemplates.Clear();

            foreach (var group in TemplateGroups)
            {
                foreach (var entry in group.Entries)
                {
                    var id = entry.Template.Id;

                    if (!RoomTemplates.TryGetValue(id, out var template))
                        RoomTemplates.Add(id, entry.Template);
                    else if (template != entry.Template)
                        throw new DuplicateIdException($"Duplicate room template ID: (ID = {id}, Template1 = {template}, Template2 = {entry.Template}).");
                }
            }
        }

        public RoomTemplateResource GetRoomTemplate(int id)
        {
            PopulateIfDirty();
            return RoomTemplates[id];
        }

        public RoomTemplateResource GetRoomTemplate(Uid id, LayoutPack layoutPack)
        {
            var room = layoutPack.Layout.Rooms[id];
            return GetRoomTemplate(room.Template.Id);
        }

        public async Task<List<RoomComponent>> InstantiateAllRoomsAsync(LayoutPack layoutPack, Transform parent = null)
        {
            var result = new List<RoomComponent>(layoutPack.Layout.Rooms.Count);

            foreach (var room in layoutPack.Layout.Rooms.Values)
            {
                var prefab = GetRoomTemplate(room.Template.Id).GetAssetReference();
                var handle = RoomComponent.InstantiateRoomAsync(room.Id, layoutPack, prefab, parent, true);
                var roomInstance = await handle.Task;
                result.Add(roomInstance.GetComponent<RoomComponent>());
            }

            return result;
        }

        public List<RoomComponent> InstantiateAllRooms(LayoutPack layoutPack, Transform parent = null)
        {
            var result = new List<RoomComponent>(layoutPack.Layout.Rooms.Count);

            foreach (var room in layoutPack.Layout.Rooms.Values)
            {
                var prefab = GetRoomTemplate(room.Template.Id).GetAssetReference();
                var handle = RoomComponent.InstantiateRoomAsync(room.Id, layoutPack, prefab, parent, true);
                var roomInstance = handle.WaitForCompletion();
                result.Add(roomInstance.GetComponent<RoomComponent>());
            }

            return result;
        }

        public async Task<List<RoomComponent>> InstantiateRoomsAsync(LayoutPack layoutPack, Transform parent = null, int? z = null)
        {
            z ??= layoutPack.Layout.Rooms.Values.Select(x => x.Position.Z).First();
            var result = new List<RoomComponent>();

            foreach (var room in layoutPack.Layout.Rooms.Values)
            {
                if (room.Position.Z == z)
                {
                    var prefab = GetRoomTemplate(room.Template.Id).GetAssetReference();
                    var handle = RoomComponent.InstantiateRoomAsync(room.Id, layoutPack, prefab, parent, true);
                    var roomInstance = await handle.Task;
                    result.Add(roomInstance.GetComponent<RoomComponent>());
                }
            }

            return result;
        }

        public List<RoomComponent> InstantiateRooms(LayoutPack layoutPack, Transform parent = null, int? z = null)
        {
            z ??= layoutPack.Layout.Rooms.Values.Select(x => x.Position.Z).First();
            var result = new List<RoomComponent>();

            foreach (var room in layoutPack.Layout.Rooms.Values)
            {
                if (room.Position.Z == z)
                {
                    var prefab = GetRoomTemplate(room.Template.Id).GetAssetReference();
                    var handle = RoomComponent.InstantiateRoomAsync(room.Id, layoutPack, prefab, parent, true);
                    var roomInstance = handle.WaitForCompletion();
                    result.Add(roomInstance.GetComponent<RoomComponent>());
                }
            }

            return result;
        }

        public AsyncOperationHandle<GameObject> InstantiateRoomAsync(Uid id, LayoutPack layoutPack, Transform parent = null, bool assignLayoutPosition = false)
        {
            var prefab = GetRoomTemplate(id, layoutPack).GetAssetReference();
            return RoomComponent.InstantiateRoomAsync(id, layoutPack, prefab, parent, assignLayoutPosition);
        }

        public RoomComponent InstantiateRoom(Uid id, LayoutPack layoutPack, Transform parent = null, bool assignLayoutPosition = false)
        {
            var result = InstantiateRoomAsync(id, layoutPack, parent, assignLayoutPosition).WaitForCompletion();
            return result.GetComponent<RoomComponent>();
        }
    }
}
