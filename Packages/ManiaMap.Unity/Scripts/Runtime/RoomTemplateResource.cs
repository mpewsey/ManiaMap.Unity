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
        private bool _editId;
        public bool EditId { get => _editId; set => _editId = value; }

        [SerializeField]
        private int _id = -1;
        /// <summary>
        /// The template unique ID.
        /// </summary>
        public int Id { get => _id; set => _id = value; }

        [SerializeField]
        private string _name = "<None>";
        /// <summary>
        /// The template name.
        /// </summary>
        public string Name { get => _name; set => _name = value; }

        [SerializeField]
        private string _prefabGuid;
        /// <summary>
        /// The prefab GUID.
        /// </summary>
        public string PrefabGuid { get => _prefabGuid; private set => _prefabGuid = value; }

        [SerializeField]
        private string _prefabPath;
        public string PrefabPath { get => _prefabPath; private set => _prefabPath = value; }

        [SerializeField]
        [TextArea(3, int.MaxValue)]
        private string _serializedText;
        /// <summary>
        /// The serialized text for the template.
        /// </summary>
        public string SerializedText { get => _serializedText; private set => _serializedText = value; }

        private void OnValidate()
        {
            Id = Rand.AutoAssignId(Id);
        }

        public void Initialize(RoomTemplate template, string prefabGuid, string prefabPath)
        {
            SerializedText = JsonSerialization.GetJsonString(template, new JsonWriterSettings());
            PrefabGuid = prefabGuid;
            PrefabPath = prefabPath;
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