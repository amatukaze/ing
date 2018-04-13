using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sakuno.KanColle.Amatsukaze.Game.Events;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Knowledge;

namespace Sakuno.KanColle.Amatsukaze.Game.Test
{
    [TestClass]
    public class MasterDataTest
    {
        private static MasterDataUpdate parseResult;
        [ClassInitialize]
        public static void LoadData(TestContext context)
        {
            var provider = new UnitTestProvider();
            var gameListener = new GameListener(provider);

            gameListener.MasterDataUpdated.Received += u => parseResult = u;

            using (var stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(typeof(MasterDataTest), "Data.masterdata.json"))
                provider.Push("api_start2", DateTimeOffset.Now, string.Empty, stream);
        }
        [TestMethod]
        public void TestDataLoading()
        {
            Assert.IsNotNull(parseResult);
        }
        [TestMethod]
        public void TestShipInfoFieldMap()
        {
            var mutsuki = parseResult.ShipInfos.First();
            Assert.AreEqual(mutsuki.Id, 1);
            Assert.AreEqual(mutsuki.Name, "ÄÀÔÂ");
            Assert.AreEqual(mutsuki.Phonetic, "¤à¤Ä¤­");
            Assert.IsFalse(mutsuki.IsAbyssal);
            Assert.IsTrue(string.IsNullOrEmpty(mutsuki.AbyssalClass));
            Assert.AreEqual(mutsuki.TypeId, 2);
            Assert.AreEqual(mutsuki.ClassId, 28);
            Assert.AreEqual(mutsuki.UpgradeConsumption.Bullet, 100);
            Assert.AreEqual(mutsuki.UpgradeTo, 254);
            Assert.AreEqual(mutsuki.UpgradeLevel, 20);
            Assert.AreEqual(mutsuki.Speed, ShipSpeed.Fast);
            Assert.AreEqual(mutsuki.FireRange, FireRange.Short);
            Assert.AreEqual(mutsuki.SlotCount, 2);
            Assert.AreEqual(mutsuki.ConstructionTime, TimeSpan.FromMinutes(18));
            Assert.AreEqual(mutsuki.DismantleAcquirement.Fuel, 1);
            CollectionAssert.AreEqual(mutsuki.PowerupWorth.ToArray(), new[] { 1, 1, 0, 0 });
            Assert.AreEqual(mutsuki.Rarity, 3);

            var fuso = parseResult.ShipInfos.Single(x => x.Id == 26);
            CollectionAssert.AreEqual(fuso.Aircraft.ToArray(), new[] { 3, 3, 3, 3, 0 });
        }
        [TestMethod]
        public void TestAbyssal()
        {
            var i = parseResult.ShipInfos.Single(x => x.Id == 1514);
            Assert.IsTrue(i.IsAbyssal);
            Assert.AreEqual(i.AbyssalClass, "elite");
            Assert.IsTrue(string.IsNullOrEmpty(i.Phonetic));
            Assert.IsNull(i.Introduction);
        }
        [TestMethod]
        public void TestArrayToMaterials()
        {
            var mustuki = parseResult.ShipInfos.First();
            Assert.AreEqual(mustuki.DismantleAcquirement.Fuel, 1);
            Assert.AreEqual(mustuki.DismantleAcquirement.Bullet, 1);
            Assert.AreEqual(mustuki.DismantleAcquirement.Steel, 4);
        }
        [TestMethod]
        public void TestArrayToMordenize()
        {
            var mutsuki = parseResult.ShipInfos.First();
            Assert.AreEqual(mutsuki.HP.Min, 13);
            Assert.AreEqual(mutsuki.HP.Current, 13);
            Assert.AreEqual(mutsuki.HP.Max, 24);
            Assert.AreEqual(mutsuki.Armor.Max, 18);
            Assert.AreEqual(mutsuki.Firepower.Max, 29);
            Assert.AreEqual(mutsuki.Torpedo.Max, 59);
            Assert.AreEqual(mutsuki.AntiAir.Max, 29);
            Assert.AreEqual(mutsuki.Luck.Max, 49);
        }
        [TestMethod]
        public void TestBrEater()
        {
            foreach (var s in parseResult.ShipInfos)
                if (!s.IsAbyssal)
                    Assert.IsFalse(s.Introduction.Contains("<br>"));
        }
        [TestMethod]
        public void TestEquipmentFieldMap()
        {
            var _127 = parseResult.EquipmentInfos.Single(x => x.Id == 91);
            Assert.AreEqual(_127.TypeId, 1);
            Assert.AreEqual(_127.IconId, 16);
            Assert.AreEqual(_127.Accuracy, 1);
            Assert.AreEqual(_127.Evasion, 1);

            var raiden = parseResult.EquipmentInfos.Single(x => x.Id == 175);
            Assert.AreEqual(raiden.Accuracy, 0);
            Assert.AreEqual(raiden.Evasion, 0);
            Assert.AreEqual(raiden.AntiBomber, 5);
            Assert.AreEqual(raiden.Interception, 2);
        }
        [TestMethod]
        public void TestExtraProperty()
        {
            var gun = parseResult.EquipmentTypes.Single(x => x.Id == 1);
            Assert.IsFalse(gun.AvailableInExtraSlot);
            var armor = parseResult.EquipmentTypes.Single(x => x.Id == 16);
            Assert.IsTrue(armor.AvailableInExtraSlot);

            var _12 = parseResult.EquipmentInfos.First();
            Assert.AreEqual(_12.ExtraSlotAcceptingShips.Count, 0);
            var _8 = parseResult.EquipmentInfos.Single(x => x.Id == 66);
            Assert.AreNotEqual(_8.ExtraSlotAcceptingShips.Count, 0);

            foreach (var m in parseResult.Maps)
                Assert.IsNotNull(m.BgmInfo);

            var musashi = parseResult.ShipInfos.Single(x => x.Id == 148);
            Assert.AreEqual(musashi.UpgradeTo, 546);
            foreach (var r in musashi.UpgradeSpecialConsumption)
            {
                if (r.ItemId == (int)KnownUseItem.Blueprint)
                    Assert.AreEqual(r.Count, 3);
                else if (r.ItemId == (int)KnownUseItem.ActionReport)
                    Assert.AreEqual(r.Count, 1);
                else Assert.Fail();
            }
        }
    }
}
