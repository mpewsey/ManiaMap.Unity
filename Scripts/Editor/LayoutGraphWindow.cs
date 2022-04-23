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
        private const int NodePropertyCount = 5;

        /// <summary>
        /// The number of visible edge properties.
        /// </summary>
        private const int EdgePropertyCount = 9;

        /// <summary>
        /// The width of the inspector pane.
        /// </summary>
        private const float InspectorWidth = 350;

        /// <summary>
        /// The inspector pane background color.
        /// </summary>
        private static Color32 InspectorBackgroundColor { get; } = new Color32(45, 45, 45, 255);

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
            SerializedObject.ApplyModifiedProperties();
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
            GUILayout.BeginArea(new Rect(0, 0, InspectorWidth, position.height));
            EditorGUI.DrawRect(new Rect(0, 0, InspectorWidth, position.height), InspectorBackgroundColor);
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

            EditorGUILayout.LabelField(string.Empty);
            EditorGUILayout.LabelField("Selected Node", EditorStyles.boldLabel);
            var prop = SerializedObject.FindProperty("_nodes").GetArrayElementAtIndex(index);
            prop.NextVisible(true);

            for (int i = 0; i < NodePropertyCount; i++)
            {
                GUI.enabled = prop.name != "_id";
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