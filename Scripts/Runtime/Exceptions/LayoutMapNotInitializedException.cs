namespace MPewsey.ManiaMapUnity.Exceptions
{
    /// <summary>
    /// Raised is the layout map is not initialized.
    /// </summary>
    public class LayoutMapNotInitializedException : System.Exception
    {
        public LayoutMapNotInitializedException(string message) : base(message)
        {

        }
    }
}