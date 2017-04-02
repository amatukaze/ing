using Sakuno.KanColle.Amatsukaze.Game.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Sakuno.KanColle.Amatsukaze.Game
{
    public class IDTable<T> : IReadOnlyList<T>, INotifyCollectionChanged where T : IID
    {
        SortedList<int, T> r_Dictionary;

        public int Count => r_Dictionary.Count;

        public T this[int rpKey]
        {
            get
            {
                T rResult;
                if (r_Dictionary.TryGetValue(rpKey, out rResult))
                    return rResult;

                throw new KeyNotFoundException($"{rpKey} (T is {typeof(T).Name})");
            }
        }

        public IEnumerable<int> Keys => r_Dictionary.Keys;
        public IEnumerable<T> Values => r_Dictionary.Values;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public IDTable() : this(new SortedList<int, T>()) { }
        public IDTable(SortedList<int, T> rpSource)
        {
            r_Dictionary = rpSource;
        }

        internal void Add(T rpData)
        {
            r_Dictionary.Add(rpData.ID, rpData);

            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, rpData, r_Dictionary.IndexOfKey(rpData.ID)));
        }
        internal void Remove(int rpID)
        {
            T rItem;
            if (!r_Dictionary.TryGetValue(rpID, out rItem))
                return;

            r_Dictionary.Remove(rpID);

            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, rItem, rpID));
        }
        internal void Remove(T rpData)
        {
            T rItem;
            if (!r_Dictionary.TryGetValue(rpData.ID, out rItem))
                return;

            r_Dictionary.Remove(rpData.ID);

            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, rItem, rpData.ID));
        }
        internal void Clear()
        {
            r_Dictionary.Clear();

            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public bool ContainsKey(int rpKey) => r_Dictionary.ContainsKey(rpKey);
        public bool TryGetValue(int rpKey, out T ropValue) => r_Dictionary.TryGetValue(rpKey, out ropValue);

        public T GetValueOrDefault(int rpKey)
        {
            T rResult;
            r_Dictionary.TryGetValue(rpKey, out rResult);
            return rResult;
        }

        public IEnumerator<T> GetEnumerator() => r_Dictionary.Values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override string ToString() => "Count = " + Count;

        public bool UpdateRawData<TRawData>(IEnumerable<TRawData> rpRawDataCollection, Func<TRawData, T> rpValueFactory, Action<T, TRawData> rpUpdate)
            where TRawData : IID
        {
            var rResult = false;

            HashSet<int> rRemovedIDs = null;
            if (r_Dictionary.Count != 0)
                rRemovedIDs = new HashSet<int>(r_Dictionary.Keys);

            if (rpRawDataCollection != null)
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

            if (rRemovedIDs != null && rRemovedIDs.Count > 0)
            {
                foreach (var rID in rRemovedIDs)
                    Remove(rID);

                rResult = true;
            }

            return rResult;
        }
    }
}
