using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game
{
    public partial class BindableSnapshotCollection<T> : IDisposable
        where T : class
    {
        private readonly IUpdationSource source;
        private T[] snapshot;

        private IEnumerable<T> _query;
        public IEnumerable<T> Query
        {
            get => _query;
            set
            {
                _query = value ?? throw new ArgumentNullException(nameof(Query));
                Refresh();
            }
        }

        public BindableSnapshotCollection(IUpdationSource source, IEnumerable<T> query)
        {
            this.source = source ?? throw new ArgumentNullException(nameof(source));
            _query = query ?? throw new ArgumentNullException(nameof(query));
            snapshot = query.ToArray();
            source.Updated += Refresh;
        }

        public void Dispose() => source.Updated -= Refresh;

        public void Refresh()
        {
            var @new = Query.ToArray();

            snapshot = @new;
        }
    }
}
