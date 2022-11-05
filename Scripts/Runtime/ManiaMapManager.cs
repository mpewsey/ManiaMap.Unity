using MPewsey.ManiaMap.Unity.Exceptions;
using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// A manager for maintaining the current map data and state.
    /// </summary>
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

        /// <summary>
        /// The manager settings.
        /// </summary>
        public ManiaMapSettings Settings { get; set; }

        /// <summary>
        /// The current layout.
        /// </summary>
        public Layout Layout { get; private set; }

        /// <summary>
        /// The current layout state.
        /// </summary>
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
            DontDestroyOnLoad(gameObject);
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

        /// <summary>
        /// Sets the current layout and layout state to the manager.
        /// </summary>
        /// <param name="layout">The layout.</param>
        /// <param name="layoutState">The layout state.</param>
        /// <exception cref="LayoutIsNullException">Raised if the layout is null.</exception>
        /// <exception cref="LayoutStateIsNullException">Raised if the layout state is null.</exception>
        /// <exception cref="System.ArgumentException">Raised if the layout and layout state's ID's do not match.</exception>
        public void SetLayout(Layout layout, LayoutState layoutState)
        {
            if (layout == null)
                throw new LayoutIsNullException("Layout cannot be null.");
            if (layoutState == null)
                throw new LayoutStateIsNullException("Layout state cannot be null.");
            if (layout.Id != layoutState.Id)
                throw new System.ArgumentException("Layout and layout state ID's do not match.");

            Layout = layout;
            LayoutState = layoutState;
            RoomConnections = layout.GetRoomConnections();
            RoomClusters = layout.FindClusters(Settings.MaxClusterDepth);
        }

        /// <summary>
        /// Clears the current layout from the manager.
        /// </summary>
        public void ClearLayout()
        {
            Layout = null;
            LayoutState = null;
            RoomConnections = new Dictionary<Uid, List<DoorConnection>>();
            RoomClusters = new Dictionary<Uid, HashSet<Uid>>();
        }

        /// <summary>
        /// Validates whether a layout and layout state are assigned. Throws an exception if they are not.
        /// </summary>
        /// <exception cref="LayoutIsNullException">Raised if the layout is null.</exception>
        /// <exception cref="LayoutStateIsNullException">Raised if the layout state is null.</exception>
        public void Validate()
        {
            if (Layout == null)
                throw new LayoutIsNullException("Layout not set to Mania Map Manager.");
            if (LayoutState == null)
                throw new LayoutStateIsNullException("Layout state not set to Mania Map Manager.");
        }

        /// <summary>
        /// Returns true if a layout and layout state are assigned.
        /// </summary>
        public bool IsValid()
        {
            return Layout != null && LayoutState != null;
        }

        /// <summary>
        /// Returns the player GameObject based on the player tag assigned to the settings.
        /// </summary>
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