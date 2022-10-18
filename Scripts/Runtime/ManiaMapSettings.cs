using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    public class ManiaMapSettings : ScriptableObject
    {
        [SerializeField]
        private string _playerTag = "Player";
        public string PlayerTag { get => _playerTag; set => _playerTag = value; }

        public static ManiaMapSettings Create()
        {
            return CreateInstance<ManiaMapSettings>();
        }
    }
}