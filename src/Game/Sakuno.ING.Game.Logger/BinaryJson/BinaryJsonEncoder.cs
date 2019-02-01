using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Sakuno.ING.Game.Logger.BinaryJson
{
    internal class BinaryJsonEncoder
    {
        private readonly IReadOnlyDictionary<string, int> jNames;
        private readonly List<byte> buffer;

        public byte[] Result { get; }

        public BinaryJsonEncoder(ReadOnlyMemory<byte> utf8Json, IReadOnlyDictionary<string, int> jNames)
        {
            this.jNames = jNames;
            using (var document = JsonDocument.Parse(utf8Json))
            {
                buffer = new List<byte>();
                Format(document.RootElement);
                Result = buffer.ToArray();
            }
        }

        private static int GetObjectSize(JsonElement element)
        {
            int result = 0;
            foreach (var _ in element.EnumerateObject())
                result++;
            return result;
        }

        private void FormatRecordHeader(byte flag, int number)
        {
            uint n = (uint)number;
            byte lsb = (byte)(n & 0x0F);
            lsb |= flag;
            n >>= 4;
            if (n > 0)
            {
                lsb |= 0x10;
                buffer.Add(lsb);
                WriteHighNumber(n, false);
            }
            else
                buffer.Add(lsb);
        }

        private void WriteHighNumber(uint value, bool negative)
        {
            bool hasNext;
            do
            {
                byte lsb = (byte)(value & 0x7F);
                value >>= 7;
                hasNext = value > 0;
                if (value == 0 && (lsb & 0x40) != 0)
                    hasNext = true;

                if (hasNext) lsb |= 0x80;
                else if (negative) lsb |= 0x40;
                buffer.Add(lsb);
            }
            while (hasNext);
        }

        private void Format(JsonElement element)
        {
            switch (element.Type)
            {
                case JsonValueType.Object:
                    FormatRecordHeader(0b1010_0000, GetObjectSize(element));
                    foreach (var child in element.EnumerateObject())
                    {
                        WriteHighNumber((uint)jNames[child.Name], false);
                        Format(child.Value);
                    }
                    break;

                case JsonValueType.Array:
                    FormatRecordHeader(0b1000_0000, element.GetArrayLength());
                    foreach (var child in element.EnumerateArray())
                        Format(child);
                    break;

                case JsonValueType.String:
                    string str = element.GetString();
                    var u8 = Encoding.UTF8.GetBytes(str);
                    FormatRecordHeader(0b1100_0000, u8.Length);
                    buffer.AddRange(u8);
                    break;

                case JsonValueType.Number:
                    if (element.TryGetInt32(out int ivalue))
                    {
                        bool neg = ivalue < 0;
                        uint n = (uint)(neg ? -ivalue : ivalue);
                        byte lsb = (byte)(n & 0x3F);
                        n >>= 6;
                        if (neg || n > 0)
                        {
                            lsb |= 0x40;
                            buffer.Add(lsb);
                            WriteHighNumber(n, neg);
                        }
                        else
                            buffer.Add(lsb);
                    }
                    else
                    {
                        decimal dvalue = element.GetDecimal();
                        bool neg = dvalue < 0;
                        int scale = 0;
                        while (dvalue != decimal.Truncate(dvalue))
                        {
                            dvalue *= 10;
                            scale++;
                        }
                        FormatRecordHeader(0b1110_0000, scale);
                        WriteHighNumber((uint)(neg ? -dvalue : dvalue), neg);
                    }
                    break;

                case JsonValueType.True:
                    buffer.Add(1);
                    break;

                case JsonValueType.False:
                    buffer.Add(0);
                    break;

                case JsonValueType.Undefined:
                case JsonValueType.Null:
                    buffer.Add(0b0100_0000);
                    buffer.Add(0);
                    break;
                default:
                    throw new InvalidOperationException("Unknown json value type.");
            }
        }
    }
}
