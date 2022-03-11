﻿namespace MusicBrainz.Tools.Logging
{
    public class ConsoleLogger : LoggerBase
    {
        public override void Log(string message)
        {
            Console.WriteLine(GetFormattedMessage(message));
        }
    }
}