using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Editor
{
    public class LayoutGraphWindow : EditorWindow
    {
        private const int NodePropertyCount = 5;
        private const int EdgePropertyCount = 9;
        private const float InspectorWidth = 350;
        private static Color32 InspectorBackgroundColor { get; } = new Color32(45, 45, 45, 255);
        private static SerializedObject SerializedObject { get; set; }

        /// <summary>
        /// The selected element ID.
        /// </summary>
        private Uid SelectionId { get; set; }

        private Vector2 InspectorScrollPosition { get; set; }

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
            // If the layout graph does not exist, such as the Scriptable Object being destroyed,
            // then close the window.
            if (!LayoutGraphExists())
            {
                Close();
                return;
            }

            SerializedObject.Update();
            DrawInspector();
            SerializedObject.ApplyModifiedProperties();
        }

        private static bool LayoutGraphExists()
        {
            return SerializedObject != null
                && GetLayoutGraph() != null;
        }

        private static LayoutGraph GetLayoutGraph()
        {
            return SerializedObject.targetObject as LayoutGraph;
        }

        private void DrawInspector()
        {
            GUILayout.BeginArea(new Rect(0, 0, InspectorWidth, position.height));
            EditorGUI.DrawRect(new Rect(0, 0, InspectorWidth, position.height), InspectorBackgroundColor);
            InspectorScrollPosition = GUILayout.BeginScrollView(InspectorScrollPosition, GUILayout.Width(InspectorWidth), GUILayout.Height(position.height));

            if (SelectionId.Value3 == 1)
                DrawEdgeInspector();
            else
                DrawNodeInspector();

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private void DrawNodeInspector()
        {
            var graph = GetLayoutGraph();
            var index = graph.GetNodeIndex(SelectionId.Value1);

            if (GUILayout.Button("Add Node"))
            {
                var node = graph.CreateNode();
                SelectionId = node.RoomId;
                return;
            }

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
            }
        }

        private void DrawEdgeInspector()
        {
            var graph = GetLayoutGraph();
            var index = graph.GetEdgeIndex(SelectionId.Value1, SelectionId.Value2);

            if (GUILayout.Button("Add Node"))
            {
                var node = graph.CreateNode();
                SelectionId = node.RoomId;
                return;
            }

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
                GUI.enabled = prop.name != "_fromNode" || prop.name != "_toNode";
                EditorGUILayout.PropertyField(prop, true);
                prop.NextVisible(false);
            }
        }
    }
}