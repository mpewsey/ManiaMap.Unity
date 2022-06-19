using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Drawing.Editor
{
    /// <summary>
    /// The LayoutMap editor.
    /// </summary>
    public static class LayoutMapEditor
    {
        /// <summary>
        /// Creates a new layout map Game Object.
        /// </summary>
        [MenuItem("GameObject/Mania Map/Layout Map", priority = 20)]
        [MenuItem("Mania Map/Create Layout Map", priority = 100)]
        public static void CreateLayoutMap()
        {
            var obj = new GameObject("Layout Map");
            obj.transform.SetParent(Selection.activeTransform);
            var layoutMap = obj.AddComponent<LayoutMap>();
            layoutMap.CreateLayersContainer();
        }
    }
}
