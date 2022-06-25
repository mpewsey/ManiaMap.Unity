using System;

namespace MPewsey.ManiaMap.Unity.Exceptions
{
    /// <summary>
    /// Exception raised when a generation step is missing an input argument.
    /// </summary>
    public class MissingInputException : Exception
    {
        public MissingInputException(string message) : base(message)
        {

        }
    }
}
