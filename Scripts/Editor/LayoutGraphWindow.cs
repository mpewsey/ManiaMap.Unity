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
        /// A list of currently selected nodes.
        /// </summary>
        private List<Object> TargetNodes { get; set; } = new List<Object>();

        /// <summary>
        /// A list of currently selected edges.
        /// </summary>
        private List<Object> TargetEdges { get; set; } = new List<Object>();

        /// <summary>
        /// The current graph editor.
        /// </summary>
        private UnityEditor.Editor GraphEditor { get; set; }

        /// <summary>
        /// The current node editor.
        /// </summary>
        private UnityEditor.Editor NodeEditor { get; set; }

        /// <summary>
        /// The current edge editor.
        /// </summary>
        private UnityEditor.Editor EdgeEditor { get; set; }

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

        private void OnDestroy()
        {
            DestroyImmediate(GraphEditor);
            DestroyImmediate(NodeEditor);
            DestroyImmediate(EdgeEditor);
        }

        /// <summary>
        /// Returns the layout graph editor.
        /// </summary>
        private UnityEditor.Editor GetGraphEditor()
        {
            if (GraphEditor != null)
                return GraphEditor;

            GraphEditor = UnityEditor.Editor.CreateEditor(SerializedObject.targetObject, typeof(LayoutGraphWindowEditor));
            return GraphEditor;
        }

        /// <summary>
        /// Returns the node editor.
        /// </summary>
        /// <param name="nodes">A list of nodes to assign to the editor.</param>
        private UnityEditor.Editor GetNodeEditor(List<Object> nodes)
        {
            if (NodeEditor == null)
            {
                NodeEditor = UnityEditor.Editor.CreateEditor(nodes.ToArray());
                return NodeEditor;
            }

            if (!nodes.SequenceEqual(NodeEditor.targets))
            {
                DestroyImmediate(NodeEditor);
                NodeEditor = UnityEditor.Editor.CreateEditor(nodes.ToArray());
                return NodeEditor;
            }

            return NodeEditor;
        }

        /// <summary>
        /// Returns the edge editor.
        /// </summary>
        /// <param name="edges">A list of edges to assign to the editor.</param>
        private UnityEditor.Editor GetEdgeEditor(List<Object> edges)
        {
            if (EdgeEditor == null)
            {
                EdgeEditor = UnityEditor.Editor.CreateEditor(edges.ToArray());
                return EdgeEditor;
            }

            if (!edges.SequenceEqual(EdgeEditor.targets))
            {
                DestroyImmediate(EdgeEditor);
                EdgeEditor = UnityEditor.Editor.CreateEditor(edges.ToArray());
                return EdgeEditor;
            }

            return EdgeEditor;
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
            EditorGUIUtility.labelWidth = InspectorLabelWidth;
            var editor = GetGraphEditor();
            editor.OnInspectorGUI();
        }

        /// <summary>
        /// Draws the node inspector.
        /// </summary>
        private void DrawNodeInspector()
        {
            TargetNodes.RemoveAll(x => x == null);

            if (TargetNodes.Count == 0)
                return;

            EditorGUIUtility.labelWidth = InspectorLabelWidth;
            GUILayout.Label("Selected Nodes", EditorStyles.boldLabel);
            GUI.enabled = false;
            var ids = TargetNodes.Cast<LayoutNode>().Select(x => x.Id);
            EditorGUILayout.TextField("Id", string.Join(", ", ids));
            var editor = GetNodeEditor(TargetNodes);
            editor.OnInspectorGUI();
        }

        /// <summary>
        /// Draws the edge inspector.
        /// </summary>
        private void DrawEdgeInspector()
        {
            TargetEdges.RemoveAll(x => x == null);

            if (TargetEdges.Count == 0)
                return;

            EditorGUIUtility.labelWidth = InspectorLabelWidth;
            GUILayout.Label("Selected Edges", EditorStyles.boldLabel);
            GUI.enabled = false;
            var ids = TargetEdges.Cast<LayoutEdge>().Select(x => $"({x.FromNode}, {x.ToNode})");
            EditorGUILayout.TextField("Id", string.Join(", ", ids));
            var editor = GetEdgeEditor(TargetEdges);
            editor.OnInspectorGUI();
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
                GUI.backgroundColor = node.Color;

                if (GUI.Button(new Rect(node.Position, NodeSize), $"({node.Id}) : {node.Name}"))
                {
                    ClearControl();
                    TargetNodes.Clear();
                    TargetNodes.Add(node);
                }

                GUI.backgroundColor = Color.white;
            }
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
            AssetDatabase.AddObjectToAsset(node, SerializedObject.targetObject);
            TargetNodes.Clear();
            TargetNodes.Add(node);
        }
    }
}
