using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Editor
{
    public class NewLayoutGraphWindow : EditorWindow
    {
        private SerializedObject SerializedObject { get; set; }
        private LayoutGraphWindowSettings Settings { get; set; }
        private ILayoutGraphWindowTool ActiveTool { get; set; } = new LayoutGraphWindowSelectTool();
        private bool ShowNodeFoldout { get; set; } = true;
        private bool ShowEdgeFoldout { get; set; } = true;
        private Vector2 PlotScrollPosition { get; set; }
        private Vector2 InspectorScrollPosition { get; set; }
        public HashSet<LayoutNode> SelectedNodes { get; } = new HashSet<LayoutNode>();
        public HashSet<LayoutEdge> SelectedEdges { get; } = new HashSet<LayoutEdge>();
        private Dictionary<int, Vector2> NodePositions { get; } = new Dictionary<int, Vector2>();
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
            ActiveTool.OnGUIEnd();
        }

        private void OnLostFocus()
        {
            ActiveTool.OnLostFocus();
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

        public void ToggleElementSelection(Object element)
        {
            switch (element)
            {
                case LayoutNode node:
                    ToggleNodeSelection(node);
                    break;
                case LayoutEdge edge:
                    ToggleEdgeSelection(edge);
                    break;
            }
        }

        private void ToggleNodeSelection(LayoutNode node)
        {
            if (!SelectedNodes.Add(node))
                SelectedNodes.Remove(node);
        }

        private void ToggleEdgeSelection(LayoutEdge edge)
        {
            if (!SelectedEdges.Add(edge))
                SelectedEdges.Remove(edge);
        }

        public void DrawDragArea(Vector2 dragStart)
        {
            var rect = new Rect(dragStart, Event.current.mousePosition - dragStart);
            Handles.DrawSolidRectangleWithOutline(rect, Settings.DragAreaColor, Settings.DragOutlineColor);
        }

        private void SaveAsset()
        {
            if (LayoutGraphExists())
            {
                SerializedObject.ApplyModifiedProperties();
                AssetDatabase.SaveAssetIfDirty(GetLayoutGraph());
            }
        }

        private void DrawMenu()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            DrawToolsMenu();
            GUILayout.FlexibleSpace();
            DrawMenuAreaButton();
            GUILayout.EndHorizontal();
        }

        private void ChooseSelectTool()
        {
            ActiveTool = new LayoutGraphWindowSelectTool();
        }

        private void ChooseDrawTool()
        {
            ActiveTool = new LayoutGraphWindowDrawTool();
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
                menu.AddItem(new GUIContent("Draw\t2"), ActiveTool is LayoutGraphWindowDrawTool, ChooseDrawTool);
                menu.AddItem(new GUIContent("Move\t3"), ActiveTool is LayoutGraphWindowMoveTool, ChooseMoveTool);
                menu.AddItem(new GUIContent("Pan\t4"), ActiveTool is LayoutGraphWindowPanTool, ChoosePanTool);
                menu.DropDown(new Rect(0, 5, 0, 16));
            }
        }

        private void DrawMenuAreaButton()
        {
            if (GUI.Button(new Rect(0, 0, position.width, Settings.MenuHeight), "", GUIStyle.none))
            {
                GUI.FocusControl(null);
            }
        }

        private void DrawInspector()
        {
            GUILayout.BeginArea(new Rect(position.width - Settings.InspectorWidth, Settings.MenuHeight, Settings.InspectorWidth, position.height - Settings.MenuHeight), GUI.skin.box);
            InspectorScrollPosition = GUILayout.BeginScrollView(InspectorScrollPosition);
            DrawGraphInspector();
            DrawNodeInspector();
            DrawEdgeInspector();
            DrawInspectorAreaButton();
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private void DrawHeadingBox(Rect rect)
        {
            GUI.backgroundColor = Settings.HeadingColor;
            GUI.Box(rect, "", GUI.skin.box);
            GUI.backgroundColor = Color.white;
        }

        private void DrawGraphInspector()
        {
            UnityEditor.Editor.CreateCachedEditor(SerializedObject.targetObject, typeof(LayoutGraphWindowEditor), ref graphEditor);
            EditorGUIUtility.labelWidth = Settings.InspectorLabelWidth;
            EditorGUILayout.LabelField(SerializedObject.targetObject.name);
            DrawHeadingBox(GUILayoutUtility.GetLastRect());
            graphEditor.OnInspectorGUI();
        }

        private void DrawNodeInspector()
        {
            UnityEditor.Editor.CreateCachedEditor(SelectedNodes.ToArray(), null, ref nodeEditor);
            EditorGUILayout.Space();
            EditorGUIUtility.labelWidth = Settings.InspectorLabelWidth;
            ShowNodeFoldout = EditorGUILayout.Foldout(ShowNodeFoldout, "Selected Nodes");
            DrawHeadingBox(GUILayoutUtility.GetLastRect());
            EditorGUI.indentLevel++;

            if (ShowNodeFoldout)
                DrawNodeInspectorFields();

            EditorGUI.indentLevel--;
        }

        private void DrawNodeInspectorFields()
        {
            if (SelectedNodes.Count == 0)
            {
                EditorGUILayout.LabelField("None");
                return;
            }

            var ids = string.Join(", ", SelectedNodes.OrderBy(x => x.Id).Select(x => x.Id));
            EditorGUILayout.LabelField("ID", ids);
            nodeEditor.OnInspectorGUI();
        }

        private void DrawEdgeInspector()
        {
            UnityEditor.Editor.CreateCachedEditor(SelectedEdges.ToArray(), null, ref edgeEditor);
            EditorGUILayout.Space();
            EditorGUIUtility.labelWidth = Settings.InspectorLabelWidth;
            ShowEdgeFoldout = EditorGUILayout.Foldout(ShowEdgeFoldout, "Selected Edges");
            DrawHeadingBox(GUILayoutUtility.GetLastRect());
            EditorGUI.indentLevel++;

            if (ShowEdgeFoldout)
                DrawEdgeInspectorFields();

            EditorGUI.indentLevel--;
        }

        private void DrawEdgeInspectorFields()
        {
            if (SelectedEdges.Count == 0)
            {
                EditorGUILayout.LabelField("None");
                return;
            }

            var ids = string.Join(", ", SelectedEdges.OrderBy(x => new EdgeIndexes(x.FromNode, x.ToNode)).Select(x => $"({x.FromNode}, {x.ToNode})"));
            EditorGUILayout.LabelField("ID", ids);
            edgeEditor.OnInspectorGUI();
        }

        private void DrawInspectorAreaButton()
        {
            if (GUI.Button(new Rect(0, 0, Settings.InspectorWidth, position.height - Settings.MenuHeight), "", GUI.skin.box))
            {
                GUI.FocusControl(null);
            }
        }

        private void DrawPlot()
        {
            GUILayout.BeginArea(new Rect(0, Settings.MenuHeight, position.width - Settings.InspectorWidth, position.height - Settings.MenuHeight));
            PlotScrollPosition = GUILayout.BeginScrollView(PlotScrollPosition);
            SetNodePositions();
            DrawEdgeLines();
            DrawEdges();
            DrawNodes();
            DrawPlotArea();
            ActiveTool.OnDrawPlotEnd();
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private void DrawPlotArea()
        {
            var size = new Vector2(position.width - Settings.InspectorWidth, position.height - Settings.MenuHeight);
            var rect = new Rect(InspectorScrollPosition, size);

            if (rect.Contains(Event.current.mousePosition))
            {
                ActiveTool.OnAreaEvent();
            }
        }

        private void SetNodePositions()
        {
            var graph = GetLayoutGraph();

            foreach (var node in graph.GetNodes())
            {
                NodePositions[node.Id] = node.Position;
            }
        }

        private void DrawEdgeLines()
        {
            var graph = GetLayoutGraph();

            foreach (var edge in graph.GetEdges())
            {
                DrawEdgeLine(edge);
            }
        }

        private void DrawEdges()
        {
            var graph = GetLayoutGraph();

            foreach (var edge in graph.GetEdges())
            {
                DrawEdge(edge);
            }
        }

        private void DrawNodes()
        {
            var graph = GetLayoutGraph();

            foreach (var node in graph.GetNodes())
            {
                DrawNode(node);
            }
        }

        private void DrawEdgeLine(LayoutEdge edge)
        {
            var offset = 0.5f * Settings.NodeSize;
            var fromPosition = NodePositions[edge.FromNode] + offset;
            var toPosition = NodePositions[edge.ToNode] + offset;

            // Draw line.
            Handles.color = edge.Color;
            Handles.DrawLine(fromPosition, toPosition);
            Handles.color = Color.white;
        }

        private void DrawEdge(LayoutEdge edge)
        {
            var fromPosition = NodePositions[edge.FromNode];
            var toPosition = NodePositions[edge.ToNode];
            var position = 0.5f * (fromPosition + toPosition + Settings.NodeSize - Settings.EdgeSize);
            var rect = new Rect(position, Settings.EdgeSize);

            // Draw edge button.
            GUI.backgroundColor = edge.Color;
            GUI.Box(rect, edge.Name, GUI.skin.button);
            GUI.backgroundColor = Color.white;

            if (rect.Contains(Event.current.mousePosition))
            {
                // Draw hover graphic.
                GUI.backgroundColor = Settings.HoverColor;
                GUI.Box(rect, "", GUI.skin.button);
                GUI.backgroundColor = Color.white;
                ActiveTool.OnEdgeEvent(edge);
            }

            if (SelectedEdges.Contains(edge))
            {
                // Draw selected graphic.
                GUI.backgroundColor = Settings.SelectedColor;
                GUI.Box(rect, "", GUI.skin.button);
                GUI.backgroundColor = Color.white;
            }
        }

        private void DrawNode(LayoutNode node)
        {
            var rect = new Rect(node.Position, Settings.NodeSize);

            // Draw node button.
            GUI.backgroundColor = node.Color;
            GUI.Box(rect, $"{node.Id} : {node.Name}", GUI.skin.button);
            GUI.backgroundColor = Color.white;

            if (rect.Contains(Event.current.mousePosition))
            {
                // Draw hover graphic.
                GUI.backgroundColor = Settings.HoverColor;
                GUI.Box(rect, "", GUI.skin.button);
                GUI.backgroundColor = Color.white;
                ActiveTool.OnNodeEvent(node);
            }

            if (SelectedNodes.Contains(node))
            {
                // Draw selected graphic.
                GUI.backgroundColor = Settings.SelectedColor;
                GUI.Box(rect, "", GUI.skin.button);
                GUI.backgroundColor = Color.white;
            }
        }
    }
}