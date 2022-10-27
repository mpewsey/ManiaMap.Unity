using UnityEngine;
using UnityEngine.Events;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// Represents a collectable spot.
    /// </summary>
    [RequireComponent(typeof(CommonEvents))]
    public class CollectableSpot : CellChild
    {
        /// <summary>
        /// An event that passes a CollectableSpot argument.
        /// </summary>
        [System.Serializable]
        public class CollectableSpotEvent : UnityEvent<CollectableSpot> { }

        [SerializeField]
        private int _id;
        /// <summary>
        /// The unique location ID, relative to the cell.
        /// </summary>
        public int Id { get => _id; set => _id = value; }

        [SerializeField]
        private CollectableGroup _group;
        /// <summary>
        /// The collectable group.
        /// </summary>
        public CollectableGroup Group { get => _group; set => _group = value; }

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

        /// <summary>
        /// The assigned collectable ID.
        /// </summary>
        public int CollectableId => RoomData.Collectables.TryGetValue(Id, out int value) ? value : int.MinValue;

        /// <summary>
        /// True if the collectable spot exists.
        /// </summary>
        public bool Exists => RoomData.Collectables.ContainsKey(Id);

        /// <summary>
        /// True if the collectable spot is already acquired.
        /// </summary>
        public bool IsAcquired => RoomState.AcquiredCollectables.Contains(Id);

        private void Start()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes the collectable spot based on the layout and layout state
        /// assigned to the current manager.
        /// </summary>
        private void Initialize()
        {
            if (Exists)
                OnSpotExists.Invoke(this);
            else
                OnNoSpotExists.Invoke(this);
        }

        /// <summary>
        /// If the collectable spot exists and has not already been acquired,
        /// adds it to the current layout state's acquired collectables
        /// and marks it as acquired. Returns true if this action is performed.
        /// </summary>
        public bool Acquire()
        {
            if (Exists && !IsAcquired)
                return RoomState.AcquiredCollectables.Add(Id);
            return false;
        }

        /// <summary>
        /// Auto assigns elements to the collectable spot.
        /// </summary>
        public override void AutoAssign()
        {
            base.AutoAssign();
            Id = Database.AutoAssignId(Id);
        }
    }
}