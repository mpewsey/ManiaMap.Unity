using System;
using System.IO;
using System.Linq;
using UnityEditor;

namespace MPewsey.ManiaMapUnity.Editor
{
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
    }
}