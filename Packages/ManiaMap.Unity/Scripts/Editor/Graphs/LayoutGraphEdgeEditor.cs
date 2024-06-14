using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Graphs.Editor
{
    /// <summary>
    /// The LayoutGraphEdge editor.
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(LayoutGraphEdge))]
    public class LayoutGraphEdgeEditor : UnityEditor.Editor
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
