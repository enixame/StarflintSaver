using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace StarFlintSaver.Windows.WindowsFeatures
{
    public class ModernFolderBrowerDialog : IFolderBrowerDialog
    {
        public string InitialFolder { get; set; }
        public string DefaultFolder { get; set; }
        public string SelectFolder { get; private set; }

        public DialogResult ShowDialog()
        {
            return ShowFolderDialog(WindowWrapper.CurrentWindow);
        }

        public DialogResult ShowDialog(IntPtr owner)
        {
            return ShowFolderDialog(new WindowWrapper(owner));
        }

        private DialogResult ShowFolderDialog(IWin32Window owner)
        {
            var fileDialog = (IFileDialog)new FileOpenDialogRCW();

            fileDialog.GetOptions(out uint options);

            options |= NativeMethods.FOS_PICKFOLDERS |
                       NativeMethods.FOS_FORCEFILESYSTEM |
                       NativeMethods.FOS_NOVALIDATE |
                       NativeMethods.FOS_NOTESTFILECREATE |
                       NativeMethods.FOS_DONTADDTORECENT;

            fileDialog.SetOptions(options);
            FolderInit(fileDialog);

            return ShowDialog(owner, fileDialog);
        }

        private DialogResult ShowDialog(IWin32Window owner, IFileDialog fileDialog)
        {
            if (fileDialog.Show(owner.Handle) == HRESULT.S_OK)
            {
                if (fileDialog.GetResult(out IShellItem shellItem) == HRESULT.S_OK)
                {
                    if (shellItem.GetDisplayName(NativeMethods.SIGDN_FILESYSPATH, out IntPtr pszString) == HRESULT.S_OK)
                    {
                        if (pszString != IntPtr.Zero)
                        {
                            try
                            {
                                SelectFolder = Marshal.PtrToStringAuto(pszString);
                                return DialogResult.Ok;
                            }
                            finally
                            {
                                Marshal.FreeCoTaskMem(pszString);
                            }
                        }
                    }
                }
            }
            return DialogResult.Cancel;
        }

        private void FolderInit(IFileDialog fileDialog)
        {
            if (InitialFolder != null)
            {
                if (NativeMethods.ShCreateItemFromParsingName(InitialFolder, out IShellItem directoryShellItem) == HRESULT.S_OK)
                {
                    fileDialog.SetFolder(directoryShellItem);
                }
            }

            if (DefaultFolder != null)
            {
                if (NativeMethods.ShCreateItemFromParsingName(DefaultFolder, out IShellItem directoryShellItem) == HRESULT.S_OK)
                {
                    fileDialog.SetDefaultFolder(directoryShellItem);
                }
            }
        }
    }
}
