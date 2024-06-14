using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Editor
{
    /// <summary>
    /// The RoomTemplateResource editor.
    /// </summary>
    [CustomEditor(typeof(RoomTemplateResource))]
    public class RoomTemplateResourceEditor : UnityEditor.Editor
    {
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

        private static bool PrefabGuidIsValid(string guid)
        {
            if (string.IsNullOrWhiteSpace(guid))
                return false;

            var assetPath = AssetDatabase.GUIDToAssetPath(guid);

            if (string.IsNullOrEmpty(assetPath))
                return false;

            return true;
        }

        private static void OpenPrefab(RoomTemplateResource template)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(template.PrefabGuid);
            var obj = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            AssetDatabase.OpenAsset(obj);
        }

        private void DrawInspector()
        {
            var editId = ((RoomTemplateResource)serializedObject.targetObject).EditId;

            GUI.enabled = false;
            var prop = serializedObject.GetIterator();
            var enterChildren = true;

            while (prop.NextVisible(enterChildren))
            {
                GUI.enabled = prop.name == "_editId" || (prop.name == "_id" && editId);
                EditorGUILayout.PropertyField(prop, true);
                enterChildren = false;
            }

            GUI.enabled = true;
        }
    }
}