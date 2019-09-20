using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Sakuno.ING.Game
{
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(IdTableDebuggerProxy<,,,>))]
    internal class IdTable<TId, T, TRaw, TOwner> : BindableObject, ITable<TId, T>
        where TId : struct
        where T : class, IUpdatable<TId, TRaw>
        where TRaw : IIdentifiable<TId>
    {
        public event Action Updated;
        private readonly Func<TId, TOwner, T> dummyCreation;
        private readonly Func<TRaw, TOwner, DateTimeOffset, T> creation;

        private static int Compare(TId left, TId right)
            => Unsafe.As<TId, int>(ref left) - Unsafe.As<TId, int>(ref right);

        internal readonly List<T> list = new List<T>();
        private readonly TOwner owner;
        public IdTable(TOwner owner)
        {
            {
                var argId = Expression.Parameter(typeof(TId));
                var argOwner = Expression.Parameter(typeof(TOwner));
                var ctor = typeof(T).GetConstructor(new[] { typeof(TId), typeof(TOwner) });
                var call = Expression.New(ctor, argId, argOwner);
                dummyCreation = Expression.Lambda<Func<TId, TOwner, T>>(call, argId, argOwner).Compile();
            }
            {
                var argRaw = Expression.Parameter(typeof(TRaw));
                var argOwner = Expression.Parameter(typeof(TOwner));
                var argTime = Expression.Parameter(typeof(DateTimeOffset));
                var ctor = typeof(T).GetConstructor(new[] { typeof(TRaw), typeof(TOwner), typeof(DateTimeOffset) });
                var call = Expression.New(ctor, argRaw, argOwner, argTime);
                creation = Expression.Lambda<Func<TRaw, TOwner, DateTimeOffset, T>>(call, argRaw, argOwner, argTime).Compile();
            }
            this.owner = owner;
            DefaultView = new BindableSnapshotCollection<T>(this, this.OrderBy(x => x.Id));
        }

        public T this[TId id]
        {
            get
            {
                if (TryGetValue(id, out var item))
                    return item;

                item = dummyCreation(id, owner);
                Add(item);
                return item;
            }
        }

        public T this[TId? id]
            => id is TId valid ? this[valid] : null;

        public T[] this[IReadOnlyCollection<TId> ids]
        {
            get
            {
                var result = new T[ids.Count];
                int i = 0;
                foreach (var id in ids)
                    result[i++] = this[id];
                return result;
            }
        }

        public IBindableCollection<T> DefaultView { get; }
        public int Count => list.Count;

        public void BatchUpdate(IEnumerable<TRaw> source, DateTimeOffset timeStamp, bool removal = true)
        {
            int i = 0;
            foreach (var raw in source)
            {
                while (i < list.Count && Compare(list[i].Id, raw.Id) < 0)
                    if (removal)
                        list.RemoveAt(i);
                    else
                        i++;

                if (i < list.Count && EqualityComparer<TId>.Default.Equals(list[i].Id, raw.Id))
                    list[i++].Update(raw, timeStamp);
                else
                    list.Insert(i++, creation(raw, owner, timeStamp));
            }
            Updated?.Invoke();
            NotifyPropertyChanged(nameof(Count));
        }

        public void Add(TRaw raw, DateTimeOffset timeStamp) => Add(creation(raw, owner, timeStamp));

        public void Add(T item)
        {
            int i;
            for (i = 0; i < list.Count; i++)
            {
                if (Compare(list[i].Id, item.Id) > 0)
                {
                    list.Insert(i, item);
                    break;
                }
                else if (EqualityComparer<TId>.Default.Equals(list[i].Id, item.Id))
                {
                    list[i] = item;
                    break;
                }
            }
            if (i == list.Count)
                list.Add(item);
            Updated?.Invoke();
            NotifyPropertyChanged(nameof(Count));
        }

        public T Remove(TId id)
        {
            if (TryGetValue(id, out T item))
            {
                Remove(item);
                NotifyPropertyChanged(nameof(Count));
                return item;
            }
            return null;
        }

        public bool Remove(T item)
        {
            var result = list.Remove(item);
            if (result)
            {
                Updated?.Invoke();
                NotifyPropertyChanged(nameof(Count));
            }
            return result;
        }

        public int RemoveAll(Predicate<T> predicate)
        {
            var result = list.RemoveAll(predicate);
            if (result > 0)
            {
                Updated?.Invoke();
                NotifyPropertyChanged(nameof(Count));
            }
            return result;
        }

        public void Clear()
        {
            list.Clear();
            Updated?.Invoke();
            NotifyPropertyChanged(nameof(Count));
        }

        public T TryGet(TId id)
        {
            _ = TryGetValue(id, out T result);
            return result;
        }

        public bool TryGetValue(TId id, out T item)
        {
            item = default;
            if (Unsafe.As<TId, int>(ref id) < 0)
                throw new ArgumentException("Negative id is not valid.");

            int lo = 0, hi = list.Count - 1;
            while (lo <= hi)
            {
                int i = lo + ((hi - lo) >> 1);
                var t = list[i];

                int order = Compare(t.Id, id);
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

            return false;
        }

        public List<T>.Enumerator GetEnumerator() => list.GetEnumerator();
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
