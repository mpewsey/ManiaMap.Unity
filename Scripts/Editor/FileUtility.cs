using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;

namespace MPewsey.ManiaMapUnity.Editor
{
    /// <summary>
    /// Contains methods for dealing with files in in %Unity.
    /// </summary>
    public static class FileUtility
    {
        /// <summary>
        /// Creates the directory in the assets folder if it does not already exist.
        /// The path must begin with the Assets directory.
        /// </summary>
        /// <param name="path">The directory path.</param>
        /// <exception cref="ArgumentException">Raised if the specified path is empty or does not begin with the Assets directory.</exception>
        public static void CreateDirectory(string path)
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

        /// <summary>
        /// Replaces all invalid filename characters in the string with the specified character.
        /// </summary>
        /// <param name="str">The original string.</param>
        /// <param name="replacement">The replacement character.</param>
        public static string ReplaceInvalidFileNameCharacters(string str, char replacement = '_')
        {
            var characters = Path.GetInvalidFileNameChars();

            // Make sure replacement character itself is valid.
            if (characters.Contains(replacement))
                throw new ArgumentException($"Invalid replacement character: {replacement}.");

            var builder = new StringBuilder(str);

            foreach (var character in characters)
            {
                builder.Replace(character, replacement);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Returns an array of prefab asset GUID's at the specified search paths.
        /// </summary>
        /// <param name="searchPaths">An array of search paths.</param>
        public static string[] FindPrefabGuids(string[] searchPaths)
        {
            return AssetDatabase.FindAssets("t:prefab", searchPaths);
        }

        /// <summary>
        /// Return an array of prefab paths at the specified search paths.
        /// </summary>
        /// <param name="searchPaths">An array of search paths.</param>
        public static string[] FindPrefabPaths(string[] searchPaths)
        {
            var paths = FindPrefabGuids(searchPaths);

            for (int i = 0; i < paths.Length; i++)
            {
                paths[i] = AssetDatabase.GUIDToAssetPath(paths[i]);
            }

            return paths;
        }
    }
}
