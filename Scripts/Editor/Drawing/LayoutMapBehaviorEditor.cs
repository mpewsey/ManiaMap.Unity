using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Drawing.Editor
{
    /// <summary>
    /// The LayoutMap editor.
    /// </summary>
    public static class LayoutMapBehaviorEditor
    {
        /// <summary>
        /// Creates a new layout map Game Object.
        /// </summary>
        [MenuItem("GameObject/Mania Map/Layout Map Book", priority = 20)]
        [MenuItem("Mania Map/Create Layout Map Book", priority = 100)]
        public static void CreateLayoutMap()
        {
            var obj = new GameObject("Layout Map Book");
            obj.transform.SetParent(Selection.activeTransform);
            obj.AddComponent<LayoutMapBook>();
        }
    }
}
