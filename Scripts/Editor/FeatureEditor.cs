using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Editor
{
    /// <summary>
    /// The Feature editor.
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Feature))]
    public class FeatureEditor : UnityEditor.Editor
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
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Draws an error box if a cell has not been assigned to the collectable spot.
        /// </summary>
        private void DrawCellErrorBox()
        {
            if (GetFeature().Cell == null)
                EditorGUILayout.HelpBox("No cell assigned to feature.", MessageType.Error, true);
        }

        /// <summary>
        /// Returns true if the target object is an unopened prefab being inspected.
        /// </summary>
        private bool TargetIsPrefabAsset()
        {
            return GetFeature().gameObject.scene.name == null;
        }

        /// <summary>
        /// Returns true if multiple target objects are selected.
        /// </summary>
        private bool MultipleTargetsSelected()
        {
            return serializedObject.targetObjects.Length > 1;
        }

        private Feature GetFeature()
        {
            return (Feature)serializedObject.targetObject;
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
        /// Auto assigns elements to the target collectable spot.
        /// </summary>
        private void AutoAssign()
        {
            var spot = GetFeature();
            spot.AutoAssign();
            EditorUtility.SetDirty(spot);
            Log.Success("Auto assigned feature.");
        }
    }
}
