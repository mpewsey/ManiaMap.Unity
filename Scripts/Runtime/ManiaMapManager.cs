using MPewsey.ManiaMap.Unity.Exceptions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

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

        private ManiaMapSettings _settings;
        /// <summary>
        /// The manager settings.
        /// </summary>
        public ManiaMapSettings Settings
        {
            get
            {
                AssertIsInitialized();
                return _settings;
            }
            private set => _settings = value;
        }

        private Layout _layout;
        /// <summary>
        /// The current layout.
        /// </summary>
        public Layout Layout
        {
            get
            {
                AssertIsInitialized();
                return _layout;
            }
            private set => _layout = value;
        }

        private LayoutState _layoutState;
        /// <summary>
        /// The current layout state.
        /// </summary>
        public LayoutState LayoutState
        {
            get
            {
                AssertIsInitialized();
                return _layoutState;
            }
            private set => _layoutState = value;
        }

        private Dictionary<Uid, List<DoorConnection>> _roomConnections = new Dictionary<Uid, List<DoorConnection>>();
        /// <summary>
        /// A dictionary of door connections by room ID.
        /// </summary>
        private Dictionary<Uid, List<DoorConnection>> RoomConnections
        {
            get
            {
                AssertIsInitialized();
                return _roomConnections;
            }
            set => _roomConnections = value;
        }

        private Dictionary<Uid, HashSet<Uid>> _roomClusters = new Dictionary<Uid, HashSet<Uid>>();
        /// <summary>
        /// A dictionary of room clusters by room ID.
        /// </summary>
        private Dictionary<Uid, HashSet<Uid>> RoomClusters
        {
            get
            {
                AssertIsInitialized();
                return _roomClusters;
            }
            set => _roomClusters = value;
        }

        /// <summary>
        /// A dictionary of room handles by template ID.
        /// </summary>
        private Dictionary<int, RoomHandle> RoomHandles { get; } = new Dictionary<int, RoomHandle>();

        /// <summary>
        /// True if the object has been initialized.
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// The event invoked each time a generation message is logged.
        /// </summary>
        public UnityEvent<string> OnLog { get; } = new UnityEvent<string>();

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Settings = ManiaMapSettings.GetSettings();
            Common.Logging.Logger.AddListener(OnLog.Invoke);
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

            Common.Logging.Logger.RemoveListener(OnLog.Invoke);
            ReleaseRoomHandles();
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

            IsInitialized = true;
            Settings = settings != null ? settings : Settings != null ? Settings : ManiaMapSettings.GetSettings();
            Layout = layout;
            LayoutState = layoutState;
            RoomConnections = layout.GetRoomConnections();
            RoomClusters = layout.FindClusters(Settings.MaxClusterDepth);
            ReleaseRoomHandles();
        }

        /// <summary>
        /// Checks that the manager is initialized and throws an exception if it isn't.
        /// </summary>
        private void AssertIsInitialized()
        {
            if (!IsInitialized)
                throw new ManiaMapManagerNotInitializedException("Mania Map Manager must be initialized prior to accessing initialized members.");
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
        /// Returns the player GameObject based on the player tag assigned to the settings.
        /// </summary>
        public GameObject GetPlayer()
        {
            return GameObject.FindGameObjectWithTag(Settings.PlayerTag);
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
        /// Returns the room cluster by room ID.
        /// </summary>
        /// <param name="id">The room ID.</param>
        public IEnumerable<Uid> GetRoomCluster(Uid id)
        {
            if (RoomClusters.TryGetValue(id, out var cluster))
                return cluster;
            return Enumerable.Empty<Uid>();
        }

        /// <summary>
        /// Releases all room handles.
        /// </summary>
        public void ReleaseRoomHandles()
        {
            foreach (var handle in RoomHandles.Values)
            {
                Addressables.Release(handle.Handle);
            }

            RoomHandles.Clear();
        }

        /// <summary>
        /// Loads the rooms in the room cluster.
        /// </summary>
        /// <param name="id">The room ID.</param>
        /// <param name="database">The addressable database.</param>
        public void LoadRoomCluster(Uid id, RoomAddressableDatabase database)
        {
            IncrementRoomHandles();
            CreateRoomHandles(id, database);
            ReleaseStaleRoomHandles();
        }

        /// <summary>
        /// Creates load handles for the rooms in the cluster and sets the stale count
        /// to zero for any already existing.
        /// </summary>
        /// <param name="id">The room ID.</param>
        /// <param name="database">The addressable database.</param>
        private void CreateRoomHandles(Uid id, RoomAddressableDatabase database)
        {
            foreach (var roomId in GetRoomCluster(id))
            {
                var room = Layout.Rooms[roomId];
                var templateId = room.Template.Id;

                if (!RoomHandles.TryGetValue(templateId, out var handle))
                {
                    var prefab = database.GetPrefab(templateId);
                    handle = new RoomHandle(prefab.LoadAssetAsync());
                    RoomHandles.Add(templateId, handle);
                }

                handle.StaleCount = 0;
            }
        }

        /// <summary>
        /// Increments the stale counts for the room handles.
        /// </summary>
        private void IncrementRoomHandles()
        {
            foreach (var handle in RoomHandles.Values)
            {
                handle.StaleCount++;
            }
        }

        /// <summary>
        /// Releases any room handles that have exceeded the maximum stale count.
        /// </summary>
        private void ReleaseStaleRoomHandles()
        {
            var maxStaleCount = Settings.MaxStaleCount;

            foreach (var id in RoomHandles.Keys.ToList())
            {
                var handle = RoomHandles[id];

                if (handle.StaleCount > maxStaleCount)
                {
                    Addressables.Release(handle.Handle);
                    RoomHandles.Remove(id);
                }
            }
        }
    }
}