namespace MusicBrainzConsoleApp.Logging
{
    public static class LoggerFactory
    {
        public static LoggerBase CreateFileLogger(string filePath)
        {
            return new FileLogger(filePath);
        }
        public static LoggerBase CreateDBLogger(string connectionString)
        {
            return new DBLogger(connectionString);
        }
        public static LoggerBase CreateConsoleLogger()
        {
            return new ConsoleLogger();
        }
    }
}
