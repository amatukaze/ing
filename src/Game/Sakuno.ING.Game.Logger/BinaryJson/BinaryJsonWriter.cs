using System.Collections.Generic;
using System.Text;

namespace Sakuno.ING.Game.Logger.BinaryJson
{
    internal class BinaryJsonWriter
    {
        private readonly List<byte> buffer = new List<byte>();
        private void WriteRecordHeader(byte flag, int number)
        {
            uint n = (uint)number;
            byte lsb = (byte)(n & 0x0F);
            lsb |= flag;
            n >>= 4;
            if (n > 0)
            {
                lsb |= 0x10;
                buffer.Add(lsb);
                WriteRemainNumber(n, false);
            }
            else
                buffer.Add(lsb);
        }

        private void WriteRemainNumber(uint value, bool negative)
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

        public void WriteJName(int index) => WriteRemainNumber((uint)index, false);
        public void WriteArraySize(int size) => WriteRecordHeader(0b1000_0000, size);
        public void WriteStartObject() => buffer.Add(0b1010_0000);
        public void WriteEndObject() => buffer.Add(0b1010_1111);

        public void WriteString(string str)
        {
            var u8 = Encoding.UTF8.GetBytes(str);
            WriteRecordHeader(0b1100_0000, u8.Length);
            buffer.AddRange(u8);
        }

        public void WriteInteger(int ivalue)
        {
            bool neg = ivalue < 0;
            uint n = (uint)(neg ? -ivalue : ivalue);
            byte lsb = (byte)(n & 0x3F);
            n >>= 6;
            if (neg || n > 0)
            {
                lsb |= 0x40;
                buffer.Add(lsb);
                WriteRemainNumber(n, neg);
            }
            else
                buffer.Add(lsb);
        }

        public void WriteDecimal(decimal dvalue)
        {
            bool neg = dvalue < 0;
            int scale = 0;
            while (dvalue != decimal.Truncate(dvalue))
            {
                dvalue *= 10;
                scale++;
            }
            WriteRecordHeader(0b1110_0000, scale);
            WriteRemainNumber((uint)(neg ? -dvalue : dvalue), neg);
        }

        public void WriteNull()
        {
            buffer.Add(0b0100_0000);
            buffer.Add(0);
        }

        public byte[] Complete() => buffer.ToArray();
    }
}
