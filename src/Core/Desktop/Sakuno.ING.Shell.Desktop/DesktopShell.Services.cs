using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Sakuno.ING.Shell.Desktop
{
    partial class DesktopShell : IShell
    {
        public ValueTask<FileInfo> OpenFileAsync(params string[] extensions)
        {
            var dialog = new CommonOpenFileDialog
            {
                ShowHiddenItems = true
            };
            foreach (string ext in extensions)
                dialog.Filters.Add(new CommonFileDialogFilter(ext, "*." + ext));
            dialog.Filters.Add(new CommonFileDialogFilter("*", "*"));

            FileInfo result = null;
            if (ShowModal(dialog) == CommonFileDialogResult.Ok)
                result = new FileInfo(dialog.FileName);

            return new ValueTask<FileInfo>(result);
        }

        public ValueTask<DirectoryInfo> PickFolderAsync()
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                ShowHiddenItems = true
            };

            DirectoryInfo result = null;
            if (ShowModal(dialog) == CommonFileDialogResult.Ok)
                result = new DirectoryInfo(dialog.FileName);

            return new ValueTask<DirectoryInfo>(result);
        }

        private static Window GetForegroundWindow()
        {
            for (int i = 0; i < Application.Current.Windows.Count; i++)
            {
                var w = Application.Current.Windows[i];
                if (w.IsActive)
                    return w;
            }

            return null;
        }

        private static CommonFileDialogResult ShowModal(CommonFileDialog dialog)
        {
            var window = GetForegroundWindow();

            if (window == null)
                return dialog.ShowDialog();
            else
                return dialog.ShowDialog(window);
        }

        public ValueTask ShowMessageAsync(string detail, string title)
        {
            var dialog = new TaskDialog
            {
                Text = detail,
                InstructionText = title,
                Caption = localization.GetLocalized("Application", "Title"),
            };

            var window = GetForegroundWindow();
            if (window != null)
                dialog.OwnerWindowHandle = new WindowInteropHelper(window).Handle;

            dialog.Show();
            return default;
        }
    }
}
