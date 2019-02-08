using System;
using System.Text;

namespace Sakuno.ING.Game.Logger.BinaryJson
{
    internal class BinaryJsonReader
    {
        private ReadOnlyMemory<byte> data;
        public BinaryJsonReader(ReadOnlyMemory<byte> data)
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
        public bool IsNextObject() => data.Span[0] == 0b1010_0000;
        public bool IsEndObject() => data.Span[0] == 0;
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

        public int? ReadIntegerOrSkip()
        {
            if (IsNextInteger())
                return ReadInteger();
            else
            {
                SkipValue();
                return null;
            }
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

        public decimal? ReadNumberOrSkip()
        {
            if (IsNextInteger())
                return ReadInteger();
            else if (IsNextDecimal())
                return ReadDecimal();
            else
            {
                SkipValue();
                return null;
            }
        }

        public string ReadString()
        {
            int length = ReadHeaderNumber();
            // use span/u8string in new std
            var bytes = new byte[length];
            data.Slice(0, length).CopyTo(bytes);
            data = data.Slice(length);
            return Encoding.UTF8.GetString(bytes);
        }

        public string ReadStringOrSkip()
        {
            if (IsNextString())
                return ReadString();
            else
            {
                SkipValue();
                return null;
            }
        }

        public int ReadJName() => ReadRemainNumber().value;

        public int ReadContainerLength() => ReadHeaderNumber();
        public bool TryReadContainerLengthOrSkip(out int length)
        {
            if (IsNextArray())
            {
                length = ReadContainerLength();
                return true;
            }
            length = 0;
            return false;
        }

        public void ReadStartObject() => data = data.Slice(1);
        public bool StartObjectOrSkip()
        {
            if (IsNextObject())
            {
                ReadStartObject();
                return true;
            }
            else
            {
                SkipValue();
                return false;
            }
        }
        public bool UntilObjectEnds()
        {
            if (IsEndObject())
            {
                data = data.Slice(1);
                return false;
            }
            return true;
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
                while (UntilObjectEnds())
                {
                    ReadJName();
                    SkipValue();
                }
            }
        }
    }
}
