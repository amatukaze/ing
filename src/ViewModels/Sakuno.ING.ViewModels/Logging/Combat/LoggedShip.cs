using System.Linq;
using Sakuno.ING.Game.Logger.Entities.Combat;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.ViewModels.Logging.Combat
{
    public class LoggedShip : Ship
    {
        public LoggedShip(MasterDataRoot masterData, ShipInBattleEntity e)
        {
            Info = masterData.ShipInfos[e.Id];
            Leveling = new Leveling(e.Level);
            Firepower = e.Firepower;
            AntiAir = e.AntiAir;
            Armor = e.Armor;
            Luck = e.Luck;
            LineOfSight = e.LineOfSight;
            Evasion = e.Evasion;
            AntiSubmarine = e.AntiSubmarine;
            Fuel = e.Fuel ?? default;
            Bullet = e.Bullet ?? default;
            Slots = e.Slots?.Select(x => new ImplicitSlot(masterData.EquipmentInfos[x.Id], x.Count, x.ImprovementLevel, x.AirProficiency)).ToBindable();
            if (e.ExtraSlot.Id > 0)
                ExtraSlot = new ImplicitSlot(masterData.EquipmentInfos[e.ExtraSlot.Id], e.ExtraSlot.Count, e.ExtraSlot.ImprovementLevel, e.ExtraSlot.AirProficiency);

            DoCalculations();
        }

        public override IBindableCollection<Slot> Slots { get; }
        public override Slot ExtraSlot { get; }
    }
}
