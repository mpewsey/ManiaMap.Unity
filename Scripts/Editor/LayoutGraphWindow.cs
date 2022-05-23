using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Editor
{
    /// <summary>
    /// A window for viewing and editing a LayoutGraph.
    /// </summary>
    public class LayoutGraphWindow : EditorWindow
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
        /// If true, displays the inspector.
        /// </summary>
        private bool ShowInspector { get; set; } = true;

        /// <summary>
        /// True if the user has multiselected elements.
        /// </summary>
        private bool Multiselecting { get; set; }

        /// <summary>
        /// The active window tool.
        /// </summary>
        private Tool ActiveTool { get; set; }

        /// <summary>
        /// The position of the plot scroll view.
        /// </summary>
        private Vector2 PlotScrollPosition { get; set; }

        /// <summary>
        /// The position of the inspector scroll view.
        /// </summary>
        private Vector2 InspectorScrollPosition { get; set; }

        /// <summary>
        /// The last position of the mouse in the plot area.
        /// </summary>
        private Vector2 LastMousePlotPosition { get; set; }

        /// <summary>
        /// The position of the mouse in the plot area when starting a drag.
        /// </summary>
        private Vector2 StartMousePlotPosition { get; set; }

        /// <summary>
        /// The position set to a node when it is created.
        /// </summary>
        private Vector2 CreateNodePosition { get; set; }

        /// <summary>
        /// The start node when adding an edge.
        /// </summary>
        private LayoutNode StartNode { get; set; }

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
            var window = GetWindow<LayoutGraphWindow>("Layout Graph");
            window.SerializedObject = new SerializedObject(graph);
            window.Settings = LayoutGraphWindowSettings.GetSettings();
            window.minSize = window.Settings.MinWindowSize;
            window.maxSize = window.Settings.MaxWindowSize;
            window.SelectedNodes.Clear();
            window.SelectedEdges.Clear();
        }

        private void OnGUI()
        {
            if (!LayoutGraphExists())
                return;

            DrawMenu();
            DrawInspector();
            DrawPlot();
            RepaintIfToolActive();
        }

        private void OnLostFocus()
        {
            ActiveTool = Tool.None;
        }

        private void OnDestroy()
        {
            SaveAsset();
        }

        /// <summary>
        /// Repaints the window if a tool is currently active.
        /// </summary>
        private void RepaintIfToolActive()
        {
            if (ActiveTool != Tool.None)
                Repaint();
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
            DrawEditMenu();
            DrawViewMenu();
            GUILayout.FlexibleSpace();
            DrawMenuAreaButton();
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draws the edit menu.
        /// </summary>
        private void DrawEditMenu()
        {
            if (GUILayout.Button("Edit", EditorStyles.toolbarDropDown))
            {
                CreateNodePosition = PlotScrollPosition;
                var graph = GetLayoutGraph();
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Create Node"), false, CreateNode);
                menu.AddSeparator("");
                AddMenuItem(menu, "Deselect All _ESC", SelectedNodes.Count + SelectedEdges.Count > 0, DeselectAll);
                AddMenuItem(menu, "Select All ^a", graph.NodeCount + graph.EdgeCount > 0, SelectAll);
                AddMenuItem(menu, "Select All Nodes", graph.NodeCount > 0, SelectAllNodes);
                AddMenuItem(menu, "Select All Edges", graph.EdgeCount > 0, SelectAllEdges);
                menu.AddSeparator("");
                AddMenuItem(menu, "Delete Selected _DEL", SelectedNodes.Count + SelectedEdges.Count > 0, DeleteSelected);
                AddMenuItem(menu, "Delete Selected Nodes", SelectedNodes.Count > 0, DeleteNodes);
                AddMenuItem(menu, "Delete Selected Edges", SelectedEdges.Count > 0, DeleteEdges);
                menu.DropDown(new Rect(0, 5, 0, 16));
            }
        }

        /// <summary>
        /// Draws the view menu.
        /// </summary>
        private void DrawViewMenu()
        {
            if (GUILayout.Button("View", EditorStyles.toolbarDropDown))
            {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Toggle Inspector Display"), ShowInspector, ToggleShowInspector);
                menu.AddItem(new GUIContent("Toggle Edge Display"), ShowEdges, ToggleShowEdges);
                menu.DropDown(new Rect(40, 5, 0, 16));
            }
        }

        /// <summary>
        /// Adds a menu item that may be disabled to the menu.
        /// </summary>
        /// <param name="menu">The menu.</param>
        /// <param name="label">The item label.</param>
        /// <param name="enabled">True if the menu item is enabled.</param>
        /// <param name="func">The menu item function, called when the item is enabled.</param>
        private static void AddMenuItem(GenericMenu menu, string label, bool enabled, GenericMenu.MenuFunction func)
        {
            if (enabled)
                menu.AddItem(new GUIContent(label), false, func);
            else
                menu.AddDisabledItem(new GUIContent(label), false);
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
        /// Toggles whether the inspector is shown.
        /// </summary>
        private void ToggleShowInspector()
        {
            ShowInspector = !ShowInspector;
        }

        /// <summary>
        /// Toggles whether edges are shown.
        /// </summary>
        private void ToggleShowEdges()
        {
            ShowEdges = !ShowEdges;
        }

        /// <summary>
        /// Draws the window's inspector pane.
        /// </summary>
        private void DrawInspector()
        {
            if (ShowInspector)
            {
                GUILayout.BeginArea(new Rect(position.width - GetInspectorWidth(), Settings.MenuHeight, GetInspectorWidth(), position.height - Settings.MenuHeight), GUI.skin.box);
                InspectorScrollPosition = GUILayout.BeginScrollView(InspectorScrollPosition);
                DrawGraphInspector();
                DrawNodeInspector();
                DrawEdgeInspector();
                EditorGUILayout.Space();
                LayoutGraphEditor.DrawNodeTemplateGroupWarningBox(GetLayoutGraph());
                LayoutGraphEditor.DrawEdgeTemplateGroupWarningBox(GetLayoutGraph());
                DrawInspectorAreaButton();
                GUILayout.EndScrollView();
                GUILayout.EndArea();
            }
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
            if (!ShowEdges || SelectedEdges.Count == 0)
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
            if (GUI.Button(new Rect(0, 0, GetInspectorWidth(), position.height - Settings.MenuHeight), "", GUI.skin.box))
            {
                GUI.FocusControl(null);
            }
        }

        /// <summary>
        /// Returns the inspector width if it is visible. Otherwise, returns 0.
        /// </summary>
        private float GetInspectorWidth()
        {
            if (ShowInspector)
                return Settings.InspectorWidth;
            return 0;
        }

        /// <summary>
        /// Draws the layout graph plot.
        /// </summary>
        private void DrawPlot()
        {
            GUILayout.BeginArea(new Rect(0, Settings.MenuHeight, position.width - GetInspectorWidth(), position.height - Settings.MenuHeight));
            PlotScrollPosition = GUILayout.BeginScrollView(PlotScrollPosition);
            PaginatePlot();
            SetNodePositions();
            DrawEdgeLines();
            DrawEdges();
            DrawNodes();
            DrawPlotBoundsLabel();
            HandlePlotAreaEvent();
            HandleKeyEvent();
            MoveNodes();
            DrawMouseLine();
            DrawDragArea();
            LastMousePlotPosition = Event.current.mousePosition;
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        /// <summary>
        /// Selects the elements contained in the drag area.
        /// </summary>
        private void SelectDraggedElements()
        {
            ActiveTool = Tool.None;
            Multiselecting = true;
            SelectDraggedNodes();
            SelectDraggedEdges();
        }

        /// <summary>
        /// Selects the nodes contained in the drag area.
        /// </summary>
        private void SelectDraggedNodes()
        {
            SelectedNodes.Clear();
            var graph = GetLayoutGraph();
            var rect = GetDragRect();

            foreach (var node in graph.GetNodes())
            {
                if (rect.Contains(node.Position) && rect.Contains(node.Position + Settings.NodeSize))
                {
                    SelectedNodes.Add(node);
                }
            }
        }

        /// <summary>
        /// Selects the edges contained in the drag area.
        /// </summary>
        private void SelectDraggedEdges()
        {
            SelectedEdges.Clear();
            var graph = GetLayoutGraph();
            var rect = GetDragRect();

            foreach (var edge in graph.GetEdges())
            {
                var fromPosition = NodePositions[edge.FromNode];
                var toPosition = NodePositions[edge.ToNode];
                var position = 0.5f * (fromPosition + toPosition + Settings.NodeSize - Settings.EdgeSize);

                if (rect.Contains(position) && rect.Contains(position + Settings.EdgeSize))
                {
                    SelectedEdges.Add(edge);
                }
            }
        }

        /// <summary>
        /// Returns the rect for the drag area. The position and size of the rect are adjusted
        /// so that the size is positive. Otherwise, the contains method will not work on points.
        /// </summary>
        private Rect GetDragRect()
        {
            var position = Event.current.mousePosition;
            var size = StartMousePlotPosition - position;

            if (size.x < 0)
            {
                size.x = -size.x;
                position.x = StartMousePlotPosition.x;
            }

            if (size.y < 0)
            {
                size.y = -size.y;
                position.y = StartMousePlotPosition.y;
            }

            return new Rect(position, size);
        }

        /// <summary>
        /// If in movement mode, moves the positions of the nodes based on the change in the mouse position.
        /// </summary>
        private void MoveNodes()
        {
            if (ActiveTool == Tool.Move)
            {
                var delta = Event.current.mousePosition - LastMousePlotPosition;

                foreach (var node in SelectedNodes)
                {
                    node.Position += delta;
                }

                EditorUtility.SetDirty(SerializedObject.targetObject);
            }
        }

        /// <summary>
        /// If not in moving mode, adjusts the node positions to prevent overlapping elements.
        /// </summary>
        private void PaginatePlot()
        {
            if (ActiveTool != Tool.Move)
            {
                var graph = GetLayoutGraph();
                var spacing = Settings.Spacing + Settings.NodeSize;

                if (graph.Paginate(spacing))
                    EditorUtility.SetDirty(graph);
            }
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
            var size = new Vector2(position.width - GetInspectorWidth(), position.height - Settings.MenuHeight);
            var rect = new Rect(PlotScrollPosition, size);

            if (rect.Contains(Event.current.mousePosition))
            {
                switch (Event.current.type)
                {
                    case EventType.MouseDown when Event.current.button == LeftMouseButton:
                        DeselectAll();
                        BeginDragSelect();
                        Event.current.Use();
                        break;
                    case EventType.MouseDown when Event.current.button == RightMouseButton:
                        ShowAreaContextMenu();
                        Event.current.Use();
                        break;
                    case EventType.MouseUp when Event.current.button == LeftMouseButton && ActiveTool == Tool.DragSelect:
                        SelectDraggedElements();
                        Event.current.Use();
                        break;
                    case EventType.MouseUp when Event.current.button == LeftMouseButton:
                        ActiveTool = Tool.None;
                        Event.current.Use();
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
                case EventType.MouseDown when Event.current.button == LeftMouseButton && Event.current.control:
                    MultiselectEdge(edge);
                    Event.current.Use();
                    break;
                case EventType.MouseDown when Event.current.button == LeftMouseButton:
                    SelectEdge(edge);
                    Event.current.Use();
                    break;
                case EventType.MouseDown when Event.current.button == RightMouseButton:
                    ShowAreaContextMenu();
                    Event.current.Use();
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
                case EventType.MouseDown when Event.current.button == LeftMouseButton && Event.current.control:
                    MultiselectNode(node);
                    Event.current.Use();
                    break;
                case EventType.MouseDown when Event.current.button == LeftMouseButton && ActiveTool == Tool.AddEdge:
                    AddEdge(node);
                    Event.current.Use();
                    break;
                case EventType.MouseDown when Event.current.button == LeftMouseButton:
                    SelectNode(node);
                    Event.current.Use();
                    break;
                case EventType.MouseDown when Event.current.button == RightMouseButton:
                    ShowNodeContextMenu(node);
                    Event.current.Use();
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
                        DeselectAll();
                        Event.current.Use();
                        break;
                    case KeyCode.Delete:
                        DeleteSelected();
                        Event.current.Use();
                        break;
                    case KeyCode.A when Event.current.control:
                        SelectAll();
                        Event.current.Use();
                        break;
                    case KeyCode.RightArrow:
                    case KeyCode.D:
                        PlotScrollPosition += Vector2.right * Settings.ScrollSpeed;
                        Event.current.Use();
                        break;
                    case KeyCode.LeftArrow:
                    case KeyCode.A:
                        PlotScrollPosition += Vector2.left * Settings.ScrollSpeed;
                        Event.current.Use();
                        break;
                    case KeyCode.DownArrow:
                    case KeyCode.S:
                        PlotScrollPosition += Vector2.up * Settings.ScrollSpeed;
                        Event.current.Use();
                        break;
                    case KeyCode.UpArrow:
                    case KeyCode.W:
                        PlotScrollPosition += Vector2.down * Settings.ScrollSpeed;
                        Event.current.Use();
                        break;
                }
            }
        }

        /// <summary>
        /// Sets dragging to true and stores the current mouse position.
        /// </summary>
        private void BeginDragSelect()
        {
            ActiveTool = Tool.DragSelect;
            StartMousePlotPosition = Event.current.mousePosition;
        }

        /// <summary>
        /// If the user is dragging, draws the drag area rectangle.
        /// </summary>
        private void DrawDragArea()
        {
            if (ActiveTool == Tool.DragSelect)
            {
                var rect = new Rect(StartMousePlotPosition, Event.current.mousePosition - StartMousePlotPosition);
                Handles.DrawSolidRectangleWithOutline(rect, Settings.DragAreaColor, Settings.DragOutlineColor);
            }
        }

        /// <summary>
        /// Displays the context menu when clicking in the plot area.
        /// </summary>
        private void ShowAreaContextMenu()
        {
            CreateNodePosition = Event.current.mousePosition;
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Create Node"), false, CreateNode);
            menu.ShowAsContext();
        }

        /// <summary>
        /// Displays the context menu when clicking in a node element.
        /// </summary>
        /// <param name="node">The current layout node.</param>
        private void ShowNodeContextMenu(LayoutNode node)
        {
            StartNode = node;
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Add Edge"), false, BeginAddEdge);
            menu.ShowAsContext();
        }

        /// <summary>
        /// Begins adding an edge.
        /// </summary>
        private void BeginAddEdge()
        {
            ActiveTool = Tool.AddEdge;
            Multiselecting = false;
        }

        /// <summary>
        /// Completes the add edge operation by adding an edge between the start node and current node.
        /// </summary>
        /// <param name="node">The current node.</param>
        private void AddEdge(LayoutNode node)
        {
            ActiveTool = Tool.None;

            if (StartNode != node)
            {
                var graph = GetLayoutGraph();
                var edge = graph.AddEdge(StartNode.Id, node.Id);
                AssetDatabase.RemoveObjectFromAsset(edge);
                AssetDatabase.AddObjectToAsset(edge, graph);
                EditorUtility.SetDirty(graph);
            }
        }

        /// <summary>
        /// Draws a line to the mouse when adding an edge.
        /// </summary>
        private void DrawMouseLine()
        {
            if (ActiveTool == Tool.AddEdge && StartNode != null)
            {
                var position = StartNode.Position + 0.5f * Settings.NodeSize;
                Handles.color = Settings.AddEdgeColor;
                Handles.DrawLine(position, Event.current.mousePosition);
                Handles.color = Color.white;
            }
        }

        /// <summary>
        /// Clears the selected elements and adds the edge.
        /// </summary>
        /// <param name="edge">The edge.</param>
        private void SelectEdge(LayoutEdge edge)
        {
            if (!Multiselecting)
            {
                SelectedNodes.Clear();
                SelectedEdges.Clear();
            }

            SelectedEdges.Add(edge);
            ActiveTool = SelectedNodes.Count > 0 ? Tool.Move : Tool.None;
        }

        /// <summary>
        /// Clears the selected elements and adds the node.
        /// </summary>
        /// <param name="node">The node.</param>
        private void SelectNode(LayoutNode node)
        {
            if (!Multiselecting)
            {
                SelectedNodes.Clear();
                SelectedEdges.Clear();
            }

            SelectedNodes.Add(node);
            ActiveTool = Tool.Move;
        }

        /// <summary>
        /// Toggles the selection of the edge.
        /// </summary>
        /// <param name="edge">The edge.</param>
        private void MultiselectEdge(LayoutEdge edge)
        {
            ActiveTool = Tool.None;
            Multiselecting = true;

            if (!SelectedEdges.Add(edge))
                SelectedEdges.Remove(edge);
        }

        /// <summary>
        /// Toggles the selection of the node.
        /// </summary>
        /// <param name="node">The node.</param>
        private void MultiselectNode(LayoutNode node)
        {
            ActiveTool = Tool.None;
            Multiselecting = true;

            if (!SelectedNodes.Add(node))
                SelectedNodes.Remove(node);
        }

        /// <summary>
        /// Selects all nodes and edges in the graph.
        /// </summary>
        private void SelectAll()
        {
            Multiselecting = true;
            SelectAllEdges();
            SelectAllNodes();
        }

        /// <summary>
        /// Selects all nodes in the graph.
        /// </summary>
        private void SelectAllNodes()
        {
            Multiselecting = true;
            var graph = GetLayoutGraph();
            SelectedNodes.Clear();

            foreach (var node in graph.GetNodes())
            {
                SelectedNodes.Add(node);
            }
        }

        /// <summary>
        /// Selects all edges in the graph.
        /// </summary>
        private void SelectAllEdges()
        {
            Multiselecting = true;
            var graph = GetLayoutGraph();
            SelectedEdges.Clear();

            if (ShowEdges)
            {
                foreach (var edge in graph.GetEdges())
                {
                    SelectedEdges.Add(edge);
                }
            }
        }

        /// <summary>
        /// Creates a new node.
        /// </summary>
        private void CreateNode()
        {
            ActiveTool = Tool.None;
            var graph = GetLayoutGraph();
            var node = graph.CreateNode();
            node.Position = CreateNodePosition;
            AssetDatabase.AddObjectToAsset(node, graph);
            EditorUtility.SetDirty(graph);
            SelectedNodes.Clear();
            SelectedNodes.Add(node);
        }

        /// <summary>
        /// Deselects all nodes and edges.
        /// </summary>
        private void DeselectAll()
        {
            ActiveTool = Tool.None;
            Multiselecting = false;
            SelectedNodes.Clear();
            SelectedEdges.Clear();
        }

        /// <summary>
        /// Deletes all selected nodes and edges.
        /// </summary>
        private void DeleteSelected()
        {
            ActiveTool = Tool.None;
            Multiselecting = false;
            DeleteEdges();
            DeleteNodes();
        }

        /// <summary>
        /// Deletes all selected edges.
        /// </summary>
        private void DeleteEdges()
        {
            if (ShowEdges)
            {
                ActiveTool = Tool.None;
                var graph = GetLayoutGraph();

                foreach (var edge in SelectedEdges)
                {
                    graph.RemoveEdge(edge.FromNode, edge.ToNode);
                    AssetDatabase.RemoveObjectFromAsset(edge);
                }

                EditorUtility.SetDirty(graph);
            }

            SelectedEdges.Clear();
        }

        /// <summary>
        /// Deletes all selected nodes.
        /// </summary>
        private void DeleteNodes()
        {
            ActiveTool = Tool.None;
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

            EditorUtility.SetDirty(graph);
            SelectedNodes.Clear();
        }

        /// <summary>
        /// The layout graph window tool.
        /// </summary>
        private enum Tool
        {
            /// No tool.
            None,
            /// The drag select tool. Enabled when the user drags the mouse in the plot area.
            DragSelect,
            /// The move nodes tool. Enabled when the user drags the mouse in a node or edge.
            Move,
            /// The add edge tool. Enabled when the user is adding an edge.
            AddEdge,
        }
    }
}