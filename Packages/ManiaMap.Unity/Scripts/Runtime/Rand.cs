using UnityEngine;

namespace MPewsey.ManiaMapUnity
{
    /// <summary>
    /// Contains methods for creating random ID's.
    /// </summary>
    public static class Rand
    {
        /// <summary>
        /// If the specified ID is greater than zero, returns the ID.
        /// Otherwise, returns a random positive integer.
        /// </summary>
        /// <param name="id">The ID to test for validity.</param>
        public static int AutoAssignId(int id)
        {
            return id <= 0 ? GetRandomId() : id;
        }

        /// <summary>
        /// Returns a random positive integer.
        /// </summary>
        public static int GetRandomId()
        {
            return Random.Range(1, int.MaxValue);
        }
    }
}