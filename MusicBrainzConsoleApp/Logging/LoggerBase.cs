namespace MusicBrainzConsoleApp.Logging
{
    public abstract class LoggerBase
    {
        public abstract void Log(string message);
        protected virtual string GetFormattedMessage(string message)
        {
            return $"Log ({DateTime.Now}): {message}";
        }
    }

}
