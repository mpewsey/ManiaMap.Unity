using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// Contains global settings used by Mania Map components.
    /// </summary>
    public class ManiaMapSettings : ScriptableObject
    {
        [SerializeField]
        private string _playerTag = "Player";
        /// <summary>
        /// The tag assigned to the player.
        /// </summary>
        public string PlayerTag { get => _playerTag; set => _playerTag = value; }

        [SerializeField]
        private RoomPositionOption _roomPositionOption = RoomPositionOption.Origin;
        /// <summary>
        /// The room position option.
        /// </summary>
        public RoomPositionOption RoomPositionOption
        {
            get => _roomPositionOption;
            set => _roomPositionOption = value != RoomPositionOption.UseManagerSettings ? value : RoomPositionOption.Origin;
        }

        [SerializeField]
        private int _maxClusterDepth = 1;
        /// <summary>
        /// The maximum cluster depth used to calculate room clusters for a layout
        /// when it it assigned to the manager.
        /// </summary>
        public int MaxClusterDepth
        {
            get => _maxClusterDepth;
            set => _maxClusterDepth = Mathf.Max(value, 1);
        }

        private void OnValidate()
        {
            MaxClusterDepth = MaxClusterDepth;
            RoomPositionOption = RoomPositionOption;
        }

        /// <summary>
        /// Attempts to load the settings from teh resources folder. If they do not exist,
        /// returns a new settings object.
        /// </summary>
        public static ManiaMapSettings GetSettings()
        {
            var settings = Resources.Load<ManiaMapSettings>("ManiaMap/ManiaMapSettings");

            if (settings != null)
                return settings;

            return CreateInstance<ManiaMapSettings>();
        }

        /// <summary>
        /// Returns the controlling room position option.
        /// </summary>
        /// <param name="option">The local room position option.</param>
        public RoomPositionOption GetRoomPositionOption(RoomPositionOption option)
        {
            return option != RoomPositionOption.UseManagerSettings ? option : RoomPositionOption;
        }
    }
}