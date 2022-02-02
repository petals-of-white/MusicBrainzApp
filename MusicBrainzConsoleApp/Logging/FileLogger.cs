using System.Diagnostics;

namespace MusicBrainzConsoleApp.Logging
{
    public class FileLogger : LoggerBase
    {
        private string _filePath;
        public FileLogger(string filePath)
        {
            _filePath = filePath;
        }

        public override void Log(string message)
        {
            try
            {
                File.AppendAllText(_filePath, GetFormattedMessage(message) + "\n");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

        }

    }

}
