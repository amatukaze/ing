using System.ComponentModel;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public abstract class Calculated<T> where T : IBindable
    {
        public T Raw { get; }

        public Calculated(T raw)
        {
            Raw = raw;

            if (raw is IBindableWithChildren<IBindable> parent)
            {
                parent.ChildrenChanged += CollectionChangedHandler;
                CollectionChangedHandler(parent.Children);
            }
            Update();
        }

        protected abstract void Update();
        private void PropertyChangedHandler(object sender, PropertyChangedEventArgs e) => Update();

        private IBindableCollection<IBindable> oldCollection;
        private void CollectionChangedHandler(IBindableCollection<IBindable> newCollection)
        {
            if (newCollection == oldCollection) return;
            if (oldCollection != null)
                oldCollection.ChildrenPropertyChanged -= PropertyChangedHandler;
            if (newCollection != null)
                newCollection.ChildrenPropertyChanged += PropertyChangedHandler;
            oldCollection = newCollection;
        }
    }
}
