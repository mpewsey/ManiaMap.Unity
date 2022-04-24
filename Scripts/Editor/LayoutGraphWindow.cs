using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Editor
{
    public class LayoutGraphWindow : EditorWindow
    {
        private static SerializedObject SerializedObject { get; set; }

        private Uid SelectionId { get; set; }

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
