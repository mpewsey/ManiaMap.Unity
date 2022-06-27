using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// A room database entry containing a room ID and room data source.
    /// </summary>
    /// <typeparam name="T">The room data source type.</typeparam>
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
        private T _roomData;
        /// <summary>
        /// The room data source.
        /// </summary>
        public T RoomData { get => _roomData; set => _roomData = value; }

        /// <summary>
        /// Initializes a new entry.
        /// </summary>
        /// <param name="id">The room ID.</param>
        /// <param name="roomData">The room data source.</param>
        public RoomDatabaseEntry(int id, T roomData)
        {
            _id = id;
            _roomData = roomData;
        }
    }
}