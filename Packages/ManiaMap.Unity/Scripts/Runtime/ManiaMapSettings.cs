using UnityEngine;

namespace MPewsey.ManiaMapUnity
{
    /// <summary>
    /// Contains settings used by Mania Map components.
    /// </summary>
    public class ManiaMapSettings : ScriptableObject
    {
        [SerializeField]
        private int _cellLayer;
        /// <summary>
        /// The physics layer assigned to room cell area triggers.
        /// </summary>
        public int CellLayer { get => _cellLayer; set => _cellLayer = value; }

        [SerializeField]
        private LayerMask _triggeringLayers;
        /// <summary>
        /// The physics layers that trigger room cell area triggers.
        /// </summary>
        public LayerMask TriggeringLayers { get => _triggeringLayers; set => _triggeringLayers = value; }

        /// <summary>
        /// Loads the settings from ManiaMap/ManiaMapSettings in a Resource folder if it exists.
        /// Otherwise, returns a new settings instance.
        /// </summary>
        public static ManiaMapSettings LoadSettings()
        {
            var settings = Resources.Load<ManiaMapSettings>("ManiaMap/ManiaMapSettings");

            if (settings != null)
                return settings;

            return CreateInstance<ManiaMapSettings>();
        }
    }
}