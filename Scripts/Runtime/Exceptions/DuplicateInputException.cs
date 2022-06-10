using System;

namespace MPewsey.ManiaMap.Unity.Exceptions
{
    public class DuplicateInputException : Exception
    {
        public DuplicateInputException(string message) : base(message)
        {

        }
    }
}
