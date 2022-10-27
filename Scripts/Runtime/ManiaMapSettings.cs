using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    public class ManiaMapSettings : ScriptableObject
    {
        [SerializeField]
        private string _playerTag = "Player";
        public string PlayerTag { get => _playerTag; set => _playerTag = value; }

        [SerializeField]
        private int _maxClusterDepth = 1;
        public int MaxClusterDepth
        {
            get => _maxClusterDepth;
            set => _maxClusterDepth = Mathf.Max(value, 1);
        }

        private void OnValidate()
        {
            MaxClusterDepth = MaxClusterDepth;
        }

        public static ManiaMapSettings LoadSettings()
        {
            return Resources.Load<ManiaMapSettings>("ManiaMap/ManiaMapSettings");
        }

        public static ManiaMapSettings GetSettings()
        {
            var settings = LoadSettings();

            if (settings != null)
                return settings;

            return CreateInstance<ManiaMapSettings>();
        }
    }
}