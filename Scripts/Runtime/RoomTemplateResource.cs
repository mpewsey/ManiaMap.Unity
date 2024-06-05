using MPewsey.Common.Serialization;
using MPewsey.ManiaMap;
using MPewsey.ManiaMapUnity.Exceptions;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MPewsey.ManiaMapUnity
{
    /// <summary>
    /// A container for storing a serialized room template.
    /// </summary>
    public class RoomTemplateResource : ScriptableObject
    {
        [SerializeField]
        private int _id = -1;
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

        /// <summary>
        /// Initializes the template based on the specified generation template.
        /// </summary>
        /// <param name="template">The generation template.</param>
        /// <param name="prefabGuid">The prefab GUID.</param>
        public void Initialize(RoomTemplate template, string prefabGuid = null)
        {
            Id = template.Id;
            Name = template.Name;
            PrefabGuid = prefabGuid;
            SerializedText = JsonSerialization.GetJsonString(template, new JsonWriterSettings());
        }

        public RoomTemplate GetMMRoomTemplate()
        {
            if (string.IsNullOrWhiteSpace(SerializedText))
                throw new RoomTemplateNotInitializedException($"Serialized text has not been assigned: {this}");

            return JsonSerialization.LoadJsonString<RoomTemplate>(SerializedText);
        }

        public AssetReferenceGameObject GetAssetReference()
        {
            return new AssetReferenceGameObject(PrefabGuid);
        }
    }
}