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
        private List<RoomDatabaseEntry<T>> _entries = new List<RoomDatabaseEntry<T>>();
        /// <summary>
        /// A list of database entries.
        /// </summary>
        public List<RoomDatabaseEntry<T>> Entries { get => _entries; set => _entries = value; }

        /// <summary>
        /// A dictionary of room data sources by room ID.
        /// </summary>
        private Dictionary<int, T> RoomDataDictionary { get; } = new Dictionary<int, T>();

        private void Awake()
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