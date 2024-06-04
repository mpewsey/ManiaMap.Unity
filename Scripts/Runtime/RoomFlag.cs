using UnityEngine;

namespace MPewsey.ManiaMapUnity
{
    /// <summary>
    /// A unique room flag.
    /// </summary>
    public class RoomFlag : CellChild
    {
        [SerializeField] private int _id;
        /// <summary>
        /// The unique flag ID.
        /// </summary>
        public int Id { get => _id; set => _id = value; }

        /// <summary>
        /// True if the object has been initialized.
        /// </summary>
        public bool IsInitialized { get; private set; }

        private void OnValidate()
        {
            Id = ManiaMapManager.AutoAssignId(Id);
        }

        private void Awake()
        {
            Room.OnInitialize.AddListener(Initialize);
        }

        private void OnDestroy()
        {
            Room.OnInitialize.RemoveListener(Initialize);
        }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Initialize()
        {
            if (!IsInitialized)
            {
                IsInitialized = true;
                OnInitialize.Invoke();
            }
        }

        /// <summary>
        /// True if the flag is set.
        /// </summary>
        public bool Exists()
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