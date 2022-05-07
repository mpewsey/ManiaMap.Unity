using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Editor
{
    /// <summary>
    /// A window for viewing and editing a LayoutGraph.
    /// </summary>
    public class NewLayoutGraphWindow : EditorWindow
    {
        /// <summary>
        /// The number corresponding to the left mouse button.
        /// </summary>
        private const int LeftMouseButton = 0;

        /// <summary>
        /// The number corresponding to the right mouse button.
        /// </summary>
        private const int RightMouseButton = 1;

        /// <summary>
        /// The target serialized object.
        /// </summary>
        private SerializedObject SerializedObject { get; set; }

        /// <summary>
        /// The window settings.
        /// </summary>
        private LayoutGraphWindowSettings Settings { get; set; }

        /// <summary>
        /// If true, shows the foldout for the selected nodes.
        /// </summary>
        private bool ShowNodeFoldout { get; set; } = true;

        /// <summary>
        /// If true shows the foldout for the selected edges.
        /// </summary>
        private bool ShowEdgeFoldout { get; set; } = true;

        /// <summary>
        /// If true, displays the edge elements.
        /// </summary>
        private bool ShowEdges { get; set; } = true;

        /// <summary>
        /// The position of the plot scroll view.
        /// </summary>
        private Vector2 PlotScrollPosition { get; set; }

        /// <summary>
        /// The position of the inspector scroll view.
        /// </summary>
        private Vector2 InspectorScrollPosition { get; set; }

        /// <summary>
        /// A set of selected nodes.
        /// </summary>
        public HashSet<LayoutNode> SelectedNodes { get; } = new HashSet<LayoutNode>();

        /// <summary>
        /// A set of selected edges.
        /// </summary>
        public HashSet<LayoutEdge> SelectedEdges { get; } = new HashSet<LayoutEdge>();

        /// <summary>
        /// A dictionary of node positions by ID.
        /// </summary>
        private Dictionary<int, Vector2> NodePositions { get; } = new Dictionary<int, Vector2>();

        /// <summary>
        /// The cached graph editor for the inspector pane.
        /// </summary>
        private UnityEditor.Editor graphEditor;

        /// <summary>
        /// The cached node editor for the inspector pane.
        /// </summary>
        private UnityEditor.Editor nodeEditor;

        /// <summary>
        /// The cached edge editor for the inspector pane.
        /// </summary>
        private UnityEditor.Editor edgeEditor;

        /// <summary>
        /// Show the layout graph window for the specified graph.
        /// </summary>
        /// <param name="graph">The layout graph.</param>
        public static void ShowWindow(LayoutGraph graph)
        {
            var window = GetWindow<NewLayoutGraphWindow>("Layout Graph");
            window.SerializedObject = new SerializedObject(graph);
            window.Settings = LayoutGraphWindowSettings.GetSettings();
            window.minSize = window.Settings.MinWindowSize;
            window.maxSize = window.Settings.MaxWindowSize;
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

        private void OnDestroy()
        {
            SaveAsset();
        }

        /// <summary>
        /// Returns true if the serialized object and target layout graph exist.
        /// </summary>
        private bool LayoutGraphExists()
        {
            return SerializedObject != null
                && GetLayoutGraph() != null;
        }

        /// <summary>
        /// Returns the target layout graph.
        /// </summary>
        private LayoutGraph GetLayoutGraph()
        {
            return SerializedObject.targetObject as LayoutGraph;
        }

        /// <summary>
        /// If the layout graph exists, applies the serialized properties and
        /// saves the asset if it is dirty.
        /// </summary>
        private void SaveAsset()
        {
            if (LayoutGraphExists())
            {
                SerializedObject.ApplyModifiedProperties();
                AssetDatabase.SaveAssetIfDirty(GetLayoutGraph());
            }
        }

        /// <summary>
        /// Draws the window's menu bar.
        /// </summary>
        private void DrawMenu()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.FlexibleSpace();
            DrawMenuAreaButton();
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draws an invisible button over the menu area to catch otherwise unprocessed clicks.
        /// </summary>
        private void DrawMenuAreaButton()
        {
            if (GUI.Button(new Rect(0, 0, position.width, Settings.MenuHeight), "", GUIStyle.none))
            {
                GUI.FocusControl(null);
            }
        }

        /// <summary>
        /// Draws the window's inspector pane.
        /// </summary>
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

        /// <summary>
        /// Draws a box colored by the heading color specified in the settings.
        /// </summary>
        /// <param name="rect">The rect defining the area.</param>
        private void DrawHeadingBox(Rect rect)
        {
            GUI.backgroundColor = Settings.HeadingColor;
            GUI.Box(rect, "", GUI.skin.box);
            GUI.backgroundColor = Color.white;
        }

        /// <summary>
        /// Draws the inspector for the layout graph fields.
        /// </summary>
        private void DrawGraphInspector()
        {
            UnityEditor.Editor.CreateCachedEditor(SerializedObject.targetObject, typeof(LayoutGraphWindowEditor), ref graphEditor);
            EditorGUIUtility.labelWidth = Settings.InspectorLabelWidth;
            EditorGUILayout.LabelField(SerializedObject.targetObject.name);
            DrawHeadingBox(GUILayoutUtility.GetLastRect());
            graphEditor.OnInspectorGUI();
        }

        /// <summary>
        /// Draws the inspector for the selected nodes.
        /// </summary>
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

        /// <summary>
        /// Draws the fields for the selected nodes inspector.
        /// </summary>
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

        /// <summary>
        /// Draws the inspector for the selected edges.
        /// </summary>
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

        /// <summary>
        /// Draws the fields for the selected edges inspector.
        /// </summary>
        private void DrawEdgeInspectorFields()
        {
            if (SelectedEdges.Count == 0 || !ShowEdges)
            {
                EditorGUILayout.LabelField("None");
                return;
            }

            var ids = string.Join(", ", SelectedEdges.OrderBy(x => new EdgeIndexes(x.FromNode, x.ToNode)).Select(x => $"({x.FromNode}, {x.ToNode})"));
            EditorGUILayout.LabelField("ID", ids);
            edgeEditor.OnInspectorGUI();
        }

        /// <summary>
        /// Draws an invisible button over the inspector area to handle otherwise unprocessed events.
        /// </summary>
        private void DrawInspectorAreaButton()
        {
            if (GUI.Button(new Rect(0, 0, Settings.InspectorWidth, position.height - Settings.MenuHeight), "", GUI.skin.box))
            {
                GUI.FocusControl(null);
            }
        }

        /// <summary>
        /// Draws the layout graph plot.
        /// </summary>
        private void DrawPlot()
        {
            GUILayout.BeginArea(new Rect(0, Settings.MenuHeight, position.width - Settings.InspectorWidth, position.height - Settings.MenuHeight));
            PlotScrollPosition = GUILayout.BeginScrollView(PlotScrollPosition);
            SetNodePositions();
            DrawEdgeLines();
            DrawEdges();
            DrawNodes();
            DrawPlotBoundsLabel();
            HandlePlotAreaEvent();
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        /// <summary>
        /// Adds an invisible label the size of the plot bounds so that the plot scroll view works.
        /// </summary>
        private void DrawPlotBoundsLabel()
        {
            var graph = GetLayoutGraph();

            if (graph.NodeCount > 0)
            {
                var rect = graph.GetRect();
                var size = Settings.NodeSize + Settings.PlotPadding;
                size.x += rect.width + rect.x;
                size.y += rect.height + rect.y;
                GUILayout.Label("", GUILayout.Width(size.x), GUILayout.Height(size.y));
            }
        }

        /// <summary>
        /// Adds the current node positions to the node positions dictionary for fast lookup.
        /// </summary>
        private void SetNodePositions()
        {
            var graph = GetLayoutGraph();

            foreach (var node in graph.GetNodes())
            {
                NodePositions[node.Id] = node.Position;
            }
        }

        /// <summary>
        /// Draws lines for the edges of the graph.
        /// </summary>
        private void DrawEdgeLines()
        {
            var graph = GetLayoutGraph();

            foreach (var edge in graph.GetEdges())
            {
                DrawEdgeLine(edge);
            }
        }

        /// <summary>
        /// Draws the line for the specified edge.
        /// </summary>
        /// <param name="edge">The graph edge.</param>
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

        /// <summary>
        /// Draws the elements for the edges of the graph.
        /// </summary>
        private void DrawEdges()
        {
            if (ShowEdges)
            {
                var graph = GetLayoutGraph();

                foreach (var edge in graph.GetEdges())
                {
                    DrawEdge(edge);
                }
            }
        }

        /// <summary>
        /// Draws the element for the specified edge.
        /// </summary>
        /// <param name="edge">The graph edge.</param>
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
                HandleEdgeEvent(edge);
            }

            if (SelectedEdges.Contains(edge))
            {
                // Draw selected graphic.
                GUI.backgroundColor = Settings.SelectedColor;
                GUI.Box(rect, "", GUI.skin.button);
                GUI.backgroundColor = Color.white;
            }
        }

        /// <summary>
        /// Draws the nodes of the graph.
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
        /// Draws the specified node.
        /// </summary>
        /// <param name="node">The graph node.</param>
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
                HandleNodeEvent(node);
            }

            if (SelectedNodes.Contains(node))
            {
                // Draw selected graphic.
                GUI.backgroundColor = Settings.SelectedColor;
                GUI.Box(rect, "", GUI.skin.button);
                GUI.backgroundColor = Color.white;
            }
        }

        /// <summary>
        /// Handles events if the cursor is in the plot area.
        /// </summary>
        private void HandlePlotAreaEvent()
        {
            var size = new Vector2(position.width - Settings.InspectorWidth, position.height - Settings.MenuHeight);
            var rect = new Rect(InspectorScrollPosition, size);

            if (rect.Contains(Event.current.mousePosition))
            {
                switch (Event.current.type)
                {
                    case EventType.MouseDown when Event.current.button == LeftMouseButton:
                        // Begin drag select.
                        break;
                    case EventType.MouseDown when Event.current.button == RightMouseButton:
                        // Show create node context menu.
                        break;
                    case EventType.MouseUp when Event.current.button == LeftMouseButton:
                        // End drag select.
                        break;
                }
            }
        }

        /// <summary>
        /// Handles events if the cursor is in an edge element.
        /// </summary>
        /// <param name="edge">The hovered edge.</param>
        private void HandleEdgeEvent(LayoutEdge edge)
        {
            switch (Event.current.type)
            {
                case EventType.MouseDown when Event.current.button == LeftMouseButton:
                    // Select or begin drag.
                    break;
                case EventType.MouseDown when Event.current.button == LeftMouseButton && Event.current.control:
                    // Multiselect.
                    break;
                case EventType.MouseDown when Event.current.button == RightMouseButton:
                    // Show create node context menu.
                    break;
            }
        }

        /// <summary>
        /// Handles events if the cursor is in a node element.
        /// </summary>
        /// <param name="node">The hovered node.</param>
        private void HandleNodeEvent(LayoutNode node)
        {
            switch (Event.current.type)
            {
                case EventType.MouseDown when Event.current.button == LeftMouseButton:
                    // Select or begin drag.
                    break;
                case EventType.MouseDown when Event.current.button == LeftMouseButton && Event.current.control:
                    // Multiselect.
                    break;
                case EventType.MouseDown when Event.current.button == RightMouseButton:
                    // Show create edge context menu.
                    break;
            }
        }

        /// <summary>
        /// Handles events if a key is pressed.
        /// </summary>
        private void HandleKeyEvent()
        {
            if (Event.current.type == EventType.KeyDown)
            {
                switch (Event.current.keyCode)
                {
                    case KeyCode.Escape:
                        // Deselect all.
                        break;
                    case KeyCode.Delete:
                        // Delete selected.
                        break;
                }
            }
        }
    }
}