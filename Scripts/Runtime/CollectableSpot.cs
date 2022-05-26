using UnityEngine;
using UnityEngine.Events;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// Represents a collectable spot.
    /// </summary>
    public class CollectableSpot : MonoBehaviour
    {
        [System.Serializable]
        public class CollectableSpotEvent : UnityEvent<CollectableSpot> { }

        [SerializeField]
        private int _id;
        /// <summary>
        /// The unique location ID, relative to the cell.
        /// </summary>
        public int Id { get => _id; set => _id = value; }

        [SerializeField]
        private Cell _cell;
        /// <summary>
        /// The cell in which the collectable is located.
        /// </summary>
        public Cell Cell { get => _cell; set => _cell = value; }

        [SerializeField]
        private CollectableGroup _group;
        /// <summary>
        /// The collectable group.
        /// </summary>
        public CollectableGroup Group { get => _group; set => _group = value; }

        [SerializeField]
        private CollectableSpotEvent _onAcquisition = new CollectableSpotEvent();
        /// <summary>
        /// The event triggered when the collectable spot is acquired.
        /// The collectable spot is passed to the event.
        /// </summary>
        public CollectableSpotEvent OnAcquisition { get => _onAcquisition; set => _onAcquisition = value; }

        [SerializeField]
        private CollectableSpotEvent _onSpotExists = new CollectableSpotEvent();
        /// <summary>
        /// The event triggered when a collectable spot exists at this location.
        /// The collectable spot is passed to the event.
        /// </summary>
        public CollectableSpotEvent OnSpotExists { get => _onSpotExists; set => _onSpotExists = value; }

        [SerializeField]
        private CollectableSpotEvent _onNoSpotExists = new CollectableSpotEvent();
        /// <summary>
        /// The event triggered when a collectable spot does not exist at this location.
        /// The collectable spot is passed to the event.
        /// </summary>
        public CollectableSpotEvent OnNoSpotExists { get => _onNoSpotExists; set => _onNoSpotExists = value; }

        public Cell FindClosestCell()
        {
            Cell closest = null;
            var minDistance = float.PositiveInfinity;

            foreach (var cell in FindObjectsOfType<Cell>())
            {
                if (cell.IsEmpty)
                    continue;

                var delta = cell.transform.position - transform.position;
                var distance = delta.sqrMagnitude;

                if (distance < minDistance)
                {
                    closest = cell;
                    minDistance = distance;
                }
            }

            return closest;
        }
    }
}