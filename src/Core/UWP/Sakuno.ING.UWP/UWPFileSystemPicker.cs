using System;
using System.IO;
using System.Threading.Tasks;
using Sakuno.ING.Composition;
using Sakuno.ING.Shell;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Sakuno.ING.UWP
{
    [Export(typeof(IFileSystemPickerService))]
    internal class UWPFileSystemPicker : IFileSystemPickerService
    {
        public async ValueTask<FileInfo> OpenFileAsync(params string[] extensions)
        {
            var picker = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.ComputerFolder
            };
            foreach (string ext in extensions)
                picker.FileTypeFilter.Add("." + ext);
            picker.FileTypeFilter.Add("*");

            var file = await picker.PickSingleFileAsync();
            if (file == null) return null;

            var temp = await file.CopyAsync(ApplicationData.Current.TemporaryFolder, Path.GetRandomFileName());
            return new FileInfo(temp.Path);
        }

        public async ValueTask<DirectoryInfo> PickFolderAsync()
        {
            var picker = new FolderPicker
            {
                SuggestedStartLocation = PickerLocationId.ComputerFolder
            };
            picker.FileTypeFilter.Add("*");

            var folder = await picker.PickSingleFolderAsync();
            if (folder == null) return null;

            var temp = await ApplicationData.Current.TemporaryFolder.CreateFolderAsync(Path.GetRandomFileName());
            await CopyFolderAsync(folder, temp);
            return new DirectoryInfo(temp.Path);
        }

        private async ValueTask CopyFolderAsync(StorageFolder source, StorageFolder destination)
        {
            foreach (var file in await source.GetFilesAsync())
                await file.CopyAsync(destination);
            foreach (var folder in await source.GetFoldersAsync())
                await CopyFolderAsync(folder, await destination.CreateFolderAsync(folder.Name));
        }
    }
}
