using System;

namespace StarFlintSaver.Windows.WindowsFeatures
{
    public static class FolderBrowserDialogFactory
    {
        public static IFolderBrowerDialog CreateFolderBrowserDialog(bool useLegacy = false)
        {
            if (Environment.OSVersion.Version.Major >= 6 && !useLegacy)
            {
                return ModernFolderBrower.ModernFolderBrowerDialog;
            }
            else
            {
                return LegacyFolderBrowser.LegacyFolderBrowserDialog;
            }
        }

        private class ModernFolderBrower
        {
            public static readonly IFolderBrowerDialog ModernFolderBrowerDialog = new ModernFolderBrowerDialog();
        }

        private class LegacyFolderBrowser
        {
            public static readonly IFolderBrowerDialog LegacyFolderBrowserDialog = new LegacyFolderBrowserDialog();
        }
    }
}
