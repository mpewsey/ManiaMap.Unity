using UnityEngine;

namespace MPewsey.ManiaMap.Unity
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

        [SerializeField] private RoomFlagEvent _onInitialize;
        /// <summary>
        /// The event invoked after the object is initialized.
        /// </summary>
        public RoomFlagEvent OnInitialize { get => _onInitialize; set => _onInitialize = value; }

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
            Room().OnInitialize.AddListener(Initialize);
        }

        private void OnDestroy()
        {
            Room().OnInitialize.RemoveListener(Initialize);
        }

        /// <summary>
        /// Initializes the object.
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
        /// True if the flag is set.
        /// </summary>
        public bool Exists()
        {
            return RoomState().Flags.Contains(Id);
        }

        /// <summary>
        /// Sets the flag. Returns true if the flag was not already set.
        /// </summary>
        public bool SetFlag()
        {
            return RoomState().Flags.Add(Id);
        }

        /// <summary>
        /// Removes the flag. Returns true if the flag was removed.
        /// </summary>
        public bool RemoveFlag()
        {
            return RoomState().Flags.Remove(Id);
        }

        /// <summary>
        /// Toggles the flag. Returns true if the flag is now set. Otherwise, returns false.
        /// </summary>
        public bool ToggleFlag()
        {
            var flags = RoomState().Flags;

            if (!flags.Add(Id))
            {
                flags.Remove(Id);
                return false;
            }

            return true;
        }
    }
}