namespace Sakuno.KanColle.Amatsukaze
{
    public class ManualNotifyObject : BindableObject
    {
        protected virtual void NotifyAllPropertyChanged() => NotifyPropertyChanged(string.Empty);
    }
}
