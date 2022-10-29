using System;
using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Drawing
{
    /// <summary>
    /// A structure containing map tile type flags and an associated color.
    /// </summary>
    public struct MapTileHash : IEquatable<MapTileHash>
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
        public MapTileHash(long flags, Color32 color)
        {
            Flags = flags;
            Color = color;
        }

        public override bool Equals(object obj)
        {
            return obj is MapTileHash hash && Equals(hash);
        }

        public bool Equals(MapTileHash other)
        {
            return Flags == other.Flags &&
                   EqualityComparer<Color32>.Default.Equals(Color, other.Color);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Flags, Color);
        }

        public static bool operator ==(MapTileHash left, MapTileHash right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(MapTileHash left, MapTileHash right)
        {
            return !(left == right);
        }
    }
}
