using Newtonsoft.Json;
using Sakuno.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class WindowsPreference
    {
        ListDictionary<string, WindowPreference> r_Landscape;
        [JsonIgnore]
        public IDictionary<string, WindowPreference> Landscape
        {
            get
            {
                if (r_Landscape == null)
                    r_Landscape = new ListDictionary<string, WindowPreference>(StringComparer.OrdinalIgnoreCase);
                return r_Landscape;
            }
        }
        [JsonProperty("landscape")]
        public WindowPreference[] LandscapeArray
        {
            get { return r_Landscape?.Values.ToArray(); }
            set
            {
                if (value == null || value.Length == 0)
                    r_Landscape = null;
                else
                {
                    r_Landscape = new ListDictionary<string, WindowPreference>();

                    foreach (var rPreference in value)
                        r_Landscape.Add(rPreference.Name, rPreference);
                }
            }
        }

        ListDictionary<string, WindowPreference> r_Portrait;
        [JsonIgnore]
        public IDictionary<string, WindowPreference> Portrait
        {
            get
            {
                if (r_Portrait == null)
                    r_Portrait = new ListDictionary<string, WindowPreference>(StringComparer.OrdinalIgnoreCase);
                return r_Portrait;
            }
        }
        [JsonProperty("portrait")]
        public WindowPreference[] PortraitArray
        {
            get { return r_Portrait?.Values.ToArray(); }
            set
            {
                if (value == null || value.Length == 0)
                    r_Portrait = null;
                else
                {
                    r_Portrait = new ListDictionary<string, WindowPreference>();

                    foreach (var rPreference in value)
                        r_Portrait.Add(rPreference.Name, rPreference);
                }
            }
        }
    }
}
