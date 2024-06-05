using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Editor
{
    /// <summary>
    /// The RoomTemplate editor.
    /// </summary>
    [CustomEditor(typeof(RoomTemplateResource))]
    public class RoomTemplateResourceEditor : UnityEditor.Editor
    {
        /// <summary>
        /// If the assigned prefab GUID is valid, opens the prefab.
        /// </summary>
        [UnityEditor.Callbacks.OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            var path = AssetDatabase.GetAssetPath(instanceId);
            var template = AssetDatabase.LoadAssetAtPath<RoomTemplateResource>(path);

            if (template == null)
                return false;

            if (!PrefabGuidIsValid(template.PrefabGuid))
                return false;

            OpenPrefab(template);
            return true;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var template = (RoomTemplateResource)serializedObject.targetObject;

            if (PrefabGuidIsValid(template.PrefabGuid) && GUILayout.Button("Open Prefab"))
                OpenPrefab(template);

            DrawInspector();
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Returns true if the GUID is not null and is in the project database.
        /// </summary>
        /// <param name="guid">The GUID.</param>
        private static bool PrefabGuidIsValid(string guid)
        {
            if (string.IsNullOrWhiteSpace(guid))
                return false;

            var assetPath = AssetDatabase.GUIDToAssetPath(guid);

            if (string.IsNullOrEmpty(assetPath))
                return false;

            return true;
        }

        /// <summary>
        /// Opens the prefab for the specified room template.
        /// </summary>
        /// <param name="template">The room template.</param>
        private static void OpenPrefab(RoomTemplateResource template)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(template.PrefabGuid);
            var obj = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            AssetDatabase.OpenAsset(obj);
        }

        /// <summary>
        /// Draws the inspector with all fields inactive.
        /// </summary>
        private void DrawInspector()
        {
            GUI.enabled = false;
            var prop = serializedObject.GetIterator();
            var enterChildren = true;

            while (prop.NextVisible(enterChildren))
            {
                EditorGUILayout.PropertyField(prop, true);
                enterChildren = false;
            }

            GUI.enabled = true;
        }
    }
}