using Microsoft.Extensions.Configuration;

namespace HelperLibrary
{
    public static class ConfigHelper
    {
        private static string _configFilePath = "appsettings.json";
        private static IConfigurationRoot buildConfiguration()
        {
            return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(_configFilePath).Build();
        }

        public static string ConfigFilePath
        {
            get { return _configFilePath; }
            set
            {
                _configFilePath = value;
                Configuration = buildConfiguration();

            }
        }
        public static IConfigurationRoot Configuration { get; private set; } = buildConfiguration();

        public static string GetConnectionString(string connectionStringName = "Default")
        {
            return Configuration.GetConnectionString(connectionStringName);

        }



    }
}
