using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Editor
{
    /// <summary>
    /// The ManiaMapSettings editor.
    /// </summary>
    [CustomEditor(typeof(ManiaMapSettings))]
    public class ManiaMapSettingsEditor : UnityEditor.Editor
    {
        /// <summary>
        /// If a ManiaMap/ManiaMapSettings resource does not already exist, creates it.
        /// </summary>
        [MenuItem("Mania Map/Create Mania Map Settings", priority = 100000)]
        public static void CreateManiaMapSettings()
        {
            var settings = Resources.Load<ManiaMapSettings>("ManiaMap/ManiaMapSettings");

            if (settings != null)
            {
                Debug.Log("Settings file already exists.");
                EditorGUIUtility.PingObject(settings);
                return;
            }

            FileUtility.CreateDirectory("Assets/Resources/ManiaMap");
            settings = CreateInstance<ManiaMapSettings>();
            var path = "Assets/Resources/ManiaMap/ManiaMapSettings.asset";
            AssetDatabase.CreateAsset(settings, path);
            Debug.Log($"<color=#00FF00><b>Mania Map settings created at: {path}</b></color>");
            EditorGUIUtility.PingObject(settings);
        }
    }
}
