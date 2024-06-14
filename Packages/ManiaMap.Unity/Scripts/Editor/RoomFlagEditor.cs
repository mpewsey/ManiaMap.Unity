using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Editor
{
    /// <summary>
    /// The RoomFlag editor.
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(RoomFlag))]
    public class RoomFlagEditor : CellChildEditor
    {
        [MenuItem("GameObject/Mania Map/Room Flag", priority = 20)]
        [MenuItem("Mania Map/Create Room Flag", priority = 100)]
        public static void CreateRoomFlag()
        {
            var obj = new GameObject("Room Flag");
            obj.transform.SetParent(Selection.activeTransform);
            obj.AddComponent<RoomFlag>();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawInspector();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawInspector()
        {
            var editId = ((RoomFlag)serializedObject.targetObject).EditId;

            GUI.enabled = false;
            var prop = serializedObject.GetIterator();
            var enterChildren = true;

            while (prop.NextVisible(enterChildren))
            {
                var disabled = prop.name == "m_Script" || (prop.name == "_id" && !editId);
                GUI.enabled = !disabled;
                EditorGUILayout.PropertyField(prop, true);
                enterChildren = false;
            }

            GUI.enabled = true;
        }
    }
}
