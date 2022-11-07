using UnityEditor;

namespace MPewsey.ManiaMap.Unity.Editor
{
    [CustomEditor(typeof(TemplateGroup))]
    public class TemplateGroupEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            CreateEntriesFromTemplates();
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
        /// Adds elements from the templates list to the entries list.
        /// </summary>
        private void CreateEntriesFromTemplates()
        {
            var group = GetTemplateGroup();

            if (group.CreateEntriesFromTemplates())
                EditorUtility.SetDirty(group);
        }
    }
}