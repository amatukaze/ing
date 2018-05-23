namespace Sakuno.ING
{
    public class ManualNotifyObject : BindableObject
    {
        protected virtual void NotifyAllPropertyChanged() => NotifyPropertyChanged(string.Empty);
    }
}
