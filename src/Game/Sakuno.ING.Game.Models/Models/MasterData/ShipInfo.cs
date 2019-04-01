using System;
using System.Linq;

namespace Sakuno.ING.Game.Models.MasterData
{
    public partial class ShipInfo
    {
        public ShipName Name { get; private set; } = new ShipName();

        partial void CreateCore()
        {
            Name.Translation = owner.Localization?.GetLocalized("ShipName", Id.ToString());
        }

        partial void UpdateCore(RawShipInfo raw, DateTimeOffset timeStamp)
        {
            var translation = Name.Translation;
            Name = new ShipName(raw.Name, raw.Phonetic, raw.AbyssalClass)
            {
                Translation = translation
            };
            NotifyPropertyChanged(nameof(Name));
            Type = owner.ShipTypes[raw.TypeId];
            CanUpgrade = raw.UpgradeTo != 0;
            UpgradeTo = owner.ShipInfos[raw.UpgradeTo];
            TotalAircraft = Aircraft?.Sum();
        }

        public override string ToString() => $"ShipInfo {Id}: {Name.Origin}";
    }
}
