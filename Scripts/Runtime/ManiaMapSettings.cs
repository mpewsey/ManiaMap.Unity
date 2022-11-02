using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// Contains global settings used by Mania Map components.
    /// </summary>
    public class ManiaMapSettings : ScriptableObject
    {
        [SerializeField]
        private string _playerTag = "Player";
        /// <summary>
        /// The tag assigned to the player.
        /// </summary>
        public string PlayerTag { get => _playerTag; set => _playerTag = value; }

        [SerializeField]
        private int _maxClusterDepth = 1;
        /// <summary>
        /// The maximum cluster depth used to calculate room clusters for a layout
        /// when it it assigned to the manager.
        /// </summary>
        public int MaxClusterDepth
        {
            get => _maxClusterDepth;
            set => _maxClusterDepth = Mathf.Max(value, 1);
        }

        private void OnValidate()
        {
            MaxClusterDepth = MaxClusterDepth;
        }

        /// <summary>
        /// Loads the settings from the resources folder. If they do not exist, returns null.
        /// </summary>
        public static ManiaMapSettings Load()
        {
            return Resources.Load<ManiaMapSettings>("ManiaMap/ManiaMapSettings");
        }

        /// <summary>
        /// Creates a new settings instance.
        /// </summary>
        public static ManiaMapSettings Create()
        {
            return CreateInstance<ManiaMapSettings>();
        }

        /// <summary>
        /// Attempts to load the settings from teh resources folder. If they do not exist,
        /// returns a new settings object.
        /// </summary>
        public static ManiaMapSettings GetSettings()
        {
            var settings = Load();

            if (settings != null)
                return settings;

            return Create();
        }
    }
}