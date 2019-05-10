using System.Linq;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Logger.Entities.Combat
{
    public class ShipInBattleEntity
    {
        public ShipInfoId Id { get; set; }
        public int Level { get; set; }
        public ShipMordenizationStatus Firepower { get; set; }
        public ShipMordenizationStatus Torpedo { get; set; }
        public ShipMordenizationStatus AntiAir { get; set; }
        public ShipMordenizationStatus Armor { get; set; }
        public ShipMordenizationStatus Luck { get; set; }
        public ShipMordenizationStatus LineOfSight { get; set; }
        public ShipMordenizationStatus Evasion { get; set; }
        public ShipMordenizationStatus AntiSubmarine { get; set; }
        public SlotInBattleEntity[] Slots { get; set; }
        public SlotInBattleEntity ExtraSlot { get; set; }
        public ClampedValue? Fuel { get; set; }
        public ClampedValue? Bullet { get; set; }

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
                    Id = x?.Equipment?.Info?.Id ?? default,
                    Count = x.Aircraft,
                    AirProficiency = x?.Equipment?.AirProficiency ?? 0,
                    ImprovementLevel = x?.Equipment?.ImprovementLevel ?? 0
                }).ToArray();
            ExtraSlot = new SlotInBattleEntity
            {
                Id = ship.ExtraSlot?.Equipment?.Info?.Id ?? default,
                ImprovementLevel = ship.ExtraSlot?.Equipment?.ImprovementLevel ?? 0
            };
            Fuel = ship.Fuel;
            Bullet = ship.Bullet;
        }
    }
}
