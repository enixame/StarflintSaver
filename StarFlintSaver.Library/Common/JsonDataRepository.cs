using StarFlintSaver.Library.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StarFlintSaver.Library.Common
{
    public class JsonDataRepository : IJsonDataRepository
    {
        public Task<IList<SaveFile>> LoadSaveFilesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
