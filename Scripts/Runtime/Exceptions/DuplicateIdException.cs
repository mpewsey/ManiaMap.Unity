namespace MPewsey.ManiaMapUnity.Exceptions
{
    /// <summary>
    /// Exception raised when a duplicate ID exists.
    /// </summary>
    public class DuplicateIdException : System.Exception
    {
        public DuplicateIdException(string message) : base(message)
        {

        }
    }
}