using System.Linq;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Logger.Entities.Combat
{
    public class ShipInBattleEntity
    {
        public ShipInfoId Id { get; internal set; }
        public int Level { get; internal set; }
        public ShipMordenizationStatus Firepower { get; internal set; }
        public ShipMordenizationStatus Torpedo { get; internal set; }
        public ShipMordenizationStatus AntiAir { get; internal set; }
        public ShipMordenizationStatus Armor { get; internal set; }
        public ShipMordenizationStatus Luck { get; internal set; }
        public ShipMordenizationStatus LineOfSight { get; internal set; }
        public ShipMordenizationStatus Evasion { get; internal set; }
        public ShipMordenizationStatus AntiSubmarine { get; internal set; }
        public SlotInBattleEntity[] Slots { get; internal set; }
        public SlotInBattleEntity? ExtraSlot { get; internal set; }
        public ClampedValue? Fuel { get; internal set; }
        public ClampedValue? Bullet { get; internal set; }

        public ShipInBattleEntity() { }

        public ShipInBattleEntity(Ship ship)
        {
            Id = ship.Info.Id;
            Level = ship.Leveling.Level;
            Firepower = ship.Firepower;
            Torpedo = ship.Torpedo;
            AntiAir = ship.AntiAir;
            Armor = ship.Armor;
            Luck = ship.Luck;
            LineOfSight = ship.LineOfSight;
            Evasion = ship.Evasion;
            AntiSubmarine = ship.AntiSubmarine;
            Slots = ship.Slots
                .Select(x => new SlotInBattleEntity
                {
                    Id = x.Equipment?.Info.Id ?? default,
                    Count = x.Aircraft
                }).ToArray();
            ExtraSlot = new SlotInBattleEntity
            {
                Id = ship.ExtraSlot?.Equipment?.Info.Id ?? default
            };
            Fuel = ship.Fuel;
            Bullet = ship.Bullet;
        }
    }
}
