using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MPewsey.ManiaMap.Unity.Editor
{
    /// <summary>
    /// The RoomAddressableDatabase editor.
    /// </summary>
    [CustomEditor(typeof(RoomAddressableDatabase))]
    public class RoomAddressableDatabaseEditor : UnityEditor.Editor
    {
        /// <summary>
        /// Creates a new database Game Object.
        /// </summary>
        [MenuItem("GameObject/Mania Map/Room Addressable Database", priority = 20)]
        [MenuItem("Mania Map/Create Room Addressable Database", priority = 100)]
        public static void CreateDatabase()
        {
            var obj = new GameObject("Room Addressable Database");
            obj.transform.SetParent(Selection.activeTransform);
            obj.AddComponent<RoomAddressableDatabase>();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawAutoAssignButton();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Returns the target database.
        /// </summary>
        private RoomAddressableDatabase GetRoomAddressableDatabase()
        {
            return (RoomAddressableDatabase)serializedObject.targetObject;
        }

        /// <summary>
        /// Draws the auto assign button.
        /// </summary>
        private void DrawAutoAssignButton()
        {
            if (GUILayout.Button("Auto Assign"))
                AutoAssign();
        }

        /// <summary>
        /// Adds all addressable room prefabs in the project to the database.
        /// </summary>
        private void AutoAssign()
        {
            var db = GetRoomAddressableDatabase();
            db.Entries.Clear();

            foreach (var guid in FileUtility.FindPrefabGuids(db.SearchPaths))
            {
                AddAddressableEntry(guid);
            }

            EditorUtility.SetDirty(db);
            Log.Success("Added prefabs to database.");
        }

        /// <summary>
        /// If the asset with the specified GUID is an addressable room, adds it to the database.
        /// </summary>
        /// <param name="guid">The asset GUID.</param>
        private void AddAddressableEntry(string guid)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (!prefab.TryGetComponent(out Room room))
                return;

            var entry = AddressableAssetSettingsDefaultObject.Settings.FindAssetEntry(guid);

            if (entry == null)
                return;

            var db = GetRoomAddressableDatabase();
            var reference = new AssetReferenceGameObject(guid);
            db.Entries.Add(new RoomDatabaseEntry<AssetReferenceGameObject>(room.Id, reference));
        }
    }
}