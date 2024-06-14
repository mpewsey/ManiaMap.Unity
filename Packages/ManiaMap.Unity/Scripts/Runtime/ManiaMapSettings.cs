using UnityEngine;

namespace MPewsey.ManiaMapUnity
{
    /// <summary>
    /// Contains global settings used by Mania Map components.
    /// </summary>
    public class ManiaMapSettings : ScriptableObject
    {
        [SerializeField]
        private int _cellLayer;
        public int CellLayer { get => _cellLayer; set => _cellLayer = value; }

        [SerializeField]
        private LayerMask _triggeringLayers;
        public LayerMask TriggeringLayers { get => _triggeringLayers; set => _triggeringLayers = value; }

        /// <summary>
        /// Attempts to load the settings from teh resources folder. If they do not exist,
        /// returns a new settings object.
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