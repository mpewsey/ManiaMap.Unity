using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace MPewsey.ManiaMap.Unity.Editor
{
    public class LayoutGraphWindow : EditorWindow
    {
        /// <summary>
        /// The path to the layout graph asset.
        /// </summary>
        private static string Path { get; set; }

        /// <summary>
        /// The current layout graph.
        /// </summary>
        private LayoutGraph Graph { get; set; }

        /// <summary>
        /// The selected element ID.
        /// </summary>
        private Uid SelectionId { get; set; }

        private TwoPaneSplitView SplitView { get; set; }
        private VisualElement LeftPane { get; set; }
        private VisualElement RightPane { get; set; }

        public static void ShowWindow(LayoutGraph graph)
        {
            Path = AssetDatabase.GetAssetPath(graph);
            var window = GetWindow<LayoutGraphWindow>();
            window.titleContent = new GUIContent("Layout Graph Editor");
            window.minSize = new Vector2(450, 200);
            window.maxSize = new Vector2(1920, 720);
        }

        public void CreateGUI()
        {
            SplitView = new TwoPaneSplitView(0, 250, TwoPaneSplitViewOrientation.Horizontal);
            LeftPane = new VisualElement();
            RightPane = new VisualElement();

            rootVisualElement.Add(SplitView);
            SplitView.Add(LeftPane);
            SplitView.Add(RightPane);
        }

        public void OnGUI()
        {
            Graph = AssetDatabase.LoadAssetAtPath<LayoutGraph>(Path);

            if (Graph == null)
            {
                Close();
                return;
            }

            DrawInspector();
        }

        private void DrawInspector()
        {
            LeftPane.Clear();

            if (SelectionId.Value3 == 1)
            {
                DrawEdgeInspector();
                return;
            }

            DrawNodeInspector();
        }

        private void DrawNodeInspector()
        {
            var index = Graph.GetNodeIndex(SelectionId.Value1);

            if (index < 0)
                return;

            var serializedObject = new SerializedObject(Graph);
            var prop = serializedObject.FindProperty("_nodes").GetArrayElementAtIndex(index);
            var inspector = new InspectorElement(prop.objectReferenceValue);
            LeftPane.Add(inspector);
        }

        private void DrawEdgeInspector()
        {
            var index = Graph.GetEdgeIndex(SelectionId.Value1, SelectionId.Value2);

            if (index < 0)
                return;

            var serializedObject = new SerializedObject(Graph);
            var prop = serializedObject.FindProperty("_edges").GetArrayElementAtIndex(index);
            var inspector = new InspectorElement(prop.objectReferenceValue);
            LeftPane.Add(inspector);
        }
    }
}