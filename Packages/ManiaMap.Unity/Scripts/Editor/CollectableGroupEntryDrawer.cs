using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Editor
{
    /// <summary>
    /// The CollectableGroupEntry custom property drawer. The drawer is drawn without the typical dropdown.
    /// </summary>
    [CustomPropertyDrawer(typeof(CollectableGroupEntry))]
    public class CollectableGroupEntryDrawer : PropertyDrawer
    {
        private const float Spacing = 2;
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
