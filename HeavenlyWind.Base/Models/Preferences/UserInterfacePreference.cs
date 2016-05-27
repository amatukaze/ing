using Newtonsoft.Json;
using System.Globalization;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class UserInterfacePreference
    {
        [JsonProperty("font")]
        public string Font { get; set; }

        public UserInterfacePreference()
        {
            var rCultures = StringResources.GetAncestorsAndSelfCultureNames(CultureInfo.CurrentCulture).ToArray();
            if (rCultures.Any(r => r.OICEquals("zh-Hans")))
                Font = "Microsoft YaHei UI, Segoe UI";
            else if (rCultures.Any(r => r.OICEquals("zh-Hant")))
                Font = "Microsoft JhengHei UI, Segoe UI";
            else
                Font = "Meiryo UI, Segoe UI";
        }
    }
}
