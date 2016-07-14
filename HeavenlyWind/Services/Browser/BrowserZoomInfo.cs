using Sakuno.KanColle.Amatsukaze.Models;
using System.Windows.Input;

namespace Sakuno.KanColle.Amatsukaze.Services.Browser
{
    class BrowserZoomInfo : ZoomInfo
    {
        public BrowserZoomInfo(double rpZoom, ICommand rpCommand) : base(rpZoom, rpCommand)
        {
            r_IsSelected = Preference.Current.Browser.Zoom == rpZoom;
        }
    }
}
