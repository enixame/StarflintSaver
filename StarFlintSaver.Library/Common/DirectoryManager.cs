using System;
using System.Collections.Generic;
using System.IO;

namespace StarFlintSaver.Library.Common
{
    public sealed class DirectoryManager : IDirectoryManager
    {
        private string _rootDirectory;
        private string _starFlintSaverBaseDirectory;
        private const string StarFlintSaverDirectory = "StarFlintSaver";

        public DirectoryManager()
        {
            _rootDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            _starFlintSaverBaseDirectory = Path.Combine(_rootDirectory, StarFlintSaverDirectory);
        }

        /// <summary>
        /// Future purpose
        /// </summary>
        /// <param name="directoryName">directory name</param>
        /// <returns></returns>
        public DirectoryInfo CreateNewDirectory(string directoryName)
        {
            if (!Directory.Exists(_starFlintSaverBaseDirectory))
            {
                throw new InvalidOperationException($"{_starFlintSaverBaseDirectory} does not exist.");
            }

            string newDirectory = Path.Combine(_starFlintSaverBaseDirectory, directoryName);
            if (Directory.Exists(newDirectory))
            {
                throw new InvalidOperationException($"{newDirectory} already exist.");
            }

            return Directory.CreateDirectory(newDirectory);
        }

        public string GetRootDirectory()
        {
            return _starFlintSaverBaseDirectory;
        }

        public IEnumerable<FileInfo> GetRootDirectorySaveFiles()
        {
            var directoryInfo = new DirectoryInfo(_starFlintSaverBaseDirectory);
            return directoryInfo.EnumerateFiles();
        }
    }
}
