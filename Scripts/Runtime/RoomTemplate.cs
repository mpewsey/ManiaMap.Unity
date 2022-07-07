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
        public int Id { get => _id; private set => _id = value; }

        [SerializeField]
        private string _name;
        public string Name { get => _name; private set => _name = value; }

        [SerializeField]
        [TextArea(3, int.MaxValue)]
        private string _serializedText = string.Empty;
        public string SerializedText { get => _serializedText; private set => _serializedText = value; }

        public void Init(ManiaMap.RoomTemplate template)
        {
            var settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "",
                NewLineChars = "\n",
            };

            Id = template.Id;
            Name = template.Name;
            SerializedText = Serialization.GetXmlString(template, settings);
        }

        public void Init(Room room)
        {
            Init(room.GetTemplate());
        }

        public ManiaMap.RoomTemplate GetTemplate()
        {
            return Serialization.LoadXmlString<ManiaMap.RoomTemplate>(SerializedText);
        }
    }
}