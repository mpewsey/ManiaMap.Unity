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
                db.AddEntry(room.Id, room);
            }
        }
    }
}