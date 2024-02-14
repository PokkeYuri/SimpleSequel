namespace SimpleSequel
{
    public class SimpleSequelException : Exception
    {
        public string SqlStatement { get; } = string.Empty;

        public SimpleSequelException(string message) : base(message) { }
        public SimpleSequelException(string message, string sqlStatement, Exception? innerException = null) : base(message, innerException)
        {
            SqlStatement = sqlStatement;
        }

        public static SimpleSequelException NewOpException(Exception ex, string sqlStatement)
            => new SimpleSequelException($"An error occurred during database operation. More details in inner Expression!", sqlStatement, ex);

        public static SimpleSequelException NewNotInitException()
            => new SimpleSequelException("SimpleSequelManager not initialized! Please run 'SimpleSequelManager.Initialize()' befor getting Instance!");
    }
}
