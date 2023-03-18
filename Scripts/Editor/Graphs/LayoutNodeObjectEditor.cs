using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Graphs.Editor
{
    /// <summary>
    /// The LayoutNode editor.
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(LayoutNodeObject))]
    public class LayoutNodeObjectEditor : UnityEditor.Editor
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