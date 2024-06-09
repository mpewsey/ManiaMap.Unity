using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Editor
{
    /// <summary>
    /// The TemplateGroupEntry custom property drawer.
    /// </summary>
    [CustomPropertyDrawer(typeof(TemplateGroupEntry))]
    public class TemplateGroupEntryDrawer : PropertyDrawer
    {
        /// <summary>
        /// The spacing between properties.
        /// </summary>
        private const float Spacing = 2;

        /// <summary>
        /// The top and bottom padding.
        /// </summary>
        private const float Padding = 4;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.y += Padding;

            foreach (SerializedProperty prop in property)
            {
                position.height = EditorGUI.GetPropertyHeight(prop);
                EditorGUI.PropertyField(position, prop, new GUIContent(prop.displayName));
                position.y += position.height + Spacing;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = Padding;

            foreach (SerializedProperty prop in property)
            {
                height += EditorGUI.GetPropertyHeight(prop) + Spacing;
            }

            height += Padding - Spacing;
            return height;
        }
    }
}
