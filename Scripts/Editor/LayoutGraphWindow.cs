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
        /// The number of node properties to display.
        /// </summary>
        private const int NodePropertyCount = 5;

        /// <summary>
        /// The number of edge properties to display.
        /// </summary>
        private const int EdgePropertyCount = 8;

        /// <summary>
        /// The height of the menu.
        /// </summary>
        private const float MenuHeight = 20;

        /// <summary>
        /// The width of the inspector.
        /// </summary>
        private const float InspectorWidth = 300;

        /// <summary>
        /// The width of the labels in the inspector.
        /// </summary>
        private const float InspectorLabelWidth = 100;

        /// <summary>
        /// The target serialized object.
        /// </summary>
        private static SerializedObject SerializedObject { get; set; }

        /// <summary>
        /// The size of the plotted node elements.
        /// </summary>
        private Vector2 NodeSize { get; } = new Vector2(150, 30);

        /// <summary>
        /// The size of the plotted edge elements.
        /// </summary>
        private Vector2 EdgeSize { get; } = new Vector2(125, 30);

        /// <summary>
        /// The minimum spacing between node elements.
        /// </summary>
        private Vector2 Spacing { get; } = new Vector2(20, 20);

        /// <summary>
        /// The padding added to the right and bottom of the plot area.
        /// </summary>
        private Vector2 PlotPadding { get; } = new Vector2(20, 20);

        /// <summary>
        /// The current inspector scroll position.
        /// </summary>
        private Vector2 InspectorScrollPosition { get; set; }

        /// <summary>
        /// The current plot scroll position.
        /// </summary>
        private Vector2 PlotScrollPosition { get; set; }

        /// <summary>
        /// The selection ID.
        /// </summary>
        private Uid Selection { get; set; } = new Uid(0, 0, 2);

        /// <summary>
        /// Shows the window for the specified layout graph.
        /// </summary>
        /// <param name="graph">The target layout graph.</param>
        public static void ShowWindow(LayoutGraph graph)
        {
            SerializedObject = new SerializedObject(graph);
            var window = GetWindow<LayoutGraphWindow>();
            window.titleContent = new GUIContent("Layout Graph Editor");
            window.minSize = new Vector2(450, 200);
            window.maxSize = new Vector2(1920, 720);
        }

        /// <summary>
        /// Returns the target layout graph.
        /// </summary>
        private static LayoutGraph GetLayoutGraph()
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
            HandleWindowClick();
            SerializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// If the window is clicked anywhere, remove focus from the current control.
        /// </summary>
        private void HandleWindowClick()
        {
            if (GUI.Button(new Rect(0, 0, position.width, position.height), "", GUIStyle.none))
            {
                ClearControl();
            }
        }

        /// <summary>
        /// Draws the node inspector.
        /// </summary>
        private void DrawNodeInspector()
        {
            if (Selection.Value3 != 0)
                return;

            var index = GetLayoutGraph().GetNodeIndex(Selection.Value1);

            if (index < 0)
            {
                ClearSelection();
                return;
            }

            EditorGUIUtility.labelWidth = InspectorLabelWidth;
            GUILayout.Label("Selected Node", EditorStyles.boldLabel);
            var nodes = SerializedObject.FindProperty("_nodes");
            var prop = nodes.GetArrayElementAtIndex(index);
            var enterChildren = true;

            for (int i = 0; i < NodePropertyCount; i++)
            {
                prop.NextVisible(enterChildren);
                GUI.enabled = prop.name != "_id" && prop.name != "_position";
                EditorGUILayout.PropertyField(prop, true);
                enterChildren = false;
            }
        }

        /// <summary>
        /// Draws the edge inspector.
        /// </summary>
        private void DrawEdgeInspector()
        {
            if (Selection.Value3 != 1)
                return;

            var index = GetLayoutGraph().GetEdgeIndex(Selection.Value1, Selection.Value2);

            if (index < 0)
            {
                ClearSelection();
                return;
            }

            EditorGUIUtility.labelWidth = InspectorLabelWidth;
            GUILayout.Label("Selected Edge", EditorStyles.boldLabel);
            var edges = SerializedObject.FindProperty("_edges");
            var prop = edges.GetArrayElementAtIndex(index);
            var enterChildren = true;

            for (int i = 0; i < EdgePropertyCount; i++)
            {
                prop.NextVisible(enterChildren);
                GUI.enabled = prop.name != "_fromNode" && prop.name != "_toNode";
                EditorGUILayout.PropertyField(prop, true);
                enterChildren = false;
            }
        }

        /// <summary>
        /// Returns true if the serialized object and target layout graph are not null.
        /// </summary>
        private static bool LayoutGraphExists()
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
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Create Node"), false, CreateNode);
                menu.DropDown(new Rect(0, 5, 0, 16));
            }
        }

        /// <summary>
        /// Draws the inspector pane.
        /// </summary>
        private void DrawInspector()
        {
            GUILayout.BeginArea(new Rect(0, MenuHeight, InspectorWidth, position.height - MenuHeight), GUI.skin.box);
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
            GUI.enabled = false;
            EditorGUIUtility.labelWidth = InspectorLabelWidth;
            EditorGUILayout.TextField("Filename", SerializedObject.targetObject.name);
            var prop = SerializedObject.GetIterator();
            prop.NextVisible(true);
            GUI.enabled = true;

            while (prop.NextVisible(false))
            {
                EditorGUILayout.PropertyField(prop, true);
            }
        }

        /// <summary>
        /// Draws a horizontal separator.
        /// </summary>
        private static void DrawHorizontalSeparator()
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }

        /// <summary>
        /// Draws the graph plot area.
        /// </summary>
        private void DrawPlot()
        {
            GUILayout.BeginArea(new Rect(InspectorWidth, MenuHeight, position.width - InspectorWidth, position.height - MenuHeight));
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
                var width = rect.width + rect.x + NodeSize.x + PlotPadding.x;
                var height = rect.height + rect.y + NodeSize.y + PlotPadding.y;
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
                position += 0.5f * (NodeSize - EdgeSize);
                GUI.backgroundColor = edge.Color;

                if (GUI.Button(new Rect(position, EdgeSize), edge.Name))
                {
                    ClearControl();
                    Selection = edge.RoomId;
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
                GUI.backgroundColor = node.Color;

                if (GUI.Button(new Rect(node.Position, NodeSize), $"({node.Id}) : {node.Name}"))
                {
                    ClearControl();
                    Selection = node.RoomId;
                }

                GUI.backgroundColor = Color.white;
            }
        }

        /// <summary>
        /// Clears the current selection.
        /// </summary>
        private void ClearSelection()
        {
            Selection = new Uid(0, 0, 2);
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
            GetLayoutGraph().Paginate(NodeSize + Spacing);
        }

        /// <summary>
        /// Adds a new node to the graph.
        /// </summary>
        private void CreateNode()
        {
            ClearControl();
            var node = GetLayoutGraph().CreateNode();
            Selection = node.RoomId;
        }
    }
}
