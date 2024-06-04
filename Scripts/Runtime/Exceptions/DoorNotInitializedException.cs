namespace MPewsey.ManiaMapUnity.Exceptions
{
    /// <summary>
    /// Exception raised when a door is not initialized.
    /// </summary>
    public class DoorNotInitializedException : System.Exception
    {
        public DoorNotInitializedException(string message) : base(message)
        {

        }
    }
}