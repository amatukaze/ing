using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace Sakuno.KanColle.Amatsukaze.Game
{
    public class IdTable<T, TRaw> : KeyedCollection<int, T>, ITable<T>
        where T : Calculated<TRaw>
        where TRaw : IIdentifiable
    {
        protected override int GetKeyForItem(T item) => item.Id;

        public event Action Updated;
        public event Action<IdTable<T, TRaw>> BatchUpdated;
        private static readonly Func<TRaw, ITableProvider, T> creation;

        static IdTable()
        {
            var arg = Expression.Parameter(typeof(TRaw));
            var argOwner = Expression.Parameter(typeof(ITableProvider));
            var ctor = typeof(T).GetConstructor(new[] { typeof(TRaw), typeof(ITableProvider) });
            var call = Expression.New(ctor, arg);
            creation = Expression.Lambda<Func<TRaw, ITableProvider, T>>(call, arg, argOwner).Compile();
        }

        private readonly ITableProvider owner;
        public IdTable(ITableProvider owner) => this.owner = owner;

        public new T this[int id] => TryGetValue(id, out var item) ? item : null;

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
                        item = creation(raw, owner);
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
                        Add(creation(raw, owner));
            }
            BatchUpdated?.Invoke(this);
            Updated?.Invoke();
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
