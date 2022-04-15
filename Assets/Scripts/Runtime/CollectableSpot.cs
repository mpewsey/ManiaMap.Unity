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
        /// The collectable ID.
        /// </summary>
        public int Id { get; set; } = int.MinValue;

        /// <summary>
        /// The collectable cell position.
        /// </summary>
        [field: SerializeField]
        public Vector2 Position { get; set; }

        /// <summary>
        /// The event triggered when a collectable spot does not exist at this location.
        /// </summary>
        [field: SerializeField]
        public UnityEvent OnNoSpotExists { get; set; }
    }
}