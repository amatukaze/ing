using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sakuno.KanColle.Amatsukaze.Data;
using Windows.Storage;

namespace Sakuno.KanColle.Amatsukaze.UWP.Data
{
    internal class DataService : IDataService
    {
        private StorageFolder dataRoot;
        private StorageFile dbFile;

        internal async Task InitializeAsync()
        {
            if (dataRoot != null) return;
            dataRoot = ApplicationData.Current.RoamingFolder;
            dbFile = await dataRoot.CreateFileAsync("ing.db", CreationCollisionOption.OpenIfExists);
        }

        public void ConfigureDbContext(DbContextOptionsBuilder builder)
        {
            if (dbFile == null)
                throw new InvalidOperationException("Data service not initialized.");

            builder.UseSqlite("Data Source=" + dbFile.Path);
        }

        public Task<Stream> ReadFile(string filename)
            => dataRoot.OpenStreamForReadAsync(filename);

        public Task<Stream> WriteFile(string filename)
            => dataRoot.OpenStreamForWriteAsync(filename, CreationCollisionOption.ReplaceExisting);
    }
}
