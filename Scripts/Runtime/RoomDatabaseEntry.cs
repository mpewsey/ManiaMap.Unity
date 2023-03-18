using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// A room database entry containing a room ID and room data source.
    /// </summary>
    /// <typeparam name="T">The room prefab type.</typeparam>
    [System.Serializable]
    public struct RoomDatabaseEntry<T>
    {
        [SerializeField]
        private int _id;
        /// <summary>
        /// The room ID.
        /// </summary>
        public int Id { get => _id; set => _id = value; }

        [SerializeField]
        private T _prefab;
        /// <summary>
        /// The room prefab.
        /// </summary>
        public T Prefab { get => _prefab; set => _prefab = value; }

        /// <summary>
        /// Initializes a new entry.
        /// </summary>
        /// <param name="id">The room ID.</param>
        /// <param name="prefab">The room prefab.</param>
        public RoomDatabaseEntry(int id, T prefab)
        {
            _id = id;
            _prefab = prefab;
        }
    }
}