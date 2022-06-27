using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Editor
{
    /// <summary>
    /// The RoomPrefabDatabase editor.
    /// </summary>
    [CustomEditor(typeof(RoomPrefabDatabase))]
    public class RoomPrefabDatabaseEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!TargetIsPrefabAsset())
            {
                DrawPopulatePrefabsButton();
            }

            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Returns true if the target object is an unopened prefab being inspected.
        /// </summary>
        private bool TargetIsPrefabAsset()
        {
            return GetRoomPrefabDatabase().gameObject.scene.name == null;
        }

        /// <summary>
        /// Returns the target database.
        /// </summary>
        private RoomPrefabDatabase GetRoomPrefabDatabase()
        {
            return (RoomPrefabDatabase)serializedObject.targetObject;
        }

        /// <summary>
        /// Draws the populate prefabs button.
        /// </summary>
        private void DrawPopulatePrefabsButton()
        {
            if (GUILayout.Button("Populate Prefabs"))
            {
                PopulatePrefabs();
            }
        }

        /// <summary>
        /// Populates the database with all prefabs in the project.
        /// </summary>
        private void PopulatePrefabs()
        {
            var guids = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets" });
            var db = GetRoomPrefabDatabase();
            db.Entries.Clear();

            foreach (var guid in guids)
            {
                AddPrefabEntry(AssetDatabase.GUIDToAssetPath(guid));
            }

            Debug.Log($"<color=#00FF00><b>Added prefabs to database.</b></color>");
        }

        /// <summary>
        /// Adds a database entry for the prefab at the specified path if it has a Room component.
        /// </summary>
        /// <param name="path">The prefab path.</param>
        private void AddPrefabEntry(string path)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefab.TryGetComponent(out Room room))
            {
                var db = GetRoomPrefabDatabase();
                db.Entries.Add(new RoomDatabaseEntry<Room>(room.Id, room));
            }
        }
    }
}