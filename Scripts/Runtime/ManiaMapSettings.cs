using UnityEngine;

namespace MPewsey.ManiaMapUnity
{
    /// <summary>
    /// Contains global settings used by Mania Map components.
    /// </summary>
    public class ManiaMapSettings : ScriptableObject
    {
        [SerializeField]
        private LayerMask _cellLayer;
        public LayerMask CellLayer { get => _cellLayer; set => _cellLayer = value; }

        [SerializeField]
        private LayerMask _triggeringLayer;
        public LayerMask TriggeringLayer { get => _triggeringLayer; set => _triggeringLayer = value; }

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