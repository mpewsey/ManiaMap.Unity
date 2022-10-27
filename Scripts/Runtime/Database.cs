using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// Contains methods related to ID generation.
    /// </summary>
    public static class Database
    {
        /// <summary>
        /// If the ID is less than or equal to zero, returns a random positive integer. Otherwise, returns the ID.
        /// </summary>
        /// <param name="id">The original ID.</param>
        public static int AutoAssignId(int id)
        {
            if (id <= 0)
                return Random.Range(1, int.MaxValue);
            return id;
        }
    }
}