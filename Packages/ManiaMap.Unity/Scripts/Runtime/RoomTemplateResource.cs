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
        /// <summary>
        /// If true, the ID can be edited in the inspector.
        /// </summary>
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
        /// <summary>
        /// The path of the prefab from the project root when the template was last updated.
        /// </summary>
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

        /// <summary>
        /// Sets the properties for the room template.
        /// </summary>
        /// <param name="template">The Mania Map room template.</param>
        /// <param name="prefabGuid">The prefab GUID.</param>
        /// <param name="prefabPath">The prefab path.</param>
        public void Initialize(RoomTemplate template, string prefabGuid, string prefabPath)
        {
            SerializedText = JsonSerialization.GetJsonString(template, new JsonWriterSettings());
            PrefabGuid = prefabGuid;
            PrefabPath = prefabPath;
        }

        /// <summary>
        /// Creates a new Mania Map room template from the serialized text and returns it.
        /// </summary>
        /// <exception cref="RoomTemplateNotInitializedException">Raised if the serialized text has not been assigned.</exception>
        public RoomTemplate GetMMRoomTemplate()
        {
            if (string.IsNullOrWhiteSpace(SerializedText))
                throw new RoomTemplateNotInitializedException($"Serialized text has not been assigned: {this}");

            return JsonSerialization.LoadJsonString<RoomTemplate>(SerializedText);
        }

        /// <summary>
        /// Returns the asset reference based on the assigned prefab GUID.
        /// </summary>
        public AssetReferenceGameObject GetAssetReference()
        {
            return new AssetReferenceGameObject(PrefabGuid);
        }
    }
}