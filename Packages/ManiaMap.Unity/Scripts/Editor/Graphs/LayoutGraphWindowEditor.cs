using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Graphs.Editor
{
    /// <summary>
    /// The LayoutGraphWindow editor.
    /// </summary>
    public class LayoutGraphWindowEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var prop = serializedObject.GetIterator();
            prop.NextVisible(true);
            GUI.enabled = true;

            while (prop.NextVisible(false))
            {
                EditorGUILayout.PropertyField(prop, true);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
