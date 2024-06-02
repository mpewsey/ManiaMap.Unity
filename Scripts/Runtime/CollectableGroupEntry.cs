using System;
using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMapUnity
{
    /// <summary>
    /// A structure containing a collectable and a quantity.
    /// </summary>
    [Serializable]
    public struct CollectableGroupEntry : IEquatable<CollectableGroupEntry>
    {
        [SerializeField]
        private CollectableObject _collectable;
        /// <summary>
        /// The collectable.
        /// </summary>
        public CollectableObject Collectable { get => _collectable; set => _collectable = value; }

        [SerializeField]
        private int _quantity;
        /// <summary>
        /// The quantity.
        /// </summary>
        public int Quantity { get => _quantity; set => _quantity = value; }

        /// <summary>
        /// Initializes a new entry.
        /// </summary>
        /// <param name="collectable">The collectable.</param>
        /// <param name="quantity">The quantity.</param>
        public CollectableGroupEntry(CollectableObject collectable, int quantity)
        {
            _collectable = collectable;
            _quantity = quantity;
        }

        public override bool Equals(object obj)
        {
            return obj is CollectableGroupEntry entry && Equals(entry);
        }

        public bool Equals(CollectableGroupEntry other)
        {
            return EqualityComparer<CollectableObject>.Default.Equals(Collectable, other.Collectable) &&
                   Quantity == other.Quantity;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Collectable, Quantity);
        }

        public static bool operator ==(CollectableGroupEntry left, CollectableGroupEntry right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CollectableGroupEntry left, CollectableGroupEntry right)
        {
            return !(left == right);
        }
    }
}
