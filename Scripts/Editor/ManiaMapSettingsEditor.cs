using System;
using System.IO;
using System.Linq;
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

            CreateDirectory("Assets/Resources/ManiaMap");
            settings = CreateInstance<ManiaMapSettings>();
            var path = "Assets/Resources/ManiaMap/ManiaMapSettings.asset";
            AssetDatabase.CreateAsset(settings, path);
            Debug.Log($"<color=#00FF00><b>Mania map settings created at: {path}</b></color>");
            EditorGUIUtility.PingObject(settings);
        }

        /// <summary>
        /// Creates the directory in the assets folder if it does not already exist.
        /// The path must begin with the Assets directory.
        /// </summary>
        /// <param name="path">The directory path.</param>
        /// <exception cref="ArgumentException">Raised if the specified path is empty or does not begin with the Assets directory.</exception>
        private static void CreateDirectory(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
                return;

            var separators = new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };
            var directories = path.Split(separators);

            if (directories.Length == 0)
                throw new ArgumentException($"Path is empty: {path}.");
            if (directories[0] != "Assets")
                throw new ArgumentException($"Path does not begin with Assets directory: {path}.");

            for (int i = 1; i < directories.Length; i++)
            {
                var directory = Path.Combine(directories.Take(i + 1).ToArray());

                if (!AssetDatabase.IsValidFolder(directory))
                {
                    var root = Path.Combine(directories.Take(i).ToArray());
                    AssetDatabase.CreateFolder(root, directories[i]);
                }
            }
        }
    }
}
