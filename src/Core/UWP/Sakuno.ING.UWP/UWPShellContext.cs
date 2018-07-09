using System;
using System.IO;
using System.Threading.Tasks;
using Sakuno.ING.Composition;
using Sakuno.ING.IO;
using Sakuno.ING.Localization;
using Sakuno.ING.Shell;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.UWP
{
    [Export(typeof(IShellContextService))]
    internal class UWPShellContextService : IShellContextService
    {
        private readonly ILocalizationService localization;
        public UWPShellContextService(ILocalizationService localization) => this.localization = localization;

        public IShellContext Capture() => new UWPShellContext(localization, Window.Current ?? throw new InvalidOperationException("Not called from UI thread."));
    }

    internal class UWPShellContext : IShellContext
    {
        private readonly ILocalizationService localization;
        private readonly Window window;

        public UWPShellContext(ILocalizationService localization, Window window)
        {
            this.localization = localization;
            this.window = window;
        }

        public async Task<IFileFacade> OpenFileAsync(params string[] extensions)
        {
            var fileSource = new TaskCompletionSource<StorageFile>();
            await window.Dispatcher.RunAsync(default, async () =>
            {
                var picker = new FileOpenPicker
                {
                    SuggestedStartLocation = PickerLocationId.ComputerFolder
                };
                foreach (string ext in extensions)
                    picker.FileTypeFilter.Add("." + ext);
                picker.FileTypeFilter.Add("*");

                fileSource.SetResult(await picker.PickSingleFileAsync());
            });
            var file = await fileSource.Task;
            return file == null ? null : new StorageFileFacade(file);
        }

        public async Task<IFolderFacade> PickFolderAsync()
        {
            var folderSource = new TaskCompletionSource<StorageFolder>();
            await window.Dispatcher.RunAsync(default, async () =>
            {
                var picker = new FolderPicker
                {
                    SuggestedStartLocation = PickerLocationId.ComputerFolder
                };
                picker.FileTypeFilter.Add("*");

                folderSource.SetResult(await picker.PickSingleFolderAsync());
            });
            var folder = await folderSource.Task;
            return folder == null ? null : new StorageFolderFacade(folder);
        }

        private class StorageFileFacade : IFileFacade
        {
            private readonly StorageFile storageFile;
            public StorageFileFacade(StorageFile storageFile) => this.storageFile = storageFile;

            public string FullName => storageFile.Path;

            public ValueTask<Stream> OpenReadAsync()
                => new ValueTask<Stream>(storageFile.OpenStreamForReadAsync());

            public async ValueTask<string> GetAccessPathAsync()
                => (await storageFile.CopyAsync(ApplicationData.Current.TemporaryFolder, Path.GetRandomFileName(), NameCollisionOption.ReplaceExisting)).Path;
        }

        private class StorageFolderFacade : IFolderFacade
        {
            private readonly StorageFolder storageFolder;
            public StorageFolderFacade(StorageFolder storageFolder) => this.storageFolder = storageFolder;

            public string FullName => storageFolder.Path;

            public async ValueTask<IFileFacade> GetFileAsync(string filename)
                => new StorageFileFacade(await storageFolder.GetFileAsync(filename));
            public async ValueTask<IFolderFacade> GetFolderAsync(string foldername)
                => new StorageFolderFacade(await storageFolder.GetFolderAsync(foldername));
        }

        public async Task ShowMessageAsync(string detail, string title)
        {
            var taskSource = new TaskCompletionSource<Task>();
            await window.Dispatcher.RunAsync(default,
                () => taskSource.SetResult(new ContentDialog
                {
                    Title = title,
                    Content = detail,
                    CloseButtonText = "OK"
                }.ShowAsync().AsTask()));
            await taskSource.Task;
        }
    }
}
