using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Editor
{
    public class LayoutGraphWindowSettings : ScriptableObject
    {
        /// <summary>
        /// The height of the menu.
        /// </summary>
        public float MenuHeight { get; } = 20;

        [Header("Inspector Pane")]

        [SerializeField]
        private float _inspectorWidth = 300;
        /// <summary>
        /// The width of the inspector pane.
        /// </summary>
        public float InspectorWidth { get => _inspectorWidth; set => _inspectorWidth = Mathf.Max(value, 300); }

        [SerializeField]
        private float _inspectorLabelWidth = 100;
        /// <summary>
        /// The width of the labels in the inspector.
        /// </summary>
        public float InspectorLabelWidth { get => _inspectorLabelWidth; set => _inspectorLabelWidth = Mathf.Max(value, 100); }

        [Header("Plot Area")]

        [SerializeField]
        private Vector2 _nodeSize = new Vector2(150, 30);
        /// <summary>
        /// The size of the plotted node elements.
        /// </summary>
        public Vector2 NodeSize { get => _nodeSize; set => _nodeSize = Vector2.Max(value, new Vector2(100, 20)); }

        [SerializeField]
        private Vector2 _edgeSize = new Vector2(125, 30);
        /// <summary>
        /// The size of the plotted edge elements.
        /// </summary>
        public Vector2 EdgeSize { get => _edgeSize; set => _edgeSize = Vector2.Max(value, new Vector2(100, 20)); }

        [SerializeField]
        private Vector2 _spacing = new Vector2(20, 20);
        /// <summary>
        /// The minimum spacing between node elements.
        /// </summary>
        public Vector2 Spacing { get => _spacing; set => _spacing = Vector2.Max(value, Vector2.zero); }

        [SerializeField]
        private Vector2 _plotPadding = new Vector2(20, 20);
        /// <summary>
        /// The padding added to the right and bottom of the plot area.
        /// </summary>
        public Vector2 PlotPadding { get => _plotPadding; set => _plotPadding = Vector2.Max(value, Vector2.zero); }

        [SerializeField]
        private Color32 _hoverColor = new Color(0, 0, 1, 0.6f);
        /// <summary>
        /// The color overlayed on hovered node and edge elements.
        /// </summary>
        public Color32 HoverColor { get => _hoverColor; set => _hoverColor = value; }

        [SerializeField]
        private Color32 _selectedColor = new Color(1, 0, 0, 0.6f);
        /// <summary>
        /// The color overlayed on selected node and edge elements.
        /// </summary>
        public Color32 SelectedColor { get => _selectedColor; set => _selectedColor = value; }

        /// <summary>
        /// Returns the layout graph window settings. If the settings exist in the project,
        /// these settings are loaded and returned. Otherwise, the default settings are returned.
        /// </summary>
        public static LayoutGraphWindowSettings GetSettings()
        {
            var guids = AssetDatabase.FindAssets("t:LayoutGraphWindowSettings");

            if (guids.Length == 0)
            {
                return CreateInstance<LayoutGraphWindowSettings>();
            }

            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<LayoutGraphWindowSettings>(path);
        }

        private void OnValidate()
        {
            InspectorWidth = InspectorWidth;
            InspectorLabelWidth = InspectorLabelWidth;
            NodeSize = NodeSize;
            EdgeSize = EdgeSize;
            Spacing = Spacing;
            PlotPadding = PlotPadding;
        }
    }
}
