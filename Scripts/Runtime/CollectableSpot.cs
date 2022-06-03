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

        private void Awake()
        {
            Cell.Room.OnRoomInit.AddListener(OnRoomInit);
        }

        private void OnValidate()
        {
            AutoAssignId();
        }

        private void OnDestroy()
        {
            Cell.Room.OnRoomInit.RemoveListener(OnRoomInit);
        }

        /// <summary>
        /// Initializes the collectable spot based on the layout and layout state.
        /// </summary>
        /// <param name="room">The parent room.</param>
        private void OnRoomInit(Room room)
        {
            var manager = ManiaManager.Current;
            var roomData = manager.Layout.Rooms[room.RoomId];
            var state = manager.LayoutState.RoomStates[room.RoomId];

            IsAcquired = state.AcquiredCollectables.Contains(Id);
            Exists = roomData.Collectables.TryGetValue(Id, out int collectableId);
            CollectableId = Exists ? collectableId : int.MinValue;

            if (Exists)
                OnSpotExists.Invoke(this);
            else
                OnNoSpotExists.Invoke(this);
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