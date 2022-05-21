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
            DrawCreateCellsButton();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawSaveButton()
        {
            if (GUILayout.Button("Save"))
            {
                SaveTemplate();
            }
        }

        private void DrawCreateCellsButton()
        {
            if (GUILayout.Button("Create Cells"))
            {
                var template = (RoomTemplate)serializedObject.targetObject;
                template.CreateCells();
            }
        }

        private void SaveTemplate()
        {
            var target = (RoomTemplate)serializedObject.targetObject;
            var template = target.GetTemplate();
            var path = Path.Combine("Assets", "ManiaMap", "RoomTemplates", $"{template.Id}_{template.Name}.xml");

            // Create Mania Map directory if it doesn't exist.
            if (!AssetDatabase.IsValidFolder("Assets/ManiaMap"))
                AssetDatabase.CreateFolder("Assets", "ManiaMap");

            // Create Room Templates directory if it doesn't exist.
            if (!AssetDatabase.IsValidFolder("Assets/ManiaMap/RoomTemplates"))
                AssetDatabase.CreateFolder("Assets/ManiaMap", "RoomTemplates");

            Serialization.SaveXml(path, template);
            AssetDatabase.Refresh();
        }

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
