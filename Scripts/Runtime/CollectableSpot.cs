using UnityEngine;
using UnityEngine.Events;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// Represents a collectable spot.
    /// </summary>
    public class CollectableSpot : MonoBehaviour
    {
        /// <summary>
        /// The cell in which the collectable is located.
        /// </summary>
        [field: SerializeField]
        public Cell Cell { get; set; }

        /// <summary>
        /// The event triggered when a collectable spot does not exist at this location.
        /// </summary>
        [field: SerializeField]
        public UnityEvent OnNoSpotExists { get; set; }

        /// <summary>
        /// The collectable ID.
        /// </summary>
        public int Id { get; set; } = int.MinValue;
    }
}