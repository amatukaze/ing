using Sakuno.ING.Game.Logger.Binary;
using Sakuno.ING.Game.Logger.Entities.Combat;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;
using Xunit;

namespace Sakuno.ING.Game.Tests
{
    public static class BinarySerializeTest
    {
        [Fact]
        public static void TestConvertFleet()
        {
            ShipMordenizationStatus CreateParameter(int current, int displaying)
            {
                return new ShipMordenizationStatus
                {
                    Min = current,
                    Max = current,
                    Displaying = displaying
                };
            }

            var s = new ShipInBattleEntity
            {
                Id = (ShipInfoId)1234,
                Level = 100,
                Firepower = CreateParameter(123, 123),
                Torpedo = CreateParameter(321, 321),
                AntiAir = CreateParameter(111, 111),
                Armor = CreateParameter(222, 222),
                Slots = new[]
                {
                    new SlotInBattleEntity { Id = (EquipmentInfoId)432, AirProficiency = 1, ImprovementLevel = 2, Count = (3, 4)},
                    new SlotInBattleEntity { Id = (EquipmentInfoId)321, AirProficiency = 2, ImprovementLevel = 3},
                },
                Fuel = (100, 100),
                Bullet = (100, 100)
            };
            var binary = new[] { s }.Store();
            var parsed = BinaryObjectExtensions.ParseFleet(binary);
            Assert.Single(parsed);
            var ship = parsed[0];
            Assert.Equal(1234, ship.Id);
            Assert.Equal(100, ship.Level);
            Assert.Equal(123, ship.Firepower.Current);
            Assert.Equal(321, ship.Torpedo.Current);
            Assert.Equal(111, ship.AntiAir.Current);
            Assert.Equal(222, ship.Armor.Current);
            Assert.Equal(100, ship.Fuel.Value.Current);
            Assert.Equal(100, ship.Fuel.Value.Max);
            Assert.Equal(100, ship.Bullet.Value.Current);
            Assert.Equal(100, ship.Bullet.Value.Max);
            Assert.Equal(2, ship.Slots.Length);
            Assert.Equal(432, ship.Slots[0].Id);
            Assert.Equal(1, ship.Slots[0].AirProficiency);
            Assert.Equal(2, ship.Slots[0].ImprovementLevel);
            Assert.Equal(3, ship.Slots[0].Count.Current);
            Assert.Equal(4, ship.Slots[0].Count.Max);
            Assert.Equal(321, ship.Slots[1].Id);
            Assert.Equal(2, ship.Slots[1].AirProficiency);
            Assert.Equal(3, ship.Slots[1].ImprovementLevel);
            Assert.Equal(0, ship.Slots[1].Count.Current);
            Assert.Equal(0, ship.Slots[1].Count.Max);
        }

        [Fact]
        public static void TestConvertAirForce()
        {
            var g = new AirForceInBattle
            {
                Id = (AirForceGroupId)1,
                Squadrons = new[]
                {
                    new SlotInBattleEntity { Id = (EquipmentInfoId)432, AirProficiency = 1, ImprovementLevel = 2, Count = (3, 4)},
                    new SlotInBattleEntity { Id = (EquipmentInfoId)321, AirProficiency = 2, ImprovementLevel = 3},
                }
            };
            var binary = new[] { g }.Store();
            var parsed = BinaryObjectExtensions.ParseAirForce(binary);
            Assert.Single(parsed);
            var group = parsed[0];
            Assert.Equal(1, group.Id);
            Assert.Equal(2, group.Squadrons.Count);
            Assert.Equal(432, group.Squadrons[0].Id);
            Assert.Equal(1, group.Squadrons[0].AirProficiency);
            Assert.Equal(2, group.Squadrons[0].ImprovementLevel);
            Assert.Equal(3, group.Squadrons[0].Count.Current);
            Assert.Equal(4, group.Squadrons[0].Count.Max);
            Assert.Equal(321, group.Squadrons[1].Id);
            Assert.Equal(2, group.Squadrons[1].AirProficiency);
            Assert.Equal(3, group.Squadrons[1].ImprovementLevel);
        }
    }
}
