using System;
using System.IO;
using System.Threading.Tasks;
using Sakuno.ING.IO;
using Sakuno.ING.Shell;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.UWP
{
    partial class UWPShell : IShell
    {
        public async ValueTask<IFileFacade> OpenFileAsync(params string[] extensions)
        {
            var picker = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.ComputerFolder
            };
            foreach (string ext in extensions)
                picker.FileTypeFilter.Add("." + ext);
            picker.FileTypeFilter.Add("*");

            var file = await picker.PickSingleFileAsync();
            return file == null ? null : new StorageFileFacade(file);
        }

        public async ValueTask<IFolderFacade> PickFolderAsync()
        {
            var picker = new FolderPicker
            {
                SuggestedStartLocation = PickerLocationId.ComputerFolder
            };
            picker.FileTypeFilter.Add("*");

            var folder = await picker.PickSingleFolderAsync();
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

        public ValueTask ShowMessageAsync(string detail, string title)
            => new ValueTask(new ContentDialog
            {
                Title = title,
                Content = detail,
                CloseButtonText = "OK"
            }.ShowAsync().AsTask());
    }
}
