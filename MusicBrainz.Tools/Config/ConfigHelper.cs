using Microsoft.Extensions.Configuration;

namespace MusicBrainz.Tools.Config
{
    public static class ConfigHelper
    {
        private static string _configFilePath = "appsettings.json";

        private static IConfigurationRoot BuildConfiguration()
        {
            return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(_configFilePath).Build();
        }

        public static string ConfigFilePath
        {
            get => _configFilePath;
            set
            {
                _configFilePath = value;
                Configuration = BuildConfiguration();
            }
        }

        public static IConfigurationRoot Configuration { get; private set; } = BuildConfiguration();

        public static string GetConnectionString(string connectionStringName = "Default")
        {
            return Configuration.GetConnectionString(connectionStringName);
        }

        /// <summary>
        /// Shorthand for GetSection("TransferPaths") ["ImportFolderPath"]
        /// </summary>
        /// <returns></returns>
        public static string GetImportFolder()
        {
            return Configuration.GetSection("TransferPaths") ["ImportFolderPath"];
        }

        /// <summary>
        /// Shorthand for GetSection("TransferPaths") ["ExportFolderPath"];
        /// </summary>
        /// <returns></returns>
        public static string GetExportFolder()
        {
            return Configuration.GetSection("TransferPaths") ["ExportFolderPath"];
        }
    }
}
