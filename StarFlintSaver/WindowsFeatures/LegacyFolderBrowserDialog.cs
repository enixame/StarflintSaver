using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace StarFlintSaver.Windows.WindowsFeatures
{
    internal sealed class LegacyFolderBrowserDialog : IFolderBrowerDialog
    {
        private static readonly int s_numberOfBytesToAllocate = (260 + 1) * Marshal.SystemDefaultCharSize;

        public string DefaultFolder { get; set; }

        public string InitialFolder { get; set; }

        public string SelectFolder { get; private set; }

        public DialogResult ShowDialog()
        {
            return ShowDialog(new System.Windows.Interop.WindowInteropHelper(Application.Current.MainWindow).Handle);
        }

        public DialogResult ShowDialog(IntPtr onwer)
        {
            BROWSEINFO browseInfo = new BROWSEINFO
            {
                hwndOwner = onwer,
                ulFlags =
                     NativeMethods.BIF_SHAREABLE
                    | NativeMethods.BIF_STATUSTEXT
            };
            FolderInit(browseInfo);

            var pidlRet = NativeMethods.SHBrowseForFolder(browseInfo);
            if (pidlRet != IntPtr.Zero)
            {
                var pszSelectedPath = Marshal.AllocHGlobal(s_numberOfBytesToAllocate);
                try
                {
                    var bRetPath = NativeMethods.SHGetPathFromIDList(pidlRet, pszSelectedPath);
                    if (bRetPath)
                    {
                        SelectFolder = Marshal.PtrToStringUni(pszSelectedPath);
                    }

                    return DialogResult.Ok;
                }
                finally
                {
                    Marshal.FreeCoTaskMem(pidlRet);

                    if (pszSelectedPath != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(pszSelectedPath);
                    }
                }
            }

            return DialogResult.Cancel;
        }

        private void FolderInit(BROWSEINFO browseInfo)
        {
            if (!string.IsNullOrEmpty(InitialFolder))
            {
                browseInfo.lpfn = new BrowseCallbackProc(OnBrowseEvent);
            }

            IntPtr pidlRoot;
            if (!string.IsNullOrEmpty(DefaultFolder))
            {
                pidlRoot = NativeMethods.PathToAbsolutePidl(DefaultFolder);
            }
            else
            {
                var computerFolderGuid = new Guid("0AC0837C-BBF8-452A-850D-79D08E667CA7");
                _ = NativeMethods.SHGetKnownFolderIDList(ref computerFolderGuid, 0, IntPtr.Zero, out pidlRoot);
            }

            browseInfo.pidlRoot = pidlRoot;
        }

        private uint OnBrowseEvent(IntPtr hwnd, uint msg, IntPtr lParam, IntPtr lpData)
        {
            switch (msg)
            {
                case NativeMethods.BFFM_INITIALIZED: // Required to set InitialFolder
                    {
                        // Win32.SendMessage(new HandleRef(null, hWnd), BFFM_SETSELECTIONA, 1, lpData);
                        // Use BFFM_SETSELECTIONW if passing a Unicode string, i.e. native CLR Strings.
                        NativeMethods.SendMessage(new HandleRef(null, hwnd), NativeMethods.BFFM_SETSELECTIONW, 1, InitialFolder);
                        break;
                    }

                case NativeMethods.BFFM_SELCHANGED:
                    {
                        IntPtr pathPtr = Marshal.AllocHGlobal(s_numberOfBytesToAllocate);
                        if (NativeMethods.SHGetPathFromIDList(lParam, pathPtr))
                        {
                            NativeMethods.SendMessage(new HandleRef(null, hwnd), NativeMethods.BFFM_SETSTATUSTEXTW, 0, pathPtr);
                        }
                        Marshal.FreeHGlobal(pathPtr);
                        break;
                    }
            }

            return 0;
        }
    }
}
