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
            DrawEditButton();
            DrawProperties();
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Draws the edit button.
        /// </summary>
        private void DrawEditButton()
        {
            if (GUILayout.Button("Edit"))
            {
                var graph = (LayoutGraph)serializedObject.targetObject;
                LayoutGraphWindow.ShowWindow(graph);
            }
        }

        /// <summary>
        /// Draws the graph properties.
        /// </summary>
        private void DrawProperties()
        {
            var prop = serializedObject.GetIterator();
            var enterChildren = true;
            GUI.enabled = false;

            while (prop.NextVisible(enterChildren))
            {
                GUI.enabled = prop.name == "_id" || prop.name == "_name";
                EditorGUILayout.PropertyField(prop, true);
                enterChildren = false;
            }

            GUI.enabled = true;
        }
    }
}