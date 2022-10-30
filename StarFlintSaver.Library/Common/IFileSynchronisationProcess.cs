using StarFlintSaver.Library.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StarFlintSaver.Library.Common
{
    public interface IFileSynchronisationProcess
    {
        Task<IEnumerable<SaveFile>> GetAllFilesFromRootDirectoryAsync();
    }
}
