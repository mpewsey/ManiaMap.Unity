using System.IO;
using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Editor
{
    [CustomEditor(typeof(RoomTemplate))]
    public class RoomTemplateEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawSaveButton();
            DrawUpdateCellsButton();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
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
        /// Draws the update cells button.
        /// </summary>
        private void DrawUpdateCellsButton()
        {
            if (GUILayout.Button("Update Cells"))
            {
                UpdateCells();
            }
        }

        /// <summary>
        /// Creates or updates the room template cells.
        /// </summary>
        private void UpdateCells()
        {
            var template = (RoomTemplate)serializedObject.targetObject;
            template.CreateCells();
            Debug.Log("<color=#00FF00><b>Cells updated.</b></color>");
        }

        /// <summary>
        /// Saves the generation room template to an XML file in the Assets/ManiaMap/RoomTemplates
        /// directory. If the folders in the file path do not already exists, they are created.
        /// </summary>
        private void SaveTemplate()
        {
            var target = (RoomTemplate)serializedObject.targetObject;
            var template = target.GetTemplate();
            var root = Path.Combine("Assets", "ManiaMap", "RoomTemplates");
            var filename = AssetsUtility.ReplaceInvalidFileNameCharacters($"{template.Id}_{template.Name}.xml", '_');
            var path = Path.Combine(root, filename);
            AssetsUtility.CreateDirectory(root);
            Serialization.SaveXml(path, template);
            AssetDatabase.Refresh();
            Debug.Log($"<color=#00FF00><b>Saved room template to {path}.</b></color>");
        }

        /// <summary>
        /// Creates a Game Object with the room template component.
        /// </summary>
        [MenuItem("GameObject/Mania Map/Room Template")]
        public static void CreateRoomTemplate()
        {
            var obj = new GameObject("Room Template");
            var template = obj.AddComponent<RoomTemplate>();
            template.CreateCellContainer();
            obj.transform.SetParent(Selection.activeTransform);
        }
    }
}
