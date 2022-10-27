using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// A room database with references to room addressables.
    /// </summary>
    public class RoomAddressableDatabase : RoomDatabase<AssetReferenceGameObject>
    {
        /// <summary>
        /// Instantiates the room asynchronously based on current layout.
        /// </summary>
        /// <param name="id">The room ID.</param>
        /// <param name="parent">The parent of the instantiated room.</param>
        /// <param name="position">The option guiding the positioning of the room.</param>
        public AsyncOperationHandle<GameObject> InstantiateRoomAsync(Uid id, Transform parent = null, RoomPositionOption position = RoomPositionOption.Default)
        {
            var data = ManiaMapManager.Current.LayoutData;
            var roomData = data.Layout.Rooms[id];
            var prefab = GetRoomData(roomData.Template.Id);
            return Room.InstantiateRoomAsync(id, prefab, parent, position);
        }

        /// <summary>
        /// Instantiates all rooms in the specified layer of the current layout.
        /// Returns a list of the rooms.
        /// </summary>
        /// <param name="z">The layer.</param>
        /// <param name="parent">The parent of the instantiated rooms.</param>
        public List<Room> InstantiateLayer(int z, Transform parent = null)
        {
            var result = new List<Room>();
            var data = ManiaMapManager.Current.LayoutData;

            foreach (var room in data.Layout.Rooms.Values)
            {
                if (room.Position.Z == z)
                {
                    var handle = InstantiateRoomAsync(room.Id, parent, RoomPositionOption.Layout);
                    var instance = handle.WaitForCompletion().GetComponent<Room>();
                    result.Add(instance);
                }
            }

            return result;
        }
    }
}
