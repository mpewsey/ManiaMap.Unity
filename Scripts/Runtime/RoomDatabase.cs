using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// A base class for creating a room data source database.
    /// </summary>
    /// <typeparam name="T">The room prefab type.</typeparam>
    public abstract class RoomDatabase<T> : ScriptableObject
    {
        [SerializeField]
        private string[] _searchPaths = new string[] { "Assets" };
        /// <summary>
        /// An array of paths to search for rooms.
        /// </summary>
        public string[] SearchPaths { get => _searchPaths; set => _searchPaths = value; }

        [SerializeField]
        protected List<Entry<T>> _entries = new List<Entry<T>>();
        /// <summary>
        /// A list of database entries.
        /// </summary>
        public List<Entry<T>> Entries { get => _entries; set => _entries = value; }

        /// <summary>
        /// A dictionary of room data sources by room ID.
        /// </summary>
        protected Dictionary<int, T> RoomPrefabDictionary { get; } = new Dictionary<int, T>();

        private void OnEnable()
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
                RoomPrefabDictionary.Add(entry.Id, entry.Prefab);
            }
        }

        /// <summary>
        /// Adds an entry to the database.
        /// </summary>
        /// <param name="id">The room ID.</param>
        /// <param name="prefab">The room prefab.</param>
        public void AddEntry(int id, T prefab)
        {
            RoomPrefabDictionary.Add(id, prefab);
            Entries.Add(new Entry<T>(id, prefab));
        }

        /// <summary>
        /// Returns the room data source for the specified ID.
        /// </summary>
        /// <param name="id">The room ID.</param>
        public T GetRoomPrefab(int id)
        {
            return RoomPrefabDictionary[id];
        }

        /// <summary>
        /// A room database entry containing a room ID and room data source.
        /// </summary>
        /// <typeparam name="U">The room prefab type.</typeparam>
        [System.Serializable]
        public struct Entry<U>
        {
            [SerializeField]
            private int _id;
            /// <summary>
            /// The room ID.
            /// </summary>
            public int Id { get => _id; set => _id = value; }

            [SerializeField]
            private U _prefab;
            /// <summary>
            /// The room prefab.
            /// </summary>
            public U Prefab { get => _prefab; set => _prefab = value; }

            /// <summary>
            /// Initializes a new entry.
            /// </summary>
            /// <param name="id">The room ID.</param>
            /// <param name="prefab">The room prefab.</param>
            public Entry(int id, U prefab)
            {
                _id = id;
                _prefab = prefab;
            }
        }
    }
}