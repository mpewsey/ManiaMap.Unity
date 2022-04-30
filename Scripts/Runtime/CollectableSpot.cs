using UnityEngine;
using UnityEngine.Events;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// Represents a collectable spot.
    /// </summary>
    public class CollectableSpot : MonoBehaviour
    {
        [SerializeField]
        private Cell _cell;
        /// <summary>
        /// The cell in which the collectable is located.
        /// </summary>
        public Cell Cell { get => _cell; set => _cell = value; }

        [SerializeField]
        private UnityEvent _onNoSpotExists = new UnityEvent();
        /// <summary>
        /// The event triggered when a collectable spot does not exist at this location.
        /// </summary>
        public UnityEvent OnNoSpotExists { get => _onNoSpotExists; set => _onNoSpotExists = value; }

        /// <summary>
        /// The collectable ID.
        /// </summary>
        public int Id { get; set; } = int.MinValue;
    }
}