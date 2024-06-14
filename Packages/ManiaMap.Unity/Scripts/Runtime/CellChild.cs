using UnityEngine;
using UnityEngine.Events;

namespace MPewsey.ManiaMapUnity
{
    /// <summary>
    /// An object tied to the cell index of a RoomComponent.
    /// </summary>
    public abstract class CellChild : MonoBehaviour
    {
        [Header("Cell Child:")]
        [SerializeField]
        protected bool _autoAssignCell = true;
        /// <summary>
        /// If true, the cell will be automatically assigned to the closest cell when update and save operations
        /// are performed.
        /// </summary>
        public bool AutoAssignCell { get => _autoAssignCell; set => _autoAssignCell = value; }

        /// <summary>
        /// True if the object has been initialized.
        /// </summary>
        public bool IsInitialized { get; protected set; }

        [SerializeField]
        protected Vector2Int _cellIndex;
        /// <summary>
        /// The containing cell index.
        /// </summary>
        public Vector2Int CellIndex { get => _cellIndex; set => _cellIndex = Vector2Int.Max(value, Vector2Int.zero); }

        [SerializeField]
        protected RoomComponent _room;
        /// <summary>
        /// The containing room.
        /// </summary>
        public RoomComponent Room { get => _room; set => _room = value; }

        [SerializeField] private UnityEvent _onInitialize = new UnityEvent();
        /// <summary>
        /// The event invoked after the object is initialized.
        /// </summary>
        public UnityEvent OnInitialize { get => _onInitialize; set => _onInitialize = value; }

        /// <summary>
        /// The row index.
        /// </summary>
        public int Row { get => CellIndex.x; set => CellIndex = new Vector2Int(value, CellIndex.y); }

        /// <summary>
        /// The column index.
        /// </summary>
        public int Column { get => CellIndex.y; set => CellIndex = new Vector2Int(CellIndex.x, value); }

        protected virtual void Awake()
        {
            Room.OnInitialize.AddListener(Initialize);
        }

        protected virtual void OnDestroy()
        {
            Room.OnInitialize.RemoveListener(Initialize);
        }

        /// <summary>
        /// Performs auto assignment on the object.
        /// </summary>
        /// <param name="room">The containing room.</param>
        public virtual void AutoAssign(RoomComponent room)
        {
            Room = room;

            if (AutoAssignCell)
                CellIndex = room.FindClosestActiveCellIndex(transform.position);
        }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        protected virtual void Initialize()
        {
            if (!IsInitialized)
            {
                IsInitialized = true;
                OnInitialize.Invoke();
            }
        }
    }
}