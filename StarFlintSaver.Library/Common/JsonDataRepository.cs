using StarFlintSaver.Library.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StarFlintSaver.Library.Common
{
    public class JsonDataRepository : IJsonDataRepository
    {
        private readonly IJsonDirectoryRepository _directoryRepository;
        private readonly IJsonSaveFileRepository _saveFileRepository;

        public JsonDataRepository(IJsonDirectoryRepository directoryRepository, IJsonSaveFileRepository saveFileRepository)
        {
            _directoryRepository = directoryRepository;
            _saveFileRepository = saveFileRepository;
        }

        public Task<IList<SaveFile>> LoadSaveFilesAsync()
        {
            return null;
        }
    }
}
