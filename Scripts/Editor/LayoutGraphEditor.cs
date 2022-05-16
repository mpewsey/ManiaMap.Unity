using System.Collections.Generic;
using System.Linq;
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
            EditorGUILayout.Space();
            DrawNodeTemplateGroupWarningBox(GetLayoutGraph());
            DrawEdgeTemplateGroupWarningBox(GetLayoutGraph());
            serializedObject.ApplyModifiedProperties();
        }

        private LayoutGraph GetLayoutGraph()
        {
            return (LayoutGraph)serializedObject.targetObject;
        }

        /// <summary>
        /// Draws the edit button.
        /// </summary>
        private void DrawEditButton()
        {
            if (GUILayout.Button("Edit"))
            {
                LayoutGraphWindow.ShowWindow(GetLayoutGraph());
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

        /// <summary>
        /// Draws a warning box if any nodes in the graph do not have template groups assigned.
        /// </summary>
        public static void DrawNodeTemplateGroupWarningBox(LayoutGraph graph)
        {
            var messages = new List<string>();
            messages.Add("Nodes are missing template groups:");

            foreach (var node in graph.GetNodes().OrderBy(x => x.Id))
            {
                if (node.TemplateGroup == null)
                    messages.Add($"  * {node.Id} : {node.Name}");
            }

            if (messages.Count > 1)
                EditorGUILayout.HelpBox(string.Join('\n', messages), MessageType.Warning, true);
        }

        /// <summary>
        /// Draws a warning box if any edges in the graph with non-zero room chances do not
        /// have template groups assigned.
        /// </summary>
        public static void DrawEdgeTemplateGroupWarningBox(LayoutGraph graph)
        {
            var messages = new List<string>();
            messages.Add("Edges with non-zero room chances are missing template groups:");

            foreach (var edge in graph.GetEdges().OrderBy(x => new EdgeIndexes(x.FromNode, x.ToNode)))
            {
                if (edge.RoomChance > 0 && edge.TemplateGroup == null)
                    messages.Add($"  * ({edge.FromNode}, {edge.ToNode}) : {edge.Name}");
            }

            if (messages.Count > 1)
                EditorGUILayout.HelpBox(string.Join('\n', messages), MessageType.Warning, true);
        }
    }
}