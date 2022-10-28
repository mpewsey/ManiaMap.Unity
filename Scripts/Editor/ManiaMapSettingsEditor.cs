using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Editor
{
    /// <summary>
    /// The ManiaMapSettings editor.
    /// </summary>
    public static class ManiaMapSettingsEditor
    {
        /// <summary>
        /// If Mania Map Settings do not already exist, creates them in the
        /// resources folder.
        /// </summary>
        [MenuItem("Mania Map/Create Mania Map Settings", priority = 1000)]
        public static void CreateSettings()
        {
            var settings = ManiaMapSettings.LoadSettings();

            if (settings != null)
            {
                Debug.Log("Settings already exist.");
                return;
            }

            FileUtility.CreateDirectory("Assets/Resources/ManiaMap");
            settings = ScriptableObject.CreateInstance<ManiaMapSettings>();
            var path = "Assets/Resources/ManiaMap/ManiaMapSettings.asset";
            AssetDatabase.CreateAsset(settings, path);
            Log.Success($"Settings created at {path}");
        }
    }
}