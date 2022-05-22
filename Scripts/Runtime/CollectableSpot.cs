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
        private CollectableGroup _group;
        /// <summary>
        /// The collectable group.
        /// </summary>
        public CollectableGroup Group { get => _group; set => _group = value; }

        [SerializeField]
        private UnityEvent _onSpotExists = new UnityEvent();
        /// <summary>
        /// The event triggered when a collectable spot exists at this location.
        /// </summary>
        public UnityEvent OnSpotExists { get => _onSpotExists; set => _onSpotExists = value; }

        [SerializeField]
        private UnityEvent _onNoSpotExists = new UnityEvent();
        /// <summary>
        /// The event triggered when a collectable spot does not exist at this location.
        /// </summary>
        public UnityEvent OnNoSpotExists { get => _onNoSpotExists; set => _onNoSpotExists = value; }

        /// <summary>
        /// The collectable ID.
        /// </summary>
        public int CollectableId { get; set; } = int.MinValue;

        /// <summary>
        /// True if the collectable is acquired.
        /// </summary>
        public bool IsAcquired { get; set; } = true;

        /// <summary>
        /// True if the collectable spot exists.
        /// </summary>
        public bool Exists { get; set; }

        private void Awake()
        {
            Init();
        }

        /// <summary>
        /// Initializes the collectable spot based on the current room.
        /// </summary>
        private void Init()
        {
            var manager = ManiaManager.Current;
            CollectableId = manager.GetCollectableId(Cell.Index);
            IsAcquired = manager.CollectableAcquired(Cell.Index);
            Exists = manager.CollectableExists(Cell.Index);

            if (Exists)
                OnSpotExists.Invoke();
            else
                OnNoSpotExists.Invoke();
        }
    }
}