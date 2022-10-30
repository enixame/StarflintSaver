﻿using System.Collections.Generic;
using System.IO;

namespace StarFlintSaver.Library.Common
{
    public interface IDirectoryManager
    {
        string GetRootDirectory();
        IEnumerable<FileInfo> GetRootDirectorySaveFiles();
        DirectoryInfo CreateNewDirectory(string directoryName);
    }
}