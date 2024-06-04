using UnityEngine;
using UnityEngine.Events;

namespace MPewsey.ManiaMapUnity
{
    /// <summary>
    /// An object that has a Cell as a parent.
    /// </summary>
    public abstract class CellChild : MonoBehaviour
    {
        [SerializeField]
        protected RoomComponent _room;
        public RoomComponent Room { get => _room; set => _room = value; }

        [SerializeField]
        protected bool _autoAssignCell = true;
        /// <summary>
        /// If true, the cell will be automatically assigned to the closest cell when update and save operations
        /// are performed.
        /// </summary>
        public bool AutoAssignCell { get => _autoAssignCell; set => _autoAssignCell = value; }

        [SerializeField]
        protected Vector2Int _cellIndex;
        public Vector2Int CellIndex { get => _cellIndex; set => _cellIndex = Vector2Int.Max(value, Vector2Int.zero); }

        [SerializeField] private UnityEvent _onInitialize = new UnityEvent();
        /// <summary>
        /// The event invoked after the object is initialized.
        /// </summary>
        public UnityEvent OnInitialize { get => _onInitialize; set => _onInitialize = value; }

        public int Row { get => CellIndex.x; set => CellIndex = new Vector2Int(value, CellIndex.y); }
        public int Column { get => CellIndex.y; set => CellIndex = new Vector2Int(CellIndex.x, value); }

        /// <summary>
        /// If auto assign is enabled, assigns the closest cell to the object.
        /// </summary>
        public virtual void AutoAssign(RoomComponent room)
        {
            Room = room;

            if (AutoAssignCell)
                CellIndex = room.FindClosestActiveCellIndex(transform.position);
        }
    }
}