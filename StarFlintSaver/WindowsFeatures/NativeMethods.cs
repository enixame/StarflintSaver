using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace StarFlintSaver.Windows.WindowsFeatures
{
    internal class NativeMethods
    {
        #region FolderBrowseDialog

        public const uint FOS_PICKFOLDERS = 0x00000020;
        public const uint FOS_FORCEFILESYSTEM = 0x00000040;
        public const uint FOS_NOVALIDATE = 0x00000100;
        public const uint FOS_NOTESTFILECREATE = 0x00010000;
        public const uint FOS_DONTADDTORECENT = 0x02000000;

        public const uint SIGDN_FILESYSPATH = 0x80058000;

        #endregion

        #region Legacy

        // Browsing for directory.
        public const uint BIF_RETURNONLYFSDIRS = 0x00000001;  // For finding a folder to start document searching
        public const uint BIF_DONTGOBELOWDOMAIN = 0x00000002;  // For starting the Find Computer
        public const uint BIF_STATUSTEXT = 0x00000004;   // Top of the dialog has 2 lines of text for BROWSEINFO.lpszTitle and one line if
                                                        // this flag is set.  Passing the message BFFM_SETSTATUSTEXTA to the hwnd can set the
                                                        // rest of the text.  This is not used with BIF_USENEWUI and BROWSEINFO.lpszTitle gets
                                                        // all three lines of text.
        public const uint BIF_RETURNFSANCESTORS = 0x00000008;
        public const uint BIF_EDITBOX = 0x00000010;   // Add an editbox to the dialog
        public const uint BIF_VALIDATE = 0x00000020;   // insist on valid result (or CANCEL)

        public const uint BIF_NEWDIALOGSTYLE = 0x00000040;   // Use the new dialog layout with the ability to resize
                                                            // Caller needs to call OleInitialize() before using this API

        public const uint BIF_USENEWUI = (BIF_NEWDIALOGSTYLE | BIF_EDITBOX);

        public const uint BIF_BROWSEINCLUDEURLS = 0x00000080;   // Allow URLs to be displayed or entered. (Requires BIF_USENEWUI)
        public const uint BIF_UAHINT = 0x00000100;   // Add a UA hint to the dialog, in place of the edit box. May not be combined with BIF_EDITBOX
        public const uint BIF_NONEWFOLDERBUTTON = 0x00000200;   // Do not add the "New Folder" button to the dialog.  Only applicable with BIF_NEWDIALOGSTYLE.
        public const uint BIF_NOTRANSLATETARGETS = 0x00000400;   // don't traverse target as shortcut

        public const uint BIF_BROWSEFORCOMPUTER = 0x00001000;  // Browsing for Computers.
        public const uint BIF_BROWSEFORPRINTER = 0x00002000;  // Browsing for Printers
        public const uint BIF_BROWSEINCLUDEFILES = 0x00004000;  // Browsing for Everything
        public const uint BIF_SHAREABLE = 0x00008000;  // sharable resources displayed (remote shares, requires BIF_USENEWUI)
        public const uint BIF_BROWSEFILEJUNCTIONS = 0x00010000;  // allow folder junctions like zip files and libraries to be browsed

        // Constants for sending and receiving messages in BrowseCallBackProc
        public const uint WM_USER = 0x400;
        public const uint BFFM_INITIALIZED = 1;
        public const uint BFFM_SELCHANGED = 2;
        public const uint BFFM_VALIDATEFAILEDA = 3;
        public const uint BFFM_VALIDATEFAILEDW = 4;
        public const uint BFFM_IUNKNOWN = 5; // provides IUnknown to client. lParam: IUnknown*
        public const uint BFFM_SETSTATUSTEXTA = WM_USER + 100;
        public const uint BFFM_ENABLEOK = WM_USER + 101;
        public const uint BFFM_SETSELECTIONA = WM_USER + 102;
        public const uint BFFM_SETSELECTIONW = WM_USER + 103;
        public const uint BFFM_SETSTATUSTEXTW = WM_USER + 104;
        public const uint BFFM_SETOKTEXT = WM_USER + 105; // Unicode only
        public const uint BFFM_SETEXPANDED = WM_USER + 106; // Unicode only

        #endregion

        protected NativeMethods()
        {

        }

        #region Shell32

        [DllImport("Shell32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool SHGetPathFromIDList(IntPtr pidl, IntPtr pszPath);

        [DllImport("Shell32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern HRESULT SHILCreateFromPath([MarshalAs(UnmanagedType.LPWStr)] string pszPath, ref IntPtr ppIdl, ref uint rgflnOut);

        [DllImport("Shell32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern HRESULT SHGetKnownFolderIDList(ref Guid rfid, uint dwFlags, IntPtr hToken, out IntPtr ppidl);

        [DllImport("Shell32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr SHBrowseForFolder([In] BROWSEINFO lpbi);

        [DllImport("shell32.dll", EntryPoint = "SHCreateItemFromParsingName", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern HRESULT SHCreateItemFromParsingName_([MarshalAs(UnmanagedType.LPWStr)] string pszPath, IntPtr pbc, ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out IShellItem ppv);

        [DllImport("shell32.dll", EntryPoint = "SHGetDesktopFolder", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int SHGetDesktopFolder_([MarshalAs(UnmanagedType.Interface)] out IShellFolder ppshf);

        #endregion

        #region User32

        [DllImport("user32.dll", PreserveSig = true)]
        public static extern IntPtr SendMessage(HandleRef hWnd, uint Msg, uint wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, uint msg, uint wParam, string lParam);

        #endregion

        [DllImport("ole32.dll", EntryPoint = "CreateBindCtx")]
        public static extern int CreateBindCtx_(int reserved, out IBindCtx ppbc);

        private static IBindCtx CreateBindCtx()
        {
            Marshal.ThrowExceptionForHR(CreateBindCtx_(0, out IBindCtx result));
            return result;
        }

        private static IShellFolder SHGetDesktopFolder()
        {
            Marshal.ThrowExceptionForHR(SHGetDesktopFolder_(out IShellFolder result));
            return result;
        }

        private static IntPtr GetShellFolderChildrenRelativePIDL(IShellFolder parentFolder, string displayName)
        {
            _ = CreateBindCtx();

            uint pdwAttributes = 0;
            parentFolder.ParseDisplayName(IntPtr.Zero, null, displayName, out uint pchEaten, out IntPtr ppidl, ref pdwAttributes);

            return ppidl;
        }

        public static IntPtr PathToAbsolutePidl(string path)
        {
            var desktopFolder = SHGetDesktopFolder();
            return GetShellFolderChildrenRelativePIDL(desktopFolder, path);
        }

        public static HRESULT ShCreateItemFromParsingName(string path, out IShellItem directoryShellItem)
        {
            var riid = new Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE");
            return SHCreateItemFromParsingName_(path, IntPtr.Zero, ref riid, out directoryShellItem);
        }
    }
}
