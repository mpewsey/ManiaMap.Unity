using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Editor
{
    /// <summary>
    /// The CollectableSpot editor.
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CollectableSpotComponent))]
    public class CollectableSpotComponentEditor : CellChildEditor
    {
        [MenuItem("GameObject/Mania Map/Collectable Spot", priority = 20)]
        [MenuItem("Mania Map/Create Collectable Spot", priority = 100)]
        public static void CreateCollectableSpot()
        {
            var obj = new GameObject("Collectable Spot");
            obj.transform.SetParent(Selection.activeTransform);
            obj.AddComponent<CollectableSpotComponent>();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawInspector();
            var spot = (CollectableSpotComponent)serializedObject.targetObject;

            if (spot.Group == null)
                EditorGUILayout.HelpBox("Collectable group is not assigned.", MessageType.Error, true);

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawInspector()
        {
            var editId = ((CollectableSpotComponent)serializedObject.targetObject).EditId;

            GUI.enabled = false;
            var prop = serializedObject.GetIterator();
            var enterChildren = true;

            while (prop.NextVisible(enterChildren))
            {
                var disabled = prop.name == "m_Script" || (prop.name == "_id" && !editId);
                GUI.enabled = !disabled;
                EditorGUILayout.PropertyField(prop, true);
                enterChildren = false;
            }

            GUI.enabled = true;
        }
    }
}
