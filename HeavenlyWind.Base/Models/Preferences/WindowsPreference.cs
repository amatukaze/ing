using Sakuno.Collections;
using System;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class WindowsPreference : ModelBase
    {
        Property<WindowPreference[]> Placements { get; } = new Property<WindowPreference[]>("main.windows");

        ListDictionary<string, WindowPreference> r_Placements = new ListDictionary<string, WindowPreference>(StringComparer.OrdinalIgnoreCase);

        internal WindowsPreference()
        {
            Placements.Reloaded += delegate
            {
                if (Placements.Value != null)
                    foreach (var rPlacement in Placements.Value)
                        r_Placements.Add(rPlacement.Name, rPlacement);
            };
        }

        public WindowPreference LoadPlacement(string rpName)
        {
            WindowPreference rResult;
            r_Placements.TryGetValue(rpName, out rResult);
            return rResult;
        }
        public void SavePlacement(WindowPreference rpPlacement)
        {
            r_Placements[rpPlacement.Name] = rpPlacement;

            Placements.Value = r_Placements.Values.ToArray();
        }
    }
}
