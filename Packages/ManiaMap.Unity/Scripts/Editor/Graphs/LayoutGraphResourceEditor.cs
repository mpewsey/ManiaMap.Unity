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
    [CustomEditor(typeof(LayoutGraphResource))]
    public class LayoutGraphResourceEditor : UnityEditor.Editor
    {
        /// <summary>
        /// Shows the layout graph editor window.
        /// </summary>
        [UnityEditor.Callbacks.OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            var path = AssetDatabase.GetAssetPath(instanceId);
            var graph = AssetDatabase.LoadAssetAtPath<LayoutGraphResource>(path);

            if (graph == null)
                return false;

            LayoutGraphWindow.ShowWindow(graph);
            return true;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var graph = (LayoutGraphResource)serializedObject.targetObject;

            if (GUILayout.Button("Edit"))
                LayoutGraphWindow.ShowWindow(graph);

            DrawDefaultInspector();
            EditorGUILayout.Space();
            DrawNodeTemplateGroupErrorBox(graph);
            DrawEdgeTemplateGroupErrorBox(graph);
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Draws an error box if any nodes in the graph do not have template groups assigned.
        /// </summary>
        public static void DrawNodeTemplateGroupErrorBox(LayoutGraphResource graph)
        {
            var messages = new List<string>() { "Nodes are missing template groups:" };

            foreach (var node in graph.GetNodes().OrderBy(x => x.Id))
            {
                if (node.TemplateGroup == null)
                    messages.Add($"  * {node.Id} : {node.Name}");
            }

            if (messages.Count > 1)
                EditorGUILayout.HelpBox(string.Join('\n', messages), MessageType.Error, true);
        }

        /// <summary>
        /// Draws an error box if any edges in the graph with non-zero room chances do not
        /// have template groups assigned.
        /// </summary>
        public static void DrawEdgeTemplateGroupErrorBox(LayoutGraphResource graph)
        {
            var messages = new List<string>() { "Edges with non-zero room chances are missing template groups:" };

            foreach (var edge in graph.GetEdges().OrderBy(x => new EdgeIndexes(x.FromNode, x.ToNode)))
            {
                if (edge.RoomChance > 0 && edge.TemplateGroup == null)
                    messages.Add($"  * ({edge.FromNode}, {edge.ToNode}) : {edge.Name}");
            }

            if (messages.Count > 1)
                EditorGUILayout.HelpBox(string.Join('\n', messages), MessageType.Error, true);
        }
    }
}