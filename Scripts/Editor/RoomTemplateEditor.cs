using System.IO;
using System.Text;
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
            DrawCreateCellsButton();
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
        /// draws the create cells button.
        /// </summary>
        private void DrawCreateCellsButton()
        {
            if (GUILayout.Button("Create Cells"))
            {
                var template = (RoomTemplate)serializedObject.targetObject;
                template.CreateCells();
            }
        }

        /// <summary>
        /// Returns the filename for the generator room template. Invalid filename
        /// characters are replaced with underscores.
        /// </summary>
        /// <param name="template">The generator room template.</param>
        private string GetFilename(ManiaMap.RoomTemplate template)
        {
            var name = $"{template.Id}_{template.Name}.xml";
            var builder = new StringBuilder(name);
            var chars = Path.GetInvalidFileNameChars();

            for (int i = 0; i < name.Length; i++)
            {
                var index = name.IndexOfAny(chars, i);

                if (index >= 0)
                {
                    i = index;
                    builder[index] = '_';
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Saves the generation room template to an XML file in the Assets/ManiaMap/RoomTemplates
        /// directory. If the folders in the file path do not already exists, they are created.
        /// </summary>
        private void SaveTemplate()
        {
            var target = (RoomTemplate)serializedObject.targetObject;
            var template = target.GetTemplate();
            var path = Path.Combine("Assets", "ManiaMap", "RoomTemplates", GetFilename(template));

            // Create Mania Map directory if it doesn't exist.
            if (!AssetDatabase.IsValidFolder("Assets/ManiaMap"))
                AssetDatabase.CreateFolder("Assets", "ManiaMap");

            // Create Room Templates directory if it doesn't exist.
            if (!AssetDatabase.IsValidFolder("Assets/ManiaMap/RoomTemplates"))
                AssetDatabase.CreateFolder("Assets/ManiaMap", "RoomTemplates");

            Serialization.SaveXml(path, template);
            AssetDatabase.Refresh();
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
