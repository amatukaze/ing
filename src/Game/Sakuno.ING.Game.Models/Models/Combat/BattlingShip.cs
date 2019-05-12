using System.Linq;

namespace Sakuno.ING.Game.Models.Combat
{
    public class BattlingShip : Ship
    {
        public BattlingShip(MasterDataRoot masterData, RawShipInBattle raw)
        {
            Info = masterData.ShipInfos[raw.Id];
            Leveling = new Leveling(raw.Level);
            HP = raw.HP;
            Slots = raw.Equipment.Select(x => new ImplicitSlot(masterData.EquipmentInfos[x])).ToBindable();
            Firepower = new ShipMordenizationStatus(raw.Firepower, raw.Firepower + Slots.Sum(x => x.Equipment.Info?.Firepower ?? 0));
            Torpedo = new ShipMordenizationStatus(raw.Torpedo, raw.Torpedo + Slots.Sum(x => x.Equipment.Info?.Torpedo ?? 0));
            AntiAir = new ShipMordenizationStatus(raw.AntiAir, raw.AntiAir + Slots.Sum(x => x.Equipment.Info?.AntiAir ?? 0));
            Armor = new ShipMordenizationStatus(raw.Armor, raw.Armor + Slots.Sum(x => x.Equipment.Info?.Armor ?? 0));

            DoCalculations();
        }

        public override IBindableCollection<Slot> Slots { get; }
        public override Slot ExtraSlot => null;
    }
}
