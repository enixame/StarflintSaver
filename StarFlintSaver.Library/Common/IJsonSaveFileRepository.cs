using StarFlintSaver.Library.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StarFlintSaver.Library.Common
{
    public interface IJsonSaveFileRepository
    {
        IList<SaveFile> SaveFiles { get; }
        void AddSaveFile(SaveFile saveFile);
        void DeleteSaveFile(SaveFile saveFile);
        Task<IList<SaveFile>> LoadFromJsonDataAsync();
        Task SaveAsJsonDataAsync();
    }
}
