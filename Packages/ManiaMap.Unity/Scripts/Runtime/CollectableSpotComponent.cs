using MPewsey.Common.Mathematics;
using MPewsey.ManiaMap;
using UnityEngine;

namespace MPewsey.ManiaMapUnity
{
    /// <summary>
    /// Represents a collectable spot.
    /// </summary>
    public class CollectableSpotComponent : CellChild
    {
        [Header("Collectable Spot:")]
        [SerializeField] private bool _editId;
        /// <summary>
        /// If true, the ID can be edited in the inspector.
        /// </summary>
        public bool EditId { get => _editId; set => _editId = value; }

        [SerializeField]
        private int _id = -1;
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
        /// <summary>
        /// The manual draw weight of the collectable spot.
        /// </summary>
        public float Weight { get => _weight; set => _weight = Mathf.Max(value, 0); }

        private void OnValidate()
        {
            Weight = Weight;
        }

        /// <summary>
        /// The collectable ID. If the collectable spot does not exist in the layout, returns -1.
        /// </summary>
        public int CollectableId()
        {
            if (Room.RoomLayout.Collectables.TryGetValue(Id, out int value))
                return value;

            return -1;
        }

        /// <summary>
        /// True if the collectable spot exists.
        /// </summary>
        public bool CollectableExists()
        {
            return Room.RoomLayout.Collectables.ContainsKey(Id);
        }

        /// <summary>
        /// True if the collectable spot is already acquired.
        /// </summary>
        public bool IsAcquired()
        {
            return Room.RoomState.AcquiredCollectables.Contains(Id);
        }

        /// <summary>
        /// Returns true if the collectable can be acquired.
        /// </summary>
        public bool CanAcquire()
        {
            return CollectableExists() && !IsAcquired();
        }

        /// <summary>
        /// If the collectable spot exists and has not already been acquired,
        /// adds it to the current layout state's acquired collectables
        /// and marks it as acquired. Returns true if this action is performed.
        /// </summary>
        public bool Acquire()
        {
            if (CanAcquire())
                return Room.RoomState.AcquiredCollectables.Add(Id);

            return false;
        }

        /// <summary>
        /// Auto assigns elements to the collectable spot.
        /// </summary>
        public override void AutoAssign(RoomComponent room)
        {
            base.AutoAssign(room);
            Id = Rand.AutoAssignId(Id);
        }

        /// <summary>
        /// Returns new generation data for the collectable spot.
        /// </summary>
        public CollectableSpot GetMMCollectableSpot()
        {
            var position = new Vector2DInt(Row, Column);
            return new CollectableSpot(position, Group.Name, Weight);
        }
    }
}