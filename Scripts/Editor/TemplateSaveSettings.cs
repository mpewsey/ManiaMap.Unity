using System.IO;
using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Editor
{
    /// <summary>
    /// Settings for batch saving room templates for prefabs in a project.
    /// </summary>
    [CreateAssetMenu(menuName = "Mania Map/Settings/Template Save Settings", fileName = "TemplateSaveSettings")]
    public class TemplateSaveSettings : ScriptableObject
    {
        [SerializeField]
        private string _savePath = "Assets/ManiaMap/RoomTemplates";
        /// <summary>
        /// The save path for the generated room templates.
        /// </summary>
        public string SavePath { get => _savePath; set => _savePath = value; }

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
        [MenuItem("Mania Map/Batch Save Templates", priority = 0)]
        public static void SaveAllTemplates()
        {
            GetSettings().BatchSaveTemplates();
        }

        /// <summary>
        /// If a ManiaMap/TemplateSaveSettings asset exists within a Resources folder,
        /// loads and returns it. Otherwise, creates a new asset within the Resources folder
        /// and returns it.
        /// </summary>
        private static TemplateSaveSettings GetSettings()
        {
            var settings = Resources.Load<TemplateSaveSettings>("ManiaMap/TemplateSaveSettings");

            if (settings != null)
                return settings;

            settings = CreateInstance<TemplateSaveSettings>();
            FileUtility.CreateDirectory("Assets/Resources/ManiaMap");
            var path = "Assets/Resources/ManiaMap/TemplateSaveSettings.asset";
            AssetDatabase.CreateAsset(settings, path);
            Debug.Log($"Template save settings created at: {path}");
            return settings;
        }

        /// <summary>
        /// Saves room templates for all prefabs with Room components found at the
        /// specified search paths.
        /// </summary>
        public void BatchSaveTemplates()
        {
            CreateSaveDirectory();

            foreach (var guid in FileUtility.FindPrefabGuids(SearchPaths))
            {
                CreateRoomTemplate(guid);
            }

            Log.Success("Saved room templates.");
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

                if (!prefab.TryGetComponent(out RoomBehavior room))
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
        private void CreateRoomTemplate(RoomBehavior room, string prefabGuid)
        {
            var path = TemplateSavePath(room);
            var asset = AssetDatabase.LoadAssetAtPath<RoomTemplateObject>(path);
            var template = room.CreateData();
            EditorUtility.SetDirty(room);

            if (asset == null)
            {
                asset = CreateInstance<RoomTemplateObject>();
                asset.Initialize(template, prefabGuid);
                AssetDatabase.CreateAsset(asset, path);
            }
            else
            {
                asset.Initialize(template, prefabGuid);
                EditorUtility.SetDirty(asset);
                // AssetDatabase.SaveAssetIfDirty(asset);
            }

            Log.Success($"Saved room template to {path}.");
        }

        /// <summary>
        /// Creates the save directory within the project.
        /// </summary>
        private void CreateSaveDirectory()
        {
            FileUtility.CreateDirectory(SavePath);
        }

        /// <summary>
        /// Returns the template save path for the room.
        /// </summary>
        /// <param name="room">The room.</param>
        private string TemplateSavePath(RoomBehavior room)
        {
            var path = FileUtility.ReplaceInvalidFileNameCharacters($"{room.name} [{room.Id:x}].asset");
            return Path.Combine(SavePath, path);
        }
    }
}
