using Sakuno.KanColle.Amatsukaze.Game.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game
{
    public class IDTable<T> : IReadOnlyDictionary<int, T> where T : IID
    {
        IDictionary<int, T> r_Dictionary;
        public IEnumerable<int> Keys { get { return r_Dictionary.Keys; } }
        public IEnumerable<T> Values { get { return r_Dictionary.Values; } }
        public int Count { get { return r_Dictionary.Count; } }
        public T this[int rpKey]
        {
            get
            {
                T rResult;
                if (r_Dictionary.TryGetValue(rpKey, out rResult))
                    return rResult;

                throw new KeyNotFoundException(rpKey.ToString());
            }
        }

        public IDTable() : this(new Dictionary<int, T>()) { }
        public IDTable(IEnumerable<T> rpSource) : this(rpSource.ToDictionary(r => r.ID)) { }
        public IDTable(Dictionary<int, T> rpSource)
        {
            r_Dictionary = rpSource;
        }

        internal void Add(T rpData) => r_Dictionary.Add(rpData.ID, rpData);
        internal void Remove(int rpID) => r_Dictionary.Remove(rpID);
        internal void Remove(T rpData) => r_Dictionary.Remove(rpData.ID);
        internal void Clear() => r_Dictionary.Clear();
        public bool ContainsKey(int rpKey) => r_Dictionary.ContainsKey(rpKey);
        public bool TryGetValue(int rpKey, out T ropValue) => r_Dictionary.TryGetValue(rpKey, out ropValue);

        public T GetValueOrDefault(int rpKey)
        {
            T rResult;
            r_Dictionary.TryGetValue(rpKey, out rResult);
            return rResult;
        }

        public IEnumerator<KeyValuePair<int, T>> GetEnumerator() => r_Dictionary.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override string ToString() => $"Count = {Count}";

        public bool UpdateRawData<TRawData>(IEnumerable<TRawData> rpRawDataCollection, Func<TRawData, T> rpValueFactory, Action<T, TRawData> rpUpdate)
            where TRawData : IID
        {
            var rResult = false;

            var rRemovedIDs = new HashSet<int>(r_Dictionary.Keys);
            foreach (var rRawData in rpRawDataCollection)
            {
                rRemovedIDs.Remove(rRawData.ID);

                T rData;
                if (rpUpdate != null && r_Dictionary.TryGetValue(rRawData.ID, out rData))
                    rpUpdate(rData, rRawData);
                else
                {
                    Add(rpValueFactory(rRawData));
                    rResult = true;
                }
            }

            foreach (var rID in rRemovedIDs)
            {
                Remove(rID);
                rResult = true;
            }

            return rResult;
        }
    }
}
