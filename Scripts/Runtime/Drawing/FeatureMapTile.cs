using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Drawing
{
    /// <summary>
    /// A feature name and the associated map tile.
    /// </summary>
    [System.Serializable]
    public struct FeatureMapTile
    {
        [SerializeField]
        private string _feature;
        /// <summary>
        /// The feature name.
        /// </summary>
        public string Feature { get => _feature; set => _feature = value; }

        [SerializeField]
        private Texture2D _tile;
        /// <summary>
        /// The map tile.
        /// </summary>
        public Texture2D Tile { get => _tile; set => _tile = value; }
    }
}