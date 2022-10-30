using StarFlintSaver.Library.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StarFlintSaver.Library.Common
{
    public sealed class FileSynchronisationProcess : IFileSynchronisationProcess
    {
        private readonly IDirectoryManager _directoryManager;
        private readonly IJsonDataRepository _jsonDataRepository;

        public FileSynchronisationProcess(IDirectoryManager directoryManager, IJsonDataRepository jsonDataRepository)
        {
            _directoryManager = directoryManager;
            _jsonDataRepository = jsonDataRepository;
        }

        public async Task<IEnumerable<SaveFile>> GetAllFilesFromRootDirectoryAsync()
        {
            return await Task.Run(() => PopulateOrderedList());
        }

        private IEnumerable<SaveFile> PopulateOrderedList()
        {
            var newSaveFilesList = new List<SaveFile>();
            var allFiles = _directoryManager.GetRootDirectorySaveFiles();
            foreach (var fileInfo in allFiles)
            {
                var existingSaveFile = _jsonDataRepository.SaveFiles
                    .FirstOrDefault(saveFile => saveFile.Date == fileInfo.CreationTime && saveFile.FileName == fileInfo.FullName);
                if (existingSaveFile != null)
                {
                    newSaveFilesList.Add(existingSaveFile);
                }
                else
                {
                    var newSaveFile = new SaveFile()
                    {
                        Date = fileInfo.CreationTime,
                        FileName = fileInfo.FullName,
                        Description = string.Empty
                    };
                    newSaveFilesList.Add(newSaveFile);
                }
            }

            return newSaveFilesList.OrderBy(saveFile => saveFile.Date);
        }
    }
}
