using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Drawing.Editor
{
    public static class LayoutTilemapEditor
    {
        [MenuItem("GameObject/Mania Map/Layout Tilemap", priority = 20)]
        [MenuItem("Mania Map/Create Layout Tilemap", priority = 100)]
        public static void CreateLayoutTilemap()
        {
            var obj = new GameObject("Layout Tilemap");
            var layoutTilemap = obj.AddComponent<LayoutTilemap>();
            layoutTilemap.CreateGrid();
            obj.transform.SetParent(Selection.activeTransform);
        }
    }
}
