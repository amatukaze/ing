using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Sakuno.KanColle.Amatsukaze
{
    public class StringResources : ModelBase
    {
        public static StringResources Instance { get; } = new StringResources();

        DirectoryInfo r_StringResourceDirectory;

        Dictionary<string, LanguageInfo> r_InstalledLanguages;
        public IList<LanguageInfo> InstalledLanguages { get; private set; }

        Dictionary<string, ExtraStringResourceInfo> r_InstalledExtraResources;
        public IList<ExtraStringResourceInfo> InstalledExtraResources { get; private set; }

        public bool IsLoaded { get; private set; }

        StringResourcesItems r_Main;
        public StringResourcesItems Main
        {
            get { return r_Main; }
            private set
            {
                if (r_Main != value)
                {
                    r_Main = value;
                    OnPropertyChanged(nameof(Main));
                }
            }
        }

        public ExtraStringResources Extra { get; private set; }

        public void Initialize()
        {
            r_InstalledLanguages = new Dictionary<string, LanguageInfo>(StringComparer.InvariantCultureIgnoreCase);
            r_InstalledExtraResources = new Dictionary<string, ExtraStringResourceInfo>(StringComparer.InvariantCultureIgnoreCase);

            var rRootDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            r_StringResourceDirectory = new DirectoryInfo(Path.Combine(rRootDirectory, "Resources", "Strings"));
            if (r_StringResourceDirectory.Exists)
                foreach (var rLanguageDirectory in r_StringResourceDirectory.EnumerateDirectories())
                {
                    InitializeMainResource(rLanguageDirectory);
                    InitializeExtraResource(rLanguageDirectory);
                }

            InstalledLanguages = r_InstalledLanguages.Values.ToList().AsReadOnly();
            InstalledExtraResources = new[] { ExtraStringResourceInfo.Disabled }.Concat(r_InstalledExtraResources.Values).ToList().AsReadOnly();
        }
        void InitializeMainResource(DirectoryInfo rpDirectory)
        {
            var rResourceFile = Path.Combine(rpDirectory.FullName, "Main.xml");
            if (!File.Exists(rResourceFile))
                return;

            var rRoot = XDocument.Load(rResourceFile).Root;
            var rCultureName = rRoot.Attribute("CultureName").Value;
            var rDisplayName = rRoot.Attribute("Name").Value;

            r_InstalledLanguages.Add(rCultureName, new LanguageInfo(rpDirectory.Name, rCultureName, rDisplayName));
        }
        void InitializeExtraResource(DirectoryInfo rpDirectory)
        {
            var rFile = new FileInfo(Path.Combine(rpDirectory.FullName, "Extra.json"));
            if (!rFile.Exists)
                return;

            using (var rReader = new JsonTextReader(rFile.OpenText()))
            {
                var rInfo = JObject.Load(rReader).ToObject<ExtraStringResourceInfo>();
                rInfo.Directory = rpDirectory.Name;

                foreach (var rContent in rInfo.Contents)
                    rContent.File = new FileInfo(Path.Combine(rFile.Directory.FullName, rContent.Type + ".json"));

                r_InstalledExtraResources.Add(rInfo.Directory, rInfo);
            }
        }

        public void LoadMainResource()
        {
            if (!InstalledLanguages.Any(r => r.Directory == Preference.Current.Language))
                Preference.Current.Language = GetDefaultLanguage().Directory;

            LoadMainResource(Preference.Current.Language);
        }
        public LanguageInfo GetDefaultLanguage()
        {
            var rNames = GetAncestorsAndSelfCultureNames(CultureInfo.CurrentCulture).ToList();

            foreach (var rLanguage in InstalledLanguages)
                if (rNames.Contains(rLanguage.CultureName))
                    return rLanguage;

            return r_InstalledLanguages["en"];
        }
        static IEnumerable<string> GetAncestorsAndSelfCultureNames(CultureInfo rpCultureInfo)
        {
            do
            {
                yield return rpCultureInfo.Name;
                rpCultureInfo = rpCultureInfo.Parent;
            } while (rpCultureInfo != CultureInfo.InvariantCulture);
        }

        void LoadMainResource(string rpLanguageName)
        {
            var rMainResourceFile = Path.Combine(r_StringResourceDirectory.FullName, rpLanguageName, "Main.xml");
            if (!File.Exists(rMainResourceFile))
                throw new Exception();

            Main = new StringResourcesItems(XDocument.Load(rMainResourceFile).Root.Descendants("String").ToDictionary(r => r.Attribute("Key").Value, r => r.Value));
            IsLoaded = true;
        }

        public void LoadExtraResource(string rpLanguageName)
        {
            ExtraStringResourceInfo rInfo;
            if (rpLanguageName.IsNullOrEmpty() || !r_InstalledExtraResources.TryGetValue(rpLanguageName, out rInfo))
                return;

            Extra = new ExtraStringResources();

            foreach (var rContent in rInfo.Contents)
            {
                if (!rContent.File.Exists)
                    continue;

                using (var rReader = new JsonTextReader(rContent.File.OpenText()))
                {
                        var rTranslations = JArray.Load(rReader);
                        var rNames = rTranslations.ToDictionary(r => (int)r["id"], r => (string)r["name"]);

                    switch (rContent.Type)
                    {
                        case ExtraStringResourceType.Ship:
                            Extra.Ships = rNames;
                            break;

                        case ExtraStringResourceType.ShipType:
                            Extra.ShipTypes = rNames;
                            break;

                        case ExtraStringResourceType.Equipment:
                            Extra.Equipment = rNames;
                            break;

                        case ExtraStringResourceType.Item:
                            Extra.Items = rNames;
                            break;

                        case ExtraStringResourceType.Expedition:
                            Extra.Expeditions = rNames;
                            break;

                        case ExtraStringResourceType.Quest:
                            Extra.Quests = rNames;
                            break;

                        case ExtraStringResourceType.Area:
                            Extra.Areas = rNames;
                            break;

                        case ExtraStringResourceType.Map:
                            Extra.Maps = rNames;
                            break;
                    }
                }
            }
        }
    }

    public partial class StringResourcesItems
    {
        Dictionary<string, string> r_Items;

        internal StringResourcesItems(Dictionary<string, string> rpItems)
        {
            r_Items = rpItems;
        }

        public string GetString(string rpKey)
        {
            string rResult;
            if (!r_Items.TryGetValue(rpKey, out rResult))
                rResult = rpKey;

            return rResult;
        }
    }
}
