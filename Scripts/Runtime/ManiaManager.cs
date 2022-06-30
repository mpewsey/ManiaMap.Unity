using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// A manager for maintaining the current map data and state.
    /// </summary>
    [RequireComponent(typeof(DontDestroyOnLoad))]
    public class ManiaManager : MonoBehaviour
    {
        private static ManiaManager _current;
        /// <summary>
        /// The current manager.
        /// </summary>
        public static ManiaManager Current
        {
            get
            {
                if (_current != null)
                    return _current;

                _current = FindManager();

                if (_current != null)
                    return _current;

                _current = CreateManager();
                return _current;
            }
            private set => _current = value;
        }

        /// <summary>
        /// The current layout.
        /// </summary>
        public Layout Layout { get; private set; }

        /// <summary>
        /// The current layout state.
        /// </summary>
        public LayoutState LayoutState { get; private set; }

        /// <summary>
        /// A dictionary of adjacent rooms by room ID.
        /// </summary>
        private Dictionary<Uid, List<Uid>> RoomAdjacencies { get; set; } = new Dictionary<Uid, List<Uid>>();

        /// <summary>
        /// A dictionary of room clusters by room ID.
        /// </summary>
        private Dictionary<Uid, HashSet<Uid>> RoomClusters { get; set; } = new Dictionary<Uid, HashSet<Uid>>();

        /// <summary>
        /// Returns the first manager component in the scene. If a manager does
        /// not exist, returns null.
        /// </summary>
        private static ManiaManager FindManager()
        {
            return FindObjectOfType<ManiaManager>();
        }

        /// <summary>
        /// Creates a new manager and returns it.
        /// </summary>
        private static ManiaManager CreateManager()
        {
            var obj = new GameObject("Mania Manager");
            return obj.AddComponent<ManiaManager>();
        }

        private void Start()
        {
            if (Current != this)
                Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (Current == this)
                Current = null;
        }

        /// <summary>
        /// Initializes the manager based on a layout and layout state.
        /// </summary>
        /// <param name="layout">The layout.</param>
        /// <param name="state">The layout state.</param>
        /// <param name="maxClusterDepth">The maximum depth for calculating room clusters.</param>
        public void Init(Layout layout, LayoutState state, int maxClusterDepth = 1)
        {
            Layout = layout;
            LayoutState = state;
            RoomAdjacencies = layout.RoomAdjacencies();
            RoomClusters = layout.FindClusters(maxClusterDepth);
        }

        /// <summary>
        /// Returns a list of adjacent room ID's.
        /// </summary>
        /// <param name="id">The room ID for which adjacent rooms will be returned.</param>
        public IReadOnlyList<Uid> GetAdjacentRooms(Uid id)
        {
            if (RoomAdjacencies.TryGetValue(id, out List<Uid> rooms))
                return rooms;
            return System.Array.Empty<Uid>();
        }

        /// <summary>
        /// Returns an enumerable of rooms belonging to the room cluster.
        /// </summary>
        /// <param name="id">The room ID for which the cluster will be returned.</param>
        public IEnumerable<Uid> GetRoomCluster(Uid id)
        {
            if (RoomClusters.TryGetValue(id, out HashSet<Uid> cluster))
                return cluster;
            return System.Array.Empty<Uid>();
        }
    }
}