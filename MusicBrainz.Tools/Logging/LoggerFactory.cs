namespace MusicBrainz.Tools.Logging
{
    /// <summary>
    /// Use the LoggerFactory to create an instance of a logger.
    /// </summary>
    public abstract class LoggerFactory
    {
        public abstract LoggerBase CreateLogger();
    }
}