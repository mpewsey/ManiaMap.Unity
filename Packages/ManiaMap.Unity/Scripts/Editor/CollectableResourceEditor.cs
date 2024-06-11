using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Editor
{
    /// <summary>
    /// The CollectableResource editor.
    /// </summary>
    [CustomEditor(typeof(CollectableResource))]
    public class CollectableResourceEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawInspector();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawInspector()
        {
            var editId = ((CollectableResource)serializedObject.targetObject).EditId;

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