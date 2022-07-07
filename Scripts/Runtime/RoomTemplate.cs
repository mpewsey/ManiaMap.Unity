using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    public class RoomTemplate : ScriptableObject
    {
        [SerializeField]
        private int _id;
        public int Id { get => _id; set => _id = value; }

        [TextArea]
        [SerializeField]
        private string _serializedText;
        public string SerializedText { get => _serializedText; set => _serializedText = value; }

        public byte[] GetBytes()
        {
            return Encoding.UTF8.GetBytes(SerializedText);
        }
    }
}