using System;
using UnityEngine;

namespace MPewsey.ManiaMapUnity
{
    /// <summary>
    /// A structure containing a collectable and a quantity.
    /// </summary>
    [Serializable]
    public struct CollectableGroupEntry
    {
        [SerializeField]
        private CollectableResource _collectable;
        /// <summary>
        /// The collectable.
        /// </summary>
        public CollectableResource Collectable { get => _collectable; set => _collectable = value; }

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
        public CollectableGroupEntry(CollectableResource collectable, int quantity)
        {
            _collectable = collectable;
            _quantity = quantity;
        }
    }
}
