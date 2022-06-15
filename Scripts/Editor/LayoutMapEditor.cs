using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Drawing.Editor
{
    public static class LayoutMapEditor
    {
        [MenuItem("GameObject/Mania Map/Layout Map", priority = 20)]
        [MenuItem("Mania Map/Create Layout Map", priority = 100)]
        public static void CreateLayoutMap()
        {
            var obj = new GameObject("Layout Map");
            var layoutMap = obj.AddComponent<LayoutMap>();
            layoutMap.CreateContainer();
            obj.transform.SetParent(Selection.activeTransform);
        }
    }
}
