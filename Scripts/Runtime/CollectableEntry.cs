using System;
using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    [Serializable]
    public struct CollectableEntry : IEquatable<CollectableEntry>
    {
        [SerializeField]
        private Collectable _collectable;
        public Collectable Collectable { get => _collectable; set => _collectable = value; }

        [SerializeField]
        private int _quantity;
        public int Quantity { get => _quantity; set => _quantity = value; }

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
