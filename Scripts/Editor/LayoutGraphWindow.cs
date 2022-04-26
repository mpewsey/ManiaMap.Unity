using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Editor
{
    public class LayoutGraphWindow : EditorWindow
    {
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
        private Vector2 NodeSize { get; } = new Vector2(150, 40);

        /// <summary>
        /// The size of the plotted edge elements.
        /// </summary>
        private Vector2 EdgeSize { get; } = new Vector2(125, 40);

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
        /// The index of the top node at the current mouse position.
        /// </summary>
        private int HoveredNode { get; set; } = -1;

        /// <summary>
        /// The index of the top edge at the current mouse position.
        /// </summary>
        private int HoveredEdge { get; set; } = -1;

        private int SelectedNode { get; set; } = -1;
        private int SelectedEdge { get; set; } = -1;

        /// <summary>
        /// Shows the window for the specified layout graph.
        /// </summary>
        /// <param name="graph">The target layout graph.</param>
        public static void ShowWindow(LayoutGraph graph)
        {
            SerializedObject = new SerializedObject(graph);
            var window = GetWindow<LayoutGraphWindow>();
            window.titleContent = new GUIContent("Graph");
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
            {
                Close();
                return;
            }

            SerializedObject.Update();
            SetWindowTitle();
            DrawMenuBar();
            DrawPlot();
            DrawInspector();
            SerializedObject.ApplyModifiedProperties();
        }

        private void DrawNodeInspector()
        {
            var count = typeof(LayoutNode).GetProperties().Length - 2;
            var nodes = SerializedObject.FindProperty("_nodes");
            var prop = nodes.GetArrayElementAtIndex(HoveredNode);
            var enterChildren = true;

            for (int i = 0; i < count; i++)
            {
                prop.NextVisible(enterChildren);
                GUI.enabled = prop.name != "_id" && prop.name != "_position";
                EditorGUILayout.PropertyField(prop, true);
                enterChildren = false;
            }
        }

        private void DrawEdgeInspector()
        {

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
        /// Sets the window title.
        /// </summary>
        private void SetWindowTitle()
        {
            var name = SerializedObject.targetObject.name;
            titleContent.text = $"Graph [{name}]";
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
            DrawLayoutGraphInspector();
            DrawHorizontalSeparator();

            Debug.Log(Event.current.type);

            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                if (HoveredNode >= 0)
                {
                    SelectedNode = HoveredNode;
                    SelectedEdge = -1;
                }
                else if (HoveredEdge >= 0)
                {
                    SelectedNode = -1;
                    SelectedEdge = HoveredEdge;
                }
            }

            if (SelectedNode >= 0)
            {
                DrawNodeInspector();
            }
            else if (SelectedEdge >= 0)
            {
                DrawEdgeInspector();
            }

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        /// <summary>
        /// Draws the layout graph properties in the inspector.
        /// </summary>
        private void DrawLayoutGraphInspector()
        {
            EditorGUIUtility.labelWidth = InspectorLabelWidth;
            var prop = SerializedObject.GetIterator();
            var enterChildren = true;

            while (prop.NextVisible(enterChildren))
            {
                if (prop.name != "_nodes" && prop.name != "_edges")
                {
                    GUI.enabled = prop.name == "_id" || prop.name == "_name";
                    EditorGUILayout.PropertyField(prop, true);
                }

                enterChildren = false;
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
        /// Adds an empty label with the bounds of the plot. Without this, the scroll view won't work.
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
            HoveredNode = -1;
            var graph = GetLayoutGraph();
            var areaStyle = ElementAreaStyle();
            var labelStyle = ElementLabelStyle();
            var positions = graph.GetNodes().ToDictionary(x => x.Id, x => x.Position);
            var edges = graph.GetEdges();
            var edgeRect = new Rect(Vector2.zero, EdgeSize);

            for (int i = 0; i < edges.Count; i++)
            {
                var edge = edges[i];
                var position = 0.5f * (positions[edge.FromNode] + positions[edge.ToNode]);
                position += 0.5f * (NodeSize - EdgeSize);
                GUI.color = edge.Color;
                GUILayout.BeginArea(new Rect(position, EdgeSize), areaStyle);
                GUI.color = Color.white;
                GUILayout.Label(edge.Name, labelStyle);

                if (edgeRect.Contains(Event.current.mousePosition))
                    HoveredEdge = i;

                GUILayout.EndArea();
            }
        }

        /// <summary>
        /// Draws the nodes of the plot.
        /// </summary>
        private void DrawNodes()
        {
            HoveredNode = -1;
            var graph = GetLayoutGraph();
            var areaStyle = ElementAreaStyle();
            var labelStyle = ElementLabelStyle();
            var nodes = graph.GetNodes();
            var nodeRect = new Rect(Vector2.zero, NodeSize);

            for (int i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];
                GUI.color = node.Color;
                GUILayout.BeginArea(new Rect(node.Position, NodeSize), areaStyle);
                GUI.color = Color.white;
                GUILayout.Label($"({node.Id}) : {node.Name}", labelStyle);

                if (nodeRect.Contains(Event.current.mousePosition))
                    HoveredNode = i;

                GUILayout.EndArea();
            }
        }

        /// <summary>
        /// The style of the node and edge area elements.
        /// </summary>
        private static GUIStyle ElementAreaStyle()
        {
            var style = new GUIStyle(GUI.skin.button);
            style.padding.top = 10;
            style.padding.bottom = 10;
            return style;
        }

        /// <summary>
        /// The style of the node and edge label elements.
        /// </summary>
        private static GUIStyle ElementLabelStyle()
        {
            var style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.MiddleCenter;
            return style;
        }

        /// <summary>
        /// Adds a new node to the graph.
        /// </summary>
        private void CreateNode()
        {
            GetLayoutGraph().CreateNode();
        }
    }
}
