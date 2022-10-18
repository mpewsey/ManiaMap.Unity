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

        public LayoutData LayoutData { get; set; }
        public ManiaMapSettings Settings { get; set; } = ManiaMapSettings.Create();

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

        public GameObject GetPlayer()
        {
            return GameObject.FindGameObjectWithTag(Settings.PlayerTag);
        }
    }
}