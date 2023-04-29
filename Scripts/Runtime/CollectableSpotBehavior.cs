using MPewsey.Common.Mathematics;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// Represents a collectable spot.
    /// </summary>
    public class CollectableSpotBehavior : CellChild
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
        private float _weight = 1;
        public float Weight { get => _weight; set => _weight = Mathf.Max(value, 0); }

        [SerializeField]
        private CollectableSpotBehaviorEvent _onInitialize = new CollectableSpotBehaviorEvent();
        /// <summary>
        /// The event invoked after the object is initialized.
        /// </summary>
        public CollectableSpotBehaviorEvent OnInitialize { get => _onInitialize; set => _onInitialize = value; }

        /// <summary>
        /// True if the collectable spot is initialized.
        /// </summary>
        public bool IsInitialized { get; private set; }

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

        private void OnValidate()
        {
            Id = ManiaMapManager.AutoAssignId(Id);
            Weight = Weight;
        }

        private void Awake()
        {
            Room().OnInitialize.AddListener(Initialize);
        }

        private void OnDestroy()
        {
            Room().OnInitialize.RemoveListener(Initialize);
        }

        /// <summary>
        /// Initializes the collectable spot.
        /// </summary>
        private void Initialize()
        {
            if (!IsInitialized)
            {
                IsInitialized = true;
                OnInitialize.Invoke(this);
            }
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

        /// <summary>
        /// Returns new generation data for the collectable spot.
        /// </summary>
        public CollectableSpot CreateData()
        {
            var position = new Vector2DInt(Cell.Index.x, Cell.Index.y);
            return new CollectableSpot(position, Group.Name, Weight);
        }
    }
}