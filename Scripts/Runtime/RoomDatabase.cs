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
        protected string[] _searchPaths = new string[] { "Assets" };
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

        protected Dictionary<int, T> _prefabDictionary = new Dictionary<int, T>();
        /// <summary>
        /// A dictionary of room data sources by room ID.
        /// </summary>
        protected Dictionary<int, T> PrefabDictionary
        {
            get
            {
                EnsureIsInitialized();
                return _prefabDictionary;
            }
            set => _prefabDictionary = value;
        }

        /// <summary>
        /// True if the prefab dictionary has been initialized.
        /// </summary>
        protected bool IsInitialized { get; set; }

        protected void OnDisable()
        {
            IsInitialized = false;
        }

        /// <summary>
        /// Initializes the database by creating the prefab dictionary.
        /// </summary>
        public void Initialize()
        {
            PrefabDictionary = CreatePrefabDictionary();
            IsInitialized = true;
        }

        /// <summary>
        /// If the database has not been initialized, initializes it.
        /// </summary>
        protected void EnsureIsInitialized()
        {
            if (!IsInitialized)
                Initialize();
        }

        /// <summary>
        /// Creates the room data dictionary based on the current database entries list.
        /// </summary>
        protected Dictionary<int, T> CreatePrefabDictionary()
        {
            var dict = new Dictionary<int, T>(Entries.Count);

            foreach (var entry in Entries)
            {
                dict.Add(entry.Id, entry.Prefab);
            }

            return dict;
        }

        /// <summary>
        /// Adds an entry to the database.
        /// </summary>
        /// <param name="id">The room ID.</param>
        /// <param name="prefab">The room prefab.</param>
        public void AddEntry(int id, T prefab)
        {
            PrefabDictionary.Add(id, prefab);
            Entries.Add(new RoomDatabaseEntry<T>(id, prefab));
        }

        /// <summary>
        /// Returns the room prefab for the specified ID.
        /// </summary>
        /// <param name="id">The room ID.</param>
        public T GetPrefab(int id)
        {
            return PrefabDictionary[id];
        }
    }
}