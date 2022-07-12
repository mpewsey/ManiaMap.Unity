using UnityEngine;
using UnityEngine.Events;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// Represents a collectable spot.
    /// </summary>
    [RequireComponent(typeof(CommonEvents))]
    public class CollectableSpot : MonoBehaviour
    {
        /// <summary>
        /// An event that passes a CollectableSpot argument.
        /// </summary>
        [System.Serializable]
        public class CollectableSpotEvent : UnityEvent<CollectableSpot> { }

        [SerializeField]
        private bool _autoAssignCell = true;
        /// <summary>
        /// If true, the cell will be automatically assigned to the cell when update and save operations
        /// are performed.
        /// </summary>
        public bool AutoAssignCell { get => _autoAssignCell; set => _autoAssignCell = value; }

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
        public int CollectableId { get; private set; } = int.MinValue;

        /// <summary>
        /// True if the collectable spot exists.
        /// </summary>
        public bool Exists { get; private set; }

        /// <summary>
        /// True if the collectable spot is already acquired.
        /// </summary>
        public bool IsAcquired { get; private set; } = true;

        /// <summary>
        /// Returns the room ID.
        /// </summary>
        public Uid RoomId { get => Cell.Room.RoomId; }

        private void Start()
        {
            Init();
        }

        /// <summary>
        /// Initializes the collectable spot based on the layout and layout state
        /// assigned to the current manager.
        /// </summary>
        private void Init()
        {
            var data = ManiaManager.Current.LayoutData;
            var room = data?.GetRoom(RoomId);
            var state = data?.GetRoomState(RoomId);

            if (room != null && state != null)
            {
                IsAcquired = state.AcquiredCollectables.Contains(Id);
                Exists = room.Collectables.TryGetValue(Id, out int collectableId);
                CollectableId = Exists ? collectableId : int.MinValue;
            }

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
            if (!IsAcquired && Exists)
            {
                var data = ManiaManager.Current.LayoutData;
                var state = data.GetRoomState(RoomId);
                var acquired = state.AcquiredCollectables.Add(Id);
                IsAcquired = true;
                return acquired;
            }

            return false;
        }

        /// <summary>
        /// If the ID is less than or equal to zero, assigns a random positive integer to the ID.
        /// </summary>
        private void AutoAssignId()
        {
            if (Id <= 0)
                Id = Random.Range(1, int.MaxValue);
        }

        /// <summary>
        /// Auto assigns elements to the collectable spot.
        /// </summary>
        public void AutoAssign()
        {
            AutoAssignId();

            if (AutoAssignCell)
                AssignClosestCell();
        }

        /// <summary>
        /// Assigns the closest cell to the spot.
        /// </summary>
        public void AssignClosestCell()
        {
            Cell = Cell.FindClosestCell(transform);
        }
    }
}