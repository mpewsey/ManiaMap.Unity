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
            DrawDefaultInspector();
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
        /// Shows the layout graph editor window.
        /// </summary>
        [UnityEditor.Callbacks.OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            var path = AssetDatabase.GetAssetPath(instanceId);
            var graph = AssetDatabase.LoadAssetAtPath<LayoutGraph>(path);

            if (graph == null)
                return false;

            LayoutGraphWindow.ShowWindow(graph);
            return true;
        }
    }
}