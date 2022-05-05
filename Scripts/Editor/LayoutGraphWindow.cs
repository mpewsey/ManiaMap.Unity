using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Editor
{
    public class LayoutGraphWindow : EditorWindow
    {
        const int LeftMouseButton = 0;
        const int RightMouseButton = 1;

        private SerializedObject SerializedObject { get; set; }
        private LayoutGraphWindowSettings Settings { get; set; }
        private bool Dragging { get; set; }
        private bool CreatingEdge { get; set; }
        private LayoutNode StartNode { get; set; }
        private Vector2 InspectorScrollPosition { get; set; }
        private Vector2 PlotScrollPosition { get; set; }
        private Vector2 LastMousePosition { get; set; }
        private HashSet<LayoutNode> SelectedNodes { get; } = new HashSet<LayoutNode>();
        private HashSet<LayoutEdge> SelectedEdges { get; } = new HashSet<LayoutEdge>();
        private Dictionary<int, Vector2> NodePositions { get; } = new Dictionary<int, Vector2>();
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
        }

        private void OnGUI()
        {
            if (!LayoutGraphExists())
                return;

            DrawMenu();
            DrawInspector();
            DrawPlot();
            RepaintIfEditing();
        }

        private void OnLostFocus()
        {
            Dragging = false;
            CreatingEdge = false;
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

        private void RepaintIfEditing()
        {
            if (Dragging || CreatingEdge)
            {
                Repaint();
            }
        }

        #region Plot
        private void DrawPlot()
        {
            GUILayout.BeginArea(new Rect(Settings.InspectorWidth, Settings.MenuHeight, position.width - Settings.InspectorWidth, position.height - Settings.MenuHeight));
            PlotScrollPosition = GUILayout.BeginScrollView(PlotScrollPosition);
            PaginateGraph();
            SetNodePositions();
            DrawEdgeLines();
            DrawNodes();
            DrawEdges();
            SetPlotBounds();
            HandlePlotAreaExit();
            HandlePlotAreaClick();
            DrawEdgeMouseLine();
            LastMousePosition = Event.current.mousePosition;
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private void DrawEdgeMouseLine()
        {
            if (CreatingEdge)
            {
                var start = StartNode.Position + 0.5f * Settings.NodeSize;
                var end = Event.current.mousePosition;
                Handles.color = Color.black;
                Handles.DrawLine(start, end);
            }
        }

        private void PaginateGraph()
        {
            if (!Dragging)
            {
                var graph = GetLayoutGraph();
                var spacing = Settings.NodeSize + Settings.EdgeSize + 2 * Settings.Spacing;

                if (graph.Paginate(spacing))
                {
                    EditorUtility.SetDirty(graph);
                }
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

        private void HandlePlotAreaClick()
        {
            if (Event.current.type == EventType.MouseUp)
            {
                var mouse = Event.current.mousePosition;

                if (mouse.x >= 0 && mouse.y >= 0)
                {
                    GUI.FocusControl(null);
                    SelectedNodes.Clear();
                    SelectedEdges.Clear();
                    CreatingEdge = false;
                    Event.current.Use();
                }
            }
        }

        private void HandlePlotAreaExit()
        {
            var mouse = Event.current.mousePosition;

            if (mouse.x < 0 || mouse.y < 0)
            {
                Dragging = false;
                CreatingEdge = false;
            }
        }

        private void SetPlotBounds()
        {
            var graph = GetLayoutGraph();

            if (graph.NodeCount > 0)
            {
                var rect = graph.GetRect();
                var width = rect.width + rect.x + Settings.NodeSize.x + Settings.PlotPadding.x;
                var height = rect.height + rect.y + Settings.NodeSize.y + Settings.PlotPadding.y;
                GUILayout.Label("", GUILayout.Width(width), GUILayout.Height(height));
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

        private void DrawEdgeLine(LayoutEdge edge)
        {
            var start = NodePositions[edge.FromNode] + 0.5f * Settings.NodeSize;
            var end = NodePositions[edge.ToNode] + 0.5f * Settings.NodeSize;
            Handles.color = edge.Color;
            Handles.DrawLine(start, end);
        }

        private void DrawEdges()
        {
            var graph = GetLayoutGraph();

            foreach (var edge in graph.GetEdges())
            {
                DrawEdge(edge);
            }
        }

        private void DrawEdge(LayoutEdge edge)
        {
            var position = 0.5f * (NodePositions[edge.FromNode] + NodePositions[edge.ToNode] - Settings.EdgeSize + Settings.NodeSize);
            var rect = new Rect(position, Settings.EdgeSize);
            GUI.backgroundColor = edge.Color;
            GUI.Box(rect, edge.Name, GUI.skin.button);
            GUI.backgroundColor = Color.white;

            if (Event.current.type == EventType.MouseUp)
            {
                Dragging = false;
            }

            if (rect.Contains(Event.current.mousePosition))
            {
                if (Event.current.type == EventType.MouseUp)
                {
                    if (Event.current.button == LeftMouseButton)
                    {
                        GUI.FocusControl(null);
                        SelectedEdges.Add(edge);
                        Event.current.Use();
                    }
                }
                else if (Event.current.type == EventType.MouseDown)
                {
                    if (Event.current.button == LeftMouseButton)
                    {
                        GUI.FocusControl(null);
                        Dragging = true;
                        SelectedEdges.Add(edge);
                        Event.current.Use();
                    }
                }

                GUI.backgroundColor = Settings.HoverColor;
                GUI.Box(rect, "", GUI.skin.button);
                GUI.backgroundColor = Color.white;
            }

            if (SelectedEdges.Contains(edge))
            {
                GUI.backgroundColor = Settings.SelectedColor;
                GUI.Box(rect, "", GUI.skin.button);
                GUI.backgroundColor = Color.white;
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

        private void DrawNode(LayoutNode node)
        {
            var rect = new Rect(node.Position, Settings.NodeSize);
            GUI.backgroundColor = node.Color;
            GUI.Box(rect, $"{node.Id} : {node.Name}", GUI.skin.button);
            GUI.backgroundColor = Color.white;

            if (Event.current.type == EventType.MouseUp)
            {
                Dragging = false;
            }

            if (rect.Contains(Event.current.mousePosition))
            {
                if (Event.current.type == EventType.MouseUp)
                {
                    if (Event.current.button == LeftMouseButton)
                    {
                        GUI.FocusControl(null);
                        SelectedNodes.Add(node);
                        Event.current.Use();
                    }
                    else if (Event.current.button == RightMouseButton)
                    {
                        GUI.FocusControl(null);
                        AddEdge(node);
                        Event.current.Use();
                    }
                }
                else if (Event.current.type == EventType.MouseDown)
                {
                    if (Event.current.button == LeftMouseButton)
                    {
                        GUI.FocusControl(null);
                        Dragging = true;
                        SelectedNodes.Add(node);
                        Event.current.Use();
                    }
                }

                GUI.backgroundColor = Settings.HoverColor;
                GUI.Box(rect, "", GUI.skin.button);
                GUI.backgroundColor = Color.white;
            }

            if (SelectedNodes.Contains(node))
            {
                GUI.backgroundColor = Settings.SelectedColor;
                GUI.Box(rect, "", GUI.skin.button);
                GUI.backgroundColor = Color.white;

                if (Dragging)
                {
                    node.Position += Event.current.mousePosition - LastMousePosition;
                }
            }
        }
        #endregion

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
            if (GUI.Button(new Rect(0, 0, position.width, Settings.MenuHeight), "", GUIStyle.none))
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
                menu.AddItem(new GUIContent("Create Node"), false, CreateNode);
                menu.AddSeparator("");

                if (graph.NodeCount > 0 || graph.EdgeCount > 0)
                    menu.AddItem(new GUIContent("Select All"), false, SelectAllElements);
                else
                    menu.AddDisabledItem(new GUIContent("Select All"));

                if (graph.NodeCount > 0)
                    menu.AddItem(new GUIContent("Select All Nodes"), false, SelectAllNodes);
                else
                    menu.AddDisabledItem(new GUIContent("Select All Nodes"));

                if (graph.EdgeCount > 0)
                    menu.AddItem(new GUIContent("Select All Edges"), false, SelectAllEdges);
                else
                    menu.AddDisabledItem(new GUIContent("Select All Edges"));

                menu.AddSeparator("");

                if (SelectedNodes.Count > 0 || SelectedEdges.Count > 0)
                    menu.AddItem(new GUIContent("Delete Selected"), false, DeleteSelectedElements);
                else
                    menu.AddDisabledItem(new GUIContent("Delete Selected"));

                if (SelectedNodes.Count > 0)
                    menu.AddItem(new GUIContent("Delete Selected Nodes"), false, DeleteSelectedNodes);
                else
                    menu.AddDisabledItem(new GUIContent("Delete Selected Nodes"));

                if (SelectedEdges.Count > 0)
                    menu.AddItem(new GUIContent("Delete Selected Edges"), false, DeleteSelectedEdges);
                else
                    menu.AddDisabledItem(new GUIContent("Delete Selected Edges"));

                menu.DropDown(new Rect(0, 5, 0, 16));
            }
        }
        #endregion

        #region Inspector
        private void DrawInspector()
        {
            GUILayout.BeginArea(new Rect(0, Settings.MenuHeight, Settings.InspectorWidth, position.height - Settings.MenuHeight), GUI.skin.box);
            InspectorScrollPosition = GUILayout.BeginScrollView(InspectorScrollPosition);
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
            if (GUI.Button(new Rect(0, 0, Settings.InspectorWidth, position.height - Settings.MenuHeight), "", GUIStyle.none))
            {
                GUI.FocusControl(null);
                CreatingEdge = false;
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
            UnityEditor.Editor.CreateCachedEditor(SelectedNodes.ToArray(), null, ref nodeEditor);

            if (SelectedNodes.Count > 0)
            {
                GUILayout.Label("Selected Nodes", EditorStyles.boldLabel);
                GUI.enabled = false;
                EditorGUILayout.TextField("Id", string.Join(", ", SelectedNodes.Select(x => x.Id)));
                GUI.enabled = true;
                nodeEditor.OnInspectorGUI();
            }
        }

        private void DrawEdgeInspector()
        {
            UnityEditor.Editor.CreateCachedEditor(SelectedEdges.ToArray(), null, ref edgeEditor);

            if (SelectedEdges.Count > 0)
            {
                if (SelectedNodes.Count > 0)
                    DrawHorizontalSeparator();

                GUILayout.Label("Selected Edges", EditorStyles.boldLabel);
                GUI.enabled = false;
                EditorGUILayout.TextField("Id", string.Join(", ", SelectedEdges.Select(x => $"({x.FromNode}, {x.ToNode})")));
                GUI.enabled = true;
                edgeEditor.OnInspectorGUI();
            }
        }
        #endregion

        #region Actions
        private void AddEdge(LayoutNode node)
        {
            if (!CreatingEdge)
            {
                CreatingEdge = true;
                StartNode = node;
                return;
            }

            CreatingEdge = false;
            var graph = GetLayoutGraph();
            var edge = graph.AddEdge(StartNode.Id, node.Id);
            AssetDatabase.RemoveObjectFromAsset(edge);
            AssetDatabase.AddObjectToAsset(edge, graph);
            EditorUtility.SetDirty(graph);
        }

        private void CreateNode()
        {
            GUI.FocusControl(null);
            var graph = GetLayoutGraph();
            var node = graph.CreateNode();
            SelectedNodes.Clear();
            SelectedNodes.Add(node);
            AssetDatabase.AddObjectToAsset(node, graph);
            EditorUtility.SetDirty(graph);
        }

        private void DeleteSelectedNodes()
        {
            GUI.FocusControl(null);
            var graph = GetLayoutGraph();
            var edges = graph.GetEdges().ToList();

            foreach (var node in SelectedNodes)
            {
                graph.RemoveNode(node.Id);
                AssetDatabase.RemoveObjectFromAsset(node);
            }

            foreach (var edge in edges.Except(graph.GetEdges()))
            {
                AssetDatabase.RemoveObjectFromAsset(edge);
            }

            SelectedNodes.Clear();
            EditorUtility.SetDirty(graph);
        }

        private void DeleteSelectedEdges()
        {
            GUI.FocusControl(null);
            var graph = GetLayoutGraph();

            foreach (var edge in SelectedEdges)
            {
                graph.RemoveEdge(edge.FromNode, edge.ToNode);
                AssetDatabase.RemoveObjectFromAsset(edge);
            }

            SelectedEdges.Clear();
            EditorUtility.SetDirty(graph);
        }

        private void DeleteSelectedElements()
        {
            DeleteSelectedEdges();
            DeleteSelectedNodes();
        }

        private void SelectAllNodes()
        {
            var graph = GetLayoutGraph();

            foreach (var node in graph.GetNodes())
            {
                SelectedNodes.Add(node);
            }
        }

        private void SelectAllEdges()
        {
            var graph = GetLayoutGraph();

            foreach (var edge in graph.GetEdges())
            {
                SelectedEdges.Add(edge);
            }
        }

        private void SelectAllElements()
        {
            SelectAllEdges();
            SelectAllNodes();
        }
        #endregion
    }
}
