using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Drawing
{
    /// <summary>
    /// An interface for a layer of an onion map.
    /// </summary>
    public interface IOnionMapLayer
    {
        /// <summary>
        /// The layer position.
        /// </summary>
        public float Position();

        /// <summary>
        /// Applies the specified color to the layer.
        /// </summary>
        /// <param name="color">The color.</param>
        public void Apply(Color color);
    }
}