using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// A manager for maintaining the current map data and state.
    /// </summary>
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
        /// The current room.
        /// </summary>
        public Uid CurrentRoom { get; set; } = new Uid(0, 0, -1);

        /// <summary>
        /// The current layout.
        /// </summary>
        public Layout Layout { get; set; }

        /// <summary>
        /// The current layout state.
        /// </summary>
        public LayoutState LayoutState { get; set; }

        /// <summary>
        /// Returns the first manager component in the scene. If a manager does
        /// not exist, returns null.
        /// </summary>
        private static ManiaManager FindManager()
        {
            return GameObject.FindObjectOfType<ManiaManager>();
        }

        /// <summary>
        /// Creates a new manager and returns it.
        /// </summary>
        private static ManiaManager CreateManager()
        {
            var obj = new GameObject("Mania Manager");
            return obj.AddComponent<ManiaManager>();
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
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

        public int GetCollectableId(Vector2Int index)
        {
            if (Layout.Rooms.TryGetValue(CurrentRoom, out Room room)
                && room.Collectables.TryGetValue(new Vector2DInt(index.x, index.y), out int id))
                return id;
            return int.MinValue;
        }

        public bool CollectableAcquired(Vector2Int index)
        {
            return LayoutState.RoomStates.TryGetValue(CurrentRoom, out RoomState state)
                && state.CollectedIndexes.Contains(new Vector2DInt(index.x, index.y));
        }

        public bool CollectableExists(Vector2Int index)
        {
            return Layout.Rooms.TryGetValue(CurrentRoom, out Room room)
                && room.Collectables.ContainsKey(new Vector2DInt(index.x, index.y));
        }
    }
}