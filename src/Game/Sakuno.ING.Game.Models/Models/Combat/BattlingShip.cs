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
            Firepower = new ShipMordenizationStatus(raw.Firepower);
            Torpedo = new ShipMordenizationStatus(raw.Torpedo);
            AntiAir = new ShipMordenizationStatus(raw.AntiAir);
            Armor = new ShipMordenizationStatus(raw.Armor);

            DoCalculations();
        }

        public override IBindableCollection<Slot> Slots { get; }
        public override Slot ExtraSlot => null;
    }
}
