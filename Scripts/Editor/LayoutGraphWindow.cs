using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Editor
{
    public class LayoutGraphWindow : EditorWindow
    {
        private SerializedObject SerializedObject { get; set; }
        private LayoutGraphWindowSettings Settings { get; set; }
        private LayoutGraphPlot Plot { get; set; }
        private Vector2 ScrollPosition { get; set; }
        private UnityEditor.Editor graphEditor;
        private UnityEditor.Editor nodeEditor;
        private UnityEditor.Editor edgeEditor;

        public static void ShowWindow(LayoutGraph graph)
        {
            var window = GetWindow<LayoutGraphWindow>();
            window.titleContent = new GUIContent("Layout Graph Editor");
            window.minSize = new Vector2(450, 200);
            window.maxSize = new Vector2(1920, 720);
            window.SerializedObject = new SerializedObject(graph);
            window.Settings = LayoutGraphWindowSettings.GetSettings();
            window.Plot = new LayoutGraphPlot(graph);
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
            Plot.OnLostFocus();
        }

        private void OnDestroy()
        {
            SaveAsset();
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

        private void SaveAsset()
        {
            if (LayoutGraphExists())
            {
                SerializedObject.ApplyModifiedProperties();
                AssetDatabase.SaveAssetIfDirty(GetLayoutGraph());
            }
        }

        private void DrawPlot()
        {
            var width = position.width - Settings.InspectorWidth;
            var height = position.height - Settings.MenuHeight;
            var rect = new Rect(Settings.InspectorWidth, Settings.MenuHeight, width, height);
            GUILayout.BeginArea(rect);
            Plot.OnGUI();
            GUILayout.EndArea();
        }

        #region Menu
        private void DrawMenu()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            DrawEditMenuButton();
            HandleMenuClick();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private void HandleMenuClick()
        {
            var rect = new Rect(0, 0, position.width, Settings.MenuHeight);

            if (GUI.Button(rect, "", GUIStyle.none))
            {
                GUI.FocusControl(null);
            }
        }

        private void DrawEditMenuButton()
        {
            if (GUILayout.Button("Edit", EditorStyles.toolbarDropDown))
            {
                GUI.FocusControl(null);
                var graph = GetLayoutGraph();
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Create Node"), false, Plot.CreateNode);
                menu.AddSeparator("");

                if (graph.NodeCount > 0 || graph.EdgeCount > 0)
                    menu.AddItem(new GUIContent("Select All"), false, Plot.SelectAllElements);
                else
                    menu.AddDisabledItem(new GUIContent("Select All"));

                if (graph.NodeCount > 0)
                    menu.AddItem(new GUIContent("Select All Nodes"), false, Plot.SelectAllNodes);
                else
                    menu.AddDisabledItem(new GUIContent("Select All Nodes"));

                if (graph.EdgeCount > 0)
                    menu.AddItem(new GUIContent("Select All Edges"), false, Plot.SelectAllEdges);
                else
                    menu.AddDisabledItem(new GUIContent("Select All Edges"));

                menu.AddSeparator("");

                if (Plot.SelectedNodes.Count > 0 || Plot.SelectedEdges.Count > 0)
                    menu.AddItem(new GUIContent("Delete Selected"), false, Plot.DeleteSelectedElements);
                else
                    menu.AddDisabledItem(new GUIContent("Delete Selected"));

                if (Plot.SelectedNodes.Count > 0)
                    menu.AddItem(new GUIContent("Delete Selected Nodes"), false, Plot.DeleteSelectedNodes);
                else
                    menu.AddDisabledItem(new GUIContent("Delete Selected Nodes"));

                if (Plot.SelectedEdges.Count > 0)
                    menu.AddItem(new GUIContent("Delete Selected Edges"), false, Plot.DeleteSelectedEdges);
                else
                    menu.AddDisabledItem(new GUIContent("Delete Selected Edges"));

                menu.DropDown(new Rect(0, 5, 0, 16));
            }
        }
        #endregion

        #region Inspector
        private void DrawInspector()
        {
            var rect = new Rect(0, Settings.MenuHeight, Settings.InspectorWidth, position.height - Settings.MenuHeight);
            GUILayout.BeginArea(rect, GUI.skin.box);
            ScrollPosition = GUILayout.BeginScrollView(ScrollPosition);
            EditorGUIUtility.labelWidth = Settings.InspectorLabelWidth;
            DrawGraphInspector();
            DrawHorizontalSeparator();
            DrawNodeInspector();
            DrawEdgeInspector();
            HandleInspectorClick();
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private void HandleInspectorClick()
        {
            var rect = new Rect(0, 0, Settings.InspectorWidth, position.height - Settings.MenuHeight);

            if (GUI.Button(rect, "", GUIStyle.none))
            {
                GUI.FocusControl(null);
            }
        }

        private void DrawHorizontalSeparator()
        {
            var style = new GUIStyle(GUI.skin.horizontalSlider);
            style.fixedHeight = 1;
            EditorGUILayout.LabelField("", style);
        }

        private void DrawGraphInspector()
        {
            var graph = GetLayoutGraph();
            UnityEditor.Editor.CreateCachedEditor(graph, typeof(LayoutGraphWindowEditor), ref graphEditor);
            graphEditor.OnInspectorGUI();
        }

        private void DrawNodeInspector()
        {
            UnityEditor.Editor.CreateCachedEditor(Plot.SelectedNodes.ToArray(), null, ref nodeEditor);

            if (Plot.SelectedNodes.Count > 0)
            {
                GUILayout.Label("Selected Nodes", EditorStyles.boldLabel);
                GUI.enabled = false;
                EditorGUILayout.TextField("Id", string.Join(", ", Plot.SelectedNodes.Select(x => x.Id)));
                GUI.enabled = true;
                nodeEditor.OnInspectorGUI();
            }
        }

        private void DrawEdgeInspector()
        {
            UnityEditor.Editor.CreateCachedEditor(Plot.SelectedEdges.ToArray(), null, ref edgeEditor);

            if (Plot.SelectedEdges.Count > 0)
            {
                GUILayout.Label("Selected Edges", EditorStyles.boldLabel);
                GUI.enabled = false;
                EditorGUILayout.TextField("Id", string.Join(", ", Plot.SelectedEdges.Select(x => $"({x.FromNode}, {x.ToNode})")));
                GUI.enabled = true;
                edgeEditor.OnInspectorGUI();
            }
        }
        #endregion
    }
}
