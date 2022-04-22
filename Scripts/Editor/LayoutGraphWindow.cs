using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace MPewsey.ManiaMap.Unity.Editor
{
    public class LayoutGraphWindow : EditorWindow
    {
        private static string LayoutGraphPath { get; set; }

        public static void ShowWindow(SerializedObject graph)
        {
            if (graph.targetObject is LayoutGraph)
            {
                LayoutGraphPath = AssetDatabase.GetAssetPath(graph.targetObject.GetInstanceID());
                var window = GetWindow<LayoutGraphWindow>();
                window.titleContent = new GUIContent("Layout Graph Editor");
                window.minSize = new Vector2(450, 200);
                window.maxSize = new Vector2(1920, 720);
            }
        }

        public void CreateGUI()
        {
            var splitView = new TwoPaneSplitView(0, 250, TwoPaneSplitViewOrientation.Horizontal);
            var leftPane = new VisualElement();
            var rightPane = new VisualElement();

            rootVisualElement.Add(splitView);
            splitView.Add(leftPane);
            splitView.Add(rightPane);
        }
    }
}