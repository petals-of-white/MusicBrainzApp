using MusicBrainz.Common.Enums;
using MusicBrainz.Tools.Config;

namespace MusicBrainz.ConsoleUI
{
    /// <summary>
    /// This class manages file read/write
    /// </summary>
    internal class EntitiesFileManager
    {
        private DirectoryInfo _exportFolder = Directory.CreateDirectory(ConfigHelper.GetExportFolder());
        private DirectoryInfo _importFolder = Directory.CreateDirectory(ConfigHelper.GetImportFolder());

        /// <summary>
        /// Please provide the same serializationManager for both
        /// EntitiesFileManager and Db Entity Serializer
        /// </summary>
        /// <param name="serializationManager"></param>
        public EntitiesFileManager(string format = ".json")
        {
            Format = format;
        }

        public DirectoryInfo ExportFolder { get => _exportFolder; }
        public string Format { get; private set; }
        public DirectoryInfo ImportFolder { get => _importFolder; }

        public IList<FileInfo> GetExportFiles()
        {
            var entityTableFiles = Enum.GetNames(typeof(Tables)).Select(x => $"{x}{Format}");

            var exportFiles = (from file in _exportFolder.GetFiles()
                               where entityTableFiles.Contains(file.Name)
                               select file).ToList();

            return exportFiles;
        }

        public IList<FileInfo> GetImportFiles()
        {
            var entityTableFiles = Enum.GetNames(typeof(Tables)).Select(x => $"{x}{Format}");

            var importFiles = (from file in _importFolder.GetFiles()
                               where entityTableFiles.Contains(file.Name)
                               select file).ToList();

            return importFiles;
        }

        public string ReadFromFile(FileInfo file) => File.ReadAllText(file.FullName);

        internal void WriteToFile(Tables tableName, string serializedContent)
        {
            string pathToExportFile = Path.Combine(ExportFolder.FullName, $"{tableName}{Format}");
            File.WriteAllText(pathToExportFile, serializedContent);
        }
    }
}