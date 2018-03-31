using Microsoft.EntityFrameworkCore;
using Sakuno.KanColle.Amatsukaze.Data;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;

namespace Sakuno.KanColle.Amatsukaze.UWP.Data
{
    internal class DataService : IDataService
    {
        private StorageFolder dataRoot;
        private StorageFile dbFile;

        internal async Task InitializeAsync()
        {
            if (dataRoot != null) return;
            StorageItemAccessList futureList = StorageApplicationPermissions.FutureAccessList;

            foreach (var entry in futureList.Entries)
                if (entry.Metadata == "data")
                {
                    dataRoot = await futureList.GetFolderAsync(entry.Token);
                    break;
                }
            if (dataRoot != null)
                dbFile = await CreateDbFileAsync();
        }

        public void ConfigureDbContext(DbContextOptionsBuilder builder)
        {
            DelayInitializeWithUIAsync().Wait();
            if (dbFile == null)
                throw new InvalidOperationException("Data service not initialized.");

            builder.UseSqlite("Data Source=" + dbFile.Path);
        }

        public async Task<Stream> ReadFile(string filename)
        {
            await DelayInitializeWithUIAsync();
            return await dataRoot.OpenStreamForReadAsync(filename);
        }

        public async Task<Stream> WriteFile(string filename)
        {
            await DelayInitializeWithUIAsync();
            return await dataRoot.OpenStreamForWriteAsync(filename, CreationCollisionOption.ReplaceExisting);
        }

        private ConfiguredTaskAwaitable<StorageFile> CreateDbFileAsync()
            => dataRoot.CreateFileAsync("ing.db", CreationCollisionOption.OpenIfExists).AsTask().ConfigureAwait(false);

        private async Task DelayInitializeWithUIAsync()
        {
            if (dataRoot != null) return;

            var dialog = new StorageSelectionDialog();
            await dialog.ShowAsync();

            dataRoot = dialog.SelectedFolder;
            StorageApplicationPermissions.FutureAccessList.Add(dataRoot, "data");
            dbFile = await CreateDbFileAsync();
        }
    }
}
