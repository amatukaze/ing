using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Sakuno.ING.Game
{
    public class IdTable<T, TRaw> : ITable<T>
        where T : Calculated<TRaw>
        where TRaw : IIdentifiable
    {
        public event Action Updated;
        private static readonly Func<int, ITableProvider, T> creation;

        static IdTable()
        {
            var argId = Expression.Parameter(typeof(int));
            var argOwner = Expression.Parameter(typeof(ITableProvider));
            var ctor = typeof(T).GetConstructor(new[] { typeof(int), typeof(ITableProvider) });
            var call = Expression.New(ctor, argId, argOwner);
            creation = Expression.Lambda<Func<int, ITableProvider, T>>(call, argId, argOwner).Compile();
        }

        private List<T> list = new List<T>();
        private readonly ITableProvider owner;
        public IdTable(ITableProvider owner)
        {
            this.owner = owner;
            DefaultView = new BindableSnapshotCollection<T>(this, this.OrderBy(x => x.Id));
        }

        public T this[int id] => TryGetValue(id, out var item) ? item : null;

        public T TryGetOrDummy(int id)
        {
            if (id <= 0) return null;

            if (TryGetValue(id, out var item))
                return item;

            item = creation(id, owner);
            Add(item);
            return item;
        }

        public IBindableCollection<T> DefaultView { get; }
        public int Count => list.Count;

        public void BatchUpdate(IEnumerable<TRaw> source, bool removal = true)
        {
            int i = 0;
            foreach (var raw in source)
            {
                while (i < list.Count && list[i].Id < raw.Id)
                    if (removal)
                        list.RemoveAt(i);
                    else
                        i++;

                if (i < list.Count && list[i].Id == raw.Id)
                    list[i].Update(raw);
                else
                {
                    var item = creation(raw.Id, owner);
                    item.Update(raw);
                    list.Insert(i, item);
                }
            }
            Updated?.Invoke();
        }

        public void Add(TRaw raw)
        {
            var item = creation(raw.Id, owner);
            item.Update(raw);
            Add(item);
        }

        public void Add(T item)
        {
            int i;
            for (i = 0; i < list.Count; i++)
            {
                if (list[i].Id > item.Id)
                {
                    list.Insert(i, item);
                    break;
                }
                else if (list[i].Id == item.Id)
                {
                    list[i] = item;
                    break;
                }
            }
            if (i == list.Count)
                list.Add(item);
            Updated?.Invoke();
        }

        public bool Remove(int id) => Remove(this[id]);
        public bool Remove(T item)
        {
            var result = list.Remove(item);
            if (result)
                Updated?.Invoke();
            return result;
        }

        public int RemoveAll(Predicate<T> predicate)
        {
            var result = list.RemoveAll(predicate);
            if (result > 0)
                Updated?.Invoke();
            return result;
        }

        public void Clear()
        {
            list.Clear();
            Updated?.Invoke();
        }

        public bool TryGetValue(int id, out T item)
        {
            int lo = 0, hi = list.Count - 1;
            while (lo <= hi)
            {
                int i = lo + ((hi - lo) >> 1);
                var t = list[i];

                int order = t.Id - id;
                if (order == 0)
                {
                    item = t;
                    return true;
                }

                if (order < 0)
                    lo = i + 1;
                else
                    hi = i - 1;
            }

            item = default;
            return false;
        }

        public List<T>.Enumerator GetEnumerator() => list.GetEnumerator();
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
