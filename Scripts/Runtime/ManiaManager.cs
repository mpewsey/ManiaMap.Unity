using System.Collections.Generic;
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
        /// The current layout.
        /// </summary>
        public Layout Layout { get; private set; }

        /// <summary>
        /// The current layout state.
        /// </summary>
        public LayoutState LayoutState { get; private set; }

        /// <summary>
        /// A dictionary of room adjacencies based on the layout.
        /// </summary>
        public Dictionary<Uid, List<Uid>> RoomAdjacencies { get; private set; }

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

        /// <summary>
        /// Initializes the manager based on a layout.
        /// </summary>
        /// <param name="layout">The layout.</param>
        public void Init(Layout layout)
        {
            Layout = layout;
            LayoutState = new LayoutState(layout);
            RoomAdjacencies = layout.RoomAdjacencies();
        }

        /// <summary>
        /// Initializes the manager based on a layout and layout state.
        /// </summary>
        /// <param name="layout">The layout.</param>
        /// <param name="state">The layout state.</param>
        public void Init(Layout layout, LayoutState state)
        {
            Layout = layout;
            LayoutState = state;
            RoomAdjacencies = layout.RoomAdjacencies();
        }
    }
}