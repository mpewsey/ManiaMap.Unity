using System;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Drawing
{
    /// <summary>
    /// A structure for specifying the padding around an object.
    /// </summary>
    [Serializable]
    public struct Padding : IEquatable<Padding>
    {
        [SerializeField]
        private int _top;
        /// <summary>
        /// The padding from the top.
        /// </summary>
        public int Top { get => _top; }

        [SerializeField]
        private int _bottom;
        /// <summary>
        /// The padding from the bottom.
        /// </summary>
        public int Bottom { get => _bottom; }

        [SerializeField]
        private int _left;
        /// <summary>
        /// The padding from the left.
        /// </summary>
        public int Left { get => _left; }

        [SerializeField]
        private int _right;
        /// <summary>
        /// The padding from the right.
        /// </summary>
        public int Right { get => _right; }

        /// <summary>
        /// Initializes equal directional padding.
        /// </summary>
        /// <param name="padding">The padding in all directions.</param>
        public Padding(int padding)
        {
            _top = padding;
            _bottom = padding;
            _left = padding;
            _right = padding;
        }

        /// <summary>
        /// Initializes padding in each direction.
        /// </summary>
        /// <param name="left">The padding to the left.</param>
        /// <param name="top">The padding to the top.</param>
        /// <param name="right">The padding to the right.</param>
        /// <param name="bottom">The padding to the bottom.</param>
        public Padding(int left, int top, int right, int bottom)
        {
            _left = left;
            _top = top;
            _right = right;
            _bottom = bottom;
        }

        public override string ToString()
        {
            return $"Padding(Left = {Left}, Top = {Top}, Right = {Right}, Bottom = {Bottom})";
        }

        public override bool Equals(object obj)
        {
            return obj is Padding padding && Equals(padding);
        }

        public bool Equals(Padding other)
        {
            return Top == other.Top &&
                   Bottom == other.Bottom &&
                   Left == other.Left &&
                   Right == other.Right;
        }

        public override int GetHashCode()
        {
            int hashCode = 920856443;
            hashCode = hashCode * -1521134295 + Top.GetHashCode();
            hashCode = hashCode * -1521134295 + Bottom.GetHashCode();
            hashCode = hashCode * -1521134295 + Left.GetHashCode();
            hashCode = hashCode * -1521134295 + Right.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(Padding left, Padding right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Padding left, Padding right)
        {
            return !(left == right);
        }
    }
}
