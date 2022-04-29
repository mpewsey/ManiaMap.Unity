using System.Collections.Generic;
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
            DrawEdges();
            DrawNodes();
            DrawPlotBounds();
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

        private void DrawPlotBounds()
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
    }
}