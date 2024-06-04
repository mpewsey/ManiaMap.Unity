using System;

namespace MPewsey.ManiaMapUnity.Exceptions
{
    /// <summary>
    /// Raised when the instantiation of a GameObject fails.
    /// </summary>
    public class InstantiationFailedException : Exception
    {
        public InstantiationFailedException(string message) : base(message)
        {

        }
    }
}