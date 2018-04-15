using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace Sakuno.KanColle.Amatsukaze.Game
{
    public class IDTable<T, TRaw> : KeyedCollection<int, T>, IUpdationSource
        where T : Calculated<TRaw>
        where TRaw : IIdentifiable
    {
        protected override int GetKeyForItem(T item) => item.Id;

        public event Action Updated;
        public event Action<IDTable<T, TRaw>> BatchUpdated;
        private static readonly Func<TRaw, T> creation;

        static IDTable()
        {
            var arg = Expression.Parameter(typeof(TRaw));
            var ctor = typeof(T).GetConstructor(new[] { typeof(TRaw) });
            var call = Expression.New(ctor, arg);
            creation = Expression.Lambda<Func<TRaw, T>>(call, arg).Compile();
        }

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
                        item = creation(raw);
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
                        Add(creation(raw));
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
