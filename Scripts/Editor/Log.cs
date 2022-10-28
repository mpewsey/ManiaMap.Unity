using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Editor
{
    /// <summary>
    /// Contains methods for logging styled messages in %Unity.
    /// </summary>
    public static class Log
    {
        /// <summary>
        /// Logs the message with the success style (green and bold).
        /// </summary>
        /// <param name="message">The logged message.</param>
        public static void Success(string message)
        {
            Debug.Log("<color=#00FF00><b>" + message + "</b></color>");
        }
    }
}