using System;
using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Drawing
{
    /// <summary>
    /// A structure containing map tile type flags and an associated color.
    /// </summary>
    public struct MapTileKey : IEquatable<MapTileKey>
    {
        /// <summary>
        /// The feature flags.
        /// </summary>
        public long Flags { get; }

        /// <summary>
        /// The tile color.
        /// </summary>
        public Color32 Color { get; }

        /// <summary>
        /// Initializes a new object.
        /// </summary>
        /// <param name="flags">The feature flags.</param>
        /// <param name="color">The tile color.</param>
        public MapTileKey(long flags, Color32 color)
        {
            Flags = flags;
            Color = color;
        }

        public override bool Equals(object obj)
        {
            return obj is MapTileKey hash && Equals(hash);
        }

        public bool Equals(MapTileKey other)
        {
            return Flags == other.Flags &&
                   EqualityComparer<Color32>.Default.Equals(Color, other.Color);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Flags, Color);
        }

        public static bool operator ==(MapTileKey left, MapTileKey right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(MapTileKey left, MapTileKey right)
        {
            return !(left == right);
        }
    }
}
