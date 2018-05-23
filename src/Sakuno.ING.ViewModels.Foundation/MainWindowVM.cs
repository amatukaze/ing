namespace Sakuno.ING.ViewModels
{
    public class MainWindowVM : ManualNotifyObject
    {
        string _title;
        public string Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
