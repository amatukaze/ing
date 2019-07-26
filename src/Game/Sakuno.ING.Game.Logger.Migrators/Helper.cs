using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sakuno.ING.IO;

namespace Sakuno.ING.Game.Logger.Migrators
{
    internal static class Helper
    {
        public static TValue TryGetOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
            => dictionary.TryGetValue(key, out TValue value) ? value : default;

        public static async ValueTask<IReadOnlyCollection<T>> ParseCsv<T>(IFileSystemFacade source, string path, int columnCount, Func<string[], T> selector, Encoding encoding = null, bool trimHeader = true)
        {
            var folder = source as IFolderFacade ?? throw new ArgumentException("Source must be a folder.");
            var paths = path.Split('/');
            for (int i = 0; i < paths.Length - 1; i++)
                folder = await folder.GetFolderAsync(paths[i]);

            var file = await folder.GetFileAsync(paths[paths.Length - 1]);
            if (file == null) return Array.Empty<T>();

            var table = new List<T>();
            using var reader = new StreamReader(await file.OpenReadAsync(), encoding ?? Encoding.UTF8);
            if (trimHeader)
                await reader.ReadLineAsync();
            while (!reader.EndOfStream)
            {
                string line = await reader.ReadLineAsync();
                if (string.IsNullOrEmpty(line)) continue;

                var s = line.Split(',');
                if (s.Length != columnCount) continue;

                try
                {
                    table.Add(selector(s.Select(x => x.Trim('\0')).ToArray()));
                }
                catch
                {
                    // fail count ++
                }
            }
            return table;
        }
    }
}
