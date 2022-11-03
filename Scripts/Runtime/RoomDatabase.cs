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
        private string[] _searchPaths = new string[] { "Assets" };
        /// <summary>
        /// An array of paths to search for rooms.
        /// </summary>
        public string[] SearchPaths { get => _searchPaths; set => _searchPaths = value; }

        [SerializeField]
        protected List<RoomDatabaseEntry<T>> _entries = new List<RoomDatabaseEntry<T>>();
        /// <summary>
        /// A list of database entries.
        /// </summary>
        public List<RoomDatabaseEntry<T>> Entries { get => _entries; set => _entries = value; }

        /// <summary>
        /// A dictionary of room data sources by room ID.
        /// </summary>
        protected Dictionary<int, T> RoomPrefabDictionary { get; } = new Dictionary<int, T>();

        private void Awake()
        {
            CreateRoomPrefabDictionary();
        }

        /// <summary>
        /// Creates the room data dictionary based on the current database entries list.
        /// </summary>
        public void CreateRoomPrefabDictionary()
        {
            RoomPrefabDictionary.Clear();
            RoomPrefabDictionary.EnsureCapacity(Entries.Count);

            foreach (var entry in Entries)
            {
                RoomPrefabDictionary.Add(entry.Id, entry.RoomPrefab);
            }
        }

        /// <summary>
        /// Returns the room data source for the specified ID.
        /// </summary>
        /// <param name="id">The room ID.</param>
        public T GetRoomPrefab(int id)
        {
            return RoomPrefabDictionary[id];
        }
    }
}