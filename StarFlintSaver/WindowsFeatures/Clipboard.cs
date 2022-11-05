using StarFlintSaver.Windows.Utils;
using System;
using System.Collections.Specialized;
using System.IO;

namespace StarFlintSaver.Windows.WindowsFeatures
{
    public sealed class Clipboard : IClipboard
    {
        public void CopyFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException(nameof(filePath), $"{nameof(filePath)} is Null or Empty.");
            }

            if (!File.Exists(filePath))
            {
                throw new InvalidOperationException($"{filePath} does not exist.");
            }

            if (System.Windows.Clipboard.ContainsFileDropList())
            {
                UiThreadDispatcher.BeginInvoke(() => 
                {
                    var fileDropList = System.Windows.Clipboard.GetFileDropList();
                    fileDropList.Clear(); // clear clipboard
                    fileDropList.Add(filePath);
                    System.Windows.Clipboard.SetFileDropList(fileDropList);
                });  
            }
            else
            {
                StringCollection files = new StringCollection();
                files.Add(filePath);
                UiThreadDispatcher.Invoke(() => System.Windows.Clipboard.SetFileDropList(files));
            }
        }
    }
}
