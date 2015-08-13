using System;

namespace Sakuno
{
    public class DisposableObject : IDisposable
    {
        EventHandler r_Disposing;
        public event EventHandler Disposing
        {
            add
            {
                ThrowIfDisposed();
                r_Disposing = (EventHandler)Delegate.Combine(r_Disposing, value);
            }
            remove
            {
                ThrowIfDisposed();
                r_Disposing = (EventHandler)Delegate.Remove(r_Disposing, value);
            }
        }

        public bool IsDisposed { get; private set; }

        protected void ThrowIfDisposed()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        ~DisposableObject()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public void Dispose(bool rpDisposing)
        {
            if (!IsDisposed)
            {

                if (rpDisposing)
                    DisposeManagedResources();
                DisposeNativeResources();

                IsDisposed = true;
            }
        }

        protected virtual void DisposeManagedResources() { }
        protected virtual void DisposeNativeResources() { }
    }
}
