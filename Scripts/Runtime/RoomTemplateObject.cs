using MPewsey.ManiaMap;
using MPewsey.Common.Serialization;
using UnityEngine;

namespace MPewsey.ManiaMapUnity
{
    /// <summary>
    /// A container for storing a serialized room template.
    /// </summary>
    public class RoomTemplateObject : ScriptableObject
    {
        [SerializeField]
        private int _id;
        /// <summary>
        /// The template unique ID.
        /// </summary>
        public int Id { get => _id; private set => _id = value; }

        [SerializeField]
        private string _name;
        /// <summary>
        /// The template name.
        /// </summary>
        public string Name { get => _name; private set => _name = value; }

        [SerializeField]
        private string _prefabGuid;
        /// <summary>
        /// The prefab GUID.
        /// </summary>
        public string PrefabGuid { get => _prefabGuid; set => _prefabGuid = value; }

        [SerializeField]
        [TextArea(3, int.MaxValue)]
        private string _serializedText = string.Empty;
        /// <summary>
        /// The serialized text for the template.
        /// </summary>
        public string SerializedText { get => _serializedText; private set => _serializedText = value; }

        private RoomTemplate _template;
        /// <summary>
        /// Returns the generation template. If the template is not already assigned, it is loaded
        /// from the serialized text then cached.
        /// </summary>
        public RoomTemplate Template
        {
            get
            {
                if (_template == null)
                    _template = JsonSerialization.LoadJsonString<RoomTemplate>(SerializedText);
                return _template;
            }
            private set => _template = value;
        }

        /// <summary>
        /// Initializes the template based on the specified generation template.
        /// </summary>
        /// <param name="template">The generation template.</param>
        /// <param name="prefabGuid">The prefab GUID.</param>
        public void Initialize(RoomTemplate template, string prefabGuid = null)
        {
            Template = null;
            Id = template.Id;
            Name = template.Name;
            PrefabGuid = prefabGuid;
            SerializedText = JsonSerialization.GetJsonString(template, JsonWriterSettings());
        }

        /// <summary>
        /// The JSON writer formatting settings.
        /// </summary>
        private static JsonWriterSettings JsonWriterSettings()
        {
            return new JsonWriterSettings();
        }
    }
}