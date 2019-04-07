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
            Name.BasicTranslation = owner.Localization?.GetLocalized("ShipNameBasic", Id.ToString());
        }

        partial void UpdateCore(RawShipInfo raw, DateTimeOffset timeStamp)
        {
            var translation = Name.Translation;
            var basic = Name.BasicTranslation;
            Name = new ShipName(raw.Name, raw.Phonetic, raw.AbyssalClass)
            {
                Translation = translation,
                BasicTranslation = basic
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
