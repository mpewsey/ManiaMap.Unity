using System;

namespace MPewsey.ManiaMapUnity.Exceptions
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
