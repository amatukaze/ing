using System.Windows.Input;

namespace Sakuno.KanColle.Amatsukaze.Services.Browser
{
    class BrowserZoomInfo : ModelBase
    {
        public double Zoom { get; }

        bool r_IsSelected;
        public bool IsSelected
        {
            get { return r_IsSelected; }
            internal set
            {
                r_IsSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        public ICommand Command { get; }

        public BrowserZoomInfo(double rpZoom, ICommand rpCommand)
        {
            Zoom = rpZoom;

            r_IsSelected = Preference.Current.Browser.Zoom == rpZoom;

            Command = rpCommand;
        }
    }
}
