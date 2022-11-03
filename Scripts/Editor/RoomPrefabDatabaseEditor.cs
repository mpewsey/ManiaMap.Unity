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
        /// <summary>
        /// Creates a new room prefab database Game Object.
        /// </summary>
        [MenuItem("GameObject/Mania Map/Room Prefab Database", priority = 20)]
        [MenuItem("Mania Map/Create Room Prefab Database", priority = 100)]
        public static void CreateDatabase()
        {
            var obj = new GameObject("Room Prefab Database");
            obj.transform.SetParent(Selection.activeTransform);
            obj.AddComponent<RoomPrefabDatabase>();
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
        private RoomPrefabDatabase GetRoomPrefabDatabase()
        {
            return (RoomPrefabDatabase)serializedObject.targetObject;
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
        /// Populates the database with all prefabs in the project.
        /// </summary>
        private void AutoAssign()
        {
            var db = GetRoomPrefabDatabase();
            db.Entries.Clear();

            foreach (var path in FileUtility.FindPrefabPaths(db.SearchPaths))
            {
                AddPrefabEntry(path);
            }

            EditorUtility.SetDirty(db);
            Log.Success("Added prefabs to database.");
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