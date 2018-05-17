using System.Windows.Input;

namespace Sakuno.KanColle.Amatsukaze.Models
{
    sealed class UIZoomInfo : ZoomInfo
    {
        public UIZoomInfo(double rpZoom, ICommand rpCommand) : base(rpZoom, rpCommand)
        {
            r_IsSelected = Preference.Instance.UI.Zoom == rpZoom;
        }
    }
}
