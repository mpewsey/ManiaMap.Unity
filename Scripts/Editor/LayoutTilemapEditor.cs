using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Drawing.Editor
{
    /// <summary>
    /// The LayoutTilemap editor.
    /// </summary>
    public static class LayoutTilemapEditor
    {
        /// <summary>
        /// Creates a new layout tilemap Game Object.
        /// </summary>
        [MenuItem("GameObject/Mania Map/Layout Tilemap", priority = 20)]
        [MenuItem("Mania Map/Create Layout Tilemap", priority = 100)]
        public static void CreateLayoutTilemap()
        {
            var obj = new GameObject("Layout Tilemap");
            obj.transform.SetParent(Selection.activeTransform);
            var layoutTilemap = obj.AddComponent<LayoutTilemap>();
            layoutTilemap.CreateGrid();
        }
    }
}
