using System;

namespace MPewsey.ManiaMap.Unity.Exceptions
{
    /// <summary>
    /// Exception raised when a duplicate generation input argument exists.
    /// </summary>
    public class DuplicateInputException : Exception
    {
        public DuplicateInputException(string message) : base(message)
        {

        }
    }
}
