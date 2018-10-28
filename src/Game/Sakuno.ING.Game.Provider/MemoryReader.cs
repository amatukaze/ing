using System;
using System.IO;

namespace Sakuno.ING.Game
{
    internal sealed class MemoryReader : TextReader
    {
        private ReadOnlyMemory<char> memory;
        public MemoryReader(ReadOnlyMemory<char> memory) => this.memory = memory;

        public override int Peek() => memory.IsEmpty ? -1 : memory.Span[0];

        public override int Read()
        {
            if (memory.IsEmpty)
                return -1;
            else
            {
                char result = memory.Span[0];
                memory = memory.Slice(1);
                return result;
            }
        }

        public override int Read(char[] buffer, int index, int count)
        {
            count = Math.Min(count, memory.Length);
            memory.Slice(0, count).CopyTo(buffer.AsMemory(index));
            memory = memory.Slice(count);
            return count;
        }

        public override string ReadToEnd() => memory.ToString();

        public override int ReadBlock(char[] buffer, int index, int count) => Read(buffer, index, count);
    }
}
