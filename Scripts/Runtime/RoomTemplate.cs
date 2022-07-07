using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    public class RoomTemplate : ScriptableObject
    {
        [SerializeField]
        private int _id;
        public int Id { get => _id; set => _id = value; }

        [SerializeField]
        private string _name;
        public string Name { get => _name; set => _name = value; }

        [SerializeField]
        [TextArea(3, int.MaxValue)]
        private string _serializedText = string.Empty;
        public string SerializedText { get => _serializedText; set => _serializedText = value; }

        public void Init(Room room)
        {
            var settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "",
                NewLineChars = "\n",
            };

            Id = room.Id;
            Name = room.Name;
            SerializedText = Serialization.GetXmlString(room.GetTemplate(), settings);
        }

        public ManiaMap.RoomTemplate GetTemplate()
        {
            return Serialization.LoadXmlString<ManiaMap.RoomTemplate>(SerializedText);
        }
    }
}