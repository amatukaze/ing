using System;
using System.Linq;

namespace Sakuno.ING.Game.Models.MasterData
{
    public partial class ShipInfo
    {
        public ShipName Name { get; } = new ShipName();

        partial void CreateDummy()
        {
            Name.Translation = owner.Localization?.GetLocalized("ShipName", Id.ToString());
        }

        partial void UpdateCore(IRawShipInfo raw, DateTimeOffset timeStamp)
        {
            if (Name.Origin != raw.Name ||
                Name.Phonetic != raw.Phonetic ||
                Name.AbyssalClass != raw.AbyssalClass)
            {
                Name.Origin = raw.Name;
                Name.Phonetic = raw.Phonetic;
                Name.AbyssalClass = raw.AbyssalClass;
                NotifyPropertyChanged(nameof(Name));
            }

            Type = owner.ShipTypes[raw.TypeId];
            CanUpgrade = raw.UpgradeTo != 0;
            UpgradeTo = owner.ShipInfos[raw.UpgradeTo];
            TotalAircraft = Aircraft?.Sum();
        }

        public override string ToString() => $"ShipInfo {Id}: {Name.Origin}";
    }
}
