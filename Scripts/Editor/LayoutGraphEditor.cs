using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Editor
{
    /// <summary>
    /// The LayoutGraph custom inspector.
    /// </summary>
    [CustomEditor(typeof(LayoutGraph))]
    public class LayoutGraphEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Add edit button at top of view.
            if (GUILayout.Button("Edit"))
            {
                var graph = (LayoutGraph)serializedObject.targetObject;
                LayoutGraphWindow.ShowWindow(graph);
            }

            // Loop over serialized fields and draw them.
            var prop = serializedObject.GetIterator();
            var enterChildren = true;

            while (prop.NextVisible(enterChildren))
            {
                GUI.enabled = prop.name == "_id" || prop.name == "_name";
                EditorGUILayout.PropertyField(prop, true);
                enterChildren = false;
                GUI.enabled = true;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}