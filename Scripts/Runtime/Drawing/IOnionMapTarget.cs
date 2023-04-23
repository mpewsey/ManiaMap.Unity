using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Drawing
{
    /// <summary>
    /// An interface for an object manipulated by an OnionMap.
    /// </summary>
    public interface IOnionMapTarget
    {
        /// <summary>
        /// Returns a minimum (X) and maximum (Y) layer range vector.
        /// </summary>
        public Vector2 LayerRange();

        /// <summary>
        /// Returns an enumerable of onion map layers for the target.
        /// </summary>
        public IEnumerable<IOnionMapLayer> Layers();
    }
}