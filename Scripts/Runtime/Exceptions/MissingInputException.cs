using System;

namespace MPewsey.ManiaMap.Unity.Exceptions
{
    public class MissingInputException : Exception
    {
        public MissingInputException(string message) : base(message)
        {

        }
    }
}
