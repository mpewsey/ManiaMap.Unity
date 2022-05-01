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
                Debug.Log("Not implemented.");
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
