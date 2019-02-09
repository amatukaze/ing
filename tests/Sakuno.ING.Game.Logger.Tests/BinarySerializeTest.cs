using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Sakuno.ING.Game.Logger.BinaryJson;
using Sakuno.ING.Game.Logger.Entities.Combat;
using Sakuno.ING.Game.Models.MasterData;
using Xunit;

namespace Sakuno.ING.Game.Tests
{
    public static class BinarySerializeTest
    {
        [Fact]
        public static void TestConvertFleet()
        {
            var fleet = new[]
            {
                new ShipInBattleEntity
                {
                    Id = (ShipInfoId)1234,
                    Level = 100,
                    Firepower = 123,
                    Torpedo = 321,
                    AntiAir = 111,
                    Armor = 222,
                    Slots = new[]
                    {
                        new SlotInBattleEntity { Id = (EquipmentInfoId)432, AirProficiency = 1, ImprovementLevel = 2, Count = 3, MaxCount = 4},
                        new SlotInBattleEntity { Id = (EquipmentInfoId)321, AirProficiency = 2, ImprovementLevel = 3},
                    },
                    Fuel = 100,
                    MaxFuel = 100,
                    Bullet = 100,
                    MaxBullet = 100
                }
            };
            var binary = ShipInBattleEntity.StoreFleet(fleet);
            var parsed = ShipInBattleEntity.ParseFleet(binary);
            Assert.Single(parsed);
            var ship = parsed[0];
            Assert.Equal(1234, ship.Id);
            Assert.Equal(100, ship.Level);
            Assert.Equal(123, ship.Firepower);
            Assert.Equal(321, ship.Torpedo);
            Assert.Equal(111, ship.AntiAir);
            Assert.Equal(222, ship.Armor);
            Assert.Equal(100, ship.Fuel);
            Assert.Equal(100, ship.MaxFuel);
            Assert.Equal(100, ship.Bullet);
            Assert.Equal(100, ship.MaxBullet);
            Assert.Equal(2, ship.Slots.Length);
            Assert.Equal(432, ship.Slots[0].Id);
            Assert.Equal(1, ship.Slots[0].AirProficiency);
            Assert.Equal(2, ship.Slots[0].ImprovementLevel);
            Assert.Equal(3, ship.Slots[0].Count);
            Assert.Equal(4, ship.Slots[0].MaxCount);
            Assert.Equal(321, ship.Slots[1].Id);
            Assert.Equal(2, ship.Slots[1].AirProficiency);
            Assert.Equal(3, ship.Slots[1].ImprovementLevel);
            Assert.Equal(0, ship.Slots[1].Count);
            Assert.Equal(0, ship.Slots[1].MaxCount);
        }

        private class IncrementDictionary : IReadOnlyDictionary<string, int>
        {
            private int number = 1;
            private readonly Dictionary<string, int> dict = new Dictionary<string, int>();

            public int this[string key]
            {
                get
                {
                    if (!dict.ContainsKey(key))
                        dict[key] = number++;
                    return dict[key];
                }
            }

            public IEnumerable<string> Keys => ((IReadOnlyDictionary<string, int>)dict).Keys;

            public IEnumerable<int> Values => ((IReadOnlyDictionary<string, int>)dict).Values;

            public int Count => dict.Count;

            public bool ContainsKey(string key) => dict.ContainsKey(key);
            public IEnumerator<KeyValuePair<string, int>> GetEnumerator() => ((IReadOnlyDictionary<string, int>)dict).GetEnumerator();
            public bool TryGetValue(string key, out int value) => dict.TryGetValue(key, out value);
            IEnumerator IEnumerable.GetEnumerator() => ((IReadOnlyDictionary<string, int>)dict).GetEnumerator();
        }

        [Fact]
        public static void TestBattleDetail()
        {
            byte[] text;
            var resolver = new IncrementDictionary();
            using (var stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(typeof(BinarySerializeTest), "Data.2018-09-19_151616.154_COMBINED_EACH_BATTLE_WATER.json"))
            {
                text = new byte[stream.Length];
                stream.Read(text, 0, (int)stream.Length);
            }
            var data = new BinaryJsonEncoder(text, resolver, "api_data").Result;
            var obj = new BattleApiDeserializer(resolver).Deserialize(data);
            Assert.NotNull(obj);
            Assert.Equal(new[] { 12, 14, 1 }, obj.api_formation);
        }
    }
}
