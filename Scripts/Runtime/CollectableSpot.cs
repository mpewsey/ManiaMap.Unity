using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// Represents a collectable spot.
    /// </summary>
    public class CollectableSpot : CellChild
    {
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
        private CollectableSpotEvent _onInitialize = new CollectableSpotEvent();
        /// <summary>
        /// The event invoked after the collectable spot is initialized. This occurs on start.
        /// </summary>
        public CollectableSpotEvent OnInitialize { get => _onInitialize; set => _onInitialize = value; }

        /// <summary>
        /// The assigned collectable ID.
        /// </summary>
        public int CollectableId() => RoomLayout().Collectables.TryGetValue(Id, out int value) ? value : int.MinValue;

        /// <summary>
        /// True if the collectable spot exists.
        /// </summary>
        public bool Exists() => RoomLayout().Collectables.ContainsKey(Id);

        /// <summary>
        /// True if the collectable spot is already acquired.
        /// </summary>
        public bool IsAcquired() => RoomState().AcquiredCollectables.Contains(Id);

        private void Awake()
        {
            Room().OnInitialize.AddListener(Initialize);
        }

        private void OnDestroy()
        {
            Room().OnInitialize.RemoveListener(Initialize);
        }

        /// <summary>
        /// Initializes the collectable spot based on the layout and layout state
        /// assigned to the current manager.
        /// </summary>
        private void Initialize()
        {
            OnInitialize.Invoke(this);
        }

        /// <summary>
        /// Returns true if the collectable can be acquired.
        /// </summary>
        public bool CanAcquire()
        {
            return Exists() && !IsAcquired();
        }

        /// <summary>
        /// If the collectable spot exists and has not already been acquired,
        /// adds it to the current layout state's acquired collectables
        /// and marks it as acquired. Returns true if this action is performed.
        /// </summary>
        public bool Acquire()
        {
            if (CanAcquire())
                return RoomState().AcquiredCollectables.Add(Id);
            return false;
        }

        /// <summary>
        /// Auto assigns elements to the collectable spot.
        /// </summary>
        public override void AutoAssign()
        {
            base.AutoAssign();
            Id = ManiaMapManager.AutoAssignId(Id);
        }
    }
}