using StarFlintSaver.Library.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StarFlintSaver.Library.Common
{
    public interface IJsonDirectoryRepository
    {
        IList<SaveFileDirectory> SaveFileDirectories { get; }
        void AddSaveFileDirectory(SaveFileDirectory saveFileDirectory);
        void DeleteSaveFileDirectory(SaveFileDirectory saveFileDirectory);
        Task<IList<SaveFileDirectory>> LoadFromJsonDataAsync();
        Task SaveAsJsonDataAsync();
    }
}
