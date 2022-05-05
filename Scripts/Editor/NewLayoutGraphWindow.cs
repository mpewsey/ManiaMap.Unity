using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Editor
{
    public class NewLayoutGraphWindow : EditorWindow
    {
        private SerializedObject SerializedObject { get; set; }
        private LayoutGraphWindowSettings Settings { get; set; }
        private ILayoutGraphWindowTool ActiveTool { get; set; } = new LayoutGraphWindowSelectTool();
        private Vector2 PlotScrollPosition { get; set; }
        private Vector2 InspectorScrollPosition { get; set; }
        public HashSet<LayoutNode> SelectedNodes { get; } = new HashSet<LayoutNode>();
        public HashSet<LayoutEdge> SelectedEdges { get; } = new HashSet<LayoutEdge>();
        private UnityEditor.Editor graphEditor;
        private UnityEditor.Editor nodeEditor;
        private UnityEditor.Editor edgeEditor;

        public static void ShowWindow(LayoutGraph graph)
        {
            var window = GetWindow<NewLayoutGraphWindow>("Layout Graph");
            window.minSize = new Vector2(450, 200);
            window.maxSize = new Vector2(1920, 720);
            window.SerializedObject = new SerializedObject(graph);
            window.Settings = LayoutGraphWindowSettings.GetSettings();
        }

        private void OnGUI()
        {
            if (!LayoutGraphExists())
                return;

            DrawMenu();
            DrawInspector();
            DrawPlot();
        }

        private void OnLostFocus()
        {

        }

        private bool LayoutGraphExists()
        {
            return SerializedObject != null
                && GetLayoutGraph() != null;
        }

        private LayoutGraph GetLayoutGraph()
        {
            return SerializedObject.targetObject as LayoutGraph;
        }

        private void DrawMenu()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            DrawToolsMenu();
            DrawEditMenu();
            GUILayout.FlexibleSpace();
            DrawMenuAreaButton();
            GUILayout.EndHorizontal();
        }

        private void ChooseSelectTool()
        {
            ActiveTool = new LayoutGraphWindowSelectTool();
        }

        private void ChooseEditTool()
        {
            ActiveTool = new LayoutGraphWindowEditTool();
        }

        private void ChooseMoveTool()
        {
            ActiveTool = new LayoutGraphWindowMoveTool();
        }

        private void ChoosePanTool()
        {
            ActiveTool = new LayoutGraphWindowPanTool();
        }

        private void DrawToolsMenu()
        {
            if (GUILayout.Button("Tools", EditorStyles.toolbarDropDown))
            {
                GUI.FocusControl(null);
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Select\t1"), ActiveTool is LayoutGraphWindowSelectTool, ChooseSelectTool);
                menu.AddItem(new GUIContent("Edit\t2"), ActiveTool is LayoutGraphWindowEditTool, ChooseEditTool);
                menu.AddItem(new GUIContent("Move\t3"), ActiveTool is LayoutGraphWindowMoveTool, ChooseMoveTool);
                menu.AddItem(new GUIContent("Pan\t4"), ActiveTool is LayoutGraphWindowPanTool, ChoosePanTool);
                menu.DropDown(new Rect(0, 5, 0, 16));
            }
        }

        private void ChooseCreateNode()
        {

        }

        private void ChooseSelectAll()
        {

        }

        private void ChooseDeleteSelected()
        {

        }

        private void DrawEditMenu()
        {
            if (GUILayout.Button("Edit", EditorStyles.toolbarDropDown))
            {
                GUI.FocusControl(null);
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Create Node"), false, ChooseCreateNode);
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Select All\tCtrl+A"), false, ChooseSelectAll);
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Delete Selected\tDelete"), false, ChooseDeleteSelected);
                menu.DropDown(new Rect(50, 5, 0, 16));
            }
        }

        private void DrawMenuAreaButton()
        {
            var rect = new Rect(0, 0, position.width, Settings.MenuHeight);
            var content = new GUIContent("", "Menu Area");

            if (GUI.Button(rect, content, GUIStyle.none))
            {
                GUI.FocusControl(null);
            }
        }

        private void DrawInspector()
        {
            GUILayout.BeginArea(new Rect(position.width - Settings.InspectorWidth, Settings.MenuHeight, Settings.InspectorWidth, position.height - Settings.MenuHeight), GUI.skin.box);
            InspectorScrollPosition = GUILayout.BeginScrollView(InspectorScrollPosition);
            DrawGraphInspector();
            DrawInspectorAreaButton();
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private void DrawGraphInspector()
        {
            UnityEditor.Editor.CreateCachedEditor(SerializedObject.targetObject, typeof(LayoutGraphWindowEditor), ref graphEditor);
            graphEditor.OnInspectorGUI();
        }

        private void DrawInspectorAreaButton()
        {
            var rect = new Rect(0, 0, Settings.InspectorWidth, position.height - Settings.MenuHeight);
            var content = new GUIContent("", "Inspector Area");

            if (GUI.Button(rect, content, GUI.skin.box))
            {
                GUI.FocusControl(null);
            }
        }

        private void DrawPlot()
        {
            GUILayout.BeginArea(new Rect(0, Settings.MenuHeight, position.width - Settings.InspectorWidth, position.height - Settings.MenuHeight));
            PlotScrollPosition = GUILayout.BeginScrollView(PlotScrollPosition);
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }
    }
}