using MPewsey.ManiaMap.Graphs;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Graphs.Editor
{
    /// <summary>
    /// The LayoutGraph custom inspector.
    /// </summary>
    [CustomEditor(typeof(LayoutGraphObject))]
    public class LayoutGraphObjectEditor : UnityEditor.Editor
    {
        /// <summary>
        /// Shows the layout graph editor window.
        /// </summary>
        [UnityEditor.Callbacks.OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            var path = AssetDatabase.GetAssetPath(instanceId);
            var graph = AssetDatabase.LoadAssetAtPath<LayoutGraphObject>(path);

            if (graph == null)
                return false;

            LayoutGraphWindow.ShowWindow(graph);
            return true;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawEditButton();
            DrawDefaultInspector();
            EditorGUILayout.Space();
            DrawNodeTemplateGroupErrorBox(GetLayoutGraph());
            DrawEdgeTemplateGroupErrorBox(GetLayoutGraph());
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Returns the target layout graph.
        /// </summary>
        private LayoutGraphObject GetLayoutGraph()
        {
            return (LayoutGraphObject)serializedObject.targetObject;
        }

        /// <summary>
        /// Draws the edit button.
        /// </summary>
        private void DrawEditButton()
        {
            if (GUILayout.Button("Edit"))
                LayoutGraphWindow.ShowWindow(GetLayoutGraph());
        }

        /// <summary>
        /// Draws an error box if any nodes in the graph do not have template groups assigned.
        /// </summary>
        public static void DrawNodeTemplateGroupErrorBox(LayoutGraphObject graph)
        {
            var messages = new List<string>() { "Nodes are missing template groups:" };

            foreach (var node in graph.GetNodes().OrderBy(x => x.Id))
            {
                if (node.TemplateGroup == null)
                    messages.Add($"  * {node.Id} : {node.Name}");
            }

            if (messages.Count <= 1)
                return;

            EditorGUILayout.HelpBox(string.Join('\n', messages), MessageType.Error, true);
        }

        /// <summary>
        /// Draws an error box if any edges in the graph with non-zero room chances do not
        /// have template groups assigned.
        /// </summary>
        public static void DrawEdgeTemplateGroupErrorBox(LayoutGraphObject graph)
        {
            var messages = new List<string>() { "Edges with non-zero room chances are missing template groups:" };

            foreach (var edge in graph.GetEdges().OrderBy(x => new EdgeIndexes(x.FromNode, x.ToNode)))
            {
                if (edge.RoomChance > 0 && edge.TemplateGroup == null)
                    messages.Add($"  * ({edge.FromNode}, {edge.ToNode}) : {edge.Name}");
            }

            if (messages.Count <= 1)
                return;

            EditorGUILayout.HelpBox(string.Join('\n', messages), MessageType.Error, true);
        }
    }
}