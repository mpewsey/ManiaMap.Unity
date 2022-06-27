using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// A base class for creating a room data source database.
    /// </summary>
    /// <typeparam name="T">The room data source type.</typeparam>
    public abstract class RoomDatabase<T> : MonoBehaviour
    {
        [SerializeField]
        protected List<RoomDatabaseEntry<T>> _entries = new List<RoomDatabaseEntry<T>>();
        /// <summary>
        /// A list of database entries.
        /// </summary>
        public List<RoomDatabaseEntry<T>> Entries { get => _entries; set => _entries = value; }

        /// <summary>
        /// A dictionary of room data sources by room ID.
        /// </summary>
        protected Dictionary<int, T> RoomDataDictionary { get; } = new Dictionary<int, T>();

        /// <summary>
        /// Instantiates the room based on current layout.
        /// </summary>
        /// <param name="id">The room ID.</param>
        /// <param name="parent">The parent of the instantiated room.</param>
        /// <param name="assignPosition">If True, the local position of the room is assigned based on the current layout.</param>
        public abstract Room InstantiateRoom(Uid id, Transform parent, bool assignPosition);

        protected void Awake()
        {
            CreateRoomDataDictionary();
        }

        /// <summary>
        /// Creates the room data dictionary based on the current database entries list.
        /// </summary>
        public void CreateRoomDataDictionary()
        {
            RoomDataDictionary.Clear();

            foreach (var entry in Entries)
            {
                RoomDataDictionary.Add(entry.Id, entry.RoomData);
            }
        }

        /// <summary>
        /// Returns the room data source for the specified ID.
        /// </summary>
        /// <param name="id">The room ID.</param>
        public T GetRoomData(int id)
        {
            return RoomDataDictionary[id];
        }
    }
}