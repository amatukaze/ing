using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Sakuno.ING.Game.Tests
{
    public static class SequenceDiffTest
    {
        private static readonly object[] pool;

        static SequenceDiffTest()
        {
            pool = new object[20];
            for (int i = 0; i < pool.Length; i++)
                pool[i] = new object();
        }

        [Fact]
        public static void DiffTest()
        {
            var s = new List<object>
            {
                pool[0],
                pool[1],
                pool[3],
                pool[4]
            };
            var t = new List<object>
            {
                pool[0],
                pool[1],
                pool[2],
                pool[3]
            };
            var result = BindableSnapshotCollection<object>.SequenceDiffer(s, t);
            Assert.Equal(2, result.Length);

            Assert.True(result[0].IsAdd);
            Assert.Equal(2, result[0].OriginalIndex);
            Assert.Same(pool[2], result[0].Item);

            Assert.False(result[1].IsAdd);
            Assert.Equal(3, result[1].OriginalIndex);
        }
        [Fact]
        public static void RandomDiffTest()
        {
            var r = new Random();
            var s = new List<object>();
            var t = new List<object>();
            for (int i = 0; i < pool.Length; i++)
            {
                var obj = pool[r.Next(pool.Length)];
                if (!s.Contains(obj))
                    s.Add(obj);
            }
            Assert.Equal(s.Distinct(), s);
            for (int i = 0; i < pool.Length; i++)
            {
                var obj = pool[r.Next(pool.Length)];
                if (!t.Contains(obj))
                    t.Add(obj);
            }
            Assert.Equal(t.Distinct(), t);

            int offset = 0;
            foreach (var a in BindableSnapshotCollection<object>.SequenceDiffer(s, t))
                if (a.IsAdd)
                    s.Insert(a.OriginalIndex + offset++, a.Item);
                else
                    s.RemoveAt(a.OriginalIndex + offset--);
            Assert.Equal(t, s);
        }
    }
}
