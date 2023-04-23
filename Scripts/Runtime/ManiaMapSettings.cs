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
            set => _roomPositionOption = value != RoomPositionOption.UseManagerSetting ? value : RoomPositionOption.Origin;
        }

        [SerializeField]
        private int _maxClusterDepth = 1;
        /// <summary>
        /// The maximum depth for computed room clusters.
        /// </summary>
        public int MaxClusterDepth { get => _maxClusterDepth; set => _maxClusterDepth = Mathf.Max(value, 0); }

        [SerializeField]
        private int _maxStaleCount = 1;
        /// <summary>
        /// The maximum count beyond which a loaded room will be considered stale.
        /// </summary>
        public int MaxStaleCount { get => _maxStaleCount; set => _maxStaleCount = Mathf.Max(value, 0); }

        private void OnValidate()
        {
            RoomPositionOption = RoomPositionOption;
            MaxClusterDepth = MaxClusterDepth;
            MaxStaleCount = MaxStaleCount;
        }

        /// <summary>
        /// Attempts to load the settings from teh resources folder. If they do not exist,
        /// returns a new settings object.
        /// </summary>
        public static ManiaMapSettings GetSettings()
        {
            var settings = Resources.Load<ManiaMapSettings>("ManiaMap/ManiaMapSettings");

            if (settings == null)
                settings = CreateInstance<ManiaMapSettings>();

            return settings;
        }

        /// <summary>
        /// Returns the controlling room position option.
        /// </summary>
        /// <param name="option">The local room position option.</param>
        public RoomPositionOption GetRoomPositionOption(RoomPositionOption option)
        {
            return option != RoomPositionOption.UseManagerSetting ? option : RoomPositionOption;
        }
    }
}