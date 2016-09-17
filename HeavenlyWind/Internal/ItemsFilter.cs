using System;

namespace Sakuno.KanColle.Amatsukaze.Internal
{
    abstract class ItemsFilter<T> : DisposableModelBase
    {
        Action r_UpdateAction;

        protected ItemsFilter(Action rpUpdateAction)
        {
            r_UpdateAction = rpUpdateAction;
        }

        public abstract bool Predicate(T rpItem);

        protected void Update() => r_UpdateAction?.Invoke();

        protected override void DisposeManagedResources() => r_UpdateAction = null;
    }
}
