namespace MusicBrainz.Tools.Logging
{
    /// <summary>
    /// Use the LoggerFactory to create an instance of a logger.
    /// </summary>
    public class FileLoggerFactory : LoggerFactory
    {
        private string _filePath;

        public FileLoggerFactory(string filePath)
        {
            _filePath = filePath;
        }

        public override LoggerBase CreateLogger() => new FileLogger(_filePath);
    }
}
