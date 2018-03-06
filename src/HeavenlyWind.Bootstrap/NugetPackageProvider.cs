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
        private const int PackagesPerPage = 20;

        public string[] SupportedTargetFrameworks { get; set; }

        public async Task<IReadOnlyList<ModuleMetadata>> SearchPackagesAsync(int page)
        {
            var api = await client.GetStreamAsync($"https://api-v2v3search-0.nuget.org/query?q=HeavenlyWind&skip={page * PackagesPerPage}&take={PackagesPerPage}&prerelease=false&semVerLevel=2.0.0");
            var json = await jsonService.ParseAsync(api);
            int count = json.SelectValue<int>("totalHits");

            var result = new ModuleMetadata[count];
            for (int i = 0; i < count; i++)
                result[i] = await GetMetadataAsync(json.SelectValue<string>($"data[{i}].id"),
                                                   json.SelectValue<string>($"data[{i}].version"));

            return result;
        }

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

        private class NugetDependency
        {
            public string id { get; set; }
            public string range { get; set; }
        }
        private class NugetDependencyGroup
        {
            public NugetDependency[] dependencies { get; set; }
            public string targetFramework { get; set; }
        }
        public async Task<ModuleMetadata> GetMetadataAsync(string id, string version)
        {
            var api = await client.GetStreamAsync($"https://api.nuget.org/v3/registration3/{id.ToLowerInvariant()}/{version.ToLowerInvariant()}.json");
            var json = await jsonService.ParseAsync(api);
            string entry = json.SelectValue<string>("catalogEntry");

            if (entry == null) return null;
            var catalog = await client.GetStreamAsync(entry);
            var metajson = await jsonService.ParseAsync(catalog);

            var deps = metajson.SelectValue<NugetDependencyGroup[]>("dependencygroups");
            NugetDependencyGroup selected = null;
            foreach (string tfm in SupportedTargetFrameworks)
            {
                selected = deps.FirstOrDefault(g => g.targetFramework.Equals(tfm, StringComparison.OrdinalIgnoreCase));
                if (selected != null) break;
            }
            var dependencies = selected?.dependencies ?? Array.Empty<NugetDependency>();

            return new ModuleMetadata
            (
                id,
                metajson.SelectValue<string>("authors"),
                version,
                metajson.SelectValue<string>("description"),
                dependencies.ToDictionary
                (
                    d => d.id,
                    d =>
                    {
                        int start = 0, length = 0;
                        string r = d.range;
                        for (int i = 0; i < r.Length; i++)
                        {
                            if (r[i] >= '0' && r[i] <= '9')
                            {
                                start = i;
                                break;
                            }
                        }
                        for (int i = 0; start + i < r.Length; i++)
                        {
                            if (r[start + i] == ',')
                            {
                                length = i;
                                break;
                            }
                        }
                        return r.Substring(start, length);
                    }
                )
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
