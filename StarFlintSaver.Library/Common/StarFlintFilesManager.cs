using System;
using System.IO;

namespace StarFlintSaver.Library.Common
{
    public sealed class StarFlintFilesManager : IStarFlintFilesManager
    {
        private const string BackupFileName = "backup.save";
        private const string Extension = ".save";

        private readonly IDirectoryManager _directoryManager;
        private readonly IStarFlintFileInfo _starFlintFileInfo;

        public StarFlintFilesManager(IDirectoryManager directoryManager, IStarFlintFileInfo starFlintFileInfo)
        {
            _directoryManager = directoryManager;
            _starFlintFileInfo = starFlintFileInfo;
        }

        public FileInfo CopyFromCurrentSaveFile()
        {
            var rootDirectory = _directoryManager.GetRootDirectory();
            CheckIfRootDirectoryExists(rootDirectory);

            string starFlintSaveFile = _starFlintFileInfo.StarFlintSaveFileName;
            string newSaveFile = Path.Combine(rootDirectory, $"{Guid.NewGuid()}{Extension}");

            File.Copy(starFlintSaveFile, newSaveFile, true);
            File.SetCreationTime(newSaveFile, DateTime.Now);
            return new FileInfo(newSaveFile);
        }

        public void LoadSave(string saveFileName)
        {
            if (!File.Exists(saveFileName))
            {
                throw new InvalidOperationException($"{saveFileName} does not exist");
            }

            string starFlintSaveFile = _starFlintFileInfo.StarFlintSaveFileName;

            CreateBackupSaveFile();
            File.Copy(saveFileName, starFlintSaveFile, true);
        }

        public void DeleteSave(string saveFileName)
        {
            if (!File.Exists(saveFileName))
            {
                throw new InvalidOperationException($"{saveFileName} does not exist");
            }

            File.Delete(saveFileName);
        }

        public void CreateBackupSaveFile()
        {
            CreateOrRestoreSaveFromBackup(false);
        }

        public void RestoreFromBackupFile()
        {
            CreateOrRestoreSaveFromBackup(true);
        }

        private void CreateOrRestoreSaveFromBackup(bool isRestore)
        {
            string starFlintSaveFile = _starFlintFileInfo.StarFlintSaveFileName;
            string backupFile = Path.Combine(_starFlintFileInfo.StarFlintRootFolder, BackupFileName);

            if (isRestore)
            {
                File.Copy(backupFile, starFlintSaveFile, true);
            }
            else
            {
                File.Copy(starFlintSaveFile, backupFile, true);
            }
        }

        private static void CheckIfRootDirectoryExists(string rootDirectory)
        {
            if (!Directory.Exists(rootDirectory))
            {
                Directory.CreateDirectory(rootDirectory);
            }
        }
    }
}
