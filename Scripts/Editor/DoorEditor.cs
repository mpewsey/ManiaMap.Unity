using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Editor
{
    /// <summary>
    /// The Door editor.
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Door))]
    public class DoorEditor : CellChildEditor
    {
        /// <summary>
        /// Creates a new door Game Object.
        /// </summary>
        [MenuItem("GameObject/Mania Map/Door", priority = 20)]
        [MenuItem("Mania Map/Create Door", priority = 100)]
        public static void CreateDoor()
        {
            var obj = new GameObject("Door");
            obj.transform.SetParent(Selection.activeTransform);
            obj.AddComponent<Door>();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawAutoAssignButton();
            DrawFields(x => x.name == "m_Script" || x.name.StartsWith("_auto"));
            DrawFields(x => x.name != "m_Script" && !x.name.StartsWith("_auto"));
            DrawCellErrorBox();
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Draws the fields for which the specified selector returns true.
        /// </summary>
        /// <param name="selector">The selector function.</param>
        private void DrawFields(System.Func<SerializedProperty, bool> selector)
        {
            var iterator = serializedObject.GetIterator();
            var enterChildren = true;

            while (iterator.NextVisible(enterChildren))
            {
                using (new EditorGUI.DisabledScope("m_Script" == iterator.propertyPath))
                {
                    if (selector.Invoke(iterator))
                        EditorGUILayout.PropertyField(iterator, true);
                }

                enterChildren = false;
            }
        }
    }
}