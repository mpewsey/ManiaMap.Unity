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
                if (_current == null)
                    _current = new GameObject("Mania Map Manager").AddComponent<ManiaMapManager>();
                return _current;
            }
            private set => _current = value;
        }

        /// <summary>
        /// The manager settings.
        /// </summary>
        public ManiaMapSettings Settings { get; private set; }

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
        /// True if the object has been initialized.
        /// </summary>
        public bool IsInitialized { get; private set; }

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
        /// If the ID is less than or equal to zero, returns a random positive integer. Otherwise, returns the ID.
        /// </summary>
        /// <param name="id">The original ID.</param>
        public static int AutoAssignId(int id)
        {
            if (id <= 0)
                return Random.Range(1, int.MaxValue);
            return id;
        }

        /// <summary>
        /// Sets the current layout and layout state to the manager and initializes it.
        /// </summary>
        /// <param name="layout">The layout.</param>
        /// <param name="layoutState">The layout state.</param>
        /// <param name="settings">
        /// The manager settings. If null, the settings will fallback to the first valid of the following:
        /// 
        /// 1. The existing manager settings, if they are not null.
        /// 2. The settings loaded from the resources path, if it exists.
        /// 3. A newly initialized settings object.
        /// </param>
        /// <exception cref="LayoutIsNullException">Raised if the layout is null.</exception>
        /// <exception cref="LayoutStateIsNullException">Raised if the layout state is null.</exception>
        /// <exception cref="System.ArgumentException">Raised if the layout and layout state's ID's do not match.</exception>
        public void Initialize(Layout layout, LayoutState layoutState, ManiaMapSettings settings = null)
        {
            if (layout == null)
                throw new LayoutIsNullException("Layout cannot be null.");
            if (layoutState == null)
                throw new LayoutStateIsNullException("Layout state cannot be null.");
            if (layout.Id != layoutState.Id)
                throw new System.ArgumentException("Layout and layout state ID's do not match.");

            Settings = settings != null ? settings : Settings != null ? Settings : ManiaMapSettings.GetSettings();
            Layout = layout;
            LayoutState = layoutState;
            RoomConnections = layout.GetRoomConnections();
            IsInitialized = true;
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
        public Room GetRoom(Uid id)
        {
            if (Layout == null)
                return null;

            Layout.Rooms.TryGetValue(id, out Room room);
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
    }
}