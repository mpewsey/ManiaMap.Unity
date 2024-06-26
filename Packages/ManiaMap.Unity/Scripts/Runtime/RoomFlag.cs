using UnityEngine;

namespace MPewsey.ManiaMapUnity
{
    /// <summary>
    /// A unique room flag.
    /// </summary>
    public class RoomFlag : CellChild
    {
        [Header("Room Flag:")]
        [SerializeField] private bool _editId;
        /// <summary>
        /// If true, the ID can be edited in the inspector.
        /// </summary>
        public bool EditId { get => _editId; set => _editId = value; }

        [SerializeField] private int _id = -1;
        /// <summary>
        /// The unique flag ID.
        /// </summary>
        public int Id { get => _id; set => _id = value; }

        public override void AutoAssign(RoomComponent room)
        {
            base.AutoAssign(room);
            Id = Rand.AutoAssignId(Id);
        }

        /// <summary>
        /// True if the flag is set.
        /// </summary>
        public bool FlagIsSet()
        {
            return Room.RoomState.Flags.Contains(Id);
        }

        /// <summary>
        /// Sets the flag. Returns true if the flag was not already set.
        /// </summary>
        public bool SetFlag()
        {
            return Room.RoomState.Flags.Add(Id);
        }

        /// <summary>
        /// Removes the flag. Returns true if the flag was removed.
        /// </summary>
        public bool RemoveFlag()
        {
            return Room.RoomState.Flags.Remove(Id);
        }

        /// <summary>
        /// Toggles the flag. Returns true if the flag is now set. Otherwise, returns false.
        /// </summary>
        public bool ToggleFlag()
        {
            if (Room.RoomState.Flags.Add(Id))
                return true;

            Room.RoomState.Flags.Remove(Id);
            return false;
        }
    }
}