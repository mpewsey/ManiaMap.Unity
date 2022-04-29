using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Editor
{
    public class LayoutGraphWindow : EditorWindow
    {
        private SerializedObject SerializedObject { get; set; }
        private LayoutGraphWindowSettings Settings { get; set; }
        private LayoutGraphPlot Plot { get; set; }

        public static void ShowWindow(LayoutGraph graph)
        {
            var window = GetWindow<LayoutGraphWindow>();
            window.titleContent = new GUIContent("Layout Graph Editor");
            window.minSize = new Vector2(450, 200);
            window.maxSize = new Vector2(1920, 720);
            window.SerializedObject = new SerializedObject(graph);
            window.Settings = LayoutGraphWindowSettings.GetSettings();
            window.Plot = new LayoutGraphPlot(graph);
        }

        public void OnGUI()
        {
            if (!LayoutGraphExists())
                return;

            DrawPlot();
        }

        private void DrawPlot()
        {
            var width = position.width - Settings.InspectorWidth;
            var height = position.height - Settings.MenuHeight;
            var rect = new Rect(Settings.InspectorWidth, Settings.MenuHeight, width, height);
            GUILayout.BeginArea(rect);
            Plot.OnGUI();
            GUILayout.EndArea();
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
    }
}
