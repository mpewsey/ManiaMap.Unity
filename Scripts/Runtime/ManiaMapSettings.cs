using UnityEngine;

namespace MPewsey.ManiaMapUnity
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
            return option != RoomPositionOption.UseManagerSetting ? option : RoomPositionOption;
        }
    }
}