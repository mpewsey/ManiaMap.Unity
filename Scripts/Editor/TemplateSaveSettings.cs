using System.IO;
using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Editor
{
    [CreateAssetMenu(menuName = "Mania Map/Template Save Settings", fileName = "TemplateSaveSettings")]
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
            if (SavePath.StartsWith("Packages/ManiaMap.Unity"))
                SavePackageTemplates();
            else
                SaveTemplates();
        }

        private void SavePackageTemplates()
        {
            if (!Directory.Exists(SavePath))
                Debug.LogError($"Path does not exist: {SavePath}");

            DeleteTempFolder();
            CreateTempFolder();

            foreach (var path in PrefabPaths())
            {
                CreatePackageRoomTemplate(path);
            }

            DeleteTempFolder();
            AssetDatabase.Refresh();
            Log.Success("Saved room templates.");
        }

        private void CreatePackageRoomTemplate(string assetPath)
        {
            using (var scope = new PrefabUtility.EditPrefabContentsScope(assetPath))
            {
                var prefab = scope.prefabContentsRoot;

                if (!prefab.TryGetComponent(out Room room))
                    return;

                Debug.Log($"Processing room at {assetPath}.");
                CreatePackageRoomTemplate(room);
            }
        }

        private void CreatePackageRoomTemplate(Room room)
        {
            var path = TempTemplateSavePath(room);
            var template = room.GetTemplate();
            EditorUtility.SetDirty(room);

            var asset = CreateInstance<RoomTemplate>();
            asset.Initialize(template);
            AssetDatabase.CreateAsset(asset, path);

            // Move file from temp path to save path.
            var packagePath = TemplateSavePath(room);
            MoveFile(path, TemplateSavePath(room));

            Log.Success($"Saved room template to {packagePath}.");
        }

        private void SaveTemplates()
        {
            CreateSaveDirectory();

            foreach (var path in PrefabPaths())
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
                AssetDatabase.SaveAssetIfDirty(asset);
            }

            Log.Success($"Saved room template to {path}.");
        }

        private static void DeleteTempFolder()
        {
            AssetDatabase.DeleteAsset("Assets/__ManiaMapTemp__");
        }

        private static void CreateTempFolder()
        {
            AssetDatabase.CreateFolder("Assets", "__ManiaMapTemp__");
        }

        private static void MoveFile(string fromPath, string toPath)
        {
            if (File.Exists(toPath))
                File.Delete(toPath);

            File.Move(fromPath, toPath);
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

        private string TempTemplateSavePath(Room room)
        {
            var path = FileUtility.ReplaceInvalidFileNameCharacters($"{room.name} [{room.Id:x}].asset");
            return Path.Combine("Assets/__ManiaMapTemp__", path);
        }

        private string[] PrefabGuids()
        {
            return AssetDatabase.FindAssets("t:prefab", SearchPaths);
        }

        private string[] PrefabPaths()
        {
            var paths = PrefabGuids();

            for (int i = 0; i < paths.Length; i++)
            {
                paths[i] = AssetDatabase.GUIDToAssetPath(paths[i]);
            }

            return paths;
        }
    }
}
