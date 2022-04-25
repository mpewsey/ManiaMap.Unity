using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Editor
{
    public class LayoutGraphWindow : EditorWindow
    {
        private const float MenuHeight = 20;
        private const int NodePropertyCount = 5;
        private const int EdgePropertyCount = 8;
        
        private static Vector2 NodeSize { get; } = new Vector2(275, 150);
        private static Vector2 EdgeSize { get; } = new Vector2(300, 300);
        private static SerializedObject SerializedObject { get; set; }

        private Vector2 ScrollPosition { get; set; }
        private bool Dragging { get; set; }

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
                menu.DropDown(new Rect(0, 5, 0, 16));
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

        private void DrawEdge(SerializedProperty prop)
        {
            GUI.color = prop.FindPropertyRelative("_color").colorValue;
            var position = Vector2.zero;
            GUILayout.BeginArea(new Rect(position, EdgeSize), GUI.skin.window);
            GUI.color = Color.white;
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
        }

        private void DrawNode(SerializedProperty prop)
        {
            GUI.color = prop.FindPropertyRelative("_color").colorValue;
            var position = prop.FindPropertyRelative("_position").vector2Value;
            GUILayout.BeginArea(new Rect(position, NodeSize), GUI.skin.window);
            GUI.color = Color.white;
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
        }

        private void ProcessMouseEvents()
        {
            const int LeftMouseButton = 0;
            const int RightMouseButton = 1;
            var window = new Rect(0, MenuHeight, position.width, position.height);

            if (!window.Contains(Event.current.mousePosition))
                return;

            if (Event.current.type == EventType.MouseDown)
            {
                if (Event.current.button == LeftMouseButton)
                {
                    if (Dragging)
                    {

                    }
                    else
                    {

                    }
                }
                else if (Event.current.button == RightMouseButton)
                {
                    DrawContextMenu();
                }
            }
            else if (Event.current.type == EventType.MouseUp)
            {
                CancelDragging();
            }
        }

        private void CancelDragging()
        {
            Dragging = false;
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
