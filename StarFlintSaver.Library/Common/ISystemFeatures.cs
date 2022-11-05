namespace StarFlintSaver.Library.Common
{
    public interface ISystemFeatures
    {
        void OpenFolder(string folderPath);

        void OpenFolderAndSelectFile(string filePath);

        void CopyFileToSystemClipboard(string filePath);
    }
}
