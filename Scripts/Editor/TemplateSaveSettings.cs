using System.IO;
using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Editor
{
    [CreateAssetMenu(menuName = "Mania Map/Settings/Template Save Settings", fileName = "TemplateSaveSettings")]
    public class TemplateSaveSettings : ScriptableObject
    {
        [SerializeField]
        private string _savePath = "Assets/ManiaMap/RoomTemplates";
        public string SavePath { get => _savePath; set => _savePath = value; }

        [SerializeField]
        private string[] _searchPaths = new string[] { "Assets" };
        public string[] SearchPaths { get => _searchPaths; set => _searchPaths = value; }

        [MenuItem("Mania Map/Batch Save Templates", priority = 0)]
        public static void SaveAllTemplates()
        {
            GetSettings().BatchSaveTemplates();
        }

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

        public void BatchSaveTemplates()
        {
            CreateSaveDirectory();

            foreach (var path in FileUtility.FindPrefabPaths(SearchPaths))
            {
                CreateRoomTemplate(path);
            }

            Log.Success("Saved room templates.");
        }

        private void CreateRoomTemplate(string assetPath)
        {
            using (var scope = new PrefabUtility.EditPrefabContentsScope(assetPath))
            {
                var prefab = scope.prefabContentsRoot;

                if (!prefab.TryGetComponent(out Room room))
                    return;

                Debug.Log($"Processing room at {assetPath}.");
                CreateRoomTemplate(room);
            }
        }

        private void CreateRoomTemplate(Room room)
        {
            var path = TemplateSavePath(room);
            var asset = AssetDatabase.LoadAssetAtPath<RoomTemplate>(path);
            var template = room.GetTemplate();
            EditorUtility.SetDirty(room);

            if (asset == null)
            {
                asset = CreateInstance<RoomTemplate>();
                asset.Initialize(template);
                AssetDatabase.CreateAsset(asset, path);
            }
            else
            {
                asset.Initialize(template);
                EditorUtility.SetDirty(asset);
                // AssetDatabase.SaveAssetIfDirty(asset);
            }

            Log.Success($"Saved room template to {path}.");
        }

        private void CreateSaveDirectory()
        {
            FileUtility.CreateDirectory(SavePath);
        }

        private string TemplateSavePath(Room room)
        {
            var path = FileUtility.ReplaceInvalidFileNameCharacters($"{room.name} [{room.Id:x}].asset");
            return Path.Combine(SavePath, path);
        }
    }
}
