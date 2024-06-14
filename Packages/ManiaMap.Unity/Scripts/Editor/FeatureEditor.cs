using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Editor
{
    /// <summary>
    /// The Feature editor.
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Feature))]
    public class FeatureEditor : CellChildEditor
    {
        [MenuItem("GameObject/Mania Map/Feature", priority = 20)]
        [MenuItem("Mania Map/Create Feature", priority = 100)]
        public static void CreateFeature()
        {
            var obj = new GameObject("Feature");
            obj.transform.SetParent(Selection.activeTransform);
            obj.AddComponent<Feature>();
        }

    }
}
