using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// An object that has a Cell as a parent.
    /// </summary>
    public abstract class CellChild : MonoBehaviour
    {
        [SerializeField]
        protected bool _autoAssignCell = true;
        /// <summary>
        /// If true, the cell will be automatically assigned to the closest cell when update and save operations
        /// are performed.
        /// </summary>
        public bool AutoAssignCell { get => _autoAssignCell; set => _autoAssignCell = value; }

        [SerializeField]
        protected CellBehavior _cell;
        /// <summary>
        /// The parent cell.
        /// </summary>
        public CellBehavior Cell { get => _cell; set => _cell = value; }

        /// <summary>
        /// The room ID.
        /// </summary>
        public Uid RoomId() => RoomLayout().Id;

        /// <summary>
        /// The parent room.
        /// </summary>
        public RoomBehavior Room() => Cell.Room;

        /// <summary>
        /// The layout.
        /// </summary>
        public Layout Layout() => Room().Layout;

        /// <summary>
        /// The layout state.
        /// </summary>
        public LayoutState LayoutState() => Room().LayoutState;

        /// <summary>
        /// Returns the room data.
        /// </summary>
        public Room RoomLayout() => Room().RoomLayout;

        /// <summary>
        /// Returns the room state.
        /// </summary>
        public RoomState RoomState() => Room().RoomState;

        /// <summary>
        /// A list of room door connections.
        /// </summary>
        public IReadOnlyList<DoorConnection> DoorConnections() => Room().DoorConnections;

        /// <summary>
        /// If auto assign is enabled, assigns the closest cell to the object.
        /// </summary>
        public virtual void AutoAssign()
        {
            if (AutoAssignCell)
                AssignClosestCell();
        }

        /// <summary>
        /// Assigns the closest cell to the door.
        /// </summary>
        public void AssignClosestCell()
        {
            Cell = CellBehavior.FindClosestCell(transform);
        }
    }
}