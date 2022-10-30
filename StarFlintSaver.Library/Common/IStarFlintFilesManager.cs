using System.IO;

namespace StarFlintSaver.Library.Common
{
    public interface IStarFlintFilesManager
    {
        FileInfo CopyFromCurrentSaveFile();
        void LoadSave(string saveFileName);
        void DeleteSave(string saveFileName);
        void CreateBackupSaveFile();
        void RestoreFromBackupFile();
    }
}
