using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Drawing.Editor
{
    /// <summary>
    /// The LayoutTileMap editor.
    /// </summary>
    public static class LayoutTileMapBookEditor
    {
        /// <summary>
        /// Creates a new layout tilemap Game Object.
        /// </summary>
        [MenuItem("GameObject/Mania Map/Layout Tile Map Book", priority = 20)]
        [MenuItem("Mania Map/Create Layout Tile Map Book", priority = 100)]
        public static void CreateLayoutTilemap()
        {
            var obj = new GameObject("Layout Tile Map Book");
            obj.transform.SetParent(Selection.activeTransform);
            var layoutTilemap = obj.AddComponent<LayoutTileMapBook>();
            layoutTilemap.CreateGrid();
        }
    }
}
