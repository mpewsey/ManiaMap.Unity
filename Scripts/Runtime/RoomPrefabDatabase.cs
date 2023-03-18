using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// A room database with references to room prefabs.
    /// </summary>
    [CreateAssetMenu(menuName = "Mania Map/Room Databases/Room Prefab Database")]
    public class RoomPrefabDatabase : RoomDatabase<RoomBehavior>
    {
        /// <summary>
        /// Instantiates the room based on current layout.
        /// </summary>
        /// <param name="id">The room ID.</param>
        /// <param name="parent">The parent of the instantiated room.</param>
        /// <param name="position">The option guiding the positioning of the room.</param>
        public RoomBehavior InstantiateRoom(Uid id, Transform parent = null,
            RoomPositionOption position = RoomPositionOption.UseManagerSettings)
        {
            var roomLayout = ManiaMapManager.Current.GetRoom(id);
            var prefab = GetRoomPrefab(roomLayout.Template.Id);
            return RoomBehavior.InstantiateRoom(id, prefab.gameObject, parent, position);
        }

        /// <summary>
        /// Instantiates all rooms in the specified layer of the current layout.
        /// Returns a list of the rooms.
        /// </summary>
        /// <param name="z">The layer.</param>
        /// <param name="parent">The parent of the instantiated rooms.</param>
        public List<RoomBehavior> InstantiateLayer(int z, Transform parent = null)
        {
            var result = new List<RoomBehavior>();
            var manager = ManiaMapManager.Current;

            foreach (var room in manager.Layout.Rooms.Values)
            {
                if (room.Position.Z == z)
                {
                    var instance = InstantiateRoom(room.Id, parent, RoomPositionOption.LayoutPosition);
                    result.Add(instance);
                }
            }

            return result;
        }
    }
}
