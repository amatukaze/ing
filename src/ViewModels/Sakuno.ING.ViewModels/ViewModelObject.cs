namespace Sakuno.ING.ViewModels;

public abstract class ViewModelObject : BindableObject, IDisposable
{
    protected readonly CompositeDisposable Disposables = [];

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing || Disposables.IsDisposed)
            return;

        Disposables.Dispose();
    }
}
