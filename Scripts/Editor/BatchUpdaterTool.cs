using System.IO;
using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Editor
{
    /// <summary>
    /// Settings for batch saving room templates for prefabs in a project.
    /// </summary>
    [CreateAssetMenu(menuName = "Mania Map/Batch Updater Tool", fileName = "BatchUpdaterTool")]
    public class BatchUpdaterTool : ScriptableObject
    {
        [SerializeField]
        private string[] _searchPaths = new string[] { "Assets" };
        /// <summary>
        /// The search paths used when searching for room prefabs.
        /// </summary>
        public string[] SearchPaths { get => _searchPaths; set => _searchPaths = value; }

        /// <summary>
        /// Batch saves room templates for prefabs based on the settings at ManiaMap/TemplateSaveSettings
        /// Resources path or the default settings.
        /// </summary>
        [MenuItem("Mania Map/Batch Update Room Templates", priority = 0)]
        public static void BatchUpdateRoomTemplates()
        {
            LoadSettings().BatchSaveTemplates();
        }

        /// <summary>
        /// If a ManiaMap/TemplateSaveSettings asset exists within a Resources folder,
        /// loads and returns it. Otherwise, creates a new asset within the Resources folder
        /// and returns it.
        /// </summary>
        private static BatchUpdaterTool LoadSettings()
        {
            var settings = Resources.Load<BatchUpdaterTool>("ManiaMap/BatchUpdaterTool");

            if (settings != null)
                return settings;

            return CreateInstance<BatchUpdaterTool>();
        }

        /// <summary>
        /// Saves room templates for all prefabs with Room components found at the
        /// specified search paths.
        /// </summary>
        public void BatchSaveTemplates()
        {
            var guids = AssetDatabase.FindAssets("t:prefab", SearchPaths);

            foreach (var guid in guids)
            {
                CreateRoomTemplate(guid);
            }

            Debug.Log("<color=#00FF00><b>Saved room templates.</b></color>");
        }

        /// <summary>
        /// Creates or overwrites the room template for the prefab at the specified
        /// project path, provided it has a Room component.
        /// </summary>
        /// <param name="guid">The asset GUID.</param>
        private void CreateRoomTemplate(string guid)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);

            using (var scope = new PrefabUtility.EditPrefabContentsScope(assetPath))
            {
                var prefab = scope.prefabContentsRoot;

                if (!prefab.TryGetComponent(out RoomComponent room))
                    return;

                Debug.Log($"Processing room at {assetPath}.");
                CreateRoomTemplate(room, guid);
            }
        }

        /// <summary>
        /// Creates or overwrites the room template for the specified room.
        /// </summary>
        /// <param name="room">The room.</param>
        /// <param name="prefabGuid">The prefab GUID.</param>
        private void CreateRoomTemplate(RoomComponent room, string prefabGuid)
        {
            var path = Path.ChangeExtension(AssetDatabase.GUIDToAssetPath(prefabGuid), ".room_template.asset");
            var asset = AssetDatabase.LoadAssetAtPath<RoomTemplateResource>(path);
            var template = room.GetMMRoomTemplate();
            EditorUtility.SetDirty(room);

            if (asset == null)
            {
                asset = CreateInstance<RoomTemplateResource>();
                asset.Initialize(template, prefabGuid);
                AssetDatabase.CreateAsset(asset, path);
            }
            else
            {
                asset.Initialize(template, prefabGuid);
                EditorUtility.SetDirty(asset);
                // AssetDatabase.SaveAssetIfDirty(asset);
            }

            Debug.Log($"<color=#00FF00><b>Saved room template to {path}.</b></color>");
        }
    }
}
