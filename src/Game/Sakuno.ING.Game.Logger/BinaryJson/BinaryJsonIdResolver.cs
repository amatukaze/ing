using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sakuno.ING.Game.Logger.Entities.Combat;

namespace Sakuno.ING.Game.Logger.BinaryJson
{
    internal class BinaryJsonIdResolver : IReadOnlyDictionary<string, int>
    {
        private readonly DbSet<JNameEntity> jNameTable;
        public BinaryJsonIdResolver(DbSet<JNameEntity> jNameTable) => this.jNameTable = jNameTable;

        public int this[string key]
        {
            get
            {
                var jName = jNameTable.SingleOrDefault(x => x.Name == key);
                if (jName == null)
                {
                    jName = new JNameEntity
                    {
                        Id = jNameTable.Max(x => x.Id) + 1,
                        Name = key
                    };
                    jNameTable.Add(jName);
                }
                return jName.Id;
            }
        }

        public IEnumerable<string> Keys => jNameTable.Select(x => x.Name);
        public IEnumerable<int> Values => jNameTable.Select(x => x.Id);
        public int Count => jNameTable.Count();

        public bool ContainsKey(string key) => jNameTable.Count(x => x.Name == key) > 0;
        public IEnumerator<KeyValuePair<string, int>> GetEnumerator()
            => jNameTable.Select(x => new KeyValuePair<string, int>(x.Name, x.Id)).GetEnumerator();
        public bool TryGetValue(string key, out int value)
        {
            var jName = jNameTable.SingleOrDefault(x => x.Name == key);
            if (jName != null)
            {
                value = jName.Id;
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
