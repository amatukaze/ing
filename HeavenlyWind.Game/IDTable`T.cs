using Sakuno.Collections;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game
{
    public class IDTable<T> : IReadOnlyDictionary<int, T> where T : IID
    {
        HybridDictionary<int, T> r_Dictionary;

        public int Count => r_Dictionary.Count;

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

        public IEnumerable<int> Keys => r_Dictionary.Keys;
        public IEnumerable<T> Values => r_Dictionary.Values;

        public IDTable() : this(new HybridDictionary<int, T>()) { }
        public IDTable(HybridDictionary<int, T> rpSource)
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

        public override string ToString() => "Count = " + Count;

        public bool UpdateRawData<TRawData>(IEnumerable<TRawData> rpRawDataCollection, Func<TRawData, T> rpValueFactory, Action<T, TRawData> rpUpdate)
            where TRawData : IID
        {
            var rResult = false;

            HashSet<int> rRemovedIDs = null;
            if (r_Dictionary.Count != 0)
                rRemovedIDs = new HashSet<int>(r_Dictionary.Keys);
            foreach (var rRawData in rpRawDataCollection)
            {
                if (rRemovedIDs != null)
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

            if (rRemovedIDs != null)
                foreach (var rID in rRemovedIDs)
                {
                    Remove(rID);
                    rResult = true;
                }

            return rResult;
        }
    }
}
