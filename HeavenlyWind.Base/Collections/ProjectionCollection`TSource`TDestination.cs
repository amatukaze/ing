using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Sakuno.KanColle.Amatsukaze.Collections
{
    public sealed class ProjectionCollection<TSource, TDestination> : DisposableObject, IList<TDestination>, IReadOnlyList<TDestination>, INotifyPropertyChanged, INotifyCollectionChanged
    {
        IReadOnlyList<TSource> _source;
        IProjector<TSource, TDestination> _projector;

        List<TSource> _sourceSnapshot;
        List<TDestination> _destination;

        public int Count => _destination.Count;

        public TDestination this[int index] => _destination[index];

        bool ICollection<TDestination>.IsReadOnly => true;

        TDestination IList<TDestination>.this[int index]
        {
            get => _destination[index];
            set => throw new NotSupportedException();
        }

        object _threadSyncLock = new object();

        public event PropertyChangedEventHandler PropertyChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public ProjectionCollection(IReadOnlyList<TSource> source, Func<TSource, TDestination> projector)
            : this(source, projector != null ? new DelegatedProjector<TSource, TDestination>(projector) : null) { }
        public ProjectionCollection(IReadOnlyList<TSource> source, IProjector<TSource, TDestination> projector)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _projector = projector ?? throw new ArgumentNullException(nameof(projector));

            _sourceSnapshot = new List<TSource>();
            _sourceSnapshot.AddRange(_source);

            _destination = new List<TDestination>();
            ProjectFromSource();

            if (_source is INotifyCollectionChanged sourceCollectionChanged)
                sourceCollectionChanged.CollectionChanged += OnSourceCollectionChanged;
        }

        void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        var newItems = new TDestination[e.NewItems.Count];

                        for (var i = 0; i < newItems.Length; i++)
                        {
                            var newSourceItem = (TSource)e.NewItems[i];
                            var newItem = _projector.Project(newSourceItem);

                            _sourceSnapshot.Insert(e.NewStartingIndex + i, newSourceItem);
                            _destination.Insert(e.NewStartingIndex + i, newItem);

                            newItems[i] = newItem;
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        var oldItems = new TDestination[e.OldItems.Count];

                        for (var i = 0; i < oldItems.Length; i++)
                        {
                            var oldSourceItem = (TSource)e.OldItems[i];
                            var oldItemIndex = _sourceSnapshot.IndexOf(oldSourceItem);

                            oldItems[i] = _destination[oldItemIndex];
                            _destination.RemoveAt(oldItemIndex);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        var newSourceItem = (TSource)e.NewItems[0];

                        var oldItem = _destination[e.OldStartingIndex];
                        var newItem = _projector.Project(newSourceItem);

                        _sourceSnapshot[e.OldStartingIndex] = newSourceItem;
                        _destination[e.OldStartingIndex] = newItem;
                    }
                    break;

                case NotifyCollectionChangedAction.Move:
                    var movedItem = _destination[e.OldStartingIndex];
                    var movedItemOfSource = _sourceSnapshot[e.OldStartingIndex];

                    _destination.RemoveAt(e.OldStartingIndex);
                    _sourceSnapshot.RemoveAt(e.OldStartingIndex);

                    _destination.Insert(e.NewStartingIndex, movedItem);
                    _sourceSnapshot.Insert(e.NewStartingIndex, movedItemOfSource);
                    break;

                case NotifyCollectionChangedAction.Reset:
                    _destination.Clear();
                    _sourceSnapshot.Clear();
                    break;
            }
        }

        void ProjectFromSource()
        {
            for (var i = 0; i < _source.Count; i++)
                _destination[i] = _projector.Project(_source[i]);
        }

        public int IndexOf(TDestination item) => _destination.IndexOf(item);

        public bool Contains(TDestination item) => _destination.Contains(item);

        public List<TDestination>.Enumerator GetEnumerator() => _destination.GetEnumerator();

        void NotifyPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        void NotifyCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs("Count"));
                propertyChanged(this, new PropertyChangedEventArgs("Item[]"));
            }

            CollectionChanged?.Invoke(this, e);
        }
        void NotifyCollectionItemChanged(NotifyCollectionChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
            CollectionChanged?.Invoke(this, e);
        }

        protected override void DisposeManagedResources()
        {
            if (_source is INotifyCollectionChanged sourceCollectionChanged)
                sourceCollectionChanged.CollectionChanged -= OnSourceCollectionChanged;
        }

        void IList<TDestination>.Insert(int index, TDestination item) => throw new NotSupportedException();
        void IList<TDestination>.RemoveAt(int index) => throw new NotSupportedException();

        void ICollection<TDestination>.Add(TDestination item) => throw new NotSupportedException();
        bool ICollection<TDestination>.Remove(TDestination item) => throw new NotSupportedException();
        void ICollection<TDestination>.Clear() => throw new NotSupportedException();

        void ICollection<TDestination>.CopyTo(TDestination[] array, int arrayIndex) =>
            _destination.CopyTo(array, arrayIndex);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        IEnumerator<TDestination> IEnumerable<TDestination>.GetEnumerator() => GetEnumerator();
    }
}
