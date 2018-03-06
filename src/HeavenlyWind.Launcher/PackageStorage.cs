using System.IO;
using System.Threading.Tasks;
using Sakuno.KanColle.Amatsukaze.Bootstrap;

namespace Sakuno.KanColle.Amatsukaze
{
    public class PackageStorage : IPackageStorage
    {
        public PackageStorage(string[] supportedTargetFrameworks) => SupportedTargetFrameworks = supportedTargetFrameworks;

        public string[] SupportedTargetFrameworks { get; }

        public async Task StageAsync(string id, string version, Stream stream)
        {
            string folder = Path.Combine(Program.StagingPackagesDirectory, id);
            Directory.CreateDirectory(folder);
            using (var file = File.Create(Path.Combine(folder, $"{id}.{version}.nupkg")))
                await stream.CopyToAsync(file);
        }

        public void Remove(string id, string version) => Directory.Delete(Path.Combine(Package.BaseDirectory, id), true);
    }
}
