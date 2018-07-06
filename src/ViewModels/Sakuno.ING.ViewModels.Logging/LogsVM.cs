using System.Collections.Generic;
using System.Linq;
using Sakuno.ING.Game;
using Sakuno.ING.Game.Logger;

namespace Sakuno.ING.ViewModels.Logging
{
    public abstract class LogsVM<T>
        where T : class, ITimedEntity
    {
        private protected abstract FilterVM<T>[] CreateFilters();
        private protected abstract IReadOnlyCollection<T> GetEntities();

        private readonly IBindableCollection<FilterVM<T>> _filters;
        public IBindableCollection<IFilterVM> Filters => _filters;

        private readonly BindableSnapshotCollection<T> snapshot = new BindableSnapshotCollection<T>();
        public IBindableCollection<T> Entities => snapshot;

        public LogsVM()
        {
            _filters = CreateFilters().ToBindable();
            foreach (var filter in _filters)
                filter.Updated += Refresh;
        }

        public void Refresh()
        {
            IEnumerable<T> source = GetEntities();
            foreach(var filter in _filters)
            {
                filter.UpdateCandidates(source);
                if (filter.IsEnabled)
                    source = source.Where(filter.Hits);
            }
            snapshot.Query = source;
        }
    }
}
