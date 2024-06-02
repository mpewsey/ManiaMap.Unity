using MPewsey.ManiaMap;
using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Drawing.Editor
{
    /// <summary>
    /// The LayoutTilemap editor.
    /// </summary>
    public static class LayoutTilemapBehaviorEditor
    {
        /// <summary>
        /// Creates a new layout tilemap Game Object.
        /// </summary>
        [MenuItem("GameObject/Mania Map/Layout Tile Map", priority = 20)]
        [MenuItem("Mania Map/Create Layout Tile Map", priority = 100)]
        public static void CreateLayoutTilemap()
        {
            var obj = new GameObject("Layout Tile Map");
            obj.transform.SetParent(Selection.activeTransform);
            var layoutTilemap = obj.AddComponent<LayoutTileMap>();
            layoutTilemap.CreateGrid();
        }
    }
}
