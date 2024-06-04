using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Editor
{
    /// <summary>
    /// The CollectableSpot editor.
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CollectableSpotBehavior))]
    public class CollectableSpotBehaviorEditor : CellChildEditor
    {
        /// <summary>
        /// Creates a new collectable spot Game Object.
        /// </summary>
        [MenuItem("GameObject/Mania Map/Collectable Spot", priority = 20)]
        [MenuItem("Mania Map/Create Collectable Spot", priority = 100)]
        public static void CreateCollectableSpot()
        {
            var obj = new GameObject("Collectable Spot");
            obj.transform.SetParent(Selection.activeTransform);
            obj.AddComponent<CollectableSpotBehavior>();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var spot = (CollectableSpotBehavior)serializedObject.targetObject;

            if (spot.Group == null)
                EditorGUILayout.HelpBox("Collectable group not assigned.", MessageType.Error, true);
        }
    }
}
