using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Editor
{
    public class LayoutGraphWindow : EditorWindow
    {
        private const int NodePropertyCount = 5;
        private const int EdgePropertyCount = 8;
        private const float NodePadding = 4;
        private const float EdgePadding = 4;
        private static Vector2 NodeSize { get; } = new Vector2(300, 150);
        private static Vector2 EdgeSize { get; } = new Vector2(300, 300);
        private static SerializedObject SerializedObject { get; set; }

        private Vector2 ScrollPosition { get; set; }
        private List<Rect> NodeRects { get; } = new List<Rect>();
        private List<Rect> EdgeRects { get; } = new List<Rect>();

        private static LayoutGraph GetLayoutGraph()
        {
            return SerializedObject.targetObject as LayoutGraph;
        }

        public static void ShowWindow(LayoutGraph graph)
        {
            SerializedObject = new SerializedObject(graph);
            var window = GetWindow<LayoutGraphWindow>();
            window.titleContent = new GUIContent("Graph");
            window.minSize = new Vector2(600, 200);
            window.maxSize = new Vector2(1920, 720);
        }

        private static bool LayoutGraphExists()
        {
            return SerializedObject != null
                && GetLayoutGraph() != null;
        }

        public void OnGUI()
        {
            if (!LayoutGraphExists())
            {
                Close();
                return;
            }
            
            SerializedObject.Update();
            SetWindowTitle();
            DrawMenuBar();
            DrawGraph();
            ProcessMouseEvents();
            SerializedObject.ApplyModifiedProperties();
        }

        private void SetWindowTitle()
        {
            var name = SerializedObject.targetObject.name;
            titleContent.text = $"Graph [{name}]";
        }

        private void DrawMenuBar()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            DrawEditMenuButton();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private void DrawEditMenuButton()
        {
            if (GUILayout.Button("Edit", EditorStyles.toolbarDropDown))
            {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Create Node"), false, CreateNode);
                menu.DropDown(new Rect(0, 8, 0, 16));
            }
        }

        private void DrawGraph()
        {
            ScrollPosition = GUILayout.BeginScrollView(ScrollPosition);
            DrawEdges();
            DrawNodes();
            GUILayout.EndScrollView();
        }

        private void DrawEdges()
        {
            var edges = SerializedObject.FindProperty("_edges");

            for (int i = 0; i < edges.arraySize; i++)
            {
                var prop = edges.GetArrayElementAtIndex(i);
                DrawEdge(prop);
            }
        }

        private void DrawNodes()
        {
            var nodes = SerializedObject.FindProperty("_nodes");

            for (int i = 0; i < nodes.arraySize; i++)
            {
                var prop = nodes.GetArrayElementAtIndex(i);
                DrawNode(prop);
            }
        }

        private Rect InnerEdgeRect()
        {
            var position = EdgePadding * Vector2.one;
            var size = EdgeSize - 2 * position;
            return new Rect(position, size);
        }

        private Rect InnerNodeRect()
        {
            var position = NodePadding * Vector2.one;
            var size = NodeSize - 2 * position;
            return new Rect(position, size);
        }

        private void DrawEdge(SerializedProperty prop)
        {
            var position = Vector2.zero;
            var color = prop.FindPropertyRelative("_color").colorValue;
            GUILayout.BeginArea(new Rect(position, NodeSize));
            EditorGUI.DrawRect(new Rect(Vector2.zero, NodeSize), color);
            GUILayout.BeginArea(InnerEdgeRect(), GUI.skin.window);
            EditorGUIUtility.labelWidth = 100;
            var enterChildren = true;

            for (int i = 0; i < EdgePropertyCount; i++)
            {
                prop.NextVisible(enterChildren);
                GUI.enabled = prop.name != "_fromNode" && prop.name != "_toNode";
                EditorGUILayout.PropertyField(prop, true);
                enterChildren = false;
            }

            GUI.enabled = true;
            GUILayout.EndArea();
            GUILayout.EndArea();
        }

        private void DrawNode(SerializedProperty prop)
        {
            var position = prop.FindPropertyRelative("_position").vector2Value;
            var color = prop.FindPropertyRelative("_color").colorValue;
            GUILayout.BeginArea(new Rect(position, NodeSize));
            EditorGUI.DrawRect(new Rect(Vector2.zero, NodeSize), color);
            GUILayout.BeginArea(InnerNodeRect(), GUI.skin.window);
            EditorGUIUtility.labelWidth = 100;
            var enterChildren = true;

            for (int i = 0; i < NodePropertyCount; i++)
            {
                prop.NextVisible(enterChildren);
                GUI.enabled = prop.name != "_id" && prop.name != "_position";
                EditorGUILayout.PropertyField(prop, true);
                enterChildren = false;
            }

            GUI.enabled = true;
            GUILayout.EndArea();
            GUILayout.EndArea();
        }

        private void ProcessMouseEvents()
        {
            const int RightMouseButton = 1;
            var window = new Rect(0, 0, position.width, position.height);

            if (!window.Contains(Event.current.mousePosition))
                return;

            if (Event.current.button == RightMouseButton)
                DrawContextMenu();
        }

        private void DrawContextMenu()
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Create Node"), false, CreateNode);
            menu.AddItem(new GUIContent("Create Edge"), false, () => Debug.Log("Create Edge"));
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Delete Node"), false, () => Debug.Log("Delete Node"));
            menu.AddItem(new GUIContent("Delete Edge"), false, () => Debug.Log("Delete Edge"));
            menu.DropDown(new Rect(Event.current.mousePosition, Vector2.zero));
        }

        private void CreateNode()
        {
            GetLayoutGraph().CreateNode();
        }
    }
}
