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
            // Update the object.
            serializedObject.Update();

            // Add edit button at top of view.
            if (GUILayout.Button("Edit"))
                LayoutGraphWindow.ShowWindow(serializedObject);

            // Loop over serialized fields and draw them.
            var prop = serializedObject.GetIterator();
            var enterChildren = true;

            while (prop.NextVisible(enterChildren))
            {
                GUI.enabled = GUIEnabledProperty(prop.displayName);
                EditorGUILayout.PropertyField(prop, true);
                enterChildren = false;
            }

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Returns true if the property name is GUI enabled.
        /// </summary>
        /// <param name="name">The property name.</param>
        private static bool GUIEnabledProperty(string name)
        {
            return name == "Id"
                || name == "Name";
        }
    }
}