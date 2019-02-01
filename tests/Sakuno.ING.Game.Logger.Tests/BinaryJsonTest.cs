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
            Assert.Equal(result, value);
        }

        [Fact]
        public static void TestInteger()
        {
            AssertEncodeEqual("1", new byte[] { 1 });
            AssertEncodeEqual("128", new byte[] { 0b0100_0000, 2 });
            AssertEncodeEqual("-1", new byte[] { 0b0100_0001, 0b0100_0000 });
        }

        [Fact]
        public static void TestDecimal()
        {
            AssertEncodeEqual("1.1", new byte[] { 0b1110_0001, 11 });
            AssertEncodeEqual("-1.1", new byte[] { 0b1110_0001, 0b0100_1011 });
        }

        [Fact]
        public static void TestArray()
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
        public static void TestObject()
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
    }
}
