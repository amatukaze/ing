using System.Collections.Generic;
using System.Text;
using Sakuno.ING.Game.Logger.BinaryJson;
using Xunit;

namespace Sakuno.ING.Game.Tests
{
    public static class BinaryJsonTest
    {
        private static readonly Dictionary<string, int> jNames = new Dictionary<string, int>
        {
            ["a"] = 1,
            ["b"] = 2
        };

        private static void AssertEncodeEqual(string json, byte[] value)
        {
            var utf8 = Encoding.UTF8.GetBytes(json);
            var result = new BinaryJsonEncoder(utf8, jNames).Result;
            Assert.Equal(value, result);
        }

        [Fact]
        public static void TestEncodeInteger()
        {
            AssertEncodeEqual("1", new byte[] { 1 });
            AssertEncodeEqual("128", new byte[] { 0b0100_0000, 2 });
            AssertEncodeEqual("-1", new byte[] { 0b0100_0001, 0b0100_0000 });
        }

        [Fact]
        public static void TestEncodeDecimal()
        {
            AssertEncodeEqual("1.1", new byte[] { 0b1110_0001, 11 });
            AssertEncodeEqual("-1.1", new byte[] { 0b1110_0001, 0b0100_1011 });
        }

        [Fact]
        public static void TestEncodeArray()
        {
            AssertEncodeEqual("[1,2,3,4]", new byte[]
            {
                0b1000_0100, 1, 2, 3, 4
            });
            AssertEncodeEqual("[[1],[2],[3]]", new byte[]
            {
                0b1000_0011,
                0b1000_0001, 1,
                0b1000_0001, 2,
                0b1000_0001, 3
            });
        }

        [Fact]
        public static void TestEncodeObject()
        {
            AssertEncodeEqual("{\"a\":1,\"b\":[1,2]}", new byte[]
            {
                0b1010_0010,
                1, 1,
                2, 0b1000_0010, 1, 2
            });
            AssertEncodeEqual("{\"a\":null,\"b\":0}", new byte[]
            {
                0b1010_0010,
                1, 0b0100_0000, 0,
                2, 0
            });
        }

        [Fact]
        public static void TestDecodeInteger()
        {
            Assert.Equal(1, new BinaryJsonDecoder(new byte[] { 1 }).ReadInteger());
            Assert.Equal(-1, new BinaryJsonDecoder(new byte[] { 0b0100_0001, 0b0100_0000 }).ReadInteger());
            Assert.Equal(128, new BinaryJsonDecoder(new byte[] { 0b0100_0000, 2 }).ReadInteger());
            for (int i = -512; i <= 512; i++)
            {
                var utf8 = Encoding.UTF8.GetBytes(i.ToString());
                var b = new BinaryJsonEncoder(utf8, jNames).Result;
                var decoder = new BinaryJsonDecoder(b);
                Assert.Equal(i, decoder.ReadInteger());
                Assert.True(decoder.Ends);
            }
        }

        [Fact]
        public static void TestDecodeNull()
            => Assert.Null(new BinaryJsonDecoder(new byte[] { 0b0100_0000, 0 }).ReadInteger());

        [Fact]
        public static void TestDecodeDecimal()
        {
            for (decimal d = -512.5m; d <= 512; d++)
            {
                var utf8 = Encoding.UTF8.GetBytes(d.ToString());
                var b = new BinaryJsonEncoder(utf8, jNames).Result;
                var decoder = new BinaryJsonDecoder(b);
                Assert.Equal(d, decoder.ReadDecimal());
                Assert.True(decoder.Ends);
            }
        }

        [Fact]
        public static void TestDecodeString()
            => Assert.Equal("abc", new BinaryJsonDecoder(new byte[] { 0b1100_0011, (byte)'a', (byte)'b', (byte)'c' }).ReadString());

        [Fact]
        public static void TestDecodeArray()
        {
            var d = new BinaryJsonDecoder(new byte[] { 0b1000_0100, 1, 2, 3, 4 });
            Assert.True(d.IsNextArray());
            Assert.Equal(4, d.ReadContainerLength());
            Assert.Equal(1, d.ReadInteger());
            Assert.Equal(2, d.ReadInteger());
            Assert.Equal(3, d.ReadInteger());
            Assert.Equal(4, d.ReadInteger());
            Assert.True(d.Ends);
        }

        [Fact]
        public static void TestDecodeSkip()
        {
            void AssertSkip(string original)
            {
                var utf8 = Encoding.UTF8.GetBytes(original);
                var b = new BinaryJsonEncoder(utf8, jNames).Result;
                var decoder = new BinaryJsonDecoder(b);
                decoder.SkipValue();
                Assert.True(decoder.Ends);
            }
            AssertSkip("1");
            AssertSkip("-123.456");
            AssertSkip("[1,2,[3,4],5]");
            AssertSkip("{\"a\":1,\"b\":2}");
            AssertSkip("{\"a\":[1,2],\"b\":[3,4,5]}");
            AssertSkip("[{\"a\":1,\"b\":2},{\"a\":3,\"b\":4}]");
        }
    }
}
