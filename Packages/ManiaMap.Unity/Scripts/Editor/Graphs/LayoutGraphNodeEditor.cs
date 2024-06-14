using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Graphs.Editor
{
    /// <summary>
    /// The LayoutGraphNode editor.
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(LayoutGraphNode))]
    public class LayoutGraphNodeEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            GUI.enabled = true;
            serializedObject.Update();
            var prop = serializedObject.GetIterator();
            prop.NextVisible(true);

            while (prop.NextVisible(false))
            {
                EditorGUILayout.PropertyField(prop, true);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}