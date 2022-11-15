using StarFlintSaver.Library.Data;
using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace StarFlintSaver.Library.Common
{
    public sealed class ConfigurationFileLoader : IConfigurationFileLoader
    {
        private const string CurrentVersion = "1.1.0";
        private const string StarFlintSaverDirectory = "StarFlintSaver";
        
        private const string DefaultSaveFileName = "starflint_file_0.save";
        private const string ConfigurationFileName = "StarFlintSaver.config";

        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private string _starFlintSaverDefaultRootDirectory;
        private string _starFlintSaverBaseDirectory;
        private readonly string _configurationBaseDirectory;
        private readonly string _configurationFile;

        private readonly object _lockObject = new object();

        private StarFlintSaverConfiguration _starFlintSaverConfiguration;

        public ConfigurationFileLoader()
        {
            _starFlintSaverDefaultRootDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            _configurationBaseDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "LocalLow", "StarFlintSaver");
            _starFlintSaverBaseDirectory = Path.Combine(_starFlintSaverDefaultRootDirectory, StarFlintSaverDirectory);
            _configurationFile = Path.Combine(_configurationBaseDirectory, ConfigurationFileName);

            _jsonSerializerOptions = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
        }

        public StarFlintSaverConfiguration GetConfiguration()
        {
            lock(_lockObject)
            {
                if (_starFlintSaverConfiguration != null)
                {
                    return _starFlintSaverConfiguration;
                }
            }

            CheckIfConfigurationDirectoryExists();
            
            return GetStarFlintSaverConfiguration();
        }

        private void CheckIfConfigurationDirectoryExists()
        {
            if (!Directory.Exists(_configurationBaseDirectory))
            {
                Directory.CreateDirectory(_configurationBaseDirectory);
            }
        }

        private StarFlintSaverConfiguration GetStarFlintSaverConfiguration()
        {
            if (!File.Exists(_configurationFile))
            {
                StarFlintSaverConfiguration starFlintSaverConfiguration = new StarFlintSaverConfiguration
                {
                    Version = CurrentVersion,
                    StarFlintSaveFileName = DefaultSaveFileName,
                    StarFlintSaverBaseDirectory = _starFlintSaverBaseDirectory,
                };

                string jsonString = JsonSerializer.Serialize(starFlintSaverConfiguration, _jsonSerializerOptions);
                File.WriteAllText(_configurationFile, jsonString);

                lock (_lockObject)
                {
                    _starFlintSaverConfiguration = starFlintSaverConfiguration;
                    return _starFlintSaverConfiguration;
                }             
            }
            else
            {
                string jsonString = File.ReadAllText(_configurationFile);
                lock (_lockObject)
                {
                    _starFlintSaverConfiguration = JsonSerializer.Deserialize<StarFlintSaverConfiguration>(jsonString, _jsonSerializerOptions);
                    return _starFlintSaverConfiguration;
                }
            }
        }

        public void UpdateStarFlintSaverBaseDirectory(string starFlintSaverBaseDirectory)
        {
            if (!Directory.Exists(starFlintSaverBaseDirectory))
            {
                throw new InvalidOperationException($"Directory {starFlintSaverBaseDirectory} does not exist.");
            }

            lock (_lockObject)
            {
                if(_starFlintSaverConfiguration == null)
                {
                    return;
                }

                _starFlintSaverConfiguration.StarFlintSaverBaseDirectory = starFlintSaverBaseDirectory;
            }

            var jsonString = JsonSerializer.Serialize(_starFlintSaverConfiguration, _jsonSerializerOptions);
            File.WriteAllText(_configurationFile, jsonString);
        }
    }
}
