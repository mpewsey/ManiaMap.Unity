using System;
using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// A structure containing a collectable and a quantity.
    /// </summary>
    [Serializable]
    public struct CollectableEntry : IEquatable<CollectableEntry>
    {
        [SerializeField]
        private Collectable _collectable;
        /// <summary>
        /// The collectable.
        /// </summary>
        public Collectable Collectable { get => _collectable; set => _collectable = value; }

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
        public CollectableEntry(Collectable collectable, int quantity)
        {
            _collectable = collectable;
            _quantity = quantity;
        }

        public override bool Equals(object obj)
        {
            return obj is CollectableEntry entry && Equals(entry);
        }

        public bool Equals(CollectableEntry other)
        {
            return EqualityComparer<Collectable>.Default.Equals(Collectable, other.Collectable) &&
                   Quantity == other.Quantity;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Collectable, Quantity);
        }

        public static bool operator ==(CollectableEntry left, CollectableEntry right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CollectableEntry left, CollectableEntry right)
        {
            return !(left == right);
        }
    }
}
