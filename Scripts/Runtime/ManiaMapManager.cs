using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// A manager for maintaining the current map data and state.
    /// </summary>
    [RequireComponent(typeof(DontDestroyOnLoad))]
    public class ManiaMapManager : MonoBehaviour
    {
        private static ManiaMapManager _current;
        /// <summary>
        /// The current manager.
        /// </summary>
        public static ManiaMapManager Current
        {
            get
            {
                if (_current != null)
                    return _current;

                _current = new GameObject("Mania Map Manager").AddComponent<ManiaMapManager>();
                return _current;
            }
            private set => _current = value;
        }

        /// <summary>
        /// Validates the current manager and returns it.
        /// </summary>
        public static ManiaMapManager CurrentValidated
        {
            get
            {
                var current = Current;
                current.Validate();
                return current;
            }
        }

        public ManiaMapSettings Settings { get; set; }
        public Layout Layout { get; private set; }
        public LayoutState LayoutState { get; private set; }

        /// <summary>
        /// A dictionary of door connections by room ID.
        /// </summary>
        private Dictionary<Uid, List<DoorConnection>> RoomConnections { get; set; } = new Dictionary<Uid, List<DoorConnection>>();

        /// <summary>
        /// A dictionary of room clusters by room ID.
        /// </summary>
        private Dictionary<Uid, HashSet<Uid>> RoomClusters { get; set; } = new Dictionary<Uid, HashSet<Uid>>();

        private void Awake()
        {
            Settings = ManiaMapSettings.GetSettings();
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

        public void SetLayout(Layout layout, LayoutState layoutState)
        {
            if (layout == null)
                throw new System.ArgumentException("Layout cannot be null.");
            if (layoutState == null)
                throw new System.ArgumentException("Layout state cannot be null.");
            if (layout.Id != layoutState.Id)
                throw new System.ArgumentException("Layout and layout state ID's do not match.");

            Layout = layout;
            LayoutState = layoutState;
            RoomConnections = layout.GetRoomConnections();
            RoomClusters = layout.FindClusters(Settings.MaxClusterDepth);
        }

        public void ClearLayout()
        {
            Layout = null;
            LayoutState = null;
            RoomConnections = new Dictionary<Uid, List<DoorConnection>>();
            RoomClusters = new Dictionary<Uid, HashSet<Uid>>();
        }

        public void Validate()
        {
            if (Layout == null || LayoutState == null)
                throw new System.ArgumentException("Layout not set to Mania Map Manager.");
        }

        public bool IsValid()
        {
            return Layout != null && LayoutState != null;
        }

        public GameObject GetPlayer()
        {
            return GameObject.FindGameObjectWithTag(Settings.PlayerTag);
        }

        /// <summary>
        /// Returns the room in the layout corresponding to the specified ID.
        /// If the ID does not exist, returns null.
        /// </summary>
        /// <param name="id">The room ID.</param>
        public ManiaMap.Room GetRoom(Uid id)
        {
            if (Layout == null)
                return null;

            Layout.Rooms.TryGetValue(id, out ManiaMap.Room room);
            return room;
        }

        /// <summary>
        /// Returns the room state in the layout corresponding to the specified ID.
        /// If the ID does not exist, returns null.
        /// </summary>
        /// <param name="id">The room ID.</param>
        public RoomState GetRoomState(Uid id)
        {
            if (LayoutState == null)
                return null;

            LayoutState.RoomStates.TryGetValue(id, out RoomState state);
            return state;
        }

        /// <summary>
        /// Returns a list of door connections by room ID.
        /// </summary>
        /// <param name="id">The room ID.</param>
        public IReadOnlyList<DoorConnection> GetDoorConnections(Uid id)
        {
            if (RoomConnections.TryGetValue(id, out var connections))
                return connections;
            return System.Array.Empty<DoorConnection>();
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