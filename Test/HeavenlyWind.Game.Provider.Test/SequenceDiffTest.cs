using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sakuno.KanColle.Amatsukaze.Game.Test
{
    [TestClass]
    public class SequenceDiffTest
    {
        private static object[] pool;
        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            pool = new object[20];
            for (int i = 0; i < pool.Length; i++)
                pool[i] = new object();
        }
        [TestMethod]
        public void DiffTest()
        {
            var s = new[]
            {
                pool[0],
                pool[1],
                pool[3],
                pool[4]
            };
            var t = new[]
            {
                pool[0],
                pool[1],
                pool[2],
                pool[3]
            };
            var result = BindableSnapshotCollection<object>.SequenceDiffer(s, t);
            Assert.AreEqual(result.Length, 2);

            Assert.IsTrue(result[0].IsAdd);
            Assert.AreEqual(result[0].OriginalIndex, 2);
            Assert.AreSame(result[0].Item, pool[2]);

            Assert.IsFalse(result[1].IsAdd);
            Assert.AreEqual(result[1].OriginalIndex, 3);
        }
        [TestMethod]
        public void RandomDiffTest()
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
            CollectionAssert.AllItemsAreUnique(s);
            for (int i = 0; i < pool.Length; i++)
            {
                var obj = pool[r.Next(pool.Length)];
                if (!t.Contains(obj))
                    t.Add(obj);
            }
            CollectionAssert.AllItemsAreUnique(t);

            int offset = 0;
            foreach (var a in BindableSnapshotCollection<object>.SequenceDiffer(s.ToArray(), t.ToArray()))
                if (a.IsAdd)
                    s.Insert(a.OriginalIndex + (offset++), a.Item);
                else
                    s.RemoveAt(a.OriginalIndex + (offset--));
            CollectionAssert.AreEqual(s, t);
        }
    }
}
