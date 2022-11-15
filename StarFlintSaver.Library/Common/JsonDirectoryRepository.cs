using StarFlintSaver.Library.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace StarFlintSaver.Library.Common
{
    public class JsonDirectoryRepository : IJsonDirectoryRepository
    {
        private const string DataFileName = "SaveFileDirectories.json";
        private const string TempFileName = "temp.json";

        private const int DefaultBufferSize = 4096;

        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly IConfigurationFileLoader _configurationFileLoader;
        private readonly IStarFlintFileInfo _starFlintFileInfo;

        private readonly object _lockObject = new object();

        public JsonDirectoryRepository(IStarFlintFileInfo starFlintFileInfo, IConfigurationFileLoader configurationFileLoader)
        {
            _starFlintFileInfo = starFlintFileInfo;
            _configurationFileLoader = configurationFileLoader;

            _jsonSerializerOptions = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public IList<SaveFileDirectory> SaveFileDirectories { get; private set; }

        public void AddSaveFileDirectory(SaveFileDirectory saveFileDirectory)
        {
            if (SaveFileDirectories == null)
            {
                throw new InvalidOperationException($"Cannot add {nameof(SaveFileDirectory)} because the collection is Null.");
            }

            lock (_lockObject)
            {
                SaveFileDirectories.Add(saveFileDirectory);
            }
        }

        public void DeleteSaveFileDirectory(SaveFileDirectory saveFileDirectory)
        {
            if (SaveFileDirectories == null)
            {
                throw new InvalidOperationException($"Cannot add {nameof(SaveFileDirectory)} because the collection is Null.");
            }

            lock (_lockObject)
            {
                SaveFileDirectories.Remove(saveFileDirectory);
            }
        }

        public async Task<IList<SaveFileDirectory>> LoadFromJsonDataAsync()
        {
            string rootDirectory = _starFlintFileInfo.StarFlintRootFolder;
            string dataFileNamePath = Path.Combine(rootDirectory, DataFileName);

            var configuration = _configurationFileLoader.GetConfiguration();
            var baseDirectory = configuration.StarFlintSaverBaseDirectory;

            if (File.Exists(dataFileNamePath))
            {
                using FileStream openStream = File.OpenRead(dataFileNamePath);
                SaveFileDirectories = await JsonSerializer.DeserializeAsync<IList<SaveFileDirectory>>(openStream, _jsonSerializerOptions);
            }
            else
            {
                SaveFileDirectories = new List<SaveFileDirectory>();
            }

            return SaveFileDirectories;
        }

        private IList<SaveFileDirectory> GetSaveFileDirectoriesListSafe()
        {
            lock (_lockObject)
            {
                return SaveFileDirectories;
            }
        }

        public async Task SaveAsJsonDataAsync()
        {
            if (SaveFileDirectories == null)
            {
                throw new InvalidOperationException($"Cannot add {nameof(SaveFileDirectory)} because the collection is Null.");
            }

            string rootDirectory = _starFlintFileInfo.StarFlintRootFolder;
            string tempDataFileName = Path.Combine(rootDirectory, TempFileName);
            string dataFileNamePath = Path.Combine(rootDirectory, DataFileName);

            try
            {
                using FileStream createStream = File.Create(tempDataFileName, DefaultBufferSize);
                await JsonSerializer.SerializeAsync(createStream, GetSaveFileDirectoriesListSafe(), _jsonSerializerOptions);
                await createStream.DisposeAsync();

                File.Copy(tempDataFileName, dataFileNamePath, true);
            }
            finally
            {
                if (File.Exists(tempDataFileName))
                {
                    File.Delete(tempDataFileName);
                }
            }
        }
    }
}
