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
    public sealed class JsonSaveFileRepository : IJsonSaveFileRepository
    {
        private const string DataFileName = "SaveFiles.json";
        private const string TempFileName = "temp.json";

        private const int DefaultBufferSize = 4096;

        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly IStarFlintFileInfo _starFlintFileInfo;

        private readonly object _lockObject = new object();

        public JsonSaveFileRepository(IStarFlintFileInfo starFlintFileInfo)
        {
            _starFlintFileInfo = starFlintFileInfo;

            _jsonSerializerOptions = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
        }

        public IList<SaveFile> SaveFiles { get; private set; }

        public void AddSaveFile(SaveFile saveFile)
        {
            if (SaveFiles == null)
            {
                throw new InvalidOperationException($"Cannot add {nameof(SaveFile)} because the collection is Null.");
            }

            lock (_lockObject)
            {
                SaveFiles.Add(saveFile);
            }
        }

        public void DeleteSaveFile(SaveFile saveFile)
        {
            if (SaveFiles == null)
            {
                throw new InvalidOperationException($"Cannot add {nameof(SaveFile)} because the collection is Null.");
            }

            lock (_lockObject)
            {
                SaveFiles.Remove(saveFile);
            }
        }

        public async Task<IList<SaveFile>> LoadFromJsonDataAsync()
        {
            string rootDirectory = _starFlintFileInfo.StarFlintRootFolder;
            string dataFileNamePath = Path.Combine(rootDirectory, DataFileName);

            if (File.Exists(dataFileNamePath))
            {
                using FileStream openStream = File.OpenRead(dataFileNamePath);
                SaveFiles = await JsonSerializer.DeserializeAsync<IList<SaveFile>>(openStream, _jsonSerializerOptions);
            }
            else
            {
                SaveFiles = new List<SaveFile>();
            }

            return SaveFiles;
        }

        private IList<SaveFile> GetSaveFilesListSafe()
        {
            lock (_lockObject)
            {
                return SaveFiles;
            }
        }

        public async Task SaveAsJsonDataAsync()
        {
            if (SaveFiles == null)
            {
                throw new InvalidOperationException($"Cannot add {nameof(SaveFile)} because the collection is Null.");
            }

            string rootDirectory = _starFlintFileInfo.StarFlintRootFolder;
            string tempDataFileName = Path.Combine(rootDirectory, TempFileName);
            string dataFileNamePath = Path.Combine(rootDirectory, DataFileName);

            try
            {
                using FileStream createStream = File.Create(tempDataFileName, DefaultBufferSize);
                await JsonSerializer.SerializeAsync(createStream, GetSaveFilesListSafe(), _jsonSerializerOptions);
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
