using System;
using System.Linq;
using System.Reflection;
using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.Knowledge;
using Xunit;

namespace Sakuno.ING.Game.Tests
{
    public static class MasterDataTest
    {
        private static MasterDataUpdate parseResult;

        static MasterDataTest()
        {
            var provider = new UnitTestProvider();
            var gameListener = new GameProvider(new UnitTestProviderSelector(provider));
            var masterData = new MasterDataRoot(gameListener);

            gameListener.MasterDataUpdated += (_, u) => parseResult = u;

            using var stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(typeof(MasterDataTest), "Data.masterdata.json");
            provider.Push("api_start2", DateTimeOffset.Now, default, stream);
        }

        [Fact]
        public static void TestDataLoading() => Assert.NotNull(parseResult);
        [Fact]
        public static void TestShipInfoFieldMap()
        {
            var mutsuki = parseResult.ShipInfos.First();
            Assert.Equal(1, mutsuki.Id);
            Assert.Equal("睦月", mutsuki.Name);
            Assert.Equal("むつき", mutsuki.Phonetic);
            Assert.False(mutsuki.IsAbyssal);
            Assert.True(string.IsNullOrEmpty(mutsuki.AbyssalClass));
            Assert.Equal(2, mutsuki.TypeId);
            Assert.Equal(28, mutsuki.ClassId);
            Assert.Equal(100, mutsuki.UpgradeConsumption.Bullet);
            Assert.Equal(254, mutsuki.UpgradeTo.Value);
            Assert.Equal(20, mutsuki.UpgradeLevel);
            Assert.Equal(ShipSpeed.Fast, mutsuki.Speed);
            Assert.Equal(FireRange.Short, mutsuki.FireRange);
            Assert.Equal(2, mutsuki.SlotCount);
            Assert.Equal(TimeSpan.FromMinutes(18), mutsuki.ConstructionTime);
            Assert.Equal(1, mutsuki.DismantleAcquirement.Fuel);
            Assert.Equal(new[] { 1, 1, 0, 0 }, mutsuki.PowerupWorth.ToArray());
            Assert.Equal(3, mutsuki.Rarity);

            var fuso = parseResult.ShipInfos.Single(x => x.Id == 26);
            Assert.Equal(new[] { 3, 3, 3, 3, 0 }, fuso.Aircraft.ToArray());
        }
        [Fact]
        public static void TestAbyssal()
        {
            var i = parseResult.ShipInfos.Single(x => x.Id == 1514);
            Assert.True(i.IsAbyssal);
            Assert.Equal("elite", i.AbyssalClass);
            Assert.True(string.IsNullOrEmpty(i.Phonetic));
            Assert.Null(i.Introduction);
        }
        [Fact]
        public static void TestArrayToMaterials()
        {
            var mustuki = parseResult.ShipInfos.First();
            Assert.Equal(1, mustuki.DismantleAcquirement.Fuel);
            Assert.Equal(1, mustuki.DismantleAcquirement.Bullet);
            Assert.Equal(4, mustuki.DismantleAcquirement.Steel);
        }
        [Fact]
        public static void TestArrayToMordenize()
        {
            var mutsuki = parseResult.ShipInfos.First();
            Assert.Equal(13, mutsuki.HP.Min);
            Assert.Equal(13, mutsuki.HP.Current);
            Assert.Equal(24, mutsuki.HP.Max);
            Assert.Equal(18, mutsuki.Armor.Max);
            Assert.Equal(29, mutsuki.Firepower.Max);
            Assert.Equal(59, mutsuki.Torpedo.Max);
            Assert.Equal(29, mutsuki.AntiAir.Max);
            Assert.Equal(49, mutsuki.Luck.Max);
        }
        [Fact]
        public static void TestBrEater()
        {
            foreach (var s in parseResult.ShipInfos)
                if (!s.IsAbyssal)
                    Assert.DoesNotContain("<br>", s.Introduction);
        }
        [Fact]
        public static void TestEquipmentFieldMap()
        {
            var _127 = parseResult.EquipmentInfos.Single(x => x.Id == 91);
            Assert.Equal(1, _127.TypeId);
            Assert.Equal(16, _127.IconId);
            Assert.Equal(1, _127.Accuracy);
            Assert.Equal(1, _127.Evasion);

            var raiden = parseResult.EquipmentInfos.Single(x => x.Id == 175);
            Assert.Equal(0, raiden.Accuracy);
            Assert.Equal(0, raiden.Evasion);
            Assert.Equal(5, raiden.AntiBomber);
            Assert.Equal(2, raiden.Interception);
        }
        [Fact]
        public static void TestExtraProperty()
        {
            var gun = parseResult.EquipmentTypes.Single(x => x.Id == 1);
            Assert.False(gun.AvailableInExtraSlot);
            var armor = parseResult.EquipmentTypes.Single(x => x.Id == 16);
            Assert.True(armor.AvailableInExtraSlot);

            var _12 = parseResult.EquipmentInfos.First();
            Assert.Equal(0, _12.ExtraSlotAcceptingShips.Count);
            var _8 = parseResult.EquipmentInfos.Single(x => x.Id == 66);
            Assert.NotEqual(0, _8.ExtraSlotAcceptingShips.Count);

            foreach (var m in parseResult.Maps)
                Assert.NotNull(m.BgmInfo);

            var musashi = parseResult.ShipInfos.Single(x => x.Id == 148);
            Assert.Equal(546, musashi.UpgradeTo.Value);
            foreach (var r in musashi.UpgradeSpecialConsumption)
            {
                if (r.ItemId == (int)KnownUseItem.Blueprint)
                    Assert.Equal(3, r.Count);
                else if (r.ItemId == (int)KnownUseItem.ActionReport)
                    Assert.Equal(1, r.Count);
                else
                    Assert.True(false);
            }
        }
    }
}
