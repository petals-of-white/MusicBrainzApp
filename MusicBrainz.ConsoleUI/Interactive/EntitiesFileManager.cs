using MusicBrainz.BLL.Exceptions;
using MusicBrainz.Common.Enums;
using MusicBrainz.Tools.Config;

namespace MusicBrainz.ConsoleUI.Interactive
{
    /// <summary>
    /// This class manages file read/write
    /// </summary>
    internal class EntitiesFileManager
    {
        private DirectoryInfo _exportFolder = Directory.CreateDirectory(ConfigHelper.GetExportFolder());
        private DirectoryInfo _importFolder = Directory.CreateDirectory(ConfigHelper.GetImportFolder());
        private DirectoryInfo _reportFolder = Directory.CreateDirectory(ConfigHelper.GetReportFolder());

        /// <summary>
        /// Please provide the same serializationManager for both EntitiesFileManager and Db Entity Serializer
        /// </summary>
        /// <param name="serializationManager"></param>
        public EntitiesFileManager(string format = "json")
        {
            Format = format;
        }

        public DirectoryInfo ExportFolder { get => _exportFolder; }
        public string Format { get; private set; }
        public DirectoryInfo ImportFolder { get => _importFolder; }
        public DirectoryInfo ReportFolder { get => _reportFolder; }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException">UF exception</exception>
        public IList<FileInfo> GetExportFiles()
        {
            var entityTableFiles = Enum.GetNames(typeof(Tables)).Select(x => $"{x}.{Format}");

            var exportFiles = (from file in _exportFolder.GetFiles()
                               where entityTableFiles.Contains(file.Name)
                               select file).ToList();

            return exportFiles;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException">UF exception</exception>

        public IList<FileInfo> GetImportFiles()
        {
            var entityTableFiles = Enum.GetNames(typeof(Tables)).Select(x => $"{x}.{Format}");

            var importFiles = (from file in _importFolder.GetFiles()
                               where entityTableFiles.Contains(file.Name)
                               select file).ToList();

            return importFiles;
        }

        public string ReadFromFile(FileInfo file) => File.ReadAllText(file.FullName);

        /// <summary>
        /// Writes serialized table entities to file
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="serializedContent"></param>
        public void WriteToFile(Tables tableName, string serializedContent)
        {
            string pathToExportFile = Path.Combine(ExportFolder.FullName, $"{tableName}.{Format}");

            File.WriteAllText(pathToExportFile, serializedContent);
        }

        /// <summary>
        /// Writes a report to a file
        /// </summary>
        /// <param name="reportName"></param>
        /// <param name="serializedReport"></param>
        public void WriteToFile(Report reportName, string serializedReport)
        {
            string pathToReportFile = Path.Combine(ReportFolder.FullName, $"{reportName}.{Format}");

            File.WriteAllText(pathToReportFile, serializedReport);
        }
    }
}
