using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;

namespace MPewsey.ManiaMap.Unity.Editor
{
    /// <summary>
    /// Contains methods for manipulating assets in Unity.
    /// </summary>
    public static class AssetsUtility
    {
        /// <summary>
        /// Creates the directory in the assets folder if it does not already exist.
        /// The path must begin with `Assets/`.
        /// </summary>
        /// <param name="path">The directory path.</param>
        public static void CreateDirectory(string path)
        {
            var separators = new char[] { '/', '\\' };
            var directories = path.Split(separators);

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
        public static string ReplaceInvalidFileNameCharacters(string str, char replacement)
        {
            return ReplaceCharacters(str, Path.GetInvalidFileNameChars(), replacement);
        }

        /// <summary>
        /// Replace all specified characters in the string with the specified replacement character.
        /// </summary>
        /// <param name="str">The original string.</param>
        /// <param name="characters">An array of characters to replace.</param>
        /// <param name="replacement">The replacement character.</param>
        public static string ReplaceCharacters(string str, char[] characters, char replacement)
        {
            var builder = new StringBuilder(str);

            for (int i = 0; i < str.Length; i++)
            {
                var index = str.IndexOfAny(characters, i);

                if (index >= 0)
                {
                    i = index;
                    builder[index] = replacement;
                }
            }

            return builder.ToString();
        }
    }
}
