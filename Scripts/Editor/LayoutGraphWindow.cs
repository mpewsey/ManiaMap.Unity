using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Editor
{
    public class LayoutGraphWindow : EditorWindow
    {
        private const float MenuHeight = 20;
        private const float InspectorWidth = 300;
        private const float InspectorLabelWidth = 100;
        private const float NodeSpacing = 20;
        private static Vector2 NodePlotSize { get; } = new Vector2(150, 40);
        private static Vector2 EdgePlotSize { get; } = new Vector2(100, 50);
        private static SerializedObject SerializedObject { get; set; }
        
        private Vector2 InspectorScrollPosition { get; set; }
        private Vector2 PlotScrollPosition { get; set; }

        public static void ShowWindow(LayoutGraph graph)
        {
            SerializedObject = new SerializedObject(graph);
            var window = GetWindow<LayoutGraphWindow>();
            window.titleContent = new GUIContent("Graph");
            window.minSize = new Vector2(450, 200);
            window.maxSize = new Vector2(1920, 720);
        }

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

        private static bool LayoutGraphExists()
        {
            return SerializedObject != null
                && GetLayoutGraph() != null;
        }

        private void SetWindowTitle()
        {
            var name = SerializedObject.targetObject.name;
            titleContent.text = $"Graph [{name}]";
        }

        private void DrawMenuBar()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            DrawEditMenuButton();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private void DrawEditMenuButton()
        {
            if (GUILayout.Button("Edit", EditorStyles.toolbarDropDown))
            {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Create Node"), false, CreateNode);
                menu.DropDown(new Rect(0, 5, 0, 16));
            }
        }

        private void DrawInspector()
        {
            GUILayout.BeginArea(new Rect(0, MenuHeight, InspectorWidth, position.height - MenuHeight), GUI.skin.box);    
            InspectorScrollPosition = GUILayout.BeginScrollView(InspectorScrollPosition);
            DrawLayoutGraphInspector();
            DrawHorizontalLine();
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

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

        private static void DrawHorizontalLine()
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }

        private void DrawPlot()
        {
            GUILayout.BeginArea(new Rect(InspectorWidth, MenuHeight, position.width - InspectorWidth, position.height - MenuHeight));
            PlotScrollPosition = GUILayout.BeginScrollView(PlotScrollPosition);
            ApplyNodeSpacing();
            DrawEdges();
            DrawNodes();
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private void ApplyNodeSpacing()
        {
            var graph = GetLayoutGraph();
            var spacing = NodePlotSize + NodeSpacing * Vector2.one;
            graph.ApplyNodeSpacing(spacing);
        }

        private static GUIStyle ElementAreaStyle()
        {
            var style = new GUIStyle(GUI.skin.window);
            style.padding.top = 10;
            style.padding.bottom = 10;
            return style;
        }

        private static GUIStyle ElementLabelStyle()
        {
            var style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.MiddleCenter;
            return style;
        }

        private void DrawEdges()
        {
            var graph = GetLayoutGraph();
            var positions = graph.GetNodes().ToDictionary(x => x.Id, x => x.Position);

            foreach (var edge in graph.GetEdges())
            {
                var position = Vector2.Lerp(positions[edge.FromNode], positions[edge.ToNode], 0.5f);
                position += 0.5f * (NodePlotSize - EdgePlotSize);
                GUI.color = edge.Color;
                GUILayout.BeginArea(new Rect(position, EdgePlotSize), ElementAreaStyle());
                GUI.color = Color.white;
                GUILayout.Label(edge.Name, ElementLabelStyle());
                GUILayout.EndArea();
            }
        }

        private void DrawNodes()
        {
            var graph = GetLayoutGraph();

            foreach (var node in graph.GetNodes())
            {
                GUI.color = node.Color;
                GUILayout.BeginArea(new Rect(node.Position, NodePlotSize), ElementAreaStyle());
                GUI.color = Color.white;
                GUILayout.Label($"({node.Id}) : {node.Name}", ElementLabelStyle());
                GUILayout.EndArea();
            }
        }

        private void CreateNode()
        {
            GetLayoutGraph().CreateNode();
        }
    }
}
