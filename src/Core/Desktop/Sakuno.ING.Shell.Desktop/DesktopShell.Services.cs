using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using Microsoft.WindowsAPICodePack.Dialogs;
using Sakuno.ING.IO;

namespace Sakuno.ING.Shell.Desktop
{
    partial class DesktopShell : IShell
    {
        public ValueTask<IFileFacade> OpenFileAsync(params string[] extensions)
        {
            var dialog = new CommonOpenFileDialog
            {
                ShowHiddenItems = true
            };
            foreach (string ext in extensions)
                dialog.Filters.Add(new CommonFileDialogFilter(ext, "*." + ext));
            dialog.Filters.Add(new CommonFileDialogFilter("*", "*"));

            if (ShowModal(dialog) == CommonFileDialogResult.Ok)
                return new ValueTask<IFileFacade>(new FilePathFacade(dialog.FileName));
            else
                return default;
        }

        public ValueTask<IFolderFacade> PickFolderAsync()
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                ShowHiddenItems = true
            };

            if (ShowModal(dialog) == CommonFileDialogResult.Ok)
                return new ValueTask<IFolderFacade>(new FolderPathFacade(dialog.FileName));
            else
                return default;
        }

        private class FilePathFacade : IFileFacade
        {
            public FilePathFacade(string path) => FullName = path;
            public string FullName { get; }

            public ValueTask<string> GetAccessPathAsync() => new ValueTask<string>(FullName);
            public ValueTask<Stream> OpenReadAsync()
                => new ValueTask<Stream>(File.OpenRead(FullName));
        }

        private class FolderPathFacade : IFolderFacade
        {
            public FolderPathFacade(string path) => FullName = path;
            public string FullName { get; }

            public ValueTask<IFileFacade> GetFileAsync(string filename)
            {
                string path = Path.Combine(FullName, filename);
                if (File.Exists(path))
                    return new ValueTask<IFileFacade>(new FilePathFacade(path));
                else
                    return default;
            }

            public ValueTask<IFolderFacade> GetFolderAsync(string foldername)
            {
                string path = Path.Combine(FullName, foldername);
                if (Directory.Exists(path))
                    return new ValueTask<IFolderFacade>(new FolderPathFacade(path));
                else
                    return default;
            }
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
