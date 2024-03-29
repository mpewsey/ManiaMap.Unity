using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Editor
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
            Log.Success($"Mania map settings created at: {path}");
            EditorGUIUtility.PingObject(settings);
        }
    }
}
