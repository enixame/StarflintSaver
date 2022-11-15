using System.Collections.Generic;

namespace StarFlintSaver.Library.Data
{
    public class SaveFileDirectory
    {
        public string Directory { get; set; }

        public IList<SaveFile> SaveFiles { get; set; }
    }
}
