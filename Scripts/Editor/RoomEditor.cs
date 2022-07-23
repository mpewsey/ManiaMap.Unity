using System.IO;
using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Editor
{
    /// <summary>
    /// The Room editor.
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Room))]
    public class RoomEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!MultipleTargetsSelected() && !TargetIsPrefabAsset())
            {
                DrawSaveButton();
                DrawUpdateRoomButton();
            }

            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Returns true if multiple targets are selected.
        /// </summary>
        private bool MultipleTargetsSelected()
        {
            return serializedObject.targetObjects.Length > 1;
        }

        /// <summary>
        /// Returns true if the target object is an unopened prefab being inspected.
        /// </summary>
        private bool TargetIsPrefabAsset()
        {
            return GetRoom().gameObject.scene.name == null;
        }

        /// <summary>
        /// Returns the target room.
        /// </summary>
        private Room GetRoom()
        {
            return (Room)serializedObject.targetObject;
        }

        /// <summary>
        /// Draws the save button.
        /// </summary>
        private void DrawSaveButton()
        {
            if (GUILayout.Button("Save"))
            {
                SaveTemplate();
            }
        }

        /// <summary>
        /// Draws the update room button.
        /// </summary>
        private void DrawUpdateRoomButton()
        {
            if (GUILayout.Button("Update Room"))
            {
                UpdateRoom();
            }
        }

        /// <summary>
        /// Updates the room cells.
        /// </summary>
        private void UpdateRoom()
        {
            var room = GetRoom();
            room.AutoAssign();
            EditorUtility.SetDirty(room);
            Debug.Log("<color=#00FF00><b>Room updated.</b></color>");
        }

        /// <summary>
        /// Saves the generation room template to an XML file in the Assets/ManiaMap/RoomTemplates
        /// directory. If the folders in the file path do not already exist, they are created.
        /// </summary>
        private void SaveTemplate()
        {
            // Create the template.
            var room = GetRoom();
            var template = room.GetTemplate();
            EditorUtility.SetDirty(room);

            // Save the template to the room templates directory.
            var path = GetTemplateSavePath(room);
            Serialization.SavePrettyXml(path, template);
            Debug.Log($"<color=#00FF00><b>Saved room template to {path}.</b></color>");
        }

        /// <summary>
        /// Creates a Game Object with the Room component.
        /// </summary>
        [MenuItem("GameObject/Mania Map/Room", priority = 20)]
        [MenuItem("Mania Map/Create Room", priority = 100)]
        public static void CreateRoomTemplate()
        {
            var obj = new GameObject("Room");
            var template = obj.AddComponent<Room>();
            template.CreateCells();
            obj.transform.SetParent(Selection.activeTransform);
        }

        /// <summary>
        /// Searches the project for all prefabs with Room components and saves
        /// room templates for the rooms to the project.
        /// </summary>
        [MenuItem("Mania Map/Batch Save Templates", priority = 0)]
        public static void SaveAllTemplates()
        {
            var guids = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets" });

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                SaveRoomTemplate(path);
            }

            AssetDatabase.Refresh();
            Debug.Log($"<color=#00FF00><b>Saved rooms.</b></color>");
        }

        /// <summary>
        /// Saves the room template for the prefab at the specified path if it has
        /// a room component at its root.
        /// </summary>
        /// <param name="path">The prefab path.</param>
        private static void SaveRoomTemplate(string path)
        {
            using (var scope = new PrefabUtility.EditPrefabContentsScope(path))
            {
                var prefab = scope.prefabContentsRoot;

                if (!prefab.TryGetComponent(out Room room))
                    return;

                Debug.Log($"Processing room at {path}.");
                var savePath = GetTemplateSavePath(room);
                var template = AssetDatabase.LoadAssetAtPath<RoomTemplate>(savePath);

                if (template == null)
                {
                    template = CreateInstance<RoomTemplate>();
                    template.Init(room.GetTemplate());
                    AssetDatabase.CreateAsset(template, savePath);
                }
                else
                {
                    template.Init(room.GetTemplate());
                    EditorUtility.SetDirty(template);
                    AssetDatabase.SaveAssetIfDirty(template);
                }
            }
        }

        /// <summary>
        /// Returns the path to the room template directory.
        /// </summary>
        private static string GetRoomTemplatesDirectory()
        {
            var path = Path.Combine("Assets", "ManiaMap", "RoomTemplates");
            FileUtility.CreateDirectory(path);
            return path;
        }

        /// <summary>
        /// Returns the template file name for the room.
        /// </summary>
        /// <param name="room">The room.</param>
        private static string GetTemplateFileName(Room room)
        {
            return FileUtility.ReplaceInvalidFileNameCharacters($"{room.name}_{room.Id}.asset");
        }

        /// <summary>
        /// Returns the template save path for the room.
        /// </summary>
        /// <param name="room">The room.</param>
        private static string GetTemplateSavePath(Room room)
        {
            return Path.Combine(GetRoomTemplatesDirectory(), GetTemplateFileName(room));
        }
    }
}
