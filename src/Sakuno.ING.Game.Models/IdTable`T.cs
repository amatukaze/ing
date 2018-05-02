using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace Sakuno.ING.Game
{
    public class IdTable<T, TRaw> : KeyedCollection<int, T>, ITable<T>
        where T : Calculated<TRaw>
        where TRaw : IIdentifiable
    {
        protected override int GetKeyForItem(T item) => item.Id;

        public event Action Updated;
        public event Action<IdTable<T, TRaw>> BatchUpdated;
        private static readonly Func<int, ITableProvider, T> creation;

        static IdTable()
        {
            var argId = Expression.Parameter(typeof(int));
            var argOwner = Expression.Parameter(typeof(ITableProvider));
            var ctor = typeof(T).GetConstructor(new[] { typeof(int), typeof(ITableProvider) });
            var call = Expression.New(ctor, argId, argOwner);
            creation = Expression.Lambda<Func<int, ITableProvider, T>>(call, argId, argOwner).Compile();
        }

        private readonly ITableProvider owner;
        public IdTable(ITableProvider owner)
        {
            this.owner = owner;
            DefaultView = new BindableSnapshotCollection<T>(this, this.OrderBy(x => x.Id));
        }

        public new T this[int id] => TryGetValue(id, out var item) ? item : null;

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

        public void BatchUpdate(IEnumerable<TRaw> source, bool removal = true)
        {
            if (removal)
            {
                foreach (var raw in source)
                    if (TryGetValue(raw.Id, out var item))
                    {
                        item.Update(raw);
                        item.UpdateFlag = true;
                    }
                    else
                    {
                        item = creation(raw.Id, owner);
                        item.Update(raw);
                        item.UpdateFlag = true;
                        Add(item);
                    }
                foreach (var item in this.ToArray())
                    if (item.UpdateFlag)
                        item.UpdateFlag = false;
                    else
                        Remove(item);
            }
            else
            {
                foreach (var raw in source)
                    if (TryGetValue(raw.Id, out var item))
                        item.Update(raw);
                    else
                    {
                        item = creation(raw.Id, owner);
                        item.Update(raw);
                        Add(item);
                    }
            }
            BatchUpdated?.Invoke(this);
            Updated?.Invoke();
        }

        public void Add(TRaw raw)
        {
            var item = creation(raw.Id, owner);
            item.Update(raw);
            Add(item);
        }

        public bool TryGetValue(int id, out T item)
        {
            if (Dictionary != null)
                return Dictionary.TryGetValue(id, out item);

            foreach (var i in this)
                if (i.Id == id)
                {
                    item = i;
                    return true;
                }

            item = default;
            return false;
        }
    }
}
