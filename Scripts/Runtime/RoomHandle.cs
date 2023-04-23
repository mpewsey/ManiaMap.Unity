using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// A class for managing a room asynchronous load handle.
    /// </summary>
    public class RoomHandle
    {
        /// <summary>
        /// The room load handle.
        /// </summary>
        public AsyncOperationHandle<GameObject> Handle { get; }

        private int _staleCount;
        /// <summary>
        /// A counter indicating how stale the loaded handle is.
        /// </summary>
        public int StaleCount { get => _staleCount; set => _staleCount = Mathf.Max(value, 0); }

        /// <summary>
        /// Initializes a new handle.
        /// </summary>
        /// <param name="handle">The room handle.</param>
        public RoomHandle(AsyncOperationHandle<GameObject> handle)
        {
            Handle = handle;
        }
    }
}