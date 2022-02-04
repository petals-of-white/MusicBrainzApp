using LogLibrary;

LoggerBase fileLogger = LoggerFactory.CreateFileLogger("text.txt");
LoggerBase consoleLogger = LoggerFactory.CreateConsoleLogger();


consoleLogger.Log("test");
fileLogger.Log("test");