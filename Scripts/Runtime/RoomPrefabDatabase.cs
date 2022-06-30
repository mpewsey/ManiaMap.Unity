using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// A room database with references to room prefabs.
    /// </summary>
    public class RoomPrefabDatabase : RoomDatabase<Room>
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
            return Room.InstantiateRoom(id, prefab.gameObject, parent, assignPosition);
        }
    }
}
