using MPewsey.ManiaMap;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace MPewsey.ManiaMapUnity
{
    /// <summary>
    /// A room database with references to room addressables.
    /// </summary>
    [CreateAssetMenu(menuName = "Mania Map/Room Databases/Room Addressable Database")]
    public class RoomAddressableDatabase : RoomDatabase<AssetReferenceGameObject>
    {
        /// <summary>
        /// Instantiates the room asynchronously based on current layout.
        /// </summary>
        /// <param name="id">The room ID.</param>
        /// <param name="parent">The parent of the instantiated room.</param>
        /// <param name="position">The option guiding the positioning of the room.</param>
        public AsyncOperationHandle<GameObject> InstantiateRoomAsync(Uid id, Transform parent = null,
            RoomPositionOption position = RoomPositionOption.UseManagerSetting)
        {
            var roomLayout = ManiaMapManager.Current.Layout.Rooms[id];
            var prefab = GetPrefab(roomLayout.Template.Id);
            return RoomComponent.InstantiateRoomAsync(id, prefab, parent, position);
        }

        /// <summary>
        /// Instantiates all rooms in the specified layer of the current layout.
        /// Returns a list of the rooms.
        /// </summary>
        /// <param name="z">The layer.</param>
        /// <param name="parent">The parent of the instantiated rooms.</param>
        public List<RoomComponent> InstantiateLayer(int z, Transform parent = null)
        {
            var result = new List<RoomComponent>();
            var manager = ManiaMapManager.Current;

            foreach (var room in manager.Layout.Rooms.Values)
            {
                if (room.Position.Z == z)
                {
                    var handle = InstantiateRoomAsync(room.Id, parent, RoomPositionOption.LayoutPosition);
                    var instance = handle.WaitForCompletion().GetComponent<RoomComponent>();
                    result.Add(instance);
                }
            }

            return result;
        }
    }
}
