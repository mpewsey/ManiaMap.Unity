using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// A room database with references to room prefabs.
    /// </summary>
    public class RoomPrefabDatabase : RoomDatabase<Room>
    {
        /// <inheritdoc/>
        public override Room InstantiateRoom(Uid id, Transform parent = null, bool assignPosition = false)
        {
            var manager = ManiaManager.Current;
            var roomData = manager.Layout.Rooms[id];
            var prefab = GetRoomData(roomData.Template.Id);
            return Room.InstantiateRoom(id, prefab.gameObject, parent, assignPosition);
        }
    }
}
