using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// A room database with references to room addressables.
    /// </summary>
    public class RoomAddressableDatabase : RoomDatabase<AssetReferenceGameObject>
    {
        /// <summary>
        /// Instantiates the room based on current layout.
        /// </summary>
        /// <param name="id">The room ID.</param>
        /// <param name="parent">The parent of the instantiated room.</param>
        /// <param name="assignPosition">If True, the local position of the room is assigned based on the current layout.</param>
        public Room InstantiateRoom(Uid id, Transform parent = null, bool assignPosition = false)
        {
            var manager = ManiaManager.Current;
            var roomData = manager.Layout.Rooms[id];
            var prefab = GetRoomData(roomData.Template.Id);
            return Room.InstantiateRoom(id, prefab, parent, assignPosition);
        }

        /// <summary>
        /// Instantiates the room asynchronously based on current layout.
        /// </summary>
        /// <param name="id">The room ID.</param>
        /// <param name="parent">The parent of the instantiated room.</param>
        /// <param name="assignPosition">If True, the local position of the room is assigned based on the current layout.</param>
        public Task<Room> InstantiateRoomAsync(Uid id, Transform parent = null, bool assignPosition = false)
        {
            var manager = ManiaManager.Current;
            var roomData = manager.Layout.Rooms[id];
            var prefab = GetRoomData(roomData.Template.Id);
            return Room.InstantiateRoomAsync(id, prefab, parent, assignPosition);
        }
    }
}
