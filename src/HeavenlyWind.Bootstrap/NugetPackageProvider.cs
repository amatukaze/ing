using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Sakuno.KanColle.Amatsukaze.Composition;
using Sakuno.KanColle.Amatsukaze.Services;

namespace Sakuno.KanColle.Amatsukaze.Bootstrap
{
    class NugetPackageProvider : IPackageProvider
    {
        private HttpClient client = new HttpClient();
        private IJsonService jsonService;
        public Task<IReadOnlyList<ModuleMetadata>> SearchPackagesAsync(int page) => throw new NotImplementedException();
        public async Task<string> GetLastestVersionAsync(string packageId)
        {
            var api = await client.GetStreamAsync($"https://api.nuget.org/v3-flatcontainer/{packageId.ToLowerInvariant()}/index.json");
            var json = await jsonService.ParseAsync(api);
            var tokens = json.SelectValues<string>("versions[*]");

            string version = null;
            foreach (var v in tokens)
                if (v.All(ch => ch == '.' || char.IsDigit(ch)))
                    version = v;

            return version;
        }

        public async Task<ModuleMetadata> GetMetadataAsync(string id, string version)
        {
            var api = await client.GetStreamAsync($"https://api.nuget.org/v3/registration3/{id.ToLowerInvariant()}/{version.ToLowerInvariant()}.json");
            var json = await jsonService.ParseAsync(api);
            string entry = json.SelectValue<string>("catalogEntry");

            if (entry == null) return null;
            var catalog = await client.GetStreamAsync(entry);
            var metajson = await jsonService.ParseAsync(catalog);
            return new ModuleMetadata
            (
                id,
                metajson.SelectValue<string>("authors"),
                version,
                metajson.SelectValue<string>("description"),
                null //TODO: implement
            );
        }

        public Task<Stream> FetchAsync(string id, string version)
        {
            string lowerId = id.ToLowerInvariant();
            string lowerVersion = version.ToLowerInvariant();
            return client.GetStreamAsync($"https://api.nuget.org/v3-flatcontainer/{lowerId}/{lowerVersion}/{lowerId}.{lowerVersion}.nupkg");
        }
    }
}
