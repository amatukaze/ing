using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sakuno.ING.Composition;
using Sakuno.ING.Services;

namespace Sakuno.ING.Bootstrap
{
    class NugetPackageProvider : IPackageProvider
    {
        private HttpClient client = new HttpClient();
        private const int PackagesPerPage = 20;

        public string[] SupportedTargetFrameworks { get; set; }

        private async Task<JToken> GetJsonAsync(string uri)
            => await JToken.LoadAsync(new JsonTextReader(new StreamReader(await client.GetStreamAsync(uri))));

        public async Task<IReadOnlyList<ModuleMetadata>> SearchPackagesAsync(int page)
        {
            var api = await GetJsonAsync($"https://api-v2v3search-0.nuget.org/query?q=Sakuno.ING&skip={page * PackagesPerPage}&take={PackagesPerPage}&prerelease=false&semVerLevel=2.0.0");
            int count = api.Value<int>("totalHit");

            var data = api.Value<JArray>("data");
            var result = new ModuleMetadata[count];
            for (int i = 0; i < count; i++)
            {
                var package = data[i];
                result[i] = await GetMetadataAsync(package.Value<string>("id"), package.Value<string>("version"));
            }

            return result;
        }

        public async Task<string> GetLastestVersionAsync(string packageId)
        {
            var api = await GetJsonAsync($"https://api.nuget.org/v3-flatcontainer/{packageId.ToLowerInvariant()}/index.json");
            foreach (var version in api["versions"].Values<string>())
            {
                if (version.All(ch => ch == '.' || char.IsDigit(ch)))
                    return version;
            }

            return null;
        }

        public async Task<ModuleMetadata> GetMetadataAsync(string id, string version)
        {
            var api = await GetJsonAsync($"https://api.nuget.org/v3/registration3/{id.ToLowerInvariant()}/{version.ToLowerInvariant()}.json");
            string entry = api.Value<string>("catalogEntry");

            if (entry == null) return null;
            var catalog = await GetJsonAsync(entry);

            var deps = catalog["dependencyGroups"];
            JToken selected = null;
            foreach (string tfm in SupportedTargetFrameworks)
            {
                selected = deps.FirstOrDefault(g => g.Value<string>("targetFramework").Equals(tfm, StringComparison.OrdinalIgnoreCase));
                if (selected != null) break;
            }
            var dependencies = selected.Value<JArray>("dependencies") ?? new JArray();

            return new ModuleMetadata
            (
                id,
                catalog.Value<string>("authors"),
                version,
                catalog.Value<string>("description"),
                dependencies.ToDictionary
                (
                    d => d.Value<string>("id"),
                    d =>
                    {
                        int start = 0, length = 0;
                        string r = d.Value<string>("range");
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
