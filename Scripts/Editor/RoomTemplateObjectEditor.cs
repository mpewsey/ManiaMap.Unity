using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Editor
{
    /// <summary>
    /// The RoomTemplate editor.
    /// </summary>
    [CustomEditor(typeof(RoomTemplateObject))]
    public class RoomTemplateObjectEditor : UnityEditor.Editor
    {
        /// <summary>
        /// If the assigned prefab GUID is valid, opens the prefab.
        /// </summary>
        [UnityEditor.Callbacks.OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            var path = AssetDatabase.GetAssetPath(instanceId);
            var template = AssetDatabase.LoadAssetAtPath<RoomTemplateObject>(path);

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
            DrawOpenPrefabButton();
            DrawInspector();
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Returns the target room template.
        /// </summary>
        /// <returns></returns>
        private RoomTemplateObject GetRoomTemplate()
        {
            return (RoomTemplateObject)serializedObject.targetObject;
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
        /// Drawsn the open prefab button if the prefab GUID is valid.
        /// </summary>
        private void DrawOpenPrefabButton()
        {
            var template = GetRoomTemplate();

            if (!PrefabGuidIsValid(template.PrefabGuid))
                return;

            if (GUILayout.Button("Open Prefab"))
                OpenPrefab(template);
        }

        /// <summary>
        /// Opens the prefab for the specified room template.
        /// </summary>
        /// <param name="template">The room template.</param>
        private static void OpenPrefab(RoomTemplateObject template)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(template.PrefabGuid);
            var obj = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            AssetDatabase.OpenAsset(obj);
        }

        /// <summary>
        /// Draws the inspector will all fields inactive.
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