using System;

namespace MPewsey.ManiaMapUnity.Exceptions
{
    /// <summary>
    /// Exception raised when a duplicate generation input argument exists.
    /// </summary>
    public class DuplicateInputException : Exception
    {
        /// <inheritdoc/>
        public DuplicateInputException(string message) : base(message)
        {

        }
    }
}
