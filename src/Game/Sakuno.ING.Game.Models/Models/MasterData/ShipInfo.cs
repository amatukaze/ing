using System;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.ING.Game.Models.MasterData
{
    public partial class ShipInfo
    {
        private ShipName _name = new ShipName();
        public ShipName Name
        {
            get => _name;
            private set => Set(ref _name, value);
        }

        partial void UpdateCore(RawShipInfo raw, DateTimeOffset timeStamp)
        {
            Name = new ShipName(Id, raw.Name, raw.Phonetic, raw.AbyssalClass);
            Type = owner.ShipTypes[raw.TypeId];
            CanUpgrade = raw.UpgradeTo != 0;
            UpgradeTo = owner.ShipInfos[raw.UpgradeTo];
            TotalAircraft = Aircraft?.Sum();
        }

        public bool CanUpgradeFrom(ShipInfoId id)
        {
            var visited = new List<ShipInfoId>();
            for (var info = owner.ShipInfos.TryGet(id); info is object; info = info.UpgradeTo)
            {
                var currentId = info.Id;
                if (currentId == Id) return true;
                if (visited.Contains(currentId)) return false;
                visited.Add(currentId);
            }
            return false;
        }

        public override string ToString() => $"ShipInfo {Id}: {Name.FullName.Origin}";
    }
}
