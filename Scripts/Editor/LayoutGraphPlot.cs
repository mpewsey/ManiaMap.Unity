using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Editor
{
    public class LayoutGraphPlot
    {
        private SerializedObject SerializedObject { get; }
        private LayoutGraphWindowSettings Settings { get; }
        public HashSet<LayoutNode> SelectedNodes { get; } = new HashSet<LayoutNode>();
        public HashSet<LayoutEdge> SelectedEdges { get; } = new HashSet<LayoutEdge>();
        private bool Dragging { get; set; }
        private Vector2 LastMousePosition { get; set; }
        private Vector2 ScrollPosition { get; set; }
        private Dictionary<int, Vector2> NodePositions { get; set; } = new Dictionary<int, Vector2>();

        public LayoutGraphPlot(LayoutGraph graph)
        {
            SerializedObject = new SerializedObject(graph);
            Settings = LayoutGraphWindowSettings.GetSettings();
        }

        private LayoutGraph GetLayoutGraph()
        {
            return SerializedObject.targetObject as LayoutGraph;
        }

        private LayoutGraphWindow GetWindow()
        {
            return EditorWindow.GetWindow<LayoutGraphWindow>();
        }

        public void OnGUI()
        {
            SerializedObject.Update();
            ScrollPosition = GUILayout.BeginScrollView(ScrollPosition);
            Paginate();
            SetNodePositions();
            DrawEdgeLines();
            DrawNodes();
            DrawEdges();
            SetPlotBounds();
            HandleAreaExit();
            HandleAreaClick();
            LastMousePosition = Event.current.mousePosition;
            GUILayout.EndScrollView();
            SerializedObject.ApplyModifiedProperties();
            RepaintIfDragging();
        }

        public void OnLostFocus()
        {
            Dragging = false;
        }

        private void RepaintIfDragging()
        {
            if (Dragging)
            {
                GetWindow().Repaint();
            }
        }

        private void Paginate()
        {
            if (!Dragging)
            {
                var graph = GetLayoutGraph();

                if (graph.Paginate(Settings.NodeSize + Settings.Spacing))
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

        private void HandleAreaClick()
        {
            if (Event.current.type == EventType.MouseUp)
            {
                var mouse = Event.current.mousePosition;

                if (mouse.x >= 0 && mouse.y >= 0)
                {
                    GUI.FocusControl(null);
                    SelectedNodes.Clear();
                    SelectedEdges.Clear();
                    Event.current.Use();
                }
            }
        }

        private void HandleAreaExit()
        {
            var mouse = Event.current.mousePosition;

            if (mouse.x < 0 || mouse.y < 0)
            {
                Dragging = false;
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
            var position = 0.5f * (NodePositions[edge.FromNode] - Settings.EdgeSize + NodePositions[edge.ToNode]);
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
                    GUI.FocusControl(null);
                    SelectedEdges.Clear();
                    SelectedEdges.Add(edge);
                    Event.current.Use();
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
                    GUI.FocusControl(null);
                    SelectedNodes.Clear();
                    SelectedNodes.Add(node);
                    Event.current.Use();
                }
                else if (Event.current.type == EventType.MouseDown)
                {
                    GUI.FocusControl(null);
                    Dragging = true;
                    SelectedNodes.Add(node);
                    Event.current.Use();
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

        #region Actions
        public void CreateNode()
        {
            GUI.FocusControl(null);
            var graph = GetLayoutGraph();
            var node = graph.CreateNode();
            SelectedNodes.Clear();
            SelectedNodes.Add(node);
            AssetDatabase.AddObjectToAsset(node, graph);
            EditorUtility.SetDirty(graph);
        }

        public void DeleteSelectedNodes()
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

        public void DeleteSelectedEdges()
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

        public void DeleteSelectedElements()
        {
            DeleteSelectedEdges();
            DeleteSelectedNodes();
        }

        public void SelectAllNodes()
        {
            var graph = GetLayoutGraph();

            foreach (var node in graph.GetNodes())
            {
                SelectedNodes.Add(node);
            }
        }

        public void SelectAllEdges()
        {
            var graph = GetLayoutGraph();

            foreach (var edge in graph.GetEdges())
            {
                SelectedEdges.Add(edge);
            }
        }

        public void SelectAllElements()
        {
            SelectAllEdges();
            SelectAllNodes();
        }
        #endregion
    }
}