using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sakuno.Collections;
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

        ListDictionary<string, LanguageInfo> r_InstalledLanguages;
        public IList<LanguageInfo> InstalledLanguages { get; private set; }

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

        StringResources()
        {
            r_InstalledLanguages = new ListDictionary<string, LanguageInfo>()
            {
                { "Japanese", new LanguageInfo("Japanese", "ja-JP", "日本語") },
                { "SimplifiedChinese", new LanguageInfo("SimplifiedChinese", "zh-Hans", "简体中文") },
                { "English", new LanguageInfo("English", "en", "English") },
            };
            InstalledLanguages = r_InstalledLanguages.Values.ToArray();
        }

        public void Initialize()
        {
            Preference.Instance.Language.Subscribe(LoadMainResource);
            Preference.Instance.ExtraResourceLanguage.Subscribe(LoadExtraResource);
        }

        public void LoadMainResource(string rpLanguage)
        {
            if (!r_InstalledLanguages.ContainsKey(rpLanguage))
            {
                Preference.Instance.Language.Value = GetDefaultLanguage().Directory;
                return;
            }

            LoadMainResourceCore(rpLanguage);
        }
        public LanguageInfo GetDefaultLanguage()
        {
            var rNames = GetAncestorsAndSelfCultureNames(CultureInfo.CurrentCulture).ToList();

            foreach (var rLanguage in InstalledLanguages)
                if (rNames.Contains(rLanguage.CultureName))
                    return rLanguage;

            return rNames.Contains("zh") ? r_InstalledLanguages["SimplifiedChinese"] : r_InstalledLanguages["English"];
        }
        public static IEnumerable<string> GetAncestorsAndSelfCultureNames(CultureInfo rpCultureInfo)
        {
            do
            {
                yield return rpCultureInfo.Name;
                rpCultureInfo = rpCultureInfo.Parent;
            } while (rpCultureInfo != CultureInfo.InvariantCulture);
        }

        void LoadMainResourceCore(string rpLanguageName)
        {
            var rAssembly = Assembly.GetExecutingAssembly();
            using (var rStream = rAssembly.GetManifestResourceStream($"Sakuno.KanColle.Amatsukaze.Resources.Strings.{Preference.Current.Language}.xml"))
            using (var rReader = new StreamReader(rStream))
            {
                Main = new StringResourcesItems(XDocument.Load(rReader).Root.Descendants("String").ToDictionary(r => r.Attribute("Key").Value, r => r.Value));
                IsLoaded = true;
            }
        }

        public void LoadExtraResource(string rpLanguageName)
        {
            var rInfo = LoadExtraResourceInfo(rpLanguageName) ?? LoadExtraResourceInfo(GetDefaultLanguage().DisplayName);
            if (rInfo == null)
            {
                Extra = null;
                OnPropertyChanged(nameof(Extra));
                return;
            }

            var rESR = new ExtraStringResources();

            foreach (var rContent in rInfo.Contents)
            {
                if (!rContent.File.Exists)
                    continue;

                using (var rReader = new JsonTextReader(rContent.File.OpenText()))
                {
                    var rTranslations = JArray.Load(rReader);
                    if (rContent.Type == ExtraStringResourceType.AbyssalShip)
                    {
                        rESR.AbyssalShip = rTranslations
                            .SelectMany(r => r["id"], (rpTranslation, rpID) => new { ID = (int)rpID, Name = (string)rpTranslation["name"] })
                            .ToHybridDictionary(r => r.ID, r => r.Name);
                    }
                    else
                    {
                        var rNames = rTranslations.ToHybridDictionary(r => (int)r["id"], r => (string)r["name"]);

                        switch (rContent.Type)
                        {
                            case ExtraStringResourceType.Ship:
                                rESR.Ships = rNames;
                                break;

                            case ExtraStringResourceType.ShipType:
                                rESR.ShipTypes = rNames;
                                break;

                            case ExtraStringResourceType.Equipment:
                                rESR.Equipment = rNames;
                                break;

                            case ExtraStringResourceType.Item:
                                rESR.Items = rNames;
                                break;

                            case ExtraStringResourceType.Expedition:
                                rESR.Expeditions = rNames;
                                break;

                            case ExtraStringResourceType.Quest:
                                rESR.Quests = rNames;
                                break;

                            case ExtraStringResourceType.Area:
                                rESR.Areas = rNames;
                                break;

                            case ExtraStringResourceType.Map:
                                rESR.Maps = rNames;
                                break;

                            case ExtraStringResourceType.ShipLocking:
                                rESR.ShipLocking = rNames;
                                break;
                        }
                    }
                }
            }

            Extra = rESR;
            OnPropertyChanged(nameof(Extra));
        }
        ExtraStringResourceInfo LoadExtraResourceInfo(string rpLanguageName)
        {
            if (rpLanguageName.IsNullOrEmpty())
                return null;

            var rStringResourceDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Resources", "Strings");

            var rFile = new FileInfo(Path.Combine(rStringResourceDirectory, rpLanguageName, "Extra.json"));
            if (!rFile.Exists)
                return null;

            using (var rReader = new JsonTextReader(rFile.OpenText()))
            {
                var rInfo = JObject.Load(rReader).ToObject<ExtraStringResourceInfo>();
                rInfo.Directory = rpLanguageName;

                foreach (var rContent in rInfo.Contents)
                    if (rContent.ShareWith.IsNullOrEmpty())
                        rContent.File = new FileInfo(Path.Combine(rFile.Directory.FullName, rContent.Type + ".json"));
                    else
                        rContent.File = new FileInfo(Path.Combine(rStringResourceDirectory, rContent.ShareWith, rContent.Type + ".json"));

                return rInfo;
            }
        }
    }

    public partial class StringResourcesItems : ModelBase
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
