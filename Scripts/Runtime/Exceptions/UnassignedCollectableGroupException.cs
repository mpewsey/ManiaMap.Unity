using System;

namespace MPewsey.ManiaMap.Unity.Exceptions
{
    /// <summary>
    /// Exception raised when a collectable group is not assigned.
    /// </summary>
    public class UnassignedCollectableGroupException : Exception
    {
        public UnassignedCollectableGroupException(string message) : base(message)
        {

        }
    }
}
