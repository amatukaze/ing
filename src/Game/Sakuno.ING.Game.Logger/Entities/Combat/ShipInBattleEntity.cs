using System.Linq;
using Sakuno.ING.Game.Logger.BinaryJson;
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

        public static byte[] StoreFleet(ShipInBattleEntity[] fleet)
        {
            var writer = new BinaryJsonWriter();
            writer.WriteArraySize(fleet.Length);

            void WriteShipParameter(BinaryJsonWriter w, ShipMordenizationStatus p)
            {
                w.WriteArraySize(2);
                w.WriteInteger(p.Current);
                w.WriteInteger(p.Displaying);
            }

            foreach (var ship in fleet)
            {
                if (ship is null)
                {
                    writer.WriteNull();
                    continue;
                }
                writer.WriteStartObject();
                writer.WriteJName(1);
                writer.WriteInteger(ship.Id);
                writer.WriteJName(2);
                writer.WriteInteger(ship.Level);
                writer.WriteJName(3);
                WriteShipParameter(writer, ship.Firepower);
                writer.WriteJName(4);
                WriteShipParameter(writer, ship.Torpedo);
                writer.WriteJName(5);
                WriteShipParameter(writer, ship.AntiAir);
                writer.WriteJName(6);
                WriteShipParameter(writer, ship.Armor);
                writer.WriteJName(16);
                WriteShipParameter(writer, ship.Luck);
                writer.WriteJName(17);
                WriteShipParameter(writer, ship.LineOfSight);
                writer.WriteJName(18);
                WriteShipParameter(writer, ship.Evasion);
                writer.WriteJName(19);
                WriteShipParameter(writer, ship.AntiSubmarine);
                if (ship.Fuel is ClampedValue fuel)
                {
                    writer.WriteJName(14);
                    writer.WriteArraySize(2);
                    writer.WriteInteger(fuel.Current);
                    writer.WriteInteger(fuel.Max);
                }
                if (ship.Bullet is ClampedValue bullet)
                {
                    writer.WriteJName(15);
                    writer.WriteArraySize(2);
                    writer.WriteInteger(bullet.Current);
                    writer.WriteInteger(bullet.Max);
                }
                if (ship.Slots != null)
                {
                    writer.WriteJName(7);
                    writer.WriteArraySize(ship.Slots.Length);
                    foreach (var e in ship.Slots)
                        WriteSlot(writer, e);
                }
                if (ship.ExtraSlot is SlotInBattleEntity ex)
                {
                    writer.WriteJName(8);
                    WriteSlot(writer, ex);
                }
                writer.WriteEndObject();
            }
            return writer.Complete();
        }

        public static byte[] StoreLandBase(SlotInBattleEntity[] group)
        {
            var writer = new BinaryJsonWriter();
            writer.WriteArraySize(group.Length);
            foreach (var e in group)
                WriteSlot(writer, e);
            return writer.Complete();
        }

        private static void WriteSlot(BinaryJsonWriter w, in SlotInBattleEntity slot)
        {
            w.WriteStartObject();
            w.WriteJName(9);
            w.WriteInteger(slot.Id);
            w.WriteJName(12);
            w.WriteInteger(slot.Count);
            w.WriteJName(13);
            w.WriteInteger(slot.MaxCount);
            if (slot.ImprovementLevel > 0)
            {
                w.WriteJName(10);
                w.WriteInteger(slot.ImprovementLevel);
            }
            if (slot.AirProficiency > 0)
            {
                w.WriteJName(11);
                w.WriteInteger(slot.AirProficiency);
            }
            w.WriteEndObject();
        }

        public static ShipInBattleEntity[] ParseFleet(byte[] data)
        {
            var reader = new BinaryJsonReader(data);
            if (!reader.IsNextArray())
                return null;
            var result = new ShipInBattleEntity[reader.ReadContainerLength()];

            (int, int) ReadArray2(BinaryJsonReader r)
            {
                if (r.TryReadContainerLengthOrSkip(out int l))
                    if (l == 2)
                    {
                        int a = r.ReadIntegerOrSkip() ?? 0;
                        int b = r.ReadIntegerOrSkip() ?? 0;
                        return (a, b);
                    }
                    else while (l-- > 0)
                            r.SkipValue();
                return default;
            }

            ShipMordenizationStatus ReadShipParameter(BinaryJsonReader r)
            {
                (int a, int b) = ReadArray2(r);
                return new ShipMordenizationStatus
                {
                    Min = a,
                    Max = a,
                    Improved = 0,
                    Displaying = b
                };
            }

            for (int i = 0; i < result.Length; i++)
            {
                if (!reader.StartObjectOrSkip())
                    continue;
                var ship = new ShipInBattleEntity();
                while (reader.UntilObjectEnds())
                {
                    switch (reader.ReadJName())
                    {
                        case 1:
                            ship.Id = (ShipInfoId)(reader.ReadIntegerOrSkip() ?? 0);
                            break;
                        case 2:
                            ship.Level = reader.ReadIntegerOrSkip() ?? 0;
                            break;
                        case 3:
                            ship.Firepower = ReadShipParameter(reader);
                            break;
                        case 4:
                            ship.Torpedo = ReadShipParameter(reader);
                            break;
                        case 5:
                            ship.AntiAir = ReadShipParameter(reader);
                            break;
                        case 6:
                            ship.Armor = ReadShipParameter(reader);
                            break;
                        case 14:
                            ship.Fuel = ReadArray2(reader);
                            break;
                        case 15:
                            ship.Bullet = ReadArray2(reader);
                            break;
                        case 16:
                            ship.Luck = ReadShipParameter(reader);
                            break;
                        case 17:
                            ship.LineOfSight = ReadShipParameter(reader);
                            break;
                        case 18:
                            ship.Evasion = ReadShipParameter(reader);
                            break;
                        case 19:
                            ship.AntiSubmarine = ReadShipParameter(reader);
                            break;
                        case 7:
                            if (reader.TryReadContainerLengthOrSkip(out int l))
                            {
                                ship.Slots = new SlotInBattleEntity[l];
                                for (int j = 0; j < l; j++)
                                    ship.Slots[j] = TryReadSlot(ref reader) ?? default;
                            }
                            break;
                        case 8:
                            ship.ExtraSlot = TryReadSlot(ref reader);
                            break;
                        default:
                            reader.SkipValue();
                            break;
                    }
                }
                result[i] = ship;
            }
            return result;
        }

        public static SlotInBattleEntity[] ParseLandBase(byte[] data)
        {
            var reader = new BinaryJsonReader(data);
            if (reader.TryReadContainerLengthOrSkip(out int l))
            {
                var result = new SlotInBattleEntity[l];
                for (int j = 0; j < l; j++)
                    result[j] = TryReadSlot(ref reader) ?? default;
                return result;
            }
            return null;
        }

        private static SlotInBattleEntity? TryReadSlot(ref BinaryJsonReader r)
        {
            if (!r.StartObjectOrSkip())
                return default;
            var e = new SlotInBattleEntity();
            while (r.UntilObjectEnds())
                switch (r.ReadJName())
                {
                    case 9:
                        e.Id = (EquipmentInfoId)(r.ReadIntegerOrSkip() ?? 0);
                        break;
                    case 10:
                        e.ImprovementLevel = r.ReadIntegerOrSkip() ?? 0;
                        break;
                    case 11:
                        e.AirProficiency = r.ReadIntegerOrSkip() ?? 0;
                        break;
                    case 12:
                        e.Count = r.ReadIntegerOrSkip() ?? 0;
                        break;
                    case 13:
                        e.MaxCount = r.ReadIntegerOrSkip() ?? 0;
                        break;
                }
            return e;
        }

        public static SlotInBattleEntity[] SelectLandBase(AirForceGroup group)
            => group.Squadrons
                .Select(x => new SlotInBattleEntity
                {
                    Id = x.Equipment?.Info.Id ?? default,
                    Count = x.AircraftCount.Current,
                    MaxCount = x.AircraftCount.Max
                }).ToArray();

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
                    Count = x.Aircraft.Current,
                    MaxCount = x.Aircraft.Max
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
