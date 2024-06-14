using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Editor
{
    /// <summary>
    /// The CollectableGroup editor.
    /// </summary>
    [CustomEditor(typeof(CollectableGroup))]
    public class CollectableGroupEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDragAndDropArea();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawDragAndDropArea()
        {
            var rect = GUILayoutUtility.GetRect(0, 50, GUILayout.ExpandWidth(true));
            var style = new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleCenter };
            GUI.Box(rect, "Drag and Drop Here to Add Collectables", style);

            if (rect.Contains(Event.current.mousePosition))
            {
                switch (Event.current.type)
                {
                    case EventType.DragUpdated:
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        Event.current.Use();
                        break;
                    case EventType.DragPerform:
                        AddDragAndDropTemplates();
                        Event.current.Use();
                        break;
                }
            }
        }

        private void AddDragAndDropTemplates()
        {
            var group = (CollectableGroup)serializedObject.targetObject;

            foreach (var obj in DragAndDrop.objectReferences)
            {
                group.AddCollectable(obj as CollectableResource);
            }

            if (DragAndDrop.objectReferences.Length > 0)
                EditorUtility.SetDirty(group);
        }
    }
}