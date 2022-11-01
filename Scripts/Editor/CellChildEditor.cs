using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Editor
{
    /// <summary>
    /// The base class for CellChild editors.
    /// </summary>
    public abstract class CellChildEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawAutoAssignButton();
            DrawDefaultInspector();
            DrawCellErrorBox();
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Draws the auto assign button if it is enabled.
        /// </summary>
        protected void DrawAutoAssignButton()
        {
            if (!AutoAssignButtonEnabled())
                return;

            if (GUILayout.Button("Auto Assign"))
                AutoAssign();
        }

        /// <summary>
        /// Calls the auto assign method on the object, sets it to dirty, and logs a success message.
        /// </summary>
        protected void AutoAssign()
        {
            var obj = GetCellChild();
            obj.AutoAssign();
            EditorUtility.SetDirty(obj);
            Log.Success($"Auto assigned {obj}.");
        }

        /// <summary>
        /// Draws an error box if a cell has not been assigned.
        /// </summary>
        protected void DrawCellErrorBox()
        {
            if (CellIsAssigned())
                return;

            EditorGUILayout.HelpBox("Cell not assigned.", MessageType.Error, true);
        }

        /// <summary>
        /// True if a cell has been assigned to the obejct.
        /// </summary>
        protected bool CellIsAssigned()
        {
            return GetCellChild().Cell != null;
        }

        /// <summary>
        /// Returns the target cell child.
        /// </summary>
        protected CellChild GetCellChild()
        {
            return (CellChild)serializedObject.targetObject;
        }

        /// <summary>
        /// True if only a single object is selected, and it is open.
        /// </summary>
        /// <returns></returns>
        protected bool AutoAssignButtonEnabled()
        {
            return !MultipleTargetsSelected() && !TargetIsPrefabAsset();
        }

        /// <summary>
        /// True if the target object is an unopened prefab being inspected.
        /// </summary>
        protected bool TargetIsPrefabAsset()
        {
            return GetCellChild().gameObject.scene.name == null;
        }

        /// <summary>
        /// True if multiple target objects are selected.
        /// </summary>
        protected bool MultipleTargetsSelected()
        {
            return serializedObject.targetObjects.Length > 1;
        }
    }
}
