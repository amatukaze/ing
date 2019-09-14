using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using Microsoft.WindowsAPICodePack.Dialogs;
using Sakuno.ING.Composition;
using Sakuno.ING.IO;
using Sakuno.ING.Localization;
using Sakuno.ING.Shell;

namespace Sakuno.ING.Views.Desktop
{
    [Export(typeof(IShellContextService))]
    internal class DesktopShellContextService : IShellContextService
    {
        private readonly ILocalizationService localization;
        public DesktopShellContextService(ILocalizationService localization)
        {
            this.localization = localization;
            Instance = this;
        }

        internal static DesktopShellContextService Instance { get; private set; }
        internal Window Window { get; set; }

        public IShellContext Capture() => new DesktopShellContext(localization, Window ?? Application.Current.MainWindow);
    }

    internal class DesktopShellContext : IShellContext
    {
        private readonly ILocalizationService localization;
        private readonly Window window;

        public DesktopShellContext(ILocalizationService localization, Window window)
        {
            this.localization = localization;
            this.window = window;
        }

        public async Task<IFileFacade> OpenFileAsync(params string[] extensions)
        {
            using var dialog = new CommonOpenFileDialog
            {
                ShowHiddenItems = true
            };
            foreach (string ext in extensions)
                dialog.Filters.Add(new CommonFileDialogFilter(ext, "*." + ext));
            dialog.Filters.Add(new CommonFileDialogFilter("*", "*"));

            if (await ShowModalAsync(dialog) == CommonFileDialogResult.Ok)
                return new FilePathFacade(dialog.FileName);
            else
                return null;
        }

        public async Task<IFolderFacade> PickFolderAsync()
        {
            using var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                ShowHiddenItems = true
            };

            if (await ShowModalAsync(dialog) == CommonFileDialogResult.Ok)
                return new FolderPathFacade(dialog.FileName);
            else
                return null;
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

            public ValueTask<IEnumerable<IFileFacade>> GetFilesAsync()
                => new ValueTask<IEnumerable<IFileFacade>>
                    (Directory.GetFiles(FullName).Select(x => new FilePathFacade(x)));
        }

        private DispatcherOperation<CommonFileDialogResult> ShowModalAsync(CommonFileDialog dialog)
            => window.Dispatcher.InvokeAsync(() => dialog.ShowDialog(window));

        public Task ShowMessageAsync(string detail, string title)
            => window.Dispatcher.InvokeAsync(() =>
                new TaskDialog
                {
                    Text = detail,
                    InstructionText = title,
                    Caption = localization.GetLocalized("Application", "Title"),
                    OwnerWindowHandle = new WindowInteropHelper(window).Handle
                }.Show()).Task;
    }
}
