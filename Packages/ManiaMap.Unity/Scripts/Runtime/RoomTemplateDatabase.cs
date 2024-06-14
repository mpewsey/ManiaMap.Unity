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
    /// A database of room templates whose room can be instantiated via Addressables.
    /// </summary>
    [CreateAssetMenu(menuName = "Mania Map/Room Template Database")]
    public class RoomTemplateDatabase : ScriptableObject
    {
        [SerializeField]
        private List<TemplateGroup> _templateGroups = new List<TemplateGroup>();
        /// <summary>
        /// A list of template groups.
        /// </summary>
        public List<TemplateGroup> TemplateGroups { get => _templateGroups; set => _templateGroups = value; }

        /// <summary>
        /// A dictionary of room templates by ID.
        /// </summary>
        private Dictionary<int, RoomTemplateResource> RoomTemplates { get; } = new Dictionary<int, RoomTemplateResource>();

        /// <summary>
        /// If true, the object is dirty and requires population.
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
        /// Returns the dictionary of room templates by ID.
        /// </summary>
        public IReadOnlyDictionary<int, RoomTemplateResource> GetRoomTemplates()
        {
            PopulateIfDirty();
            return RoomTemplates;
        }

        /// <summary>
        /// Sets the object as dirty.
        /// </summary>
        public void MarkDirty()
        {
            IsDirty = true;
        }

        /// <summary>
        /// If the object is dirty, populates the room templates dictionary.
        /// </summary>
        private void PopulateIfDirty()
        {
            if (IsDirty)
            {
                PopulateRoomTemplates();
                IsDirty = false;
            }
        }

        /// <summary>
        /// Populates the room templates dictionary.
        /// </summary>
        /// <exception cref="DuplicateIdException">Raised if two unique templates have the same ID.</exception>
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

        /// <summary>
        /// Returns the room template with the specified ID.
        /// </summary>
        /// <param name="id">The template ID.</param>
        public RoomTemplateResource GetRoomTemplate(int id)
        {
            PopulateIfDirty();
            return RoomTemplates[id];
        }

        /// <summary>
        /// Returns the room template for the specified room ID.
        /// </summary>
        /// <param name="id">The room ID.</param>
        /// <param name="layoutPack">The layout pack.</param>
        public RoomTemplateResource GetRoomTemplate(Uid id, LayoutPack layoutPack)
        {
            var room = layoutPack.Layout.Rooms[id];
            return GetRoomTemplate(room.Template.Id);
        }

        /// <summary>
        /// Instantiates all rooms in the layout asynchronously and returns the rooms as a result of the task.
        /// </summary>
        /// <param name="layoutPack">The layout pack.</param>
        /// <param name="parent">The parent transform.</param>
        public async Task<List<RoomComponent>> InstantiateAllRoomsAsync(LayoutPack layoutPack, Transform parent = null)
        {
            var result = new List<RoomComponent>(layoutPack.Layout.Rooms.Count);

            foreach (var room in layoutPack.Layout.Rooms.Values)
            {
                var prefab = GetRoomTemplate(room.Template.Id).GetAssetReference();
                var handle = RoomComponent.InstantiateRoomAsync(room.Id, layoutPack, prefab, parent, true);
                handle.Completed += handle => OnInstantiationComplete(handle, result);
                await handle.Task;
            }

            ActivateRooms(result);
            return result;
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
                var prefab = GetRoomTemplate(room.Template.Id).GetAssetReference();
                var handle = RoomComponent.InstantiateRoomAsync(room.Id, layoutPack, prefab, parent, true);
                var roomInstance = handle.WaitForCompletion();
                result.Add(roomInstance.GetComponent<RoomComponent>());
            }

            return result;
        }

        /// <summary>
        /// Instantiates the rooms in a specified layer of the layout asynchronously. Returns the rooms as a result of the task.
        /// </summary>
        /// <param name="layoutPack">The layout pack.</param>
        /// <param name="parent">The parent transform.</param>
        /// <param name="z">The layer (z) coordinate.</param>
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
                    handle.Completed += handle => OnInstantiationComplete(handle, result);
                    await handle.Task;
                }
            }

            ActivateRooms(result);
            return result;
        }

        /// <summary>
        /// Activates the rooms in the specified list.
        /// </summary>
        /// <param name="rooms">A list of rooms.</param>
        private static void ActivateRooms(List<RoomComponent> rooms)
        {
            foreach (var room in rooms)
            {
                room.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Adds the room from the operation handle result to the results list and deactivates it.
        /// </summary>
        /// <param name="handle">The room instantiation operation handle.</param>
        /// <param name="results">The room results list.</param>
        private static void OnInstantiationComplete(AsyncOperationHandle<GameObject> handle, List<RoomComponent> results)
        {
            var room = handle.Result.GetComponent<RoomComponent>();
            room.gameObject.SetActive(false);
            results.Add(room);
        }

        /// <summary>
        /// Instantiates the rooms in the specified layer of the layout.
        /// </summary>
        /// <param name="layoutPack">The layout pack.</param>
        /// <param name="parent">The parent transform.</param>
        /// <param name="z">The layer (z) coordinate.</param>
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

        /// <summary>
        /// Instantiates the specified room asynchronously. Returns an operation handle with the result.
        /// </summary>
        /// <param name="id">The room ID.</param>
        /// <param name="layoutPack">The layout pack.</param>
        /// <param name="parent">The parent transform.</param>
        /// <param name="assignLayoutPosition">If true, moves the room's local position to its position in the layout.</param>
        public AsyncOperationHandle<GameObject> InstantiateRoomAsync(Uid id, LayoutPack layoutPack, Transform parent = null, bool assignLayoutPosition = false)
        {
            var prefab = GetRoomTemplate(id, layoutPack).GetAssetReference();
            return RoomComponent.InstantiateRoomAsync(id, layoutPack, prefab, parent, assignLayoutPosition);
        }

        /// <summary>
        /// Instantiates the specified room and returns it.
        /// </summary>
        /// <param name="id">The room ID.</param>
        /// <param name="layoutPack">The layout pack.</param>
        /// <param name="parent">The parent transform.</param>
        /// <param name="assignLayoutPosition">If true, moves the room's local position to its position in the layout.</param>
        public RoomComponent InstantiateRoom(Uid id, LayoutPack layoutPack, Transform parent = null, bool assignLayoutPosition = false)
        {
            var result = InstantiateRoomAsync(id, layoutPack, parent, assignLayoutPosition).WaitForCompletion();
            return result.GetComponent<RoomComponent>();
        }
    }
}
