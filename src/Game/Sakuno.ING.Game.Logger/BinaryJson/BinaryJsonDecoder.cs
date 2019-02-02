using System;
using System.Text;

namespace Sakuno.ING.Game.Logger.BinaryJson
{
    internal class BinaryJsonDecoder
    {
        private ReadOnlyMemory<byte> data;
        public BinaryJsonDecoder(ReadOnlyMemory<byte> data)
        {
            this.data = data;
        }

        private byte ReadByte()
        {
            byte value = data.Span[0];
            data = data.Slice(1);
            return value;
        }

        public bool IsNextInteger() => (data.Span[0] & 0x80) == 0;
        public bool IsNextArray() => (data.Span[0] & 0b1110_0000) == 0b1000_0000;
        public bool IsNextObject() => (data.Span[0] & 0b1110_0000) == 0b1010_0000;
        public bool IsNextString() => (data.Span[0] & 0b1110_0000) == 0b1100_0000;
        public bool IsNextDecimal() => (data.Span[0] & 0b1110_0000) == 0b1110_0000;

        public bool Ends => data.IsEmpty;

        private (bool negative, int value) ReadRemainNumber()
        {
            int scale = 0;
            int result = 0;
            while (true)
            {
                byte lsb = ReadByte();
                if ((lsb & 0x80) != 0)
                {
                    result += (lsb & 0x7F) << scale;
                    scale += 7;
                }
                else
                    return ((lsb & 0x40) != 0, result + ((lsb & 0x3F) << scale));
            }
        }

        private int ReadHeaderNumber()
        {
            byte lsb = ReadByte();
            if ((lsb & 0x10) != 0)
                return (lsb & 0x0F) | (ReadRemainNumber().value << 4);
            else
                return lsb & 0x0F;
        }

        public int? ReadInteger()
        {
            byte lsb = ReadByte();
            if ((lsb & 0x40) != 0)
            {
                (bool neg, int r) = ReadRemainNumber();
                int n = (lsb & 0x3F) | (r << 6);
                if (n == 0) return null;
                return neg ? -n : n;
            }
            else
                return lsb;
        }

        public decimal ReadDecimal()
        {
            int scale = ReadHeaderNumber();
            (bool neg, decimal value) = ReadRemainNumber();
            while (scale > 0)
            {
                value /= 10;
                scale--;
            }
            return neg ? -value : value;
        }

        public decimal? ReadNumber()
        {
            if (IsNextInteger())
                return ReadInteger();
            else if (IsNextDecimal())
                return ReadDecimal();
            else
                return null;
        }

        public string ReadString()
        {
            int length = ReadHeaderNumber();
            // use span/u8string in new std
            var bytes = new byte[length];
            data.CopyTo(bytes);
            data = data.Slice(length);
            return Encoding.UTF8.GetString(bytes);
        }

        public int ReadJName() => ReadRemainNumber().value;

        public int ReadContainerLength() => ReadHeaderNumber();

        public T[] ReadArray<T>(Func<BinaryJsonDecoder, T> func)
        {
            int n = ReadContainerLength();
            var result = new T[n];
            for (int i = 0; i < n; i++)
                result[i] = func(this);
            return result;
        }

        public T ReadObject<T>(Action<BinaryJsonDecoder, int, T> action)
            where T : new()
        {
            int n = ReadContainerLength();
            T result = new T();
            while (n-- > 0)
                action(this, ReadJName(), result);
            return result;
        }

        public void SkipValue()
        {
            if (IsNextInteger())
                ReadInteger();
            else if (IsNextDecimal())
                ReadDecimal();
            else if (IsNextString())
                ReadString();
            else if (IsNextArray())
            {
                int n = ReadContainerLength();
                while (n-- > 0)
                    SkipValue();
            }
            else if (IsNextObject())
            {
                int n = ReadContainerLength();
                while (n-- > 0)
                {
                    ReadJName();
                    SkipValue();
                }
            }
        }
    }
}
