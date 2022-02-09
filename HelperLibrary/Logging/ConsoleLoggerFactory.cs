namespace HelperLibrary.Logging
{
    /// <summary>
    /// Use the LoggerFactory to create an instance of a logger.
    /// </summary>
    public class ConsoleLoggerFactory : LoggerFactory
    {
        public override LoggerBase CreateLogger() => new ConsoleLogger();
    }
}
