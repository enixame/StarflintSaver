using StarFlintSaver.Library.Common;
using System.Diagnostics;

namespace StarFlintSaver.Windows.WindowsFeatures
{
    public sealed class WindowsSystemFeatures : ISystemFeatures
    {
        private const string ExplorerProcessName = "explorer.exe";
        private readonly IClipboard _clipboard;

        public WindowsSystemFeatures(IClipboard clipboard)
        {
            _clipboard = clipboard;
        }

        public void OpenFolder(string folderPath)
        {
            ExecuteExplorerProcessWithArguments(folderPath, "/root");
        }

        public void OpenFolderAndSelectFile(string filePath)
        {
            ExecuteExplorerProcessWithArguments(filePath, "/select");
        }

        private static void ExecuteExplorerProcessWithArguments(string path, string command)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new System.ArgumentNullException(path, $"{nameof(path)} is Null or Empty.");
            }

            string explorerCommandArgument = $"{command}, {path}";
            Process explorerProcess = null;

            try
            {
                explorerProcess = Process.Start(ExplorerProcessName, explorerCommandArgument);
            }
            finally
            {
                explorerProcess?.Dispose();
            }
        }

        public void CopyFileToSystemClipboard(string filePath)
        {
            _clipboard.CopyFile(filePath);
        }
    }
}
