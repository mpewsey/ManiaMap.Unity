using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Editor
{
    /// <summary>
    /// The CollectableSpot editor.
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CollectableSpot))]
    public class CollectableSpotEditor : CellChildEditor
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
            obj.AddComponent<CollectableSpot>();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DrawCollectableGroupErrorBox();
        }

        /// <summary>
        /// Returns the target collectable spot.
        /// </summary>
        private CollectableSpot GetCollectableSpot()
        {
            return (CollectableSpot)serializedObject.targetObject;
        }

        /// <summary>
        /// Draws an error box if a collectable group has not been assigned to the collectable spot.
        /// </summary>
        private void DrawCollectableGroupErrorBox()
        {
            if (GroupIsAssigned())
                return;

            EditorGUILayout.HelpBox("Collectable group not assigned.", MessageType.Error, true);
        }

        /// <summary>
        /// True if the group is assigned.
        /// </summary>
        private bool GroupIsAssigned()
        {
            return GetCollectableSpot().Group != null;
        }
    }
}
