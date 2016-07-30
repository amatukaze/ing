using System;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public abstract class Property : ModelBase
    {
        internal static Dictionary<string, Property> Instances { get; } = new Dictionary<string, Property>(StringComparer.OrdinalIgnoreCase);

        public string Key { get; }

        public event Action Reloaded;

        internal Property(string rpKey)
        {
            Key = rpKey;
        }

        internal abstract void Reload(object rpValue);

        public abstract void Save();

        internal abstract void SetValue(object rpValue);

        protected void OnPropertyReloaded() => Reloaded?.Invoke();
    }
}
