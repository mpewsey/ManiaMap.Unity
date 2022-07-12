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

                _current = FindObjectOfType<ManiaManager>();

                if (_current != null)
                    return _current;

                _current = new GameObject("Mania Manager").AddComponent<ManiaManager>();
                return _current;
            }
            private set => _current = value;
        }

        public LayoutData LayoutData { get; set; }

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
    }
}