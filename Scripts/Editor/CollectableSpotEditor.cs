using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CollectableSpot))]
    public class CollectableSpotEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!MultipleTargetsSelected())
            {
                DrawAutoAssignButton();
            }

            DrawDefaultInspector();
            DrawCellErrorBox();
            DrawCollectableGroupErrorBox();
            serializedObject.ApplyModifiedProperties();
        }

        private bool MultipleTargetsSelected()
        {
            return serializedObject.targetObjects.Length > 1;
        }

        private CollectableSpot GetCollectableSpot()
        {
            return (CollectableSpot)serializedObject.targetObject;
        }

        private void DrawAutoAssignButton()
        {
            if (GUILayout.Button("Auto Assign"))
            {
                AutoAssign();
            }
        }

        private void DrawCellErrorBox()
        {
            if (GetCollectableSpot().Cell == null)
                EditorGUILayout.HelpBox("No cell assigned to collectable spot.", MessageType.Error, true);
        }

        private void DrawCollectableGroupErrorBox()
        {
            if (GetCollectableSpot().Group == null)
                EditorGUILayout.HelpBox("No collectable group assigned to collectable spot.", MessageType.Error, true);
        }

        private void AutoAssign()
        {
            var spot = GetCollectableSpot();
            spot.AutoAssign();
            Debug.Log($"<color=#00FF00><b>Auto assigned collectable spot.</b></color>");
        }

        [MenuItem("GameObject/Mania Map/Collectable Spot", priority = 20)]
        public static void CreateCollectableSpot()
        {
            var obj = new GameObject("Collectable Spot");
            obj.AddComponent<CollectableSpot>();
            obj.transform.SetParent(Selection.activeTransform);
        }
    }
}
