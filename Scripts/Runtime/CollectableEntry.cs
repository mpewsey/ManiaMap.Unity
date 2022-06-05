using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    [System.Serializable]
    public struct CollectableEntry
    {
        [SerializeField]
        private Collectable _collectable;
        public Collectable Collectable { get => _collectable; set => _collectable = value; }

        [SerializeField]
        private int _quantity;
        public int Quantity { get => _quantity; set => _quantity = value; }
    }
}
