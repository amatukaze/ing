using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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
        private static readonly Func<TId, TOwner, T> _dummyCreation;
        private static readonly Func<TRaw, TOwner, T> _creation;

        private static readonly PropertyChangedEventArgs _countChangedEventArgs;

        private readonly TOwner _owner;
        internal readonly List<T> _list = new List<T>();

        public int Count => _list.Count;

        public T this[TId id]
        {
            get
            {
                if (TryGetValue(id, out var item))
                    return item;

                item = _dummyCreation(id, _owner);
                AddOrUpdate(item, false);
                return item;
            }
        }
        [MaybeNull]
        public T this[TId? id]
        {
            get
            {
                if (id is TId validId && TryGetValue(validId, out var item))
                    return item;

                return default;
            }
        }
        public T[] this[IReadOnlyCollection<TId> ids]
        {
            get
            {
                if (ids.Count == 0)
                    return Array.Empty<T>();

                var result = new T[ids.Count];
                var i = 0;

                foreach (var id in ids)
                    result[i++] = this[id];

                return result;
            }
        }

        static IdTable()
        {
            {
                var argId = Expression.Parameter(typeof(TId));
                var argOwner = Expression.Parameter(typeof(TOwner));
                var ctor = typeof(T).GetConstructor(new[] { typeof(TId), typeof(TOwner) });
                var call = Expression.New(ctor, argId, argOwner);
                _dummyCreation = Expression.Lambda<Func<TId, TOwner, T>>(call, argId, argOwner).Compile();
            }
            {
                var argRaw = Expression.Parameter(typeof(TRaw));
                var argOwner = Expression.Parameter(typeof(TOwner));
                var ctor = typeof(T).GetConstructor(new[] { typeof(TRaw), typeof(TOwner) });
                var call = Expression.New(ctor, argRaw, argOwner);
                _creation = Expression.Lambda<Func<TRaw, TOwner, T>>(call, argRaw, argOwner).Compile();
            }

            _countChangedEventArgs = new PropertyChangedEventArgs(nameof(Count));
        }
        public IdTable(TOwner owner)
        {
            _owner = owner;
        }

        public void Add(TRaw raw) => Add(_creation(raw, _owner));
        public void Add(T item) => AddOrUpdate(item, false);

        private void AddOrUpdate(T item, bool replaceIfTrue)
        {
            var index = BinarySearch(item.Id);
            if (index >= 0)
            {
                if (replaceIfTrue)
                {
                    _list[index] = item;
                    return;
                }

                throw new InvalidOperationException("Duplicated id");
            }

            _list.Insert(~index, item);
            NotifyPropertyChanged(_countChangedEventArgs);
        }

        public T? Remove(TId id)
        {
            if (!TryGetValue(id, out var item))
                return default;

            Remove(item);
            return item;
        }
        public bool Remove(T item)
        {
            var result = _list.Remove(item);
            if (result)
                NotifyPropertyChanged(_countChangedEventArgs);

            return result;
        }
        public int RemoveAll(Predicate<T> predicate)
        {
            var result = _list.RemoveAll(predicate);
            if (result > 0)
                NotifyPropertyChanged(_countChangedEventArgs);

            return result;
        }

        public void Clear()
        {
            _list.Clear();
            NotifyPropertyChanged(_countChangedEventArgs);
        }

        public bool TryGetValue(TId id, [NotNullWhen(true)] out T? item)
        {
            if (Unsafe.As<TId, int>(ref id) < 0)
                throw new ArgumentException("Negative id is not valid.");

            var index = BinarySearch(id);
            if (index < 0)
            {
                item = default;
                return false;
            }

            item = _list[index];
            return true;
        }

        [return: MaybeNull]
        public T GetValueOrDefault(TId id)
        {
            TryGetValue(id, out var result);
            return result;
        }

        public void BatchUpdate(IEnumerable<TRaw> source, bool removal = true)
        {
            var i = 0;

            foreach (var raw in source)
            {
                while (i < _list.Count && Compare(_list[i].Id, raw.Id) < 0)
                    if (removal)
                        _list.RemoveAt(i);
                    else
                        i++;

                if (i < _list.Count && EqualityComparer<TId>.Default.Equals(_list[i].Id, raw.Id))
                    _list[i++].Update(raw);
                else
                    _list.Insert(i++, _creation(raw, _owner));
            }

            NotifyPropertyChanged(_countChangedEventArgs);
        }

        private int BinarySearch(TId id)
        {
            var left = 0;
            var right = _list.Count - 1;

            while (left <= right)
            {
                var middle = left + (right - left >> 1);

                var result = Compare(_list[middle].Id, id);
                if (result == 0)
                    return middle;

                if (result < 0)
                    left = middle + 1;
                else
                    right = middle - 1;
            }

            return ~left;
        }
        private static int Compare(TId left, TId right) => Unsafe.As<TId, int>(ref left) - Unsafe.As<TId, int>(ref right);

        public List<T>.Enumerator GetEnumerator() => _list.GetEnumerator();
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
