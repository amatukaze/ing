using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;
using Sakuno.ING.Composition;

namespace Sakuno.ING.Shell.Desktop
{
    [Export(typeof(IFileSystemPickerService))]
    internal class WindowsApiPicker : IFileSystemPickerService
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

        private static CommonFileDialogResult ShowModal(CommonFileDialog dialog)
        {
            Window window = null;
            for (int i = 0; i < Application.Current.Windows.Count; i++)
            {
                var w = Application.Current.Windows[i];
                if (w.IsActive)
                {
                    window = w;
                    break;
                }
            }

            if (window == null)
                return dialog.ShowDialog();
            else
                return dialog.ShowDialog(window);
        }
    }
}
