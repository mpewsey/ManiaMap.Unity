using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Editor
{
    /// <summary>
    /// The TemplateGroup editor.
    /// </summary>
    [CustomEditor(typeof(TemplateGroup))]
    public class TemplateGroupEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDragAndDropArea();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Returns the target template group.
        /// </summary>
        private TemplateGroup GetTemplateGroup()
        {
            return (TemplateGroup)serializedObject.targetObject;
        }

        /// <summary>
        /// Draws the drag and drop area and evaluates user input within the area.
        /// </summary>
        private void DrawDragAndDropArea()
        {
            var rect = GUILayoutUtility.GetRect(0, 50, GUILayout.ExpandWidth(true));
            var style = new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleCenter };
            GUI.Box(rect, "Drag and Drop Here to Add Room Templates", style);

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

        /// <summary>
        /// Adds any dragged and dropped templates to the group.
        /// </summary>
        private void AddDragAndDropTemplates()
        {
            var group = GetTemplateGroup();

            foreach (var obj in DragAndDrop.objectReferences)
            {
                group.AddTemplate(obj as RoomTemplateResource);
            }

            if (DragAndDrop.objectReferences.Length > 0)
                EditorUtility.SetDirty(group);
        }
    }
}