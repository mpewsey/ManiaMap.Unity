using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Editor
{
    /// <summary>
    /// A window for editing a LayoutGraph.
    /// </summary>
    public class LayoutGraphWindow : EditorWindow
    {
        /// <summary>
        /// The target serialized object.
        /// </summary>
        private SerializedObject SerializedObject { get; set; }

        /// <summary>
        /// The current window settings.
        /// </summary>
        private LayoutGraphWindowSettings Settings { get; set; }

        /// <summary>
        /// A list of currently selected nodes.
        /// </summary>
        private List<LayoutNode> TargetNodes { get; set; } = new List<LayoutNode>();

        /// <summary>
        /// A list of currently selected edges.
        /// </summary>
        private List<LayoutEdge> TargetEdges { get; set; } = new List<LayoutEdge>();

        /// <summary>
        /// The current graph editor.
        /// </summary>
        private UnityEditor.Editor graphEditor;

        /// <summary>
        /// The current node editor.
        /// </summary>
        private UnityEditor.Editor nodeEditor;

        /// <summary>
        /// The current edge editor.
        /// </summary>
        private UnityEditor.Editor edgeEditor;

        /// <summary>
        /// The current inspector scroll position.
        /// </summary>
        private Vector2 InspectorScrollPosition { get; set; }

        /// <summary>
        /// The current plot scroll position.
        /// </summary>
        private Vector2 PlotScrollPosition { get; set; }

        /// <summary>
        /// The currently displayed menu.
        /// </summary>
        private string CurrentMenu { get; set; }

        /// <summary>
        /// True if the cursor is currently dragging elements.
        /// </summary>
        private bool Dragging { get; set; }

        /// <summary>
        /// The starting mouse position.
        /// </summary>
        private Vector2 StartPosition { get; set; }

        /// <summary>
        /// Shows the window for the specified layout graph.
        /// </summary>
        /// <param name="graph">The target layout graph.</param>
        public static void ShowWindow(LayoutGraph graph)
        {
            var window = GetWindow<LayoutGraphWindow>();
            window.titleContent = new GUIContent("Layout Graph Editor");
            window.minSize = new Vector2(450, 200);
            window.maxSize = new Vector2(1920, 720);
            window.SerializedObject = new SerializedObject(graph);
            window.Settings = LayoutGraphWindowSettings.GetSettings();
        }

        /// <summary>
        /// Returns the target layout graph.
        /// </summary>
        private LayoutGraph GetLayoutGraph()
        {
            return SerializedObject.targetObject as LayoutGraph;
        }

        public void OnGUI()
        {
            if (!LayoutGraphExists())
                return;

            SerializedObject.Update();
            PaginateGraph();
            DrawMenuBar();
            DrawPlot();
            DrawInspector();
            HandlePlotAreaClick();
            HandleWindowClick();
            SerializedObject.ApplyModifiedProperties();
        }

        private void OnDestroy()
        {
            SaveAsset();
        }

        /// <summary>
        /// Saves the layout graph.
        /// </summary>
        private void SaveAsset()
        {
            if (LayoutGraphExists())
            {
                SerializedObject.ApplyModifiedProperties();
                AssetDatabase.SaveAssetIfDirty(SerializedObject.targetObject);
            }
        }

        /// <summary>
        /// If the plot area is clicked anywhere, clears the selected nodes and edges.
        /// </summary>
        private void HandlePlotAreaClick()
        {
            if (GUI.Button(new Rect(Settings.InspectorWidth, Settings.MenuHeight, position.width - Settings.InspectorWidth, position.height - Settings.MenuHeight), "", GUIStyle.none))
            {
                ClearControl();
                Dragging = false;
                TargetNodes.Clear();
                TargetEdges.Clear();
                StartPosition = Vector2.zero;
            }
        }

        /// <summary>
        /// If the window is clicked anywhere, remove focus from the current control.
        /// </summary>
        private void HandleWindowClick()
        {
            if (GUI.Button(new Rect(0, 0, position.width, position.height), "", GUIStyle.none))
            {
                ClearControl();
                Dragging = false;
                StartPosition = Vector2.zero;
            }
        }

        /// <summary>
        /// Returns true if the serialized object and target layout graph are not null.
        /// </summary>
        private bool LayoutGraphExists()
        {
            return SerializedObject != null
                && GetLayoutGraph() != null;
        }

        /// <summary>
        /// Draws the menu bar.
        /// </summary>
        private void DrawMenuBar()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            DrawEditMenuButton();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draws the edit menu button.
        /// </summary>
        private void DrawEditMenuButton()
        {
            if (GUILayout.Button("Edit", EditorStyles.toolbarDropDown))
            {
                if (CurrentMenu == "Edit")
                {
                    CurrentMenu = null;
                    return;
                }

                CurrentMenu = "Edit";
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Create Node"), false, CreateNode);
                menu.AddSeparator("");

                if (TargetNodes.Count > 0 || TargetEdges.Count > 0)
                    menu.AddItem(new GUIContent("Delete Selection"), false, DeleteElements);
                else
                    menu.AddDisabledItem(new GUIContent("Delete Selection"));

                if (TargetNodes.Count > 0)
                    menu.AddItem(new GUIContent("Delete Selected Nodes"), false, DeleteNodes);
                else
                    menu.AddDisabledItem(new GUIContent("Delete Selected Nodes"));

                if (TargetEdges.Count > 0)
                    menu.AddItem(new GUIContent("Delete Selected Edges"), false, DeleteEdges);
                else
                    menu.AddDisabledItem(new GUIContent("Delete Selected Edges"));

                menu.DropDown(new Rect(0, 5, 0, 16));
            }
        }

        /// <summary>
        /// Draws the inspector pane.
        /// </summary>
        private void DrawInspector()
        {
            GUILayout.BeginArea(new Rect(0, Settings.MenuHeight, Settings.InspectorWidth, position.height - Settings.MenuHeight), GUI.skin.box);
            InspectorScrollPosition = GUILayout.BeginScrollView(InspectorScrollPosition);
            DrawGraphInspector();
            DrawHorizontalSeparator();
            DrawNodeInspector();
            DrawEdgeInspector();
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        /// <summary>
        /// Draws the layout graph properties in the inspector.
        /// </summary>
        private void DrawGraphInspector()
        {
            EditorGUIUtility.labelWidth = Settings.InspectorLabelWidth;
            UnityEditor.Editor.CreateCachedEditor(SerializedObject.targetObject, typeof(LayoutGraphWindowEditor), ref graphEditor);
            graphEditor.OnInspectorGUI();
        }

        /// <summary>
        /// Draws the node inspector.
        /// </summary>
        private void DrawNodeInspector()
        {
            TargetNodes.RemoveAll(x => x == null);

            if (TargetNodes.Count == 0)
                return;

            EditorGUIUtility.labelWidth = Settings.InspectorLabelWidth;
            GUILayout.Label("Selected Nodes", EditorStyles.boldLabel);
            GUI.enabled = false;
            var ids = TargetNodes.Select(x => x.Id);
            EditorGUILayout.TextField("Id", string.Join(", ", ids));
            UnityEditor.Editor.CreateCachedEditor(TargetNodes.ToArray(), null, ref nodeEditor);
            nodeEditor.OnInspectorGUI();
        }

        /// <summary>
        /// Draws the edge inspector.
        /// </summary>
        private void DrawEdgeInspector()
        {
            TargetEdges.RemoveAll(x => x == null);

            if (TargetEdges.Count == 0)
                return;

            EditorGUIUtility.labelWidth = Settings.InspectorLabelWidth;
            GUILayout.Label("Selected Edges", EditorStyles.boldLabel);
            GUI.enabled = false;
            var ids = TargetEdges.Select(x => $"({x.FromNode}, {x.ToNode})");
            EditorGUILayout.TextField("Id", string.Join(", ", ids));
            UnityEditor.Editor.CreateCachedEditor(TargetEdges.ToArray(), null, ref edgeEditor);
            edgeEditor.OnInspectorGUI();
        }

        /// <summary>
        /// Draws a horizontal separator.
        /// </summary>
        private static void DrawHorizontalSeparator()
        {
            var style = new GUIStyle(GUI.skin.horizontalSlider);
            style.fixedHeight = 1;
            EditorGUILayout.LabelField("", style);
        }

        /// <summary>
        /// Draws the graph plot area.
        /// </summary>
        private void DrawPlot()
        {
            GUILayout.BeginArea(new Rect(Settings.InspectorWidth, Settings.MenuHeight, position.width - Settings.InspectorWidth, position.height - Settings.MenuHeight));
            PlotScrollPosition = GUILayout.BeginScrollView(PlotScrollPosition);
            SetPlotBounds();
            DrawEdges();
            DrawNodes();
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        /// <summary>
        /// Adds an empty label with the size of the bounds of the plot.
        /// Without this, the scroll view won't work.
        /// </summary>
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

        /// <summary>
        /// Draws the edges of the plot.
        /// </summary>
        private void DrawEdges()
        {
            var graph = GetLayoutGraph();
            var positions = graph.GetNodes().ToDictionary(x => x.Id, x => x.Position);

            foreach (var edge in graph.GetEdges())
            {
                var position = 0.5f * (positions[edge.FromNode] + positions[edge.ToNode]);
                position += 0.5f * (Settings.NodeSize - Settings.EdgeSize);
                GUI.backgroundColor = edge.Color;

                if (GUI.Button(new Rect(position, Settings.EdgeSize), edge.Name))
                {
                    ClearControl();
                    TargetEdges.Clear();
                    TargetEdges.Add(edge);
                }

                GUI.backgroundColor = Color.white;
            }
        }

        /// <summary>
        /// Draws the nodes of the plot.
        /// </summary>
        private void DrawNodes()
        {
            var graph = GetLayoutGraph();

            foreach (var node in graph.GetNodes())
            {
                DrawNode(node);
            }
        }

        /// <summary>
        /// Draws a node element.
        /// </summary>
        /// <param name="node">The node.</param>
        private void DrawNode(LayoutNode node)
        {
            GUI.backgroundColor = node.Color;
            var rect = new Rect(node.Position, Settings.NodeSize);
            GUI.Box(rect, $"({node.Id}) : {node.Name}", GUI.skin.button);

            if (rect.Contains(Event.current.mousePosition))
            {
                switch (Event.current.type)
                {
                    case EventType.MouseUp:
                        OnNodeMouseUp(node);
                        break;
                    case EventType.MouseDown:
                        OnNodeMouseDown(node);
                        break;
                    default:
                        OnNodeHover(node);
                        break;
                }
            }

            GUI.backgroundColor = Color.white;
        }

        /// <summary>
        /// Event called when the mouse up event is triggered within a node.
        /// </summary>
        /// <param name="node">The node.</param>
        private void OnNodeMouseUp(LayoutNode node)
        {
            if (new Rect(node.Position, Settings.NodeSize).Contains(StartPosition))
            {
                ClearControl();
                Dragging = false;
                TargetNodes.Clear();
                TargetNodes.Add(node);
                Event.current.Use();
            }
        }

        /// <summary>
        /// The event called when the mouse down event is triggered within a node.
        /// </summary>
        /// <param name="node">The node.</param>
        private void OnNodeMouseDown(LayoutNode node)
        {
            StartPosition = Event.current.mousePosition;
            
            if (!TargetNodes.Contains(node))
            {
                ClearControl();
                Event.current.Use();
                return;
            }

            ClearControl();
            Dragging = true;
            Event.current.Use();
        }

        /// <summary>
        /// The event called when the mouse hovers a node with no other handled events.
        /// </summary>
        /// <param name="node">The node.</param>
        private void OnNodeHover(LayoutNode node)
        {
            GUI.backgroundColor = Settings.HoverColor;
            GUI.Box(new Rect(node.Position, Settings.NodeSize), "", GUI.skin.button);
            GUI.backgroundColor = Color.white;
        }

        /// <summary>
        /// Clears focus control.
        /// </summary>
        private void ClearControl()
        {
            GUI.FocusControl(null);
        }

        /// <summary>
        /// Paginates the graph elements.
        /// </summary>
        private void PaginateGraph()
        {
            var graph = GetLayoutGraph();

            if (graph.Paginate(Settings.NodeSize + Settings.Spacing))
            {
                EditorUtility.SetDirty(graph);
            }
        }

        /// <summary>
        /// Adds a new node to the graph.
        /// </summary>
        private void CreateNode()
        {
            ClearControl();
            var graph = GetLayoutGraph();
            var node = graph.CreateNode();
            AssetDatabase.AddObjectToAsset(node, SerializedObject.targetObject);
            TargetNodes.Clear();
            TargetNodes.Add(node);
            EditorUtility.SetDirty(graph);
        }

        /// <summary>
        /// Deletes the selected nodes.
        /// </summary>
        private void DeleteNodes()
        {
            ClearControl();
            var graph = GetLayoutGraph();
            var edges = graph.GetEdges().ToList();

            foreach (var node in TargetNodes)
            {
                graph.RemoveNode(node.Id);
                AssetDatabase.RemoveObjectFromAsset(node);
            }

            foreach (var edge in edges.Except(graph.GetEdges()))
            {
                AssetDatabase.RemoveObjectFromAsset(edge);
            }

            TargetNodes.Clear();
            TargetEdges.Clear();
            EditorUtility.SetDirty(graph);
        }

        /// <summary>
        /// Deletes the selected edges.
        /// </summary>
        private void DeleteEdges()
        {
            ClearControl();
            var graph = GetLayoutGraph();

            foreach (var edge in TargetEdges)
            {
                graph.RemoveEdge(edge.FromNode, edge.ToNode);
                AssetDatabase.RemoveObjectFromAsset(edge);
            }

            TargetEdges.Clear();
            EditorUtility.SetDirty(graph);
        }

        /// <summary>
        /// Deletes the selected edges and nodes.
        /// </summary>
        private void DeleteElements()
        {
            DeleteEdges();
            DeleteNodes();
        }
    }
}
