using System.Linq;

namespace Sakuno.ING.Game.Models.Combat
{
    public class BattlingShip : Ship
    {
        public BattlingShip(MasterDataRoot masterData, RawShipInBattle raw)
        {
            Info = masterData.ShipInfos[raw.Id];
            Leveling = new Leveling(raw.Level, 0, 0, 0, false);
            HP = raw.HP;
            Equipment = raw.Equipment.Select(x => new Slot { Equipment = masterData.EquipmentInfos[x] }).ToBindable();
            Firepower = new ShipMordenizationStatus(raw.Firepower);
            Torpedo = new ShipMordenizationStatus(raw.Torpedo);
            AntiAir = new ShipMordenizationStatus(raw.AntiAir);
            Armor = new ShipMordenizationStatus(raw.Armor);
        }

        public override IBindableCollection<Slot> Equipment { get; }
    }
}
