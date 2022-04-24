using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Editor
{
    /// <summary>
    /// A custom editor window for the LayoutGraph.
    /// </summary>
    public class LayoutGraphWindow : EditorWindow
    {
        /// <summary>
        /// The number of visible node properties.
        /// </summary>
        private const int NodePropertyCount = 6;

        /// <summary>
        /// The number of visible edge properties.
        /// </summary>
        private const int EdgePropertyCount = 9;

        /// <summary>
        /// The width of the inspector pane.
        /// </summary>
        private const float InspectorWidth = 325;

        /// <summary>
        /// The width of the inspector labels.
        /// </summary>
        private const float InspectorLabelWidth = 100;

        /// <summary>
        /// The size of a plotted node.
        /// </summary>
        private static Vector2 NodeSize { get; } = new Vector2(250, 80);

        /// <summary>
        /// The target serialized object.
        /// </summary>
        private static SerializedObject SerializedObject { get; set; }

        /// <summary>
        /// The selected element ID.
        /// </summary>
        private Uid SelectionId { get; set; }

        /// <summary>
        /// The current scroll position of the inspector pane.
        /// </summary>
        private Vector2 InspectorScrollPosition { get; set; }

        /// <summary>
        /// A list of rects for plotted node elements.
        /// </summary>
        private List<Rect> NodeRects { get; } = new List<Rect>();

        /// <summary>
        /// Shows the editor window for the layout graph.
        /// </summary>
        /// <param name="graph">The layout graph.</param>
        public static void ShowWindow(LayoutGraph graph)
        {
            SerializedObject = new SerializedObject(graph);
            var window = GetWindow<LayoutGraphWindow>();
            window.titleContent = new GUIContent("Layout Graph Editor");
            window.minSize = new Vector2(600, 200);
            window.maxSize = new Vector2(1920, 720);
        }

        public void OnGUI()
        {
            if (!LayoutGraphExists())
            {
                Close();
                return;
            }

            SerializedObject.Update();
            DrawInspector();
            DrawNodes();
            SerializedObject.ApplyModifiedProperties();
        }

        private void DrawNodes()
        {
            NodeRects.Clear();
            var graph = GetLayoutGraph();
            GUILayout.BeginArea(new Rect(InspectorWidth, 0, position.width - InspectorWidth, position.height));
            EditorGUIUtility.labelWidth = 100;

            foreach (var node in graph.GetNodes())
            {
                var rect = new Rect(node.Position, NodeSize);
                NodeRects.Add(rect);
                GUILayout.BeginArea(rect);
                EditorGUI.DrawRect(rect, node.Color);
                GUILayout.FlexibleSpace();
                GUI.enabled = false;
                EditorGUILayout.IntField("Id", node.Id);
                EditorGUILayout.TextField("Name", node.Name);
                EditorGUILayout.ObjectField("Template Group", node.TemplateGroup, typeof(TemplateGroup), false);
                GUI.enabled = true;
                GUILayout.FlexibleSpace();
                GUILayout.EndArea();
            }

            GUILayout.EndArea();
        }

        /// <summary>
        /// Returns true if the serialized object and target layout graph exist.
        /// </summary>
        private static bool LayoutGraphExists()
        {
            return SerializedObject != null
                && GetLayoutGraph() != null;
        }

        /// <summary>
        /// Returns the target layout graph.
        /// </summary>
        private static LayoutGraph GetLayoutGraph()
        {
            return SerializedObject.targetObject as LayoutGraph;
        }

        /// <summary>
        /// Draws the inspector pane.
        /// </summary>
        private void DrawInspector()
        {
            GUILayout.BeginArea(new Rect(0, 0, InspectorWidth, position.height), GUI.skin.box);
            InspectorScrollPosition = GUILayout.BeginScrollView(InspectorScrollPosition, GUILayout.Width(InspectorWidth), GUILayout.Height(position.height));

            if (SelectionId.Value3 == 0)
                DrawNodeInspector();
            else
                DrawEdgeInspector();

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        /// <summary>
        /// Draws the node inspector pane.
        /// </summary>
        private void DrawNodeInspector()
        {
            var graph = GetLayoutGraph();

            if (GUILayout.Button("Add Node"))
            {
                var node = graph.CreateNode();
                SelectionId = node.RoomId;
                return;
            }

            var index = graph.GetNodeIndex(SelectionId.Value1);

            if (index < 0)
                return;

            if (GUILayout.Button("Remove Node"))
            {
                graph.RemoveNode(SelectionId.Value1);
                return;
            }

            EditorGUIUtility.labelWidth = InspectorLabelWidth;
            EditorGUILayout.LabelField(string.Empty);
            EditorGUILayout.LabelField("Selected Node", EditorStyles.boldLabel);
            var prop = SerializedObject.FindProperty("_nodes").GetArrayElementAtIndex(index);
            prop.NextVisible(true);

            for (int i = 0; i < NodePropertyCount; i++)
            {
                GUI.enabled = prop.name != "_id" && prop.name != "_position";
                EditorGUILayout.PropertyField(prop, true);
                prop.NextVisible(false);
                GUI.enabled = true;
            }
        }

        /// <summary>
        /// Draws the edge inspector pane.
        /// </summary>
        private void DrawEdgeInspector()
        {
            var graph = GetLayoutGraph();

            if (GUILayout.Button("Add Node"))
            {
                var node = graph.CreateNode();
                SelectionId = node.RoomId;
                return;
            }

            var index = graph.GetEdgeIndex(SelectionId.Value1, SelectionId.Value2);

            if (index < 0)
                return;

            if (GUILayout.Button("Remove Edge"))
            {
                graph.RemoveEdge(SelectionId.Value1, SelectionId.Value2);
                return;
            }

            EditorGUIUtility.labelWidth = InspectorLabelWidth;
            EditorGUILayout.LabelField(string.Empty);
            EditorGUILayout.LabelField("Selected Edge", EditorStyles.boldLabel);
            var prop = SerializedObject.FindProperty("_edges").GetArrayElementAtIndex(index);
            prop.NextVisible(true);

            for (int i = 0; i < EdgePropertyCount; i++)
            {
                GUI.enabled = prop.name != "_fromNode" && prop.name != "_toNode";
                EditorGUILayout.PropertyField(prop, true);
                prop.NextVisible(false);
                GUI.enabled = true;
            }
        }
    }
}