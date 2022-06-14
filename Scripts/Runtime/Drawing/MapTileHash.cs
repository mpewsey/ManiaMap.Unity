using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Drawing
{
    public struct MapTileHash : IEquatable<MapTileHash>
    {
        public int Types { get; }
        public Color32 Color { get; }

        public MapTileHash(int types, Color32 color)
        {
            Types = types;
            Color = color;
        }

        public override bool Equals(object obj)
        {
            return obj is MapTileHash hash && Equals(hash);
        }

        public bool Equals(MapTileHash other)
        {
            return Types == other.Types &&
                   EqualityComparer<Color32>.Default.Equals(Color, other.Color);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Types, Color);
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
