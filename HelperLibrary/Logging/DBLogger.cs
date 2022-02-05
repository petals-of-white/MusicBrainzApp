namespace HelperLibrary.Logging
{
    public class DBLogger : LoggerBase
    {
        private string _connectionString;
        public DBLogger(string connectionString)
        {
            _connectionString = connectionString;
        }
        public override void Log(string message)
        {
            // Logs to the database
        }
    }


}
