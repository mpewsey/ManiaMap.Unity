using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Editor
{
    /// <summary>
    /// The CollectableSpot editor.
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CollectableSpot))]
    public class CollectableSpotEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!MultipleTargetsSelected() && !TargetIsPrefabAsset())
            {
                DrawAutoAssignButton();
            }

            DrawDefaultInspector();
            DrawCellErrorBox();
            DrawCollectableGroupErrorBox();
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Returns true if the target object is an unopened prefab being inspected.
        /// </summary>
        private bool TargetIsPrefabAsset()
        {
            return GetCollectableSpot().gameObject.scene.name == null;
        }

        /// <summary>
        /// Returns true if multiple target objects are selected.
        /// </summary>
        private bool MultipleTargetsSelected()
        {
            return serializedObject.targetObjects.Length > 1;
        }

        /// <summary>
        /// Returns the target collectable spot.
        /// </summary>
        private CollectableSpot GetCollectableSpot()
        {
            return (CollectableSpot)serializedObject.targetObject;
        }

        /// <summary>
        /// Draws the auto assign button.
        /// </summary>
        private void DrawAutoAssignButton()
        {
            if (GUILayout.Button("Auto Assign"))
            {
                AutoAssign();
            }
        }

        /// <summary>
        /// Draws an error box if a cell has not been assigned to the collectable spot.
        /// </summary>
        private void DrawCellErrorBox()
        {
            if (GetCollectableSpot().Cell == null)
                EditorGUILayout.HelpBox("No cell assigned to collectable spot.", MessageType.Error, true);
        }

        /// <summary>
        /// Draws an error box if a collectable group has not been assigned to the collectable spot.
        /// </summary>
        private void DrawCollectableGroupErrorBox()
        {
            if (GetCollectableSpot().Group == null)
                EditorGUILayout.HelpBox("No collectable group assigned to collectable spot.", MessageType.Error, true);
        }

        /// <summary>
        /// Auto assigns elements to the target collectable spot.
        /// </summary>
        private void AutoAssign()
        {
            var spot = GetCollectableSpot();
            spot.AutoAssign();
            EditorUtility.SetDirty(spot);
            Debug.Log($"<color=#00FF00><b>Auto assigned collectable spot.</b></color>");
        }

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
    }
}
