using Sakuno.ING.Game.Logger.BinaryJson;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Logger.Entities.Combat
{
    public class ShipInBattleEntity
    {
        public ShipInfoId Id { get; internal set; }
        public int Level { get; internal set; }
        public int Firepower { get; internal set; }
        public int Torpedo { get; internal set; }
        public int AntiAir { get; internal set; }
        public int Armor { get; internal set; }
        public EquipmentInBattleEntity[] Slots { get; internal set; }
        public EquipmentInBattleEntity? ExtraSlot { get; internal set; }
        public int Fuel { get; internal set; }
        public int MaxFuel { get; internal set; }
        public int Bullet { get; internal set; }
        public int MaxBullet { get; internal set; }

        public static byte[] StoreFleet(ShipInBattleEntity[] fleet)
        {
            var writer = new BinaryJsonWriter();
            writer.WriteArraySize(fleet.Length);
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
                writer.WriteInteger(ship.Firepower);
                writer.WriteJName(4);
                writer.WriteInteger(ship.Torpedo);
                writer.WriteJName(5);
                writer.WriteInteger(ship.AntiAir);
                writer.WriteJName(6);
                writer.WriteInteger(ship.Armor);
                writer.WriteJName(14);
                writer.WriteInteger(ship.Fuel);
                writer.WriteJName(15);
                writer.WriteInteger(ship.MaxFuel);
                writer.WriteJName(16);
                writer.WriteInteger(ship.Bullet);
                writer.WriteJName(17);
                writer.WriteInteger(ship.MaxBullet);
                if (ship.Slots != null)
                {
                    writer.WriteJName(7);
                    writer.WriteArraySize(ship.Slots.Length);
                    foreach (var e in ship.Slots)
                        WriteSlot(writer, e);
                }
                if (ship.ExtraSlot is EquipmentInBattleEntity ex)
                {
                    writer.WriteJName(8);
                    WriteSlot(writer, ex);
                }
                writer.WriteEndObject();
            }
            return writer.Complete();
        }

        public static byte[] StoreLandBase(EquipmentInBattleEntity[] group)
        {
            var writer = new BinaryJsonWriter();
            writer.WriteArraySize(group.Length);
            foreach (var e in group)
                WriteSlot(writer, e);
            return writer.Complete();
        }

        private static void WriteSlot(BinaryJsonWriter w, in EquipmentInBattleEntity slot)
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
                            ship.Firepower = reader.ReadIntegerOrSkip() ?? 0;
                            break;
                        case 4:
                            ship.Torpedo = reader.ReadIntegerOrSkip() ?? 0;
                            break;
                        case 5:
                            ship.AntiAir = reader.ReadIntegerOrSkip() ?? 0;
                            break;
                        case 6:
                            ship.Armor = reader.ReadIntegerOrSkip() ?? 0;
                            break;
                        case 14:
                            ship.Fuel = reader.ReadIntegerOrSkip() ?? 0;
                            break;
                        case 15:
                            ship.MaxFuel = reader.ReadIntegerOrSkip() ?? 0;
                            break;
                        case 16:
                            ship.Bullet = reader.ReadIntegerOrSkip() ?? 0;
                            break;
                        case 17:
                            ship.MaxBullet = reader.ReadIntegerOrSkip() ?? 0;
                            break;
                        case 7:
                            if (reader.TryReadContainerLengthOrSkip(out int l))
                            {
                                ship.Slots = new EquipmentInBattleEntity[l];
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

        public static EquipmentInBattleEntity[] ParseLandBase(byte[] data)
        {
            var reader = new BinaryJsonReader(data);
            if (reader.TryReadContainerLengthOrSkip(out int l))
            {
                var result = new EquipmentInBattleEntity[l];
                for (int j = 0; j < l; j++)
                    result[j] = TryReadSlot(ref reader) ?? default;
                return result;
            }
            return null;
        }

        private static EquipmentInBattleEntity? TryReadSlot(ref BinaryJsonReader r)
        {
            if (!r.StartObjectOrSkip())
                return default;
            var e = new EquipmentInBattleEntity();
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
    }
}
